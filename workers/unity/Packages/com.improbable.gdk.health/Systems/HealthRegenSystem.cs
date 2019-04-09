using System.Collections.Generic;
using Improbable.Gdk.Core;
using Improbable.Worker.CInterop;
using Unity.Entities;
using UnityEngine;

namespace Improbable.Gdk.Health
{
    [UpdateInGroup(typeof(SpatialOSUpdateGroup))]
    [UpdateAfter(typeof(ServerHealthModifierSystem))]
    [AlwaysUpdateSystem]
    public class HealthRegenSystem : ComponentSystem
    {
        private WorkerSystem workerSystem;
        private ComponentUpdateSystem componentUpdateSystem;
        private CommandSystem commandSystem;

        private ComponentGroup initGroup;
        private ComponentGroup regenGroup;

        private HashSet<EntityId> recentlyDamagedCache = new HashSet<EntityId>();

        protected override void OnCreateManager()
        {
            base.OnCreateManager();

            workerSystem = World.GetExistingManager<WorkerSystem>();
            componentUpdateSystem = World.GetExistingManager<ComponentUpdateSystem>();
            commandSystem = World.GetExistingManager<CommandSystem>();

            initGroup = GetComponentGroup(ComponentType.ReadOnly<HealthRegenComponent.Component>(),
                ComponentType.Subtractive<HealthRegenData>(),
                ComponentType.ReadOnly<HealthComponent.ComponentAuthority>()
            );
            initGroup.SetFilter(HealthComponent.ComponentAuthority.Authoritative);

            regenGroup = GetComponentGroup(
                ComponentType.Create<HealthRegenComponent.Component>(),
                ComponentType.Create<HealthRegenData>(), ComponentType.ReadOnly<HealthComponent.Component>(),
                ComponentType.ReadOnly<SpatialEntityId>(),
                ComponentType.ReadOnly<HealthComponent.ComponentAuthority>()
            );
            regenGroup.SetFilter(HealthComponent.ComponentAuthority.Authoritative);
        }

        protected override void OnUpdate()
        {
            InitializeRegenData();

            ProcessDamageEvents();

            ApplyHealthRegen();
        }

        private void InitializeRegenData()
        {
            if (initGroup.IsEmptyIgnoreFilter)
            {
                return;
            }

            var entities = initGroup.GetEntityArray();
            var regenComponentData = initGroup.GetComponentDataArray<HealthRegenComponent.Component>();

            // Add the HealthRegenData if you don't currently have it.
            for (var i = 0; i < entities.Length; i++)
            {
                var healthRegenComponent = regenComponentData[i];

                var regenData = new HealthRegenData();

                if (healthRegenComponent.DamagedRecently)
                {
                    regenData.DamagedRecentlyTimer = healthRegenComponent.RegenCooldownTimer;
                    regenData.NextSpatialSyncTimer = healthRegenComponent.CooldownSyncInterval;
                }

                PostUpdateCommands.AddComponent(entities[i], regenData);
            }
        }

        private void ProcessDamageEvents()
        {
            var healthModifiedEvents = componentUpdateSystem.GetEventsReceived<HealthComponent.HealthModified.Event>();
            if (healthModifiedEvents.Count == 0)
            {
                return;
            }

            for (var i = 0; i < healthModifiedEvents.Count; ++i)
            {
                ref readonly var healthEvent = ref healthModifiedEvents[i];
                if (componentUpdateSystem.GetAuthority(healthEvent.EntityId, HealthComponent.ComponentId) ==
                    Authority.NotAuthoritative)
                {
                    continue;
                }

                if (healthEvent.Event.Payload.Modifier.Amount < 0)
                {
                    recentlyDamagedCache.Add(healthEvent.EntityId);
                }
            }

            var healthRegenComponentDataForEntity = GetComponentDataFromEntity<HealthRegenComponent.Component>();
            var healthRegenDataForEntity = GetComponentDataFromEntity<HealthRegenData>();
            foreach (var entityId in recentlyDamagedCache)
            {
                workerSystem.TryGetEntity(entityId, out var entity);
                var regenComponent = healthRegenComponentDataForEntity[entity];
                var regenData = healthRegenDataForEntity[entity];

                regenComponent.DamagedRecently = true;
                regenComponent.RegenCooldownTimer = regenComponent.RegenPauseTime;
                regenData.DamagedRecentlyTimer = regenComponent.RegenPauseTime;
                regenData.NextSpatialSyncTimer = regenComponent.CooldownSyncInterval;

                healthRegenComponentDataForEntity[entity] = regenComponent;
                healthRegenDataForEntity[entity] = regenData;
            }

            recentlyDamagedCache.Clear();
        }

        private void ApplyHealthRegen()
        {
            if (regenGroup.IsEmptyIgnoreFilter)
            {
                return;
            }

            var spatialIdData = regenGroup.GetComponentDataArray<SpatialEntityId>();
            var healthComponentData = regenGroup.GetComponentDataArray<HealthComponent.Component>();
            var healthRegenComponentData = regenGroup.GetComponentDataArray<HealthRegenComponent.Component>();
            var healthRegenData = regenGroup.GetComponentDataArray<HealthRegenData>();

            // Count down the timers, and update the HealthComponent accordingly.
            for (var i = 0; i < spatialIdData.Length; i++)
            {
                var healthComponent = healthComponentData[i];
                var regenComponent = healthRegenComponentData[i];
                var regenData = healthRegenData[i];

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
                        healthRegenComponentData[i] = regenComponent;
                    }
                    else
                    {
                        // Send a spatial update once every CooldownSyncInterval.
                        regenData.NextSpatialSyncTimer -= Time.deltaTime;
                        if (regenData.NextSpatialSyncTimer <= 0)
                        {
                            regenData.NextSpatialSyncTimer += regenComponent.CooldownSyncInterval;
                            regenComponent.RegenCooldownTimer = regenData.DamagedRecentlyTimer;
                            healthRegenComponentData[i] = regenComponent;
                        }
                    }

                    healthRegenData[i] = regenData;

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
                            spatialIdData[i].EntityId,
                            new HealthModifier()
                            {
                                Amount = regenComponent.RegenAmount
                            }
                        );
                        commandSystem.SendCommand(modifyHealthRequest);
                    }

                    healthRegenData[i] = regenData;
                }
            }
        }
    }
}
