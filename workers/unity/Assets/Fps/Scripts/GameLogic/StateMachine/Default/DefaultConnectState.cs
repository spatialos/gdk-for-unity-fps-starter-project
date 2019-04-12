using Improbable.Gdk.Core;
using Improbable.PlayerLifecycle;
using Improbable.Worker.CInterop;
using UnityEngine;

namespace Fps
{
    public class DefaultConnectState : DefaultState
    {
        public DefaultConnectState(ScreenUIController controller, ConnectionStateMachine owner) : base(controller, owner)
        {
        }

        public override void StartState()
        {
            ConnectScreenController.ConnectButton.enabled = false;
            ConnectScreenController.ConnectButton.onClick.AddListener(Connect);
            Connect();
        }

        public override void Tick()
        {
            if (Owner.ClientConnector == null)
            {
                ConnectScreenController.ConnectButton.enabled = true;
                Animator.SetTrigger("FailedToConnect");

                return;
            }

            if (Owner.ClientConnector.Worker == null)
            {
                // Worker has not connected yet. Continue waiting
                return;
            }

            ConnectScreenController.ConnectButton.onClick.RemoveListener(Connect);
            ConnectScreenController.ConnectButton.onClick.AddListener(SpawnPlayer);
            ConnectScreenController.ConnectButton.enabled = true;
            Animator.SetTrigger("Ready");
        }

        public override void ExitState()
        {
        }

        private void Connect()
        {
            Owner.CreateClientWorker();
            Owner.ClientConnector.Connect(string.Empty, false);
        }

        private void SpawnPlayer()
        {
            ConnectScreenController.ConnectButton.onClick.RemoveListener(SpawnPlayer);
            Owner.SetState(new DefaultSpawnState(Controller, Owner));
        }
    }
}
