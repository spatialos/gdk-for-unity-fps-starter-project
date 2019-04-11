namespace Fps
{
    public class ListDeploymentsState : SessionState
    {
        private readonly DeploymentListScreenController deploymentListScreenController;

        public ListDeploymentsState(ScreenUIController controller, ConnectionStateMachine owner) : base(controller,
            owner)
        {
            deploymentListScreenController = controller.FrontEndController.DeploymentListScreenController;
        }

        public override void StartState()
        {
            deploymentListScreenController.ConnectionStatusUIController.ShowDeploymentListAvailableText();
            deploymentListScreenController.JoinButton.onClick.AddListener(Connect);
            deploymentListScreenController.BackButton.onClick.AddListener(Back);
            Controller.FrontEndController.SwitchToDeploymentListScreen();
        }

        public override void ExitState()
        {
            deploymentListScreenController.JoinButton.onClick.RemoveListener(Connect);
            deploymentListScreenController.BackButton.onClick.RemoveListener(Back);
        }

        public override void Tick()
        {
            deploymentListScreenController.JoinButton.enabled =
                deploymentListScreenController.IsAvailableDeploymentHighlighted();
        }

        private void Connect()
        {
            Owner.SetState(new ConnectState(deploymentListScreenController.GetChosenDeployment(), Controller, Owner));
        }

        private void Back()
        {
            Owner.SetState(new InitState(Controller, Owner));
        }
    }
}
