using Improbable.Gdk.Core;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Improbable.Gdk.Health
{
    [UpdateInGroup(typeof(SpatialOSUpdateGroup))]
    [UpdateAfter(typeof(ServerHealthModifierSystem))]
    public class HealthRegenSystem : ComponentSystem
    {
        public struct EntitiesNeedingRegenData
        {
            public readonly int Length;
            [ReadOnly] public EntityArray Entities;
            [ReadOnly] public ComponentDataArray<HealthRegenComponent.Component> HealthRegenComponents;
            [ReadOnly] public SubtractiveComponent<HealthRegenData> DenotesMissingData;
            [ReadOnly] public ComponentDataArray<Authoritative<HealthComponent.Component>> DenotesAuthority;
        }

        public struct TakingDamage
        {
            public readonly int Length;
            public ComponentDataArray<HealthRegenData> RegenData;
            public ComponentDataArray<HealthRegenComponent.Component> HealthRegenComponents;
            [ReadOnly] public ComponentDataArray<HealthComponent.ReceivedEvents.HealthModified> HealthModifiedEvents;
            [ReadOnly] public ComponentDataArray<Authoritative<HealthComponent.Component>> DenotesAuthority;
        }

        public struct EntitiesToRegen
        {
            public readonly int Length;
            public ComponentDataArray<HealthComponent.CommandSenders.ModifyHealth> ModifyHealthCommandSenders;
            public ComponentDataArray<HealthRegenComponent.Component> HealthRegenComponents;
            public ComponentDataArray<HealthRegenData> RegenData;
            [ReadOnly] public ComponentDataArray<HealthComponent.Component> HealthComponents;
            [ReadOnly] public ComponentDataArray<SpatialEntityId> EntityId;
            [ReadOnly] public ComponentDataArray<Authoritative<HealthComponent.Component>> DenotesAuthority;
        }

        [Inject] private EntitiesNeedingRegenData needData;
        [Inject] private TakingDamage takingDamage;
        [Inject] private EntitiesToRegen toRegen;

        protected override void OnUpdate()
        {
            // Add the HealthRegenData if you don't currently have it.
            for (var i = 0; i < needData.Length; i++)
            {
                var healthRegenComponent = needData.HealthRegenComponents[i];

                var regenData = new HealthRegenData();

                if (healthRegenComponent.DamagedRecently)
                {
                    regenData.DamagedRecentlyTimer = healthRegenComponent.RegenCooldownTimer;
                    regenData.NextSpatialSyncTimer = healthRegenComponent.CooldownSyncInterval;
                }

                PostUpdateCommands.AddComponent(needData.Entities[i], regenData);
            }

            // When the HealthComponent takes a damaging event, reset the DamagedRecently timer.
            for (var i = 0; i < takingDamage.Length; i++)
            {
                var healthModifiedEvents = takingDamage.HealthModifiedEvents[i];
                var damagedRecently = false;

                foreach (var modifiedEvent in takingDamage.HealthModifiedEvents[i].Events)
                {
                    var modifier = modifiedEvent.Modifier;
                    if (modifier.Amount < 0)
                    {
                        damagedRecently = true;
                        break;
                    }
                }
                if (!damagedRecently)
                {
                    continue;
                }

                var regenComponent = takingDamage.HealthRegenComponents[i];
                var regenData = takingDamage.RegenData[i];

                regenComponent.DamagedRecently = true;
                regenComponent.RegenCooldownTimer = regenComponent.RegenPauseTime;

                regenData.DamagedRecentlyTimer = regenComponent.RegenPauseTime;
                regenData.NextSpatialSyncTimer = regenComponent.CooldownSyncInterval;

                takingDamage.HealthRegenComponents[i] = regenComponent;
                takingDamage.RegenData[i] = regenData;
            }

            // Count down the timers, and update the HealthComponent accordingly. 
            for (var i = 0; i < toRegen.Length; i++)
            {
                var healthComponent = toRegen.HealthComponents[i];
                var regenComponent = toRegen.HealthRegenComponents[i];

                var regenData = toRegen.RegenData[i];

                // Don't regen if dead.
                if (healthComponent.Health == 0)
                {
                    continue;
                }

                // If damaged recently, tick down the timer.
                if (regenComponent.DamagedRecently)
                {
                    regenData.DamagedRecentlyTimer -= Time.deltaTime;

                    if (regenData.DamagedRecentlyTimer <= 0)
                    {
                        regenData.DamagedRecentlyTimer = 0;
                        regenComponent.DamagedRecently = false;
                        regenComponent.RegenCooldownTimer = 0;
                        toRegen.HealthRegenComponents[i] = regenComponent;
                    }
                    else
                    {
                        // Send a spatial update once every CooldownSyncInterval.
                        regenData.NextSpatialSyncTimer -= Time.deltaTime;
                        if (regenData.NextSpatialSyncTimer <= 0)
                        {
                            regenData.NextSpatialSyncTimer += regenComponent.CooldownSyncInterval;
                            regenComponent.RegenCooldownTimer = regenData.DamagedRecentlyTimer;
                            toRegen.HealthRegenComponents[i] = regenComponent;
                        }
                    }

                    toRegen.RegenData[i] = regenData;

                    return;
                }

                // If not damaged recently, and not already fully healed, regen. 
                if (healthComponent.Health < healthComponent.MaxHealth)
                {
                    regenData.NextRegenTimer -= Time.deltaTime;
                    if (regenData.NextRegenTimer <= 0)
                    {
                        regenData.NextRegenTimer += regenComponent.RegenInterval;

                        // Send command to regen entity.
                        var commandSender = toRegen.ModifyHealthCommandSenders[i];
                        var modifyHealthRequest = HealthComponent.ModifyHealth.CreateRequest(
                            toRegen.EntityId[i].EntityId,
                            new HealthModifier()
                            {
                                Amount = regenComponent.RegenAmount
                            });
                        commandSender.RequestsToSend.Add(modifyHealthRequest);
                    }
                    toRegen.RegenData[i] = regenData;
                }

            }
        }
    }
}
