using Improbable.Gdk.Core;
using Improbable.PlayerLifecycle;
using Improbable.Worker.CInterop;
using UnityEngine;

namespace Fps
{
    public class LocalConnectState : SessionState
    {
        private ClientWorkerConnector connector;
        private Animator animator;
        private bool isDisconnected;

        public LocalConnectState(ScreenUIController controller, ConnectionStateMachine owner) : base(controller, owner)
        {
            animator = controller.FrontEndController.ConnectScreenController.GetComponentInChildren<Animator>(true);
        }

        public override void StartState()
        {
            Controller.FrontEndController.ConnectScreenController.ConnectButton.enabled = false;
            Controller.FrontEndController.ConnectScreenController.ConnectButton.onClick.AddListener(Connect);
            connector = Owner.CreateClientWorker();
            connector.Connect(string.Empty, false);
        }

        public override void Tick()
        {
            if (connector == null)
            {
                if (!isDisconnected)
                {
                    Controller.FrontEndController.ConnectScreenController.ConnectButton.enabled = true;
                    animator.SetTrigger("FailedToConnect");
                    UnityObjectDestroyer.Destroy(connector);
                    connector = null;
                    isDisconnected = true;
                }

                return;
            }

            if (connector.Worker == null)
            {
                // Worker has not connected yet. Continue waiting
                return;
            }

            Controller.FrontEndController.ConnectScreenController.ConnectButton.enabled = true;
            animator.SetTrigger("Ready");
        }

        public override void ExitState()
        {
            Controller.FrontEndController.ConnectScreenController.ConnectButton.onClick.RemoveListener(Connect);
        }

        private void Connect()
        {
            if (connector == null)
            {
                isDisconnected = false;
                Controller.FrontEndController.ConnectScreenController.ConnectButton.enabled = false;
                animator.SetTrigger("Retry");
                connector = Owner.CreateClientWorker();
                connector.Connect(string.Empty, false);
            }
            else
            {
                isDisconnected = false;
                Controller.FrontEndController.ConnectScreenController.ConnectButton.enabled = false;
                connector.SpawnPlayerAction("Local Player", OnPlayerResponse);
                animator.SetTrigger("Connecting");
            }
        }

        private void OnPlayerResponse(PlayerCreator.CreatePlayer.ReceivedResponse obj)
        {
            if (obj.StatusCode == StatusCode.Success)
            {
                Controller.ShowGameView();
            }
            else
            {
                Controller.FrontEndController.ConnectScreenController.ConnectButton.enabled = true;
                animator.SetTrigger("FailedToSpawn");
            }
        }
    }
}
