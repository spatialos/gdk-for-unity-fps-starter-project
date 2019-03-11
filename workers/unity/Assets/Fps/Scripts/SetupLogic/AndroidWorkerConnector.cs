using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Improbable.Gdk.Core;
using Improbable.Gdk.GameObjectCreation;
using Improbable.Gdk.Subscriptions;
using Improbable.Gdk.Mobile;
using Improbable.Gdk.PlayerLifecycle;
using Improbable.Worker.CInterop;
using Improbable.Worker.CInterop.Alpha;
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

        public GameObject LevelInstance;

        private List<TileEnabler> levelTiles = new List<TileEnabler>();
        public List<TileEnabler> LevelTiles => levelTiles;

        private ConnectionController connectionController;

        public string IpAddress { get; set; }

        private string chosenDeployment;

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

            ConnectionStateReporter.OnConnectionStateChange += OnConnectionStateChange;
        }

        private async void OnConnectionStateChange(ConnectionStateReporter.State state, string information)
        {
            if (state == ConnectionStateReporter.State.Connecting)
            {
                if (ClientWorkerHandler.IsInSessionBasedGame)
                {
                    chosenDeployment = information;
                }

                await AttemptConnect();

                if (!ClientWorkerHandler.IsInSessionBasedGame)
                {
                    GetComponent<ConnectionController>().SpawnPlayerAction("Local player");
                }
            }
            else if (state == ConnectionStateReporter.State.ShowResults)
            {
                ConnectionStateReporter.SetState(ConnectionStateReporter.State.None);
                Destroy(gameObject);
            }
        }


        protected virtual async void Start()
        {
            Application.targetFrameRate = TargetFrameRate;
#if UNITY_ANDROID && !UNITY_EDITOR
            UseIpAddressFromArguments();
#endif
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

        protected override string SelectLoginToken(List<LoginTokenDetails> loginTokens)
        {
            if (string.IsNullOrEmpty(chosenDeployment))
            {
                foreach (var loginToken in loginTokens)
                {
                    if (loginToken.Tags.Contains("state_lobby"))
                    {
                        return loginToken.LoginToken;
                    }
                }
            }

            foreach (var loginToken in loginTokens)
            {
                if (loginToken.DeploymentName == chosenDeployment)
                {
                    return loginToken.LoginToken;
                }
            }

            throw new ArgumentException("Was not able to connect to chosen deployment. Going back to beginning");
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

            var fallback = new GameObjectCreatorFromMetadata(Worker.WorkerType, Worker.Origin, Worker.LogDispatcher);

            // Set the Worker gameObject to the ClientWorker so it can access PlayerCreater reader/writers
            GameObjectCreationHelper.EnableStandardGameObjectCreation(
                world,
                new AdvancedEntityPipeline(Worker, AuthPlayer, NonAuthPlayer, fallback),
                gameObject);

            StartCoroutine(LoadWorld());
        }

        protected override void HandleWorkerConnectionFailure(string errorMessage)
        {
            connectionController.OnFailedToConnect(errorMessage);
        }

        public override void Dispose()
        {
            if (LevelInstance != null)
            {
                Destroy(LevelInstance);
            }

            base.Dispose();
        }

        // Get the world size from the config, and use it to load the appropriate level.
        protected virtual IEnumerator LoadWorld()
        {
            yield return MapBuilder.GenerateMap(
                MapBuilderSettings,
                transform,
                Worker.Connection,
                Worker.WorkerType,
                Worker.LogDispatcher,
                this);

            LevelInstance.GetComponentsInChildren<TileEnabler>(true, levelTiles);
            foreach (var tileEnabler in levelTiles)
            {
                tileEnabler.IsClient = true;
            }
        }
    }
}
