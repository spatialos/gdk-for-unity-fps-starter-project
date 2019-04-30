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
            ScreenManager.DefaultConnectButton.onClick.AddListener(Retry);
            Connect();
        }

        public override void Tick()
        {
            if (Blackboard.ClientConnector == null)
            {
                if (!ScreenManager.DefaultConnectButton.enabled)
                {
                    ScreenManager.DefaultConnectButton.enabled = true;
                    Animator.SetTrigger("FailedToConnect");
                }

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    ScreenManager.DefaultConnectButton.onClick.Invoke();
                }

                return;
            }

            if (!Blackboard.ClientConnector.HasConnected())
            {
                return;
            }

            Owner.SetState(new DefaultSpawnState(Manager, Owner));
        }

        public override void ExitState()
        {
            ScreenManager.DefaultConnectButton.onClick.RemoveListener(Retry);
        }

        private void Retry()
        {
            Animator.SetTrigger("Retry");
            Connect();
        }

        private void Connect()
        {
            ScreenManager.DefaultConnectButton.enabled = false;
            var clientWorker = Object.Instantiate(Owner.ClientWorkerConnectorPrefab, Owner.transform.position, Quaternion.identity);
            Blackboard.ClientConnector = clientWorker.GetComponent<ClientWorkerConnector>();
            Blackboard.ClientConnector.Connect();
        }
    }
}
