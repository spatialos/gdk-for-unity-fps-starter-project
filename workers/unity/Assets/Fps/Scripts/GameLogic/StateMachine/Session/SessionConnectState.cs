using UnityEngine;

namespace Fps
{
    public class ConnectState : SessionState
    {
        private string deployment;
        private bool foundDeployment;

        public ConnectState(string deployment, UIManager manager, ConnectionStateMachine owner) : base(manager, owner)
        {
            this.deployment = deployment;
        }

        public override void StartState()
        {
            // check if player select deployment, otherwise choose one
            if (string.IsNullOrEmpty(deployment))
            {
                deployment = SelectFirstDeployment();
            }

            if (string.IsNullOrEmpty(deployment))
            {
                Owner.SetState(Owner.StartState);
            }

            Manager.ScreenManager.SwitchToLobbyScreen();

            Manager.ScreenManager.LobbyScreenManager.ShowConnectingText();
            Manager.ScreenManager.LobbyScreenManager.StartButton.enabled = false;

            var clientWorker = Object.Instantiate(Owner.ClientWorkerConnectorPrefab, Owner.transform.position, Quaternion.identity);
            Owner.Blackboard.ClientConnector = clientWorker.GetComponent<ClientWorkerConnector>();
            Owner.Blackboard.ClientConnector.Connect(deployment, true);
        }

        public override void Tick()
        {
            if (Owner.Blackboard.ClientConnector.Worker == null)
            {
                // Worker has not connected yet. Continue waiting
                return;
            }

            var state = new WaitForGameState(Manager, Owner);
            var nextState = new QuerySessionStatusState(state, Manager, Owner);
            Owner.SetState(nextState);
        }

        public override void ExitState()
        {
        }

        private string SelectFirstDeployment()
        {
            foreach (var loginToken in Owner.Blackboard.LoginTokens)
            {
                if (loginToken.Tags.Contains("status_lobby") || loginToken.Tags.Contains("status_running"))
                {
                    return loginToken.DeploymentName;
                }
            }

            return string.Empty;
        }
    }
}
