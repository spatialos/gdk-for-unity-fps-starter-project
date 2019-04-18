using System;
using System.Collections.Generic;
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
                ScreenManager.DeploymentListStatus.ShowDeploymentListAvailableText();
                ScreenManager.ListDeploymentsButton.onClick.AddListener(BrowseDeployments);
                ScreenManager.QuickJoinButton.onClick.AddListener(Connect);

                ScreenManager.ListDeploymentsButton.enabled = true;
                ScreenManager.QuickJoinButton.enabled = true;
            }
            else
            {
                Manager.ScreenManager.DeploymentListStatus.ShowFailedToGetDeploymentsText("No deployment is currently available.");
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

            var deploymentData = new List<DeploymentData>();
            foreach (var loginToken in Blackboard.LoginTokens)
            {
                if (DeploymentData.TryFromTags(loginToken.DeploymentName, loginToken.Tags, out var data))
                {
                    deploymentData.Add(data);
                }
            }

            ScreenManager.InformOfDeployments(deploymentData);
            return deploymentData.Any(data => data.IsAvailable);
        }
    }
}
