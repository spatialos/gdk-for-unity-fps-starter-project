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

        private const string Small = "small";
        private const string Large = "large";

        public string forcedIpAddress;

        public int TargetFrameRate = 60;

        public GameObject SmallLevelPrefab;
        public GameObject LargeLevelPrefab;

        private GameObject levelInstance;

        private ConnectionController connectScreen;

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

            if (Application.isMobilePlatform && iOSDeviceInfo.ActiveDeviceType == MobileDeviceType.Virtual)
            {
                // TODO What should this return? No "EmulatorDefaultCallbackIp" in iOSDeviceInfo
                return "127.0.0.1";
            }

            return RuntimeConfigDefaults.ReceptionistHost;
#else
            throw new System.PlatformNotSupportedException(
                $"{nameof(iOSWorkerConnector)} can only be used for the iOS platform. Please check your build settings.");
#endif
        }

        protected virtual string GetWorkerType()
        {
            return WorkerUtils.iOSClient;
        }

        public async void Reconnect()
        {
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

            levelInstance.GetComponentsInChildren<TileEnabler>(true, levelTiles);
            foreach (var tileEnabler in levelTiles)
            {
                tileEnabler.IsClient = true;
            }
        }
    }
}
