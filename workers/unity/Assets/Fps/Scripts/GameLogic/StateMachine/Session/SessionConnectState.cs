namespace Fps
{
    public class ConnectState : SessionState
    {
        private string deployment;
        private bool foundDeployment;

        public ConnectState(string deployment, ScreenUIController controller, ConnectionStateMachine owner) : base(controller, owner)
        {
            this.deployment = deployment;
        }

        public override void StartState()
        {
            // check if player select deployment, otherwise choose one
            if (string.IsNullOrEmpty(deployment))
            {
                deployment = SelectDeployment();
            }

            if (string.IsNullOrEmpty(deployment))
            {
                Owner.SetState(Owner.StartState);
            }

            Controller.FrontEndController.SwitchToLobbyScreen();

            Controller.FrontEndController.LobbyScreenController.ConnectionStatusUIController.ShowConnectingText();
            Controller.FrontEndController.LobbyScreenController.startButton.enabled = false;

            Owner.CreateClientWorker();
            Owner.ClientConnector.Connect(deployment, true);
        }

        public override void Tick()
        {
            if (Owner.ClientConnector.Worker == null)
            {
                // Worker has not connected yet. Continue waiting
                return;
            }

            var state = new WaitForGameState(Controller, Owner);
            var nextState = new GetPlayerIdentityTokenState(state, Controller, Owner);
            Owner.SetState(nextState);
        }

        public override void ExitState()
        {
        }

        private string SelectDeployment()
        {
            foreach (var loginToken in Owner.LoginTokens)
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
