using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Improbable.Gdk.Core;
using UnityEngine;
using Improbable.Gdk.GameObjectCreation;
using Improbable.Gdk.PlayerLifecycle;
using Improbable.Worker.CInterop;
using Improbable.Worker.CInterop.Alpha;
using Unity.Entities;

namespace Fps
{
    public class ClientWorkerConnector : WorkerConnectorBase
    {
        private string deployment;
        private string playerName;
        private bool isReadyToSpawn;
        private bool wantsSpawn;
        private Action<PlayerCreator.CreatePlayer.ReceivedResponse> onPlayerResponse;
        private AdvancedEntityPipeline entityPipeline;

        protected bool UseSessionFlow => !string.IsNullOrEmpty(deployment);

        public event Action OnLostPlayerEntity;

        public async void Connect(string deployment = "")
        {
            this.deployment = deployment.Trim();
            await AttemptConnect();
        }

        public void SpawnPlayer(string playerName, Action<PlayerCreator.CreatePlayer.ReceivedResponse> onPlayerResponse)
        {
            this.onPlayerResponse = onPlayerResponse;
            this.playerName = playerName;
            wantsSpawn = true;
        }

        public bool HasConnected()
        {
            return Worker != null;
        }

        public void DisconnectPlayer()
        {
            StartCoroutine(PrepareDestroy());
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

        protected override AlphaLocatorConfig GetAlphaLocatorConfig(string workerType)
        {
            return UseSessionFlow
                ? GetAlphaLocatorConfigViaDevAuthFlow(workerType)
                : base.GetAlphaLocatorConfig(workerType);
        }

        protected override string SelectLoginToken(List<LoginTokenDetails> loginTokens)
        {
            if (UseSessionFlow)
            {
                foreach (var loginToken in loginTokens)
                {
                    if (loginToken.DeploymentName == deployment)
                    {
                        return loginToken.LoginToken;
                    }
                }
            }
            else
            {
                return base.SelectLoginToken(loginTokens);
            }

            throw new ArgumentException("Was not able to connect to deployment.");
        }

        protected override ConnectionService GetConnectionService()
        {
            return UseSessionFlow ? ConnectionService.AlphaLocator : base.GetConnectionService();
        }

        protected override void HandleWorkerConnectionEstablished()
        {
            var world = Worker.World;

            PlayerLifecycleHelper.AddClientSystems(world, autoRequestPlayerCreation: false);
            PlayerLifecycleConfig.MaxPlayerCreationRetries = 0;

            var fallback = new GameObjectCreatorFromMetadata(Worker.WorkerType, Worker.Origin, Worker.LogDispatcher);

            entityPipeline = new AdvancedEntityPipeline(Worker, GetAuthPlayerPrefabPath(),
                GetNonAuthPlayerPrefabPath(), fallback);
            entityPipeline.OnRemovedAuthoritativePlayer += RemovingAuthoritativePlayer;

            // Set the Worker gameObject to the ClientWorker so it can access PlayerCreater reader/writers
            GameObjectCreationHelper.EnableStandardGameObjectCreation(world, entityPipeline, gameObject);

            if (UseSessionFlow)
            {
                world.GetOrCreateSystem<TrackPlayerSystem>();
            }

            base.HandleWorkerConnectionEstablished();
        }

        private void RemovingAuthoritativePlayer()
        {
            Debug.LogError($"Player entity got removed while still being connected. Disconnecting...");
            OnLostPlayerEntity?.Invoke();
        }

        protected override void HandleWorkerConnectionFailure(string errorMessage)
        {
            Debug.LogError($"Connection failed: {errorMessage}");
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

        public override void Dispose()
        {
            if (entityPipeline != null)
            {
                entityPipeline.OnRemovedAuthoritativePlayer -= RemovingAuthoritativePlayer;
            }

            base.Dispose();
        }

        private IEnumerator PrepareDestroy()
        {
            yield return DeferredDisposeWorker();
            Destroy(gameObject);
        }

        private void SendRequest()
        {
            var serializedArgs = Encoding.ASCII.GetBytes(playerName);
            Worker.World.GetExistingSystem<SendCreatePlayerRequestSystem>()
                .RequestPlayerCreation(serializedArgs, onPlayerResponse);
        }
    }
}
