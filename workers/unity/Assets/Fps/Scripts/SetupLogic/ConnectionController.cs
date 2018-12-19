using Improbable;
using Improbable.Gdk.Core;
using Improbable.Gdk.GameObjectRepresentation;
using Improbable.PlayerLifecycle;
using Improbable.Worker.CInterop;
using UnityEngine;

namespace Fps
{
    public class ConnectionController : MonoBehaviour
    {
        [Require] private PlayerCreator.Requirable.CommandRequestSender commandSender;
        [Require] private PlayerCreator.Requirable.CommandResponseHandler responseHandler;

        private GameObject canvasCameraObj;
        private ScreenUIController screenUIController;
        private Animator connectButton;
        private WorkerConnector clientWorkerConnector;

        private void Start()
        {
            clientWorkerConnector = gameObject.GetComponent<WorkerConnector>();
            connectButton = screenUIController.ConnectScreen.GetComponentInChildren<Animator>();
        }

        private void OnEnable()
        {
            if (responseHandler != null)
            {
                responseHandler.OnCreatePlayerResponse += OnCreatePlayerResponse;
            }
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
        }

        public void OnFailedToConnect()
        {
            // Worker failed to connect
            connectButton.SetTrigger("FailedToConnect");
        }


        private void SpawnPlayer()
        {
            var request = new CreatePlayerRequestType(new Vector3f { X = 0, Y = 0, Z = 0 });
            commandSender.SendCreatePlayerRequest(new EntityId(1), request);
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
