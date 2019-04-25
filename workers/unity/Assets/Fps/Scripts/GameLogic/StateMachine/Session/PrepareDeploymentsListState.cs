using System.Linq;

namespace Fps
{
    public class PrepareDeploymentsListState : SessionState
    {
        public PrepareDeploymentsListState(UIManager manager, ConnectionStateMachine owner) : base(manager, owner)
        {
        }

        public override void StartState()
        {
            if (PrepareDeploymentsList())
            {
                ScreenManager.StartStatus.ShowDeploymentListAvailableText();
                ScreenManager.ListDeploymentsButton.onClick.AddListener(BrowseDeployments);
                ScreenManager.QuickJoinButton.onClick.AddListener(Connect);

                ScreenManager.ListDeploymentsButton.enabled = true;
                ScreenManager.QuickJoinButton.enabled = true;
            }
            else
            {
                Manager.ScreenManager.StartStatus.ShowFailedToGetDeploymentsText("No deployment is currently available.");
                Owner.SetState(Owner.StartState, 2f);
            }
        }

        public override void ExitState()
        {
            ScreenManager.ListDeploymentsButton.onClick.RemoveListener(BrowseDeployments);
            ScreenManager.QuickJoinButton.onClick.RemoveListener(Connect);
        }

        private void BrowseDeployments()
        {
            Owner.SetState(new ListDeploymentsState(Manager, Owner));
        }

        private void Connect()
        {
            Owner.SetState(new SessionConnectState(deployment: null, Manager, Owner));
        }

        private bool PrepareDeploymentsList()
        {
            if (Blackboard.LoginTokens == null)
            {
                Owner.SetState(Owner.StartState);
            }

            var deploymentData = Blackboard.LoginTokens
                .Select(token => (DeploymentData.TryFromLoginToken(token, out var data), data))
                .Where(pair => pair.Item1)
                .Select(pair => pair.data)
                .ToList();

            ScreenManager.InformOfDeployments(deploymentData);
            return deploymentData.Any(data => data.IsAvailable);
        }
    }
}
