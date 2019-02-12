using System.Collections.Generic;
using System.Threading.Tasks;
using Improbable.Gdk.Core;
using Improbable.Gdk.GameObjectCreation;
using Improbable.Gdk.GameObjectRepresentation;
using Improbable.Gdk.Mobile;
using Improbable.Gdk.PlayerLifecycle;
using Improbable.Worker.CInterop;
using UnityEngine;

namespace Fps
{
    public abstract class MobileWorkerConnectorBase : MobileWorkerConnector
    {
        private const string AuthPlayer = "Prefabs/MobileClient/Authoritative/Player";
        private const string NonAuthPlayer = "Prefabs/MobileClient/NonAuthoritative/Player";

        private const string Small = "small";
        private const string Large = "large";

        public int TargetFrameRate = 60;

        protected GameObject levelInstance;

        protected ConnectionController connectionController;

        private List<TileEnabler> levelTiles = new List<TileEnabler>();
        public List<TileEnabler> LevelTiles => levelTiles;

        public string IpAddress { get; set; }

        protected abstract string GetWorkerType();

        protected virtual async void Start()
        {
            Application.targetFrameRate = TargetFrameRate;
            await AttemptConnect();
        }

        protected async Task AttemptConnect()
        {
            await Connect(GetWorkerType(), new ForwardingDispatcher()).ConfigureAwait(false);
        }

        protected override string SelectDeploymentName(DeploymentList deployments)
        {
            // This could be replaced with a splash screen asking to select a deployment or some other user-defined logic.
            return deployments.Deployments[0].DeploymentName;
        }

        protected override void HandleWorkerConnectionEstablished()
        {
            var world = Worker.World;

            // Only take the Heartbeat from the PlayerLifecycleConfig Client Systems.
            world.GetOrCreateManager<HandlePlayerHeartbeatRequestSystem>();

            GameObjectRepresentationHelper.AddSystems(world);
            var fallback = new GameObjectCreatorFromMetadata(Worker.WorkerType, Worker.Origin, Worker.LogDispatcher);

            // Set the Worker gameObject to the ClientWorker so it can access PlayerCreater reader/writers
            GameObjectCreationHelper.EnableStandardGameObjectCreation(
                world,
                new AdvancedEntityPipeline(Worker, AuthPlayer, NonAuthPlayer, fallback),
                gameObject);

            LoadWorld();
        }

        protected override void HandleWorkerConnectionFailure(string errorMessage)
        {
            connectionController.OnFailedToConnect();
        }

        public override void Dispose()
        {
            if (levelInstance != null)
            {
                Destroy(levelInstance);
            }

            base.Dispose();
        }

        protected bool GetWorldLayerCount(out int worldLayerCount)
        {
            var workerSystem = Worker.World.GetExistingManager<WorkerSystem>();
            var worldSize = Worker.Connection.GetWorkerFlag("world_size");

            switch (worldSize)
            {
                case Small:
                    worldLayerCount = 4;
                    break;
                case Large:
                    worldLayerCount = 24;
                    break;
                default:
                    if (!int.TryParse(worldSize, out worldLayerCount))
                    {
                        workerSystem.LogDispatcher.HandleLog(LogType.Error,
                            new LogEvent(
                                    "Invalid world_size worker flag. Make sure that it is either small or large,")
                                .WithField("world_size", worldSize));
                        return false;
                    }

                    break;
            }

            return true;
        }

        // Get the world size from the config, and use it to load the appropriate level.
        protected virtual void LoadWorld()
        {
            if (!GetWorldLayerCount(out var worldLayerCount))
            {
                return;
            }

            levelInstance = new GameObject($"FPS-Level_{worldLayerCount}({Worker.WorkerType})");
            levelInstance.transform.position = transform.position;
            levelInstance.transform.rotation = transform.rotation;

            var mapBuilder = new MapBuilder(levelInstance);
            mapBuilder.CleanAndBuild(worldLayerCount);

            levelInstance.GetComponentsInChildren<TileEnabler>(true, levelTiles);
            foreach (var tileEnabler in levelTiles)
            {
                tileEnabler.IsClient = true;
            }
        }
    }
}
