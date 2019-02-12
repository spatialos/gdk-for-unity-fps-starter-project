using System.Threading.Tasks;
using Improbable.Gdk.Core;
using Improbable.Worker.CInterop;
using UnityEngine;

namespace Fps
{
    public abstract class WorkerConnectorBase : DefaultWorkerConnector
    {
        private const string Small = "small";
        private const string Large = "large";

        public int TargetFrameRate = 60;

        protected GameObject levelInstance;

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
            LoadWorld();
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

        // Get the world size from the config, and use it to generate the correct-sized level
        protected virtual void LoadWorld()
        {
            if (GetWorldLayerCount(out var worldLayerCount))
            {
                levelInstance = new GameObject($"FPS-Level_{worldLayerCount}({Worker.WorkerType})");
                levelInstance.transform.position = transform.position;
                levelInstance.transform.rotation = transform.rotation;

                var mapBuilder = new MapBuilder(levelInstance);
                mapBuilder.CleanAndBuild(worldLayerCount);
            }
        }
    }
}
