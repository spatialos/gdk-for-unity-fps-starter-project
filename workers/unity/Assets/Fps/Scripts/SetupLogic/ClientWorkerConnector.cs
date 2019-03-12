using System;
using System.Collections;
using System.Collections.Generic;
using Improbable.Gdk.Core;
using UnityEngine;
using Improbable.Gdk.GameObjectCreation;
using Improbable.Gdk.Subscriptions;
using Improbable.Gdk.PlayerLifecycle;
using Improbable.Worker.CInterop.Alpha;
using Unity.Entities;
using Random = System.Random;

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
        private string chosenDeployment;

        private void Awake()
        {
            connectionController = GetComponent<ConnectionController>();
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

        protected override void Start()
        {
            Application.targetFrameRate = 60;
        }

        protected override string GetWorkerType()
        {
            return WorkerUtils.UnityClient;
        }

        protected override string SelectLoginToken(List<LoginTokenDetails> loginTokens)
        {
            if (string.IsNullOrEmpty(chosenDeployment))
            {
                foreach (var loginToken in loginTokens)
                {
                    if (loginToken.Tags.Contains("state_lobby") || loginToken.Tags.Contains("state_running"))
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

        protected override AlphaLocatorConfig GetAlphaLocatorConfig(string workerType)
        {
            var pit = GetDevelopmentPlayerIdentityToken(DevelopmentAuthToken, GetPlayerId(), GetDisplayName());
            var loginTokenDetails = GetDevelopmentLoginTokens(workerType, pit);
            var loginToken = SelectLoginToken(loginTokenDetails);

            return new AlphaLocatorConfig
            {
                LocatorHost = RuntimeConfigDefaults.LocatorHost,
                LocatorParameters = new LocatorParameters
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
            if (ClientWorkerHandler.IsInSessionBasedGame)
            {
                var textAsset = Resources.Load<TextAsset>("DevAuthToken");
                if (textAsset != null)
                {
                    DevelopmentAuthToken = textAsset.text.Trim();
                }

                return ConnectionService.AlphaLocator;
            }

            return base.GetConnectionService();
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

            if (ClientWorkerHandler.IsInSessionBasedGame)
            {
                world.GetOrCreateManager<TrackPlayerSystem>();
            }

            base.HandleWorkerConnectionEstablished();
        }

        protected override void HandleWorkerConnectionFailure(string errorMessage)
        {
            connectionController.OnFailedToConnect(errorMessage);
        }

        protected override IEnumerator LoadWorld()
        {
            yield return base.LoadWorld();

            LevelInstance.GetComponentsInChildren(true, levelTiles);
            foreach (var tileEnabler in levelTiles)
            {
                tileEnabler.IsClient = true;
            }

            connectionController.OnReadyToSpawn();
        }

        public override void Dispose()
        {
            ConnectionStateReporter.OnConnectionStateChange -= OnConnectionStateChange;
            base.Dispose();
        }
    }
}
