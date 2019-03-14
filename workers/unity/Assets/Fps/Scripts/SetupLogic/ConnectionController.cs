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
        private WorkerConnector clientWorkerConnector;

        private bool isReadyToSpawn;
        private bool wantsSpawn;

        private void Start()
        {
            clientWorkerConnector = gameObject.GetComponent<WorkerConnector>();
            connectButton = screenUIController.ConnectScreen.GetComponentInChildren<Animator>();
        }

        public void InformOfUI(GameObject canvasCameraObj, ScreenUIController screenUIController)
        {
            this.canvasCameraObj = canvasCameraObj;
            this.screenUIController = screenUIController;
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

        private void Update()
        {
            if (!connectButton.isActiveAndEnabled)
            {
                return;
            }

            if (clientWorkerConnector != null && clientWorkerConnector.Worker != null)
            {
                // Worker successfully connected
                if (connectButton.GetCurrentAnimatorStateInfo(0).IsName("ConnectingState"))
                {
                    connectButton.SetTrigger("Ready");
                }
            }

            if (wantsSpawn && isReadyToSpawn)
            {
                SpawnPlayer();
                wantsSpawn = false;
            }
        }

        public void OnFailedToConnect()
        {
            // Worker failed to connect
            connectButton.SetTrigger("FailedToConnect");
        }

        public void OnReadyToSpawn()
        {
            isReadyToSpawn = true;
        }

        private void SpawnPlayer()
        {
            var request = new CreatePlayerRequestType();
            commandSender.SendCreatePlayerCommand(new EntityId(1), request, OnCreatePlayerResponse);
        }

        public void ConnectAction()
        {
            if (connectButton.GetCurrentAnimatorStateInfo(0).IsName("ReadyState"))
            {
                connectButton.SetTrigger("Connecting");
                wantsSpawn = true;
            }
            else if (connectButton.GetCurrentAnimatorStateInfo(0).IsName("FailedToSpawn"))
            {
                connectButton.SetTrigger("Retry");
                wantsSpawn = true;
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
