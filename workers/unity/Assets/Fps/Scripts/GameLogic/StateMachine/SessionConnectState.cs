namespace Fps
{
    public class ConnectState : SessionState
    {
        private string deployment;
        private bool foundDeployment;
        private ClientWorkerConnector connector;

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
                Owner.SetState(new InitState(Controller, Owner));
            }

            Controller.FrontEndController.SwitchToLobbyScreen();

            Controller.FrontEndController.LobbyScreenController.ConnectionStatusUIController.ShowConnectingText();
            connector = Owner.CreateClientWorker();
            connector.Connect(deployment, true);
        }

        public override void Tick()
        {
            if (connector.Worker == null)
            {
                // Worker has not connected yet. Continue waiting
                return;
            }

            var pit = GetDevelopmentPlayerIdentityToken();
            var loginTokens = GetDevelopmentLoginTokens(WorkerUtils.UnityClient, pit);
            foreach (var loginToken in loginTokens)
            {
                if (loginToken.DeploymentName == deployment)
                {
                    foundDeployment = true;
                    foreach (var tag in loginToken.Tags)
                    {
                        if (tag == "status_lobby" || tag == "status_running")
                        {
                            Owner.SetState(new PlayState(connector, Controller, Owner));
                            return;
                        }

                        if (tag == "status_stopping" || tag == "status_stopped")
                        {
                            Owner.SetState(new InitState(Controller, Owner));
                            return;
                        }
                    }

                    // no tags found, just return
                    Owner.SetState(new InitState(Controller, Owner));
                    return;
                }
            }
        }

        public override void ExitState()
        {
        }

        private string SelectDeployment()
        {
            var pit = GetDevelopmentPlayerIdentityToken();
            var loginTokens = GetDevelopmentLoginTokens(WorkerUtils.UnityClient, pit);
            foreach (var loginToken in loginTokens)
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
