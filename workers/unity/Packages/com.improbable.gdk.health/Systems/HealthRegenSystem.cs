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
            [ReadOnly] public SharedComponentDataArray<HealthComponent.ComponentAuthority> Authority;
        }

        public struct TakingDamage
        {
            public readonly int Length;
            public ComponentDataArray<HealthRegenData> RegenData;
            public ComponentDataArray<HealthRegenComponent.Component> HealthRegenComponents;
            [ReadOnly] public ComponentDataArray<SpatialEntityId> EntityIds;
            [ReadOnly] public SharedComponentDataArray<HealthComponent.ComponentAuthority> Authority;
        }

        public struct EntitiesToRegen
        {
            public readonly int Length;
            public ComponentDataArray<HealthRegenComponent.Component> HealthRegenComponents;
            public ComponentDataArray<HealthRegenData> RegenData;
            [ReadOnly] public ComponentDataArray<HealthComponent.Component> HealthComponents;
            [ReadOnly] public ComponentDataArray<SpatialEntityId> EntityId;
            [ReadOnly] public SharedComponentDataArray<HealthComponent.ComponentAuthority> Authority;
            [ReadOnly] public EntityArray Entities;
        }

        [Inject] private EntitiesNeedingRegenData needData;
        [Inject] private TakingDamage takingDamage;
        [Inject] private EntitiesToRegen toRegen;
        [Inject] private CommandSystem commandSystem;
        [Inject] private ComponentUpdateSystem componentUpdateSystem;

        protected override void OnUpdate()
        {
            // Add the HealthRegenData if you don't currently have it.
            for (var i = 0; i < needData.Length; i++)
            {
                if (!needData.Authority[i].HasAuthority)
                {
                    continue;
                }

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
                if (!takingDamage.Authority[i].HasAuthority)
                {
                    continue;
                }

                var events = componentUpdateSystem.GetEventsReceived<HealthComponent.HealthModified.Event>(takingDamage.EntityIds[i]
                    .EntityId);
                var damagedRecently = false;
                for (int j = 0; j < events.Count; ++j)
                {
                    var modifier = events[j].Event.Payload.Modifier;
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

                var update = new HealthRegenComponent.Update();
                var regenComponent = takingDamage.HealthRegenComponents[i];
                var regenData = takingDamage.RegenData[i];

                regenComponent.DamagedRecently = true;
                regenComponent.RegenCooldownTimer = regenComponent.RegenPauseTime;
                update.DamagedRecently = new Option<BlittableBool>(true);
                update.RegenCooldownTimer = regenComponent.RegenPauseTime;

                regenData.DamagedRecentlyTimer = regenComponent.RegenPauseTime;
                regenData.NextSpatialSyncTimer = regenComponent.CooldownSyncInterval;

                takingDamage.HealthRegenComponents[i] = regenComponent;
                componentUpdateSystem.SendUpdate(update, takingDamage.EntityIds[i].EntityId);
                takingDamage.RegenData[i] = regenData;
            }

            // Count down the timers, and update the HealthComponent accordingly.
            for (var i = 0; i < toRegen.Length; i++)
            {
                if (!toRegen.Authority[i].HasAuthority)
                {
                    continue;
                }

                var healthComponent = toRegen.HealthComponents[i];
                var update = new HealthRegenComponent.Update();
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
                        update.DamagedRecently = new Option<BlittableBool>(false);
                        update.RegenCooldownTimer = 0;
                        componentUpdateSystem.SendUpdate(update, toRegen.EntityId[i].EntityId);
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
                            update.RegenCooldownTimer = regenData.DamagedRecentlyTimer;
                            componentUpdateSystem.SendUpdate(update, toRegen.EntityId[i].EntityId);
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
                        var modifyHealthRequest = new HealthComponent.ModifyHealth.Request(
                            toRegen.EntityId[i].EntityId,
                            new HealthModifier()
                            {
                                Amount = regenComponent.RegenAmount
                            });
                        commandSystem.SendCommand(modifyHealthRequest, toRegen.Entities[i]);
                    }

                    toRegen.RegenData[i] = regenData;
                }
            }
        }
    }
}
