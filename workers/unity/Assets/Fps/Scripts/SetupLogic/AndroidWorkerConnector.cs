using System.Collections.Generic;
using System.Threading.Tasks;
using Improbable.Gdk.Core;
using Improbable.Gdk.GameObjectCreation;
using Improbable.Gdk.GameObjectRepresentation;
using Improbable.Gdk.Mobile;
using Improbable.Gdk.PlayerLifecycle;
using Improbable.Worker.CInterop;
using UnityEngine;
#if UNITY_ANDROID
using Improbable.Gdk.Mobile.Android;
#endif

namespace Fps
{
    [RequireComponent(typeof(ConnectionController))]
    public class AndroidWorkerConnector : MobileWorkerConnector, ITileProvider
    {
        private const string AuthPlayer = "Prefabs/MobileClient/Authoritative/Player";
        private const string NonAuthPlayer = "Prefabs/MobileClient/NonAuthoritative/Player";

        private const string Small = "small";
        private const string Large = "large";

        public bool ShouldConnectLocally;
        public int TargetFrameRate = 60;

        [SerializeField] private MapBuilderSettings MapBuilderSettings;

        private GameObject levelInstance;

        private List<TileEnabler> levelTiles = new List<TileEnabler>();
        public List<TileEnabler> LevelTiles => levelTiles;

        private ConnectionController connectionController;

        public string IpAddress { get; set; }

        private void Awake()
        {
            connectionController = GetComponent<ConnectionController>();

            if (!ShouldConnectLocally)
            {
                var textAsset = Resources.Load<TextAsset>("DevAuthToken");
                if (textAsset != null)
                {
                    DevelopmentAuthToken = textAsset.text.Trim();
                }
                else
                {
                    Debug.LogWarning("Unable to find DevAuthToken.txt in the Resources folder.");
                }
            }
        }

        protected virtual async void Start()
        {
            Application.targetFrameRate = TargetFrameRate;
#if UNITY_ANDROID && !UNITY_EDITOR
            UseIpAddressFromArguments();
#endif
            await AttemptConnect();
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        public void UseIpAddressFromArguments()
        {
            IpAddress = GetReceptionistHostFromArguments();

            if (string.IsNullOrEmpty(IpAddress))
            {
                IpAddress = "127.0.0.1";
            }
        }

        private string GetReceptionistHostFromArguments()
        {
            var arguments = LaunchArguments.GetArguments();
            var hostIp =
                CommandLineUtility.GetCommandLineValue(arguments, RuntimeConfigNames.ReceptionistHost, string.Empty);
            return hostIp;
        }
#endif

        protected override string GetHostIp()
        {
#if UNITY_ANDROID
            if (!string.IsNullOrEmpty(IpAddress))
            {
                return IpAddress;
            }

            if (Application.isMobilePlatform && AndroidDeviceInfo.ActiveDeviceType == MobileDeviceType.Virtual)
            {
                return AndroidDeviceInfo.EmulatorDefaultCallbackIp;
            }

            return RuntimeConfigDefaults.ReceptionistHost;
#else
            throw new System.PlatformNotSupportedException(
                $"{nameof(AndroidWorkerConnector)} can only be used for the Android platform. Please check your build settings.");
#endif
        }

        private async Task AttemptConnect()
        {
            await Connect(WorkerUtils.AndroidClient, new ForwardingDispatcher()).ConfigureAwait(false);
        }

        protected override string SelectDeploymentName(DeploymentList deployments)
        {
            // This could be replaced with a splash screen asking to select a deployment or some other user-defined logic.
            return deployments.Deployments[0].DeploymentName;
        }

        protected override ConnectionService GetConnectionService()
        {
            if (ShouldConnectLocally)
            {
                return ConnectionService.Receptionist;
            }

            return ConnectionService.AlphaLocator;
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
