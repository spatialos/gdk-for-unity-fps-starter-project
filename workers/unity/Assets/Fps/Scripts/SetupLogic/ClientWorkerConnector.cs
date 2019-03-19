using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Improbable.Gdk.GameObjectCreation;
using Improbable.Gdk.Subscriptions;
using Improbable.Gdk.PlayerLifecycle;

namespace Fps
{
    [RequireComponent(typeof(ConnectionController))]
    public class ClientWorkerConnector : WorkerConnectorBase, ITileProvider
    {
        private const string AuthPlayer = "Prefabs/UnityClient/Authoritative/Player";
        private const string NonAuthPlayer = "Prefabs/UnityClient/NonAuthoritative/Player";

        private List<TileEnabler> levelTiles = new List<TileEnabler>();
        public List<TileEnabler> LevelTiles => levelTiles;

        private ConnectionController connectionController;

        private void Awake()
        {
            connectionController = GetComponent<ConnectionController>();
        }

        protected override string GetWorkerType()
        {
            return WorkerUtils.UnityClient;
        }

        protected override void HandleWorkerConnectionEstablished()
        {
            var world = Worker.World;

            // Only take the Heartbeat from the PlayerLifecycleConfig Client Systems.
            world.GetOrCreateManager<HandlePlayerHeartbeatRequestSystem>();

            var fallback = new GameObjectCreatorFromMetadata(Worker.WorkerType, Worker.Origin, Worker.LogDispatcher);

            // Set the Worker gameObject to the ClientWorker so it can access PlayerCreater reader/writers
            GameObjectCreationHelper.EnableStandardGameObjectCreation(
                world,
                new AdvancedEntityPipeline(Worker, AuthPlayer, NonAuthPlayer, fallback),
                gameObject);

            base.HandleWorkerConnectionEstablished();
        }

        protected override void HandleWorkerConnectionFailure(string errorMessage)
        {
            connectionController.OnFailedToConnect();
        }

        protected override IEnumerator LoadWorld()
        {
            yield return base.LoadWorld();

            LevelInstance.GetComponentsInChildren<TileEnabler>(true, levelTiles);
            foreach (var tileEnabler in levelTiles)
            {
                tileEnabler.Initialize(true);
            }

            connectionController.OnReadyToSpawn();
        }
    }
}
