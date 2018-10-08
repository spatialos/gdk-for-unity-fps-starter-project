using UnityEngine;

namespace Fps
{
    public class GameLogicWorkerConnector : WorkerConnectorBase
    {
        public bool DisableRenderers = true;

        protected override async void Start()
        {
            Application.targetFrameRate = 60;
            await AttemptConnect();
        }

        protected override string GetWorkerType()
        {
            return WorkerUtils.UnityGameLogic;
        }

        protected override void HandleWorkerConnectionEstablished()
        {
            WorkerUtils.AddGameLogicSystems(Worker.World);
            base.HandleWorkerConnectionEstablished();
        }

        protected override void LoadWorld()
        {
            base.LoadWorld();

            if (DisableRenderers)
            {
                foreach (var renderer in levelInstance.GetComponentsInChildren<Renderer>())
                {
                    renderer.enabled = false;
                }
            }
        }
    }
}
