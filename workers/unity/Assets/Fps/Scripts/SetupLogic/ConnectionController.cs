using System.Collections;
using Improbable;
using Improbable.Gdk.Core;
using Improbable.Gdk.PlayerLifecycle;
using Improbable.Gdk.Subscriptions;
using Improbable.PlayerLifecycle;
using Improbable.Worker.CInterop;
using UnityEngine;

namespace Fps
{
    public class ConnectionController : MonoBehaviour
    {
        [Require] private PlayerCreatorCommandSender commandSender;

        private WorkerConnector clientWorkerConnector;

        private bool isReadyToSpawn;
        private bool wantsSpawn;

        private void Start()
        {
            clientWorkerConnector = gameObject.GetComponent<WorkerConnector>();
            clientWorkerConnector.OnWorkerCreationFinished += OnWorkerCreated;
            ConnectionStateReporter.OnConnectionStateChange += OnConnectionStateChange;
        }

        private void OnConnectionStateChange(ConnectionStateReporter.State state, string information)
        {
            if (state == ConnectionStateReporter.State.Spawning)
            {
                SpawnPlayerAction(information);
            }
        }

        private void OnWorkerCreated(Worker worker)
        {
            if (worker?.Connection.GetConnectionStatusCode() == ConnectionStatusCode.Success)
            {
                StartCoroutine(DelayedConnectedMessage());
            }
            else
            {
                ConnectionStateReporter.SetState(ConnectionStateReporter.State.ConnectionFailed,
                    worker?.Connection.GetConnectionStatusCodeDetailString());
            }
        }

        private void Update()
        {
            if (wantsSpawn && isReadyToSpawn)
            {
                SendRequest();
                wantsSpawn = false;
            }
        }


        private IEnumerator DelayedConnectedMessage()
        {
            // 1 frame delay necessary to allow [Require] components to active on ConnectionController
            yield return null;
            ConnectionStateReporter.SetState(ConnectionStateReporter.State.Connected);
        }

        private void OnCreatePlayerResponse(PlayerCreator.CreatePlayer.ReceivedResponse obj)
        {
            if (obj.StatusCode == StatusCode.Success)
            {
                ConnectionStateReporter.SetState(ConnectionStateReporter.State.Spawned);
            }
            else
            {
                ConnectionStateReporter.SetState(ConnectionStateReporter.State.SpawningFailed,
                    obj.StatusCode.ToString());
            }

            if (wantsSpawn && isReadyToSpawn)
            {
                SendRequest();
                wantsSpawn = false;
            }
        }

        public void OnFailedToConnect(string errorMessage)
        {
            ConnectionStateReporter.SetState(ConnectionStateReporter.State.ConnectionFailed, errorMessage);
        }

        public void OnDisconnected()
        {
            ConnectionStateReporter.SetState(ConnectionStateReporter.State.WorkerDisconnected);
        }

        public void OnReadyToSpawn()
        {
            isReadyToSpawn = true;
        }

        private string playerName;

        public void SpawnPlayerAction(string playerName)
        {
            this.playerName = playerName;
            wantsSpawn = true;
        }

        public void SendRequest()
        {
            var serializedArgs = PlayerLifecycleHelper.SerializeArguments(playerName);
            var request = new CreatePlayerRequestType(serializedArgs);
            commandSender.SendCreatePlayerCommand(new EntityId(1), request, OnCreatePlayerResponse);
        }

        private void OnDestroy()
        {
            ConnectionStateReporter.OnConnectionStateChange -= OnConnectionStateChange;
        }
    }
}
