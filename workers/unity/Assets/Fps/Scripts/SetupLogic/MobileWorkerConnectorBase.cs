using System;
using System.Collections;
using System.Collections.Generic;
using Improbable.Gdk.Core;
using Improbable.Gdk.GameObjectCreation;
using Improbable.Gdk.PlayerLifecycle;
using Improbable.Worker.CInterop;
using Improbable.Worker.CInterop.Alpha;
using UnityEngine;

namespace Fps
{
    [RequireComponent(typeof(ConnectionController))]
    public abstract class MobileWorkerConnectorBase : WorkerConnectorBase
    {
        private const string AuthPlayer = "Prefabs/MobileClient/Authoritative/Player";
        private const string NonAuthPlayer = "Prefabs/MobileClient/NonAuthoritative/Player";

        public List<TileEnabler> LevelTiles => levelTiles;

        protected string IpAddress;

        [SerializeField] private bool shouldConnectLocally;

        private ConnectionController connectionController;
        private List<TileEnabler> levelTiles = new List<TileEnabler>();

        private void Awake()
        {
            connectionController = GetComponent<ConnectionController>();

            if (!shouldConnectLocally)
            {
                LoadDevAuthToken();
            }
        }

        #region Connection Configuration

        protected abstract string GetHostIp();

        protected override ConnectionParameters GetConnectionParameters(string workerType, ConnectionService service)
        {
            return new ConnectionParameters
            {
                WorkerType = workerType,
                Network =
                {
                    ConnectionType = NetworkConnectionType.Kcp,
                    UseExternalIp = true,
                    Kcp = new KcpNetworkParameters
                    {
                        Heartbeat = new HeartbeatParameters()
                        {
                            IntervalMillis = 5000,
                            TimeoutMillis = 10000
                        }
                    }
                },
                EnableProtocolLoggingAtStartup = false,
                DefaultComponentVtable = new ComponentVtable(),
            };
        }

        protected override ReceptionistConfig GetReceptionistConfig(string workerType)
        {
            return new ReceptionistConfig
            {
                ReceptionistHost = GetHostIp(),
                ReceptionistPort = RuntimeConfigDefaults.ReceptionistPort,
                WorkerId = CreateNewWorkerId(workerType)
            };
        }

        protected override LocatorConfig GetLocatorConfig()
        {
            throw new NotImplementedException("The locator flow is currently not available for mobile workers.");
        }

        protected override AlphaLocatorConfig GetAlphaLocatorConfig(string workerType)
        {
            var pit = GetDevelopmentPlayerIdentityToken(DevelopmentAuthToken, GetPlayerId(), GetDisplayName());
            var loginTokenDetails = GetDevelopmentLoginTokens(workerType, pit);
            var loginToken = SelectLoginToken(loginTokenDetails);

            return new AlphaLocatorConfig
            {
                LocatorHost = RuntimeConfigDefaults.LocatorHost,
                LocatorParameters = new Improbable.Worker.CInterop.Alpha.LocatorParameters
                {
                    PlayerIdentity = new PlayerIdentityCredentials
                    {
                        PlayerIdentityToken = pit,
                        LoginToken = loginToken,
                    },
                    UseInsecureConnection = false,
                }
            };
        }

        protected override ConnectionService GetConnectionService()
        {
            if (shouldConnectLocally)
            {
                return ConnectionService.Receptionist;
            }

            return ConnectionService.AlphaLocator;
        }

        private void LoadDevAuthToken()
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

        #endregion

        protected override void HandleWorkerConnectionEstablished()
        {
            var world = Worker.World;

            PlayerLifecycleHelper.AddClientSystems(world, autoRequestPlayerCreation: false);
            PlayerLifecycleConfig.MaxPlayerCreationRetries = 0;

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

        public override void Dispose()
        {
            if (LevelInstance != null)
            {
                Destroy(LevelInstance);
            }

            base.Dispose();
        }
    }
}
