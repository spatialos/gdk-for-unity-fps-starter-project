using Improbable;
using Improbable.Gdk.GameObjectRepresentation;
using Improbable.Gdk.PlayerLifecycle;
using Improbable.PlayerLifecycle;
using Improbable.Worker;
using Improbable.Worker.Core;
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
        private ClientWorkerConnector clientWorkerConnector;

        private void Start()
        {
            clientWorkerConnector = gameObject.GetComponent<ClientWorkerConnector>();
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
            // Detect successful player creation, hide loadscreen UI in response.
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

            clientWorkerConnector = clientWorkerConnector ?? ClientWorkerHandler.ClientWorkerConnector;
        }

        public void OnFailedToConnect()
        {
            // Worker failed to connect
            connectButton.SetTrigger("FailedToConnect");
        }

        public void ConnectAction()
        {
            if (connectButton.GetCurrentAnimatorStateInfo(0).IsName("FailedToSpawn"))
            {
                connectButton.SetTrigger("Retry");
                GameLogicWorkerHandler.CreateGameLogic();
                //SpawnPlayer();
            }
            else if (connectButton.GetCurrentAnimatorStateInfo(0).IsName("FailedToConnect"))
            {
                connectButton.SetTrigger("Retry");
                ClientWorkerHandler.CreateClient();
            }
        }
    }
}
