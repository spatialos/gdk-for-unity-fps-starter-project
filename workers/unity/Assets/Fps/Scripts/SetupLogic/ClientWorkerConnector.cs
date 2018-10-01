using System.Collections.Generic;
using UnityEngine;
using Improbable.Gdk.GameObjectCreation;
using Improbable.Gdk.GameObjectRepresentation;
using Improbable.Gdk.PlayerLifecycle;

namespace Fps
{
    [RequireComponent(typeof(ConnectionController))]
    public class ClientWorkerConnector : WorkerConnectorBase
    {
        private const string AuthPlayer = "Prefabs/UnityClient/Authoritative/Player";
        private const string NonAuthPlayer = "Prefabs/UnityClient/NonAuthoritative/Player";

        private ConnectionController connectScreen;

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
