using System.Collections.Generic;
using Improbable.Gdk.Core;
using Improbable.Worker.CInterop;
using Unity.Entities;
using UnityEngine;
using Entity = Unity.Entities.Entity;

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

        private EntityQuery initGroup;
        private EntityQuery regenGroup;

        private HashSet<EntityId> recentlyDamagedCache = new HashSet<EntityId>();

        protected override void OnCreate()
        {
            base.OnCreate();

            workerSystem = World.GetExistingSystem<WorkerSystem>();
            componentUpdateSystem = World.GetExistingSystem<ComponentUpdateSystem>();
            commandSystem = World.GetExistingSystem<CommandSystem>();

            initGroup = GetEntityQuery(
                ComponentType.ReadOnly<HealthRegenComponent.Component>(),
                ComponentType.Exclude<HealthRegenData>(),
                ComponentType.ReadOnly<HealthComponent.ComponentAuthority>()
            );
            initGroup.SetFilter(HealthComponent.ComponentAuthority.Authoritative);

            regenGroup = GetEntityQuery(
                ComponentType.ReadWrite<HealthRegenComponent.Component>(),
                ComponentType.ReadWrite<HealthRegenData>(),
                ComponentType.ReadOnly<HealthComponent.Component>(),
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

            Entities.With(initGroup).ForEach((Entity entity, ref HealthRegenComponent.Component healthRegenComponent) =>
            {
                var regenData = new HealthRegenData();

                if (healthRegenComponent.DamagedRecently)
                {
                    regenData.DamagedRecentlyTimer = healthRegenComponent.RegenCooldownTimer;
                    regenData.NextSpatialSyncTimer = healthRegenComponent.CooldownSyncInterval;
                }

                PostUpdateCommands.AddComponent(entity, regenData);
            });
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

            Entities.With(regenGroup).ForEach(
                (ref SpatialEntityId spatialEntityId,
                    ref HealthComponent.Component healthComponent,
                    ref HealthRegenComponent.Component regenComponent,
                    ref HealthRegenData regenData) =>
                {
                    // Don't regen if dead.
                    if (healthComponent.Health == 0)
                    {
                        return;
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
                        }
                        else
                        {
                            // Send a spatial update once every CooldownSyncInterval.
                            regenData.NextSpatialSyncTimer -= Time.deltaTime;
                            if (regenData.NextSpatialSyncTimer <= 0)
                            {
                                regenData.NextSpatialSyncTimer += regenComponent.CooldownSyncInterval;
                                regenComponent.RegenCooldownTimer = regenData.DamagedRecentlyTimer;
                            }
                        }

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
                                spatialEntityId.EntityId,
                                new HealthModifier()
                                {
                                    Amount = regenComponent.RegenAmount
                                }
                            );
                            commandSystem.SendCommand(modifyHealthRequest);
                        }
                    }
                });
        }
    }
}
