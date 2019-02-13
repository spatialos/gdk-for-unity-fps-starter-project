using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Improbable.Gdk.Core;
using Improbable.Gdk.GameObjectCreation;
using Improbable.Gdk.GameObjectRepresentation;
using Improbable.Gdk.Mobile;
using Improbable.Gdk.PlayerLifecycle;
using Improbable.Worker.CInterop;
using UnityEngine;

#if UNITY_IOS
using Improbable.Gdk.Mobile.iOS;
#endif

namespace Fps
{
    [RequireComponent(typeof(ConnectionController))]
    public class iOSWorkerConnector : MobileWorkerConnector, ITileProvider
    {
        private const string AuthPlayer = "Prefabs/MobileClient/Authoritative/Player";
        private const string NonAuthPlayer = "Prefabs/MobileClient/NonAuthoritative/Player";

        public string forcedIpAddress;

        public int TargetFrameRate = 60;

        [SerializeField] private MapBuilderSettings MapBuilderSettings;

        private GameObject levelInstance;

        private List<TileEnabler> levelTiles = new List<TileEnabler>();
        public List<TileEnabler> LevelTiles => levelTiles;

        private ConnectionController connectionController;

        public string IpAddress { get; set; }

        private void OnValidate()
        {
            forcedIpAddress = Regex.Replace(forcedIpAddress, "[^0-9.]", "");
        }

        public async void TryConnect()
        {
            await Connect(WorkerUtils.iOSClient, new ForwardingDispatcher()).ConfigureAwait(false);
        }

        private void Awake()
        {
            connectionController = GetComponent<ConnectionController>();
        }

        protected virtual async void Start()
        {
            Application.targetFrameRate = TargetFrameRate;
            IpAddress = forcedIpAddress;

            await AttemptConnect();
        }

        protected override string GetHostIp()
        {
#if UNITY_IOS
            if (!string.IsNullOrEmpty(IpAddress))
            {
                return IpAddress;
            }

            return RuntimeConfigDefaults.ReceptionistHost;
#else
            throw new System.PlatformNotSupportedException(
                $"{nameof(iOSWorkerConnector)} can only be used for the iOS platform. Please check your build settings.");
#endif
        }

        public async void Reconnect()
        {
            await AttemptConnect();
        }

        protected async Task AttemptConnect()
        {
            await Connect(WorkerUtils.iOSClient, new ForwardingDispatcher()).ConfigureAwait(false);
        }

        protected override string SelectDeploymentName(DeploymentList deployments)
        {
            // This could be replaced with a splash screen asking to select a deployment or some other user-defined logic.
            return deployments.Deployments[0].DeploymentName;
        }

        protected override ConnectionService GetConnectionService()
        {
            // TODO UTY-1590: add cloud deployment option
            return ConnectionService.Receptionist;
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

        // Get the world size from the config, and use it to load the appropriate level.
        protected virtual void LoadWorld()
        {
            levelInstance = MapBuilder.GenerateMap(
                MapBuilderSettings,
                transform,
                Worker.Connection,
                Worker.WorkerType,
                Worker.LogDispatcher);

            levelInstance.GetComponentsInChildren<TileEnabler>(true, levelTiles);
            foreach (var tileEnabler in levelTiles)
            {
                tileEnabler.IsClient = true;
            }
        }
    }
}
