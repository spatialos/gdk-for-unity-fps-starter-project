using System.Collections.Generic;
using Improbable;
using Improbable.Gdk.Core;
using Improbable.Gdk.Guns;
using Improbable.Gdk.Health;
using Improbable.Gdk.Movement;
using Improbable.Gdk.PlayerLifecycle;
using Improbable.Gdk.StandardTypes;
using Improbable.PlayerLifecycle;

namespace Fps
{
    public static class FpsEntityTemplates
    {
        private static readonly List<string> AllWorkerAttributes =
            new List<string> { WorkerUtils.UnityGameLogic, WorkerUtils.UnityClient };

        public static EntityTemplate Spawner()
        {
            const string gameLogic = WorkerUtils.UnityGameLogic;

            return EntityBuilder.Begin()
                .AddPosition(0, 0, 0, gameLogic)
                .AddMetadata("PlayerCreator", gameLogic)
                .SetPersistence(true)
                .SetReadAcl(gameLogic)
                .AddComponent(PlayerCreator.Component.CreateSchemaComponentData(), gameLogic)
                .Build();
        }

        public static EntityTemplate Player(string workerId, Vector3f position)
        {
            const string gameLogic = WorkerUtils.UnityGameLogic;
            var client = $"workerId:{workerId}";

            var (spawnPosition, spawnYaw, spawnPitch) = SpawnPoints.GetRandomSpawnPoint();

            var serverResponse = new ServerResponse
            {
                Position = spawnPosition.ToIntAbsolute()
            };

            var serverMovement = ServerMovement.Component.CreateSchemaComponentData(serverResponse);
            var clientMovementData = new ClientRequest();
            var rotationUpdate = new RotationUpdate
            {
                Yaw = spawnYaw.ToInt1k(),
                Pitch = spawnPitch.ToInt1k()
            };
            var clientMovement = ClientMovement.Component.CreateSchemaComponentData(clientMovementData);
            var clientRotation = ClientRotation.Component.CreateSchemaComponentData(rotationUpdate);
            var shootingComponent = ShootingComponent.Component.CreateSchemaComponentData();
            var gunStateComponent = GunStateComponent.Component.CreateSchemaComponentData(false);
            var gunComponent = GunComponent.Component.CreateSchemaComponentData(PlayerGunSettings.DefaultGunIndex);
            var maxHealth = PlayerHealthSettings.MaxHealth;

            var healthComponent = HealthComponent.Component.CreateSchemaComponentData(maxHealth, maxHealth);
            var healthRegenComponent = HealthRegenComponent.Component.CreateSchemaComponentData(false,
                0,
                PlayerHealthSettings.SpatialCooldownSyncInterval,
                PlayerHealthSettings.RegenAfterDamageCooldown,
                PlayerHealthSettings.RegenInterval,
                PlayerHealthSettings.RegenAmount);

            return EntityBuilder.Begin()
                .AddPosition(spawnPosition.x, spawnPosition.y, spawnPosition.z, gameLogic)
                .AddMetadata("Player", gameLogic)
                .SetPersistence(false)
                .SetReadAcl(AllWorkerAttributes)
                .AddComponent(serverMovement, gameLogic)
                .AddComponent(clientMovement, client)
                .AddComponent(clientRotation, client)
                .AddComponent(shootingComponent, client)
                .AddComponent(gunComponent, gameLogic)
                .AddComponent(gunStateComponent, client)
                .AddComponent(healthComponent, gameLogic)
                .AddComponent(healthRegenComponent, gameLogic)
                .AddPlayerLifecycleComponents(workerId, client, gameLogic)
                .Build();
        }

        public static EntityTemplate SimulatedPlayerCoordinatorTrigger()
        {
            return EntityBuilder.Begin()
                .AddPosition(0, 0, 0, WorkerUtils.SimulatedPlayerCoordinator)
                .AddMetadata("SimulatedPlayerCoordinatorTrigger", WorkerUtils.SimulatedPlayerCoordinator)
                .SetPersistence(true)
                .SetReadAcl(WorkerUtils.SimulatedPlayerCoordinator)
                .Build();
        }
    }
}
