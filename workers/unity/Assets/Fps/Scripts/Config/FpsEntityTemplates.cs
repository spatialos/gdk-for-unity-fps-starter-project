using System.Collections.Generic;
using Fps.Guns;
using Fps.Health;
using Fps.Respawning;
using Fps.SchemaExtensions;
using Improbable;
using Improbable.Gdk.Core;
using Improbable.Gdk.PlayerLifecycle;
using Improbable.Gdk.QueryBasedInterest;
using Improbable.Generated;

namespace Fps.Config
{
    public static class FpsEntityTemplates
    {
        public static readonly EntityId LoadBalancerPartitionEntityId = new EntityId(1);
        public static readonly EntityId PlayerCreatorEntityId = new EntityId(2);

        public static EntityTemplate Spawner(Coordinates spawnerCoordinates)
        {
            var template = BaseTemplate();
            template.AddComponent(new Position.Snapshot(spawnerCoordinates));
            template.AddComponent(new Metadata.Snapshot("PlayerCreator"));
            template.AddComponent(new Persistence.Snapshot());
            template.AddComponent(new PlayerCreator.Snapshot());

            return template;
        }

        public static EntityTemplate Player(EntityId entityId, EntityId clientEntityId, byte[] args)
        {
            var (spawnPosition, spawnYaw, spawnPitch) = SpawnPoints.GetRandomSpawnPoint();

            var serverResponse = new ServerResponse
            {
                Position = spawnPosition.ToVector3Int()
            };

            var rotationUpdate = new RotationUpdate
            {
                Yaw = spawnYaw.ToInt1k(),
                Pitch = spawnPitch.ToInt1k()
            };

            var pos = new Position.Snapshot { Coords = Coordinates.FromUnityVector(spawnPosition) };
            var serverMovement = new ServerMovement.Snapshot { Latest = serverResponse };
            var clientMovement = new ClientMovement.Snapshot { Latest = new ClientRequest() };
            var clientRotation = new ClientRotation.Snapshot { Latest = rotationUpdate };
            var shootingComponent = new ShootingComponent.Snapshot();
            var gunComponent = new GunComponent.Snapshot { GunId = PlayerGunSettings.DefaultGunIndex };
            var gunStateComponent = new GunStateComponent.Snapshot { IsAiming = false };
            var healthComponent = new HealthComponent.Snapshot
            {
                Health = PlayerHealthSettings.MaxHealth,
                MaxHealth = PlayerHealthSettings.MaxHealth,
            };

            var healthRegenComponent = new HealthRegenComponent.Snapshot
            {
                CooldownSyncInterval = PlayerHealthSettings.SpatialCooldownSyncInterval,
                DamagedRecently = false,
                RegenAmount = PlayerHealthSettings.RegenAmount,
                RegenCooldownTimer = PlayerHealthSettings.RegenAfterDamageCooldown,
                RegenInterval = PlayerHealthSettings.RegenInterval,
                RegenPauseTime = 0,
            };

            var template = BaseTemplate();
            template.AddComponent(pos);
            template.AddComponent(new Metadata.Snapshot("Player"));
            template.AddComponent(serverMovement);
            template.AddComponent(clientMovement);
            template.AddComponent(clientRotation);
            template.AddComponent(shootingComponent);
            template.AddComponent(gunComponent);
            template.AddComponent(gunStateComponent);
            template.AddComponent(healthComponent);
            template.AddComponent(healthRegenComponent);
            template.AddPlayerLifecycleComponents(clientEntityId);

            // Position, Metadata, OwningWorker and ServerMovement are included in all queries, since these
            // components are required by the GameObject creator.

            // HealthComponent is needed by the LookAtRagdoll script for respawn behaviour.
            // GunComponent is needed by the GunManager script.
            var clientSelfInterest = InterestQuery.Query(Constraint.EntityId(entityId)).FilterResults(new[]
            {
                Position.ComponentId, Metadata.ComponentId, OwningWorker.ComponentId,
                ServerMovement.ComponentId, HealthComponent.ComponentId, GunComponent.ComponentId
            });

            // ClientRotation is used for rendering other players.
            // GunComponent is required by the GunManager script.
            // GunStateComponent and ShootingComponent are needed for rendering other players' shots.
            var clientRangeInterest = InterestQuery.Query(Constraint.RelativeCylinder(150)).FilterResults(new[]
            {
                Position.ComponentId, Metadata.ComponentId, OwningWorker.ComponentId,
                ServerMovement.ComponentId, ClientRotation.ComponentId, HealthComponent.ComponentId,
                GunComponent.ComponentId, GunStateComponent.ComponentId, ShootingComponent.ComponentId
            });

            var interest = InterestTemplate.Create()
                .AddQueries(ComponentSets.PlayerClientSet, clientSelfInterest, clientRangeInterest);

            template.AddComponent(interest.ToSnapshot());

            return template;
        }

        public static EntityTemplate CreateLoadBalancingPartition()
        {
            var template = BaseTemplate();
            template.AddComponent(new Position.Snapshot());
            template.AddComponent(new Metadata.Snapshot("LB Partition"));

            var query = InterestQuery.Query(Constraint.Component<Position.Component>());
            var interest = InterestTemplate.Create().AddQueries(ComponentSets.AuthorityDelegationSet, query);
            template.AddComponent(interest.ToSnapshot());
            return template;
        }

        private static EntityTemplate BaseTemplate()
        {
            var template = new EntityTemplate();
            template.AddComponent(new AuthorityDelegation.Snapshot(new Dictionary<uint, long>
            {
                { ComponentSets.AuthorityDelegationSet.ComponentSetId, LoadBalancerPartitionEntityId.Id }
            }));
            return template;
        }
    }
}
