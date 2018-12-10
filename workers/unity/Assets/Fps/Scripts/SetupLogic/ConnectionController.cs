using Improbable;
using Improbable.Gdk.Core;
using Improbable.Gdk.Subscriptions;
using Improbable.PlayerLifecycle;
using Improbable.Worker.CInterop;
using UnityEngine;

namespace Fps
{
    public class ConnectionController : MonoBehaviour
    {
        [Require] private PlayerCreatorCommandSender commandSender;

        private GameObject canvasCameraObj;
        private ScreenUIController screenUIController;
        private Animator connectButton;
        private ClientWorkerConnector clientWorkerConnector;

        private void Start()
        {
            clientWorkerConnector = gameObject.GetComponent<ClientWorkerConnector>();
            connectButton = screenUIController.ConnectScreen.GetComponentInChildren<Animator>();
        }

        public void InformOfUI(GameObject canvasCameraObj, ScreenUIController screenUIController)
        {
            this.canvasCameraObj = canvasCameraObj;
            this.screenUIController = screenUIController;
        }

        private void Update()
        {
            if (!connectButton.isActiveAndEnabled)
            {
                return;
            }

            clientWorkerConnector = clientWorkerConnector
                ? clientWorkerConnector
                : ClientWorkerHandler.ClientWorkerConnector;

            if (clientWorkerConnector != null && clientWorkerConnector.Worker != null)
            {
                // Worker successfully connected
                if (connectButton.GetCurrentAnimatorStateInfo(0).IsName("ConnectingState"))
                {
                    connectButton.SetTrigger("Ready");
                }
            }
        }

        public void OnFailedToConnect()
        {
            // Worker failed to connect
            connectButton.SetTrigger("FailedToConnect");
        }


        private void SpawnPlayer()
        {
            var request = new CreatePlayerRequestType(new Vector3f { X = 0, Y = 0, Z = 0 });
            commandSender.SendCreatePlayerCommand(new PlayerCreator.CreatePlayer.Request(new EntityId(1), request),
                OnCreatePlayerResponse);
        }

        private void OnCreatePlayerResponse(PlayerCreator.CreatePlayer.ReceivedResponse obj)
        {
            if (obj.StatusCode == StatusCode.Success)
            {
                canvasCameraObj.SetActive(false);
                screenUIController.ConnectScreen.SetActive(false);
                screenUIController.InGameHud.SetActive(true);
            }
            else
            {
                connectButton.SetTrigger("FailedToSpawn");
            }
        }

        public void ConnectAction()
        {
            if (connectButton.GetCurrentAnimatorStateInfo(0).IsName("ReadyState"))
            {
                connectButton.SetTrigger("Connecting");
                SpawnPlayer();
            }
            else if (connectButton.GetCurrentAnimatorStateInfo(0).IsName("FailedToSpawn"))
            {
                connectButton.SetTrigger("Retry");
                SpawnPlayer();
            }
            else if (connectButton.GetCurrentAnimatorStateInfo(0).IsName("FailedToConnect")
                || connectButton.GetCurrentAnimatorStateInfo(0).IsName("WorkerDisconnected"))
            {
                connectButton.SetTrigger("Retry");
                ClientWorkerHandler.CreateClient();
            }
        }
    }
}
