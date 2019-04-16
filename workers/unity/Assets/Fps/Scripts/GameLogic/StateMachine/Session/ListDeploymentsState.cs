namespace Fps
{
    public class ListDeploymentsState : SessionState
    {
        private readonly DeploymentListScreenManager deploymentListScreenManager;

        public ListDeploymentsState(UIManager manager, ConnectionStateMachine owner) : base(manager,
            owner)
        {
            deploymentListScreenManager = manager.FrontEndController.deploymentListScreenManager;
        }

        public override void StartState()
        {
            deploymentListScreenManager.ShowDeploymentListAvailableText();
            deploymentListScreenManager.JoinButton.onClick.AddListener(Connect);
            deploymentListScreenManager.BackButton.onClick.AddListener(Back);
            Manager.FrontEndController.SwitchToDeploymentListScreen();
        }

        public override void ExitState()
        {
            deploymentListScreenManager.JoinButton.onClick.RemoveListener(Connect);
            deploymentListScreenManager.BackButton.onClick.RemoveListener(Back);
        }

        public override void Tick()
        {
            deploymentListScreenManager.JoinButton.enabled =
                deploymentListScreenManager.IsHighlightedDeploymentAvailable();
        }

        private void Connect()
        {
            Owner.SetState(new ConnectState(deploymentListScreenManager.GetChosenDeployment(), Manager, Owner));
        }

        private void Back()
        {
            Owner.SetState(Owner.StartState);
        }
    }
}
