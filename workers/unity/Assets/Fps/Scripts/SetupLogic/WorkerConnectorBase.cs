using System;
using System.Collections;
using System.Threading.Tasks;
using Improbable.Gdk.Core;
using Improbable.Worker.CInterop;
using Improbable.Worker.CInterop.Alpha;
using UnityEngine;

namespace Fps
{
    public abstract class WorkerConnectorBase : DefaultWorkerConnector
    {
        public int TargetFrameRate = 60;

        [SerializeField] protected MapTemplate mapTemplate;

        [NonSerialized] internal GameObject LevelInstance;

        protected abstract string GetWorkerType();

        protected virtual void Start()
        {
            Application.targetFrameRate = TargetFrameRate;
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
            StartCoroutine(LoadWorld());
        }

        public override void Dispose()
        {
            if (LevelInstance != null)
            {
                Destroy(LevelInstance);
                LevelInstance = null;
            }

            base.Dispose();
        }

        // Get the world size from the config, and use it to generate the correct-sized level
        protected virtual IEnumerator LoadWorld()
        {
            var worldSize = GetWorldSize();
            if (worldSize <= 0)
            {
                yield break;
            }

            yield return MapBuilder.GenerateMap(
                mapTemplate,
                worldSize,
                transform,
                Worker.WorkerType,
                this);
        }

        protected int GetWorldSize()
        {
            var flagValue = Worker.Connection.GetWorkerFlag("world_size");
            if (!int.TryParse(flagValue, out var worldSize))
            {
                Worker.LogDispatcher.HandleLog(LogType.Error,
                    new LogEvent($"Invalid world_size worker flag. Expected an integer, got \"{flagValue}\""));
                return 0;
            }

            return worldSize;
        }
    }
}
