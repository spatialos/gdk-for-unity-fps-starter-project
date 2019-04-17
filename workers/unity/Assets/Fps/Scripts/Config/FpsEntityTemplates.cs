using System.Collections.Generic;
using System.Text;
using Improbable;
using Improbable.Gdk.Core;
using Improbable.Gdk.Guns;
using Improbable.Gdk.Health;
using Improbable.Gdk.Movement;
using Improbable.Gdk.PlayerLifecycle;
using Improbable.Gdk.QueryBasedInterest;
using Improbable.Gdk.Session;
using Improbable.Gdk.StandardTypes;
using Improbable.PlayerLifecycle;
using UnityEngine;

namespace Fps
{
    public static class FpsEntityTemplates
    {
        public static EntityTemplate DeploymentState()
        {
            const uint sessionTimeSeconds = 300;

            var position = new Position.Snapshot { Coords = new Vector3().ToSpatialCoordinates() };
            var metadata = new Metadata.Snapshot { EntityType = "DeploymentState" };

            var template = new EntityTemplate();
            template.AddComponent(position, WorkerUtils.UnityGameLogic);
            template.AddComponent(metadata, WorkerUtils.UnityGameLogic);
            template.AddComponent(new Persistence.Snapshot(), WorkerUtils.UnityGameLogic);
            template.AddComponent(new Session.Snapshot { Status = Status.LOBBY }, WorkerUtils.UnityGameLogic);
            template.AddComponent(new Deployment.Snapshot(), WorkerUtils.DeploymentManager);
            template.AddComponent(new Timer.Snapshot { CurrentTimeSeconds = 0, MaxTimeSeconds = sessionTimeSeconds }, WorkerUtils.UnityGameLogic);

            template.SetReadAccess(WorkerUtils.UnityGameLogic, WorkerUtils.DeploymentManager, WorkerUtils.UnityClient, WorkerUtils.AndroidClient, WorkerUtils.iOSClient);
            template.SetComponentWriteAccess(EntityAcl.ComponentId, WorkerUtils.UnityGameLogic);

            return template;
        }

        public static EntityTemplate Spawner(Coordinates spawnerCoordinates)
        {
            var position = new Position.Snapshot(spawnerCoordinates);
            var metadata = new Metadata.Snapshot("PlayerCreator");

            var template = new EntityTemplate();
            template.AddComponent(position, WorkerUtils.UnityGameLogic);
            template.AddComponent(metadata, WorkerUtils.UnityGameLogic);
            template.AddComponent(new Persistence.Snapshot(), WorkerUtils.UnityGameLogic);
            template.AddComponent(new PlayerCreator.Snapshot(), WorkerUtils.UnityGameLogic);

            template.SetReadAccess(WorkerUtils.UnityGameLogic);
            template.SetComponentWriteAccess(EntityAcl.ComponentId, WorkerUtils.UnityGameLogic);

            return template;
        }

        public static EntityTemplate SimulatedPlayerCoordinatorTrigger()
        {
            var metadata = new Metadata.Snapshot { EntityType = "SimulatedPlayerCoordinatorTrigger" };

            var template = new EntityTemplate();
            template.AddComponent(new Position.Snapshot(), WorkerUtils.SimulatedPlayerCoordinator);
            template.AddComponent(metadata, WorkerUtils.SimulatedPlayerCoordinator);
            template.AddComponent(new Persistence.Snapshot(), WorkerUtils.SimulatedPlayerCoordinator);

            template.SetReadAccess(WorkerUtils.SimulatedPlayerCoordinator);
            template.SetComponentWriteAccess(EntityAcl.ComponentId, WorkerUtils.SimulatedPlayerCoordinator);

            return template;
        }

        public static EntityTemplate Player(string workerId, byte[] args)
        {
            var client = EntityTemplate.GetWorkerAccessAttribute(workerId);

            var (spawnPosition, spawnYaw, spawnPitch) = SpawnPoints.GetRandomSpawnPoint();

            var serverResponse = new ServerResponse
            {
                Position = spawnPosition.ToIntAbsolute()
            };

            var rotationUpdate = new RotationUpdate
            {
                Yaw = spawnYaw.ToInt1k(),
                Pitch = spawnPitch.ToInt1k()
            };

            var pos = new Position.Snapshot { Coords = spawnPosition.ToSpatialCoordinates() };
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

            var playerInterest = new ComponentInterest
            {
                Queries = new List<ComponentInterest.Query>
                {
                    new ComponentInterest.Query
                    {
                        Constraint = new ComponentInterest.QueryConstraint
                        {
                            ComponentConstraint = PlayerState.ComponentId,
                            AndConstraint = new List<ComponentInterest.QueryConstraint>(),
                            OrConstraint = new List<ComponentInterest.QueryConstraint>(),
                        },
                        FullSnapshotResult = true,
                        ResultComponentId = new List<uint>(),
                    },
                    new ComponentInterest.Query
                    {
                        Constraint = new ComponentInterest.QueryConstraint
                        {
                            ComponentConstraint = Session.ComponentId,
                            AndConstraint = new List<ComponentInterest.QueryConstraint>(),
                            OrConstraint = new List<ComponentInterest.QueryConstraint>(),
                        },
                        FullSnapshotResult = true,
                        ResultComponentId = new List<uint>(),
                    }
                }
            };

            var playerStateQuery = InterestQuery.Query(Constraint.Component<PlayerState.Component>());
            var sessionQuery = InterestQuery.Query(Constraint.Component<Session.Component>());

            var interestTemplate = InterestTemplate.Create()
                .AddQueries<ClientMovement.Component>(playerStateQuery, sessionQuery);

            var interestComponent = interestTemplate.ToSnapshot();

            var playerName = Encoding.ASCII.GetString(args);

            var playerStateComponent = new PlayerState.Snapshot
            {
                Name = playerName,
                Kills = 0,
                Deaths = 0,
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
            template.AddComponent(playerStateComponent, WorkerUtils.UnityGameLogic);
            template.AddComponent(interestComponent, WorkerUtils.UnityGameLogic);
            PlayerLifecycleHelper.AddPlayerLifecycleComponents(template, workerId, WorkerUtils.UnityGameLogic);

            template.SetReadAccess(WorkerUtils.UnityClient, WorkerUtils.UnityGameLogic, WorkerUtils.AndroidClient, WorkerUtils.iOSClient);
            template.SetComponentWriteAccess(EntityAcl.ComponentId, WorkerUtils.UnityGameLogic);

            return template;
        }
    }
}
