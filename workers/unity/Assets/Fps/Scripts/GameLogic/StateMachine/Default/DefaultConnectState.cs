using UnityEngine;

namespace Fps
{
    public class DefaultConnectState : DefaultState
    {
        public DefaultConnectState(UIManager manager, ConnectionStateMachine owner) : base(manager, owner)
        {
        }

        public override void StartState()
        {
            DefaultConnectScreenManager.ConnectButton.enabled = false;
            DefaultConnectScreenManager.ConnectButton.onClick.AddListener(Connect);
            Connect();
        }

        public override void Tick()
        {
            if (Owner.ClientConnector == null)
            {
                DefaultConnectScreenManager.ConnectButton.enabled = true;
                Animator.SetTrigger("FailedToConnect");


                if (Input.GetKeyDown(KeyCode.Space))
                {
                    DefaultConnectScreenManager.ConnectButton.onClick.Invoke();
                }

                return;
            }

            if (Owner.ClientConnector.Worker == null)
            {
                // Worker has not connected yet. Continue waiting
                return;
            }

            Owner.SetState(new DefaultSpawnState(Manager, Owner));
        }

        public override void ExitState()
        {
            DefaultConnectScreenManager.ConnectButton.onClick.RemoveListener(Connect);
        }

        private void Connect()
        {
            Owner.CreateClientWorker();
            Owner.ClientConnector.Connect(string.Empty, false);
        }
    }
}
