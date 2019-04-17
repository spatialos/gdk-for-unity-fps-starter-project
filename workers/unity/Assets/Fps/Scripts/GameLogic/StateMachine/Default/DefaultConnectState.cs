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
            if (Owner.Blackboard.ClientConnector == null)
            {
                DefaultConnectScreenManager.ConnectButton.enabled = true;
                Animator.SetTrigger("FailedToConnect");


                if (Input.GetKeyDown(KeyCode.Space))
                {
                    DefaultConnectScreenManager.ConnectButton.onClick.Invoke();
                }

                return;
            }

            if (Owner.Blackboard.ClientConnector.Worker == null)
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
            var clientWorker = Object.Instantiate(Owner.ClientWorkerConnectorPrefab, Owner.transform.position, Quaternion.identity);
            Owner.Blackboard.ClientConnector = clientWorker.GetComponent<ClientWorkerConnector>();
            Owner.Blackboard.ClientConnector.Connect(string.Empty, false);
        }
    }
}
