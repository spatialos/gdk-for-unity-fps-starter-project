using System.Threading.Tasks;
using Improbable.Gdk.Core;
using Improbable.Worker.CInterop;
using UnityEngine;

namespace Fps
{
    public abstract class WorkerConnectorBase : DefaultWorkerConnector
    {
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
                levelInstance = null;
            }

            base.Dispose();
        }

        // Get the world size from the config, and use it to generate the correct-sized level
        protected virtual void LoadWorld()
        {
            levelInstance = MapBuilderUtils.GenerateMap(
                transform,
                Worker.Connection,
                Worker.WorkerType,
                Worker.World.GetExistingManager<WorkerSystem>());
        }
    }
}
