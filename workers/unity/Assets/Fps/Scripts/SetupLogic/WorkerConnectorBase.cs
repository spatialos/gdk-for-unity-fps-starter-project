using System.Threading.Tasks;
using Improbable.Gdk.Core;
using Improbable.Worker.Core;
using UnityEngine;

namespace Fps
{
    public abstract class WorkerConnectorBase : DefaultWorkerConnector
    {
        private const string Small = "small";
        private const string Large = "large";

        public int TargetFrameRate = 60;

        public GameObject SmallLevelPrefab;
        public GameObject LargeLevelPrefab;

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

        // Get the world size from the config, and use it to load the appropriate level.
        protected virtual void LoadWorld()
        {
            var workerSystem = Worker.World.GetExistingManager<WorkerSystem>();
            var worldSize = workerSystem.Connection.GetWorkerFlag("world_size");

            if (worldSize != Small && worldSize != Large)
            {
                workerSystem.LogDispatcher.HandleLog(LogType.Error,
                    new LogEvent(
                            "Invalid world_size worker flag. Make sure that it is either small, medium, or large,")
                        .WithField("world_size", worldSize));
                return;
            }

            var levelToLoad = worldSize == Large ? LargeLevelPrefab : SmallLevelPrefab;

            if (levelToLoad == null)
            {
                Debug.LogError("The level to be instantiated is null.");
                return;
            }

            levelInstance = Instantiate(levelToLoad, transform.position, transform.rotation);
            levelInstance.name = $"{levelToLoad.name}({Worker.WorkerType})";
        }
    }
}
