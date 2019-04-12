using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Improbable.Worker.CInterop.Alpha;

namespace Fps
{
    public class PrepareDeploymentsListState : SessionState
    {
        private readonly SessionScreenController sessionScreenController;

        public PrepareDeploymentsListState(ScreenUIController controller, ConnectionStateMachine owner) : base(controller, owner)
        {
            sessionScreenController = controller.FrontEndController.SessionScreenController;
        }

        public override void StartState()
        {
            if (PrepareDeploymentsList())
            {
                sessionScreenController.ConnectionStatusUIController.ShowDeploymentListAvailableText();
                sessionScreenController.browseButton.onClick.AddListener(BrowseDeployments);
                sessionScreenController.quickJoinButton.onClick.AddListener(Connect);

                sessionScreenController.browseButton.enabled = true;
                sessionScreenController.quickJoinButton.enabled = true;
            }
        }

        public override void ExitState()
        {
            sessionScreenController.browseButton.onClick.RemoveListener(BrowseDeployments);
            sessionScreenController.quickJoinButton.onClick.RemoveListener(Connect);
        }

        public override void Tick()
        {
        }

        private void BrowseDeployments()
        {
            Owner.SetState(new ListDeploymentsState(Controller, Owner));
        }

        private void Connect()
        {
            Owner.SetState(new ConnectState(null, Controller, Owner));
        }

        private bool PrepareDeploymentsList()
        {
            if (Owner.LoginTokens == null)
            {
                Owner.SetState(Owner.StartState);
            }

            bool hasAvailableDeployment = false;
            var deploymentDatas = new List<DeploymentData>();
            foreach (var loginToken in Owner.LoginTokens)
            {
                var playerCount = 0;
                var maxPlayerCount = 0;
                var isAvailable = false;
                foreach (var tag in loginToken.Tags)
                {
                    if (tag.StartsWith("players"))
                    {
                        playerCount = int.Parse(tag.Split('_').Last());
                    }
                    else if (tag.StartsWith("max_players"))
                    {
                        maxPlayerCount = int.Parse(tag.Split('_').Last());
                    }
                    else if (tag.StartsWith("status"))
                    {
                        var state = tag.Split('_').Last();
                        isAvailable = state == "lobby" || state == "running";
                        hasAvailableDeployment |= isAvailable;
                    }
                }

                var deploymentData = new DeploymentData(loginToken.DeploymentName, playerCount, maxPlayerCount,
                    isAvailable);
                deploymentDatas.Add(deploymentData);
            }

            deploymentDatas.Sort((deployment1, deployment2) =>
                String.Compare(deployment1.Name, deployment2.Name, StringComparison.Ordinal));
            Controller.FrontEndController.DeploymentListScreenController.SetDeployments(deploymentDatas.ToArray());

            if (!hasAvailableDeployment)
            {
                ShowErrorMessage("No deployment is currently available.");
                Owner.SetState(Owner.StartState);
            }

            return hasAvailableDeployment;
        }
    }
}
