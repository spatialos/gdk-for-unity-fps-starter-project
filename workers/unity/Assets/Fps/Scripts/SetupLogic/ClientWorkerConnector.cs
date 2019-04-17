using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Improbable.Gdk.Core;
using UnityEngine;
using Improbable.Gdk.GameObjectCreation;
using Improbable.Gdk.PlayerLifecycle;
using Improbable.PlayerLifecycle;
using Improbable.Worker.CInterop.Alpha;

namespace Fps
{
    public class ClientWorkerConnector : WorkerConnectorBase
    {
        public string Deployment { get; private set; }

        private string playerName;
        private bool isReadyToSpawn;
        private bool wantsSpawn;
        private bool useSessionFlow;
        private Action<PlayerCreator.CreatePlayer.ReceivedResponse> onPlayerResponse;

        public async void Connect(string deployment, bool useSessionFlow)
        {
            this.useSessionFlow = useSessionFlow;
            Deployment = deployment;
            await AttemptConnect();
        }

        public void SpawnPlayer(string playerName, Action<PlayerCreator.CreatePlayer.ReceivedResponse> onPlayerResponse)
        {
            this.onPlayerResponse = onPlayerResponse;
            this.playerName = playerName;
            wantsSpawn = true;
        }

        protected override string GetWorkerType()
        {
            return WorkerUtils.UnityClient;
        }

        protected virtual string GetAuthPlayerPrefabPath()
        {
            return "Prefabs/UnityClient/Authoritative/Player";
        }

        protected virtual string GetNonAuthPlayerPrefabPath()
        {
            return "Prefabs/UnityClient/NonAuthoritative/Player";
        }

        protected override string SelectLoginToken(List<LoginTokenDetails> loginTokens)
        {
            if (string.IsNullOrEmpty(Deployment))
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
                if (loginToken.DeploymentName == Deployment)
                {
                    return loginToken.LoginToken;
                }
            }

            throw new ArgumentException("Was not able to connect to chosen deployment. Going back to beginning");
        }

        protected override ConnectionService GetConnectionService()
        {
            if (useSessionFlow)
            {
                LoadDevAuthToken();
                return ConnectionService.AlphaLocator;
            }

            return base.GetConnectionService();
        }

        protected override void HandleWorkerConnectionEstablished()
        {
            var world = Worker.World;

            PlayerLifecycleHelper.AddClientSystems(world, false);
            PlayerLifecycleConfig.MaxPlayerCreationRetries = 0;

            var fallback = new GameObjectCreatorFromMetadata(Worker.WorkerType, Worker.Origin, Worker.LogDispatcher);

            // Set the Worker gameObject to the ClientWorker so it can access PlayerCreater reader/writers
            GameObjectCreationHelper.EnableStandardGameObjectCreation(
                world,
                new AdvancedEntityPipeline(Worker, GetAuthPlayerPrefabPath(), GetNonAuthPlayerPrefabPath(), fallback),
                gameObject);
            world.GetOrCreateManager<TrackPlayerSystem>();

            base.HandleWorkerConnectionEstablished();
        }

        protected override void HandleWorkerConnectionFailure(string errorMessage)
        {
            Debug.LogWarning($"Connection failed: {errorMessage}");
            Destroy(gameObject);
        }

        protected override IEnumerator LoadWorld()
        {
            yield return base.LoadWorld();
            isReadyToSpawn = true;
        }

        private void Update()
        {
            if (wantsSpawn && isReadyToSpawn)
            {
                wantsSpawn = false;
                SendRequest();
            }
        }

        private void SendRequest()
        {
            var serializedArgs = Encoding.ASCII.GetBytes(playerName);
            Worker.World.GetExistingManager<SendCreatePlayerRequestSystem>()
                .RequestPlayerCreation(serializedArgs, onPlayerResponse);
        }
    }
}
