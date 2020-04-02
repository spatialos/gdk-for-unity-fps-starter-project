using Fps.Guns;
using Fps.Health;
using Fps.Respawning;
using Fps.SchemaExtensions;
using Improbable;
using Improbable.Gdk.Core;
using Improbable.Gdk.PlayerLifecycle;
using Improbable.Gdk.QueryBasedInterest;

namespace Fps.Config
{
    public static class FpsEntityTemplates
    {
        public static EntityTemplate Spawner(Coordinates spawnerCoordinates)
        {
            var position = new Position.Snapshot(spawnerCoordinates);
            var metadata = new Metadata.Snapshot("PlayerCreator");

            var template = new EntityTemplate();
            template.AddComponent(position, WorkerUtils.UnityGameLogic);
            template.AddComponent(metadata, WorkerUtils.UnityGameLogic);
            template.AddComponent(new Persistence.Snapshot(), WorkerUtils.UnityGameLogic);
            template.AddComponent(new PlayerCreator.Snapshot(), WorkerUtils.UnityGameLogic);

            return template;
        }

        public static EntityTemplate Player(EntityId entityId, string workerId, byte[] args)
        {
            var client = EntityTemplate.GetWorkerAccessAttribute(workerId);

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

            var template = new EntityTemplate();
            template.AddComponent(pos, WorkerUtils.UnityGameLogic);
            template.AddComponent(new Metadata.Snapshot { EntityType = "Player" }, WorkerUtils.UnityGameLogic);
            template.AddComponent(serverMovement, WorkerUtils.UnityGameLogic);
            template.AddComponent(clientMovement, client);
            template.AddComponent(clientRotation, client);
            template.AddComponent(shootingComponent, client);
            template.AddComponent(gunComponent, WorkerUtils.UnityGameLogic);
            template.AddComponent(gunStateComponent, client);
            template.AddComponent(healthComponent, WorkerUtils.UnityGameLogic);
            template.AddComponent(healthRegenComponent, WorkerUtils.UnityGameLogic);

            PlayerLifecycleHelper.AddPlayerLifecycleComponents(template, workerId, WorkerUtils.UnityGameLogic);

            const int serverRadius = 150;
            var clientRadius = workerId.Contains(WorkerUtils.MobileClient) ? 60 : 150;

            var clientSelfInterest = InterestQuery.Query(Constraint.EntityId(entityId)).FilterResults(new[]
            {
                Position.ComponentId, Metadata.ComponentId, OwningWorker.ComponentId,
                ServerMovement.ComponentId, HealthComponent.ComponentId, GunComponent.ComponentId
            });

            var clientRangeInterest = InterestQuery.Query(Constraint.RelativeCylinder(clientRadius)).FilterResults(new[]
            {
                Position.ComponentId, Metadata.ComponentId, OwningWorker.ComponentId,
                ServerMovement.ComponentId, ClientRotation.ComponentId, HealthComponent.ComponentId,
                GunComponent.ComponentId, GunStateComponent.ComponentId, ShootingComponent.ComponentId
            });

            var serverSelfInterest = InterestQuery.Query(Constraint.EntityId(entityId)).FilterResults(new[]
            {
                ClientMovement.ComponentId, ShootingComponent.ComponentId
            });

            var serverRangeInterest = InterestQuery.Query(Constraint.RelativeCylinder(serverRadius)).FilterResults(new[]
            {
                Position.ComponentId, Metadata.ComponentId, OwningWorker.ComponentId,
                ServerMovement.ComponentId, ClientRotation.ComponentId, HealthComponent.ComponentId,
                ShootingComponent.ComponentId
            });

            var interest = InterestTemplate.Create()
                .AddQueries<ClientMovement.Component>(clientSelfInterest, clientRangeInterest)
                .AddQueries<ServerMovement.Component>(serverSelfInterest, serverRangeInterest);

            template.AddComponent(interest.ToSnapshot());

            template.SetReadAccess(WorkerUtils.UnityClient, WorkerUtils.UnityGameLogic, WorkerUtils.MobileClient);

            return template;
        }
    }
}
