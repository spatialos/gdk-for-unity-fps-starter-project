using Improbable.Gdk.Core;
using Improbable.Gdk.GameObjectCreation;
using Improbable.Gdk.GameObjectRepresentation;
using Improbable.Gdk.Guns;
using Improbable.Gdk.Health;
using Improbable.Gdk.PlayerLifecycle;
using Unity.Entities;
using UnityEngine;

namespace Fps
{
    public static class WorkerUtils
    {
        public const string UnityClient = "UnityClient";
        public const string UnityGameLogic = "UnityGameLogic";
        public const string AuthPlayer = "Prefabs/UnityClient/Authoritative/Player";
        public const string NonAuthPlayer = "Prefabs/UnityClient/NonAuthoritative/Player";

        public static void AddClientSystems(World world, GameObject workerGameObject = null)
        {
            // Only take the Heartbeat from the PlayerLifecycleConfig Client Systems.
            world.GetOrCreateManager<HandlePlayerHeartbeatRequestSystem>();

            GameObjectRepresentationHelper.AddSystems(world);
            var workerSystem = world.GetExistingManager<WorkerSystem>();
            var fallback = new GameObjectCreatorFromMetadata(workerSystem.WorkerType,
                workerSystem.Origin, workerSystem.LogDispatcher);
            var workerId = workerSystem.Connection.GetWorkerId();

            // Set the Worker gameObject to the ClientWorker so it can access PlayerCreater reader/writers
            GameObjectCreationHelper.EnableStandardGameObjectCreation(
                world,
                new AdvancedEntityPipeline(workerSystem.WorkerType, workerId, AuthPlayer, NonAuthPlayer, fallback),
                workerGameObject);
        }

        public static void AddGameLogicSystems(World world)
        {
            PlayerLifecycleHelper.AddServerSystems(world);
            GameObjectRepresentationHelper.AddSystems(world);
            GameObjectCreationHelper.EnableStandardGameObjectCreation(world);

            // Shooting
            world.GetOrCreateManager<ServerShootingSystem>();

            // Health
            world.GetOrCreateManager<ServerHealthModifierSystem>();
            world.GetOrCreateManager<HealthRegenSystem>();
        }
    }
}
