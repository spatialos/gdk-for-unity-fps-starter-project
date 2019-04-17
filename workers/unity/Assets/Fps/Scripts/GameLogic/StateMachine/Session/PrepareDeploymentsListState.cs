using System;
using System.Collections.Generic;
using System.Linq;

namespace Fps
{
    public class PrepareDeploymentsListState : SessionState
    {
        private readonly StartScreenManager startScreenManager;

        public PrepareDeploymentsListState(UIManager manager, ConnectionStateMachine owner) : base(manager, owner)
        {
            startScreenManager = manager.ScreenManager.StartScreenManager;
        }

        public override void StartState()
        {
            if (PrepareDeploymentsList())
            {
                startScreenManager.ShowDeploymentListAvailableText();
                startScreenManager.BrowseButton.onClick.AddListener(BrowseDeployments);
                startScreenManager.QuickJoinButton.onClick.AddListener(Connect);

                startScreenManager.BrowseButton.enabled = true;
                startScreenManager.QuickJoinButton.enabled = true;
            }
            else
            {
                Manager.ScreenManager.StartScreenManager.ShowFailedToGetDeploymentsText("No deployment is currently available.");
                Owner.SetState(Owner.StartState, 2f);
            }
        }

        public override void ExitState()
        {
            startScreenManager.BrowseButton.onClick.RemoveListener(BrowseDeployments);
            startScreenManager.QuickJoinButton.onClick.RemoveListener(Connect);
        }

        private void BrowseDeployments()
        {
            Owner.SetState(new ListDeploymentsState(Manager, Owner));
        }

        private void Connect()
        {
            Owner.SetState(new ConnectState(deployment: null, Manager, Owner));
        }

        private bool PrepareDeploymentsList()
        {
            if (Owner.Blackboard.LoginTokens == null)
            {
                Owner.SetState(Owner.StartState);
            }

            var hasAvailableDeployment = false;
            var deploymentData = new List<DeploymentData>();
            foreach (var loginToken in Owner.Blackboard.LoginTokens)
            {
                var data = DeploymentData.CreateFromTags(loginToken.DeploymentName, loginToken.Tags);
                hasAvailableDeployment |= data.IsAvailable;
                deploymentData.Add(data);
            }

            Manager.ScreenManager.DeploymentListScreenManager.SetDeployments(deploymentData);

            return hasAvailableDeployment;
        }
    }
}
