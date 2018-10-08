using System.Collections.Generic;
using UnityEngine;

namespace Fps
{
    [RequireComponent(typeof(ConnectionController))]
    public class ClientWorkerConnector : WorkerConnectorBase
    {
        public List<TileEnabler> LevelTiles = new List<TileEnabler>();

        private ConnectionController connectionController;

        private void Awake()
        {
            connectionController = GetComponent<ConnectionController>();
        }

        protected override async void Start()
        {
            Application.targetFrameRate = 60;
            await AttemptConnect();
        }

        protected override string GetWorkerType()
        {
            return WorkerUtils.UnityClient;
        }

        public async void Reconnect()
        {
            await AttemptConnect();
        }

        protected override void HandleWorkerConnectionEstablished()
        {
            WorkerUtils.AddClientSystems(Worker.World, gameObject);
            base.HandleWorkerConnectionEstablished();
        }

        protected override void HandleWorkerConnectionFailure()
        {
            connectionController.OnFailedToConnect();
        }

        protected override void LoadWorld()
        {
            base.LoadWorld();

            levelInstance.GetComponentsInChildren<TileEnabler>(true, LevelTiles);
            foreach (var tileEnabler in LevelTiles)
            {
                tileEnabler.IsClient = true;
            }
        }
    }
}
