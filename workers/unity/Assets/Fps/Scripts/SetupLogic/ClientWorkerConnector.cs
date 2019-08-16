using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fps.Connection;
using Improbable.Gdk.Core;
using Improbable.Gdk.GameObjectCreation;
using Improbable.Gdk.PlayerLifecycle;
using Improbable.Worker.CInterop;
using Improbable.Worker.CInterop.Alpha;
using UnityEngine;

namespace Fps
{
    public class ClientWorkerConnector : WorkerConnectorBase
    {
        protected string deployment;

        private string playerName;
        private bool isReadyToSpawn;
        private bool wantsSpawn;
        private Action<PlayerCreator.CreatePlayer.ReceivedResponse> onPlayerResponse;
        private AdvancedEntityPipeline entityPipeline;

        public bool HasConnected => Worker != null;
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

        public void DisconnectPlayer()
        {
            StartCoroutine(PrepareDestroy());
        }

        protected virtual string GetAuthPlayerPrefabPath()
        {
            return "Prefabs/UnityClient/Authoritative/Player";
        }

        protected virtual string GetNonAuthPlayerPrefabPath()
        {
            return "Prefabs/UnityClient/NonAuthoritative/Player";
        }

        protected override IConnectionHandlerBuilder GetConnectionHandlerBuilder()
        {
            var connectionParams = new ConnectionParameters
            {
                DefaultComponentVtable = new ComponentVtable(),
                WorkerType = WorkerUtils.UnityClient
            };

            var builder = new SpatialOSConnectionHandlerBuilder()
                .SetConnectionParameters(connectionParams);

            if (UseSessionFlow)
            {
                connectionParams.Network.UseExternalIp = true;
                builder.SetConnectionFlow(new ChosenDeploymentLocatorFlow(deployment,
                    new SessionConnectionFlowInitializer(new CommandLineConnectionFlowInitializer())));
            }
            else if (Application.isEditor)
            {
                builder.SetConnectionFlow(new ReceptionistFlow(CreateNewWorkerId(WorkerUtils.UnityClient)));
            }
            else
            {
                var initializer = new CommandLineConnectionFlowInitializer();

                switch (initializer.GetConnectionService())
                {
                    case ConnectionService.Receptionist:
                        builder.SetConnectionFlow(new ReceptionistFlow(CreateNewWorkerId(WorkerUtils.UnityClient),
                            initializer));
                        break;
                    case ConnectionService.Locator:
                        connectionParams.Network.UseExternalIp = true;
                        builder.SetConnectionFlow(new LocatorFlow(initializer));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return builder;
        }

        protected override void HandleWorkerConnectionEstablished()
        {
            var world = Worker.World;

            PlayerLifecycleHelper.AddClientSystems(world, autoRequestPlayerCreation: false);
            PlayerLifecycleConfig.MaxPlayerCreationRetries = 0;

            entityPipeline = new AdvancedEntityPipeline(Worker, GetAuthPlayerPrefabPath(), GetNonAuthPlayerPrefabPath());
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
