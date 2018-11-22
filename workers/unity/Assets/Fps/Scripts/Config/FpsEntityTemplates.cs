using System;
using System.Collections.Generic;
using System.Linq;
using Improbable;
using Improbable.Fps.Perf;
using Improbable.Gdk.Core;
using Improbable.Gdk.Guns;
using Improbable.Gdk.Health;
using Improbable.Gdk.Movement;
using Improbable.Gdk.PlayerLifecycle;
using Improbable.Gdk.StandardTypes;
using Improbable.PlayerLifecycle;
using Improbable.Worker.Core;

namespace Fps
{
    public static class FpsEntityTemplates
    {
        private static readonly List<string> AllWorkerAttributes =
            new List<string> { WorkerUtils.UnityGameLogic, WorkerUtils.UnityClient, WorkerUtils.SimulatedPlayer };

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

        public static EntityTemplate SimulatedPlayerCoordinatorTrigger()
        {
            return EntityBuilder.Begin()
                .AddPosition(0, 0, 0, WorkerUtils.SimulatedPlayerCoorindator)
                .AddMetadata("SimulatedPlayerCoordinatorTrigger", WorkerUtils.SimulatedPlayerCoorindator)
                .SetPersistence(true)
                .SetReadAcl(WorkerUtils.SimulatedPlayerCoorindator)
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

            // Extra components for perf testing.
            var component1 = Component1.Component.CreateSchemaComponentData(101);
            var component2 = Component2.Component.CreateSchemaComponentData(102);
            var component3 = Component3.Component.CreateSchemaComponentData(102);
            var component4 = Component4.Component.CreateSchemaComponentData(102);
            var component5 = Component5.Component.CreateSchemaComponentData(102);
            var component6 = Component6.Component.CreateSchemaComponentData(102);
            var component7 = Component7.Component.CreateSchemaComponentData(102);
            var component8 = Component8.Component.CreateSchemaComponentData(102);
            var component9 = Component9.Component.CreateSchemaComponentData(102);
            var component10 = Component10.Component.CreateSchemaComponentData(102);
            var component11 = Component11.Component.CreateSchemaComponentData(102);
            var component12 = Component12.Component.CreateSchemaComponentData(102);
            var component13 = Component13.Component.CreateSchemaComponentData(102);
            var component14 = Component14.Component.CreateSchemaComponentData(102);
            var component15 = Component15.Component.CreateSchemaComponentData(102);
            var component16 = Componen16.Component.CreateSchemaComponentData(102);
            var component17 = Component17.Component.CreateSchemaComponentData(102);
            var component18 = Component18.Component.CreateSchemaComponentData(102);
            var component19 = Component19.Component.CreateSchemaComponentData(102);
            var component20 = Component20.Component.CreateSchemaComponentData(102);

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
                .AddComponent(component1, gameLogic)
                .AddComponent(component2, gameLogic)
                .AddComponent(component3, gameLogic)
                .AddComponent(component4, gameLogic)
                .AddComponent(component5, gameLogic)
                .AddComponent(component6, gameLogic)
                .AddComponent(component7, gameLogic)
                .AddComponent(component8, gameLogic)
                .AddComponent(component9, gameLogic)
                .AddComponent(component10, gameLogic)
                .AddComponent(component11, gameLogic)
                .AddComponent(component12, gameLogic)
                .AddComponent(component13, gameLogic)
                .AddComponent(component14, gameLogic)
                .AddComponent(component15, gameLogic)
                .AddComponent(component16, gameLogic)
                .AddComponent(component17, gameLogic)
                .AddComponent(component18, gameLogic)
                .AddComponent(component19, gameLogic)
                .AddComponent(component20, gameLogic)
                .Build();
        }
    }
}

