namespace Fps
{
    public class ListDeploymentsState : SessionState
    {
        private readonly DeploymentListScreenManager deploymentListScreenManager;

        public ListDeploymentsState(UIManager manager, ConnectionStateMachine owner) : base(manager, owner)
        {
            deploymentListScreenManager = manager.ScreenManager.DeploymentListScreenManager;
        }

        public override void StartState()
        {
            deploymentListScreenManager.ShowDeploymentListAvailableText();
            deploymentListScreenManager.JoinButton.onClick.AddListener(Connect);
            deploymentListScreenManager.BackButton.onClick.AddListener(ResetToStart);
            Manager.ScreenManager.SwitchToDeploymentListScreen();
        }

        public override void ExitState()
        {
            deploymentListScreenManager.JoinButton.onClick.RemoveListener(Connect);
            deploymentListScreenManager.BackButton.onClick.RemoveListener(ResetToStart);
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

        private void ResetToStart()
        {
            Owner.SetState(Owner.StartState);
        }
    }
}
