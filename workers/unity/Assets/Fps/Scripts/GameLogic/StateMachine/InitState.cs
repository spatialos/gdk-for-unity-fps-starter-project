using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Improbable.Worker.CInterop.Alpha;

namespace Fps
{
    public class InitState : SessionState
    {
        private string pit;
        private List<LoginTokenDetails> loginTokens;
        private bool updatedDeploymentList;

        private readonly SessionScreenController sessionScreenController;
        private readonly List<DeploymentData> deploymentDatas = new List<DeploymentData>();

        public InitState(ScreenUIController controller, ConnectionStateMachine owner) : base(controller, owner)
        {
            sessionScreenController = controller.FrontEndController.SessionScreenController;
        }

        public override void StartState()
        {
            if (Owner.UseSessionBasedFlow)
            {
                sessionScreenController.ConnectionStatusUIController.ShowGetDeploymentListText();
                sessionScreenController.browseButton.onClick.AddListener(BrowseDeployments);
                sessionScreenController.quickJoinButton.onClick.AddListener(Connect);
                sessionScreenController.browseButton.enabled = false;
                sessionScreenController.quickJoinButton.enabled = false;
                Controller.FrontEndController.SwitchToSessionScreen();
            }
            else
            {
                Controller.FrontEndController.SwitchToDefaultConnectScreen();
                Owner.SetState(new LocalConnectState(Controller, Owner));
            }
        }

        public override void ExitState()
        {
            if (Owner.UseSessionBasedFlow)
            {
                sessionScreenController.browseButton.onClick.RemoveListener(BrowseDeployments);
                sessionScreenController.quickJoinButton.onClick.RemoveListener(Connect);

                deploymentDatas.Clear();
            }
        }

        public override void Tick()
        {
            if (!Owner.UseSessionBasedFlow)
            {
                return;
            }

            if (deploymentDatas.Count == 0)
            {
                if (pit == null)
                {
                    pit = GetDevelopmentPlayerIdentityToken();
                }
                else if (loginTokens == null)
                {
                    loginTokens = GetDevelopmentLoginTokens(WorkerUtils.UnityClient, pit);
                }
                else
                {
                    FindDeployments();
                    pit = null;
                    loginTokens = null;
                }
            }
            else if (!updatedDeploymentList)
            {
                sessionScreenController.ConnectionStatusUIController.ShowDeploymentListAvailableText();
                sessionScreenController.browseButton.enabled = true;
                sessionScreenController.quickJoinButton.enabled = true;

                deploymentDatas.Sort((deployment1, deployment2) =>
                    String.Compare(deployment1.Name, deployment2.Name, StringComparison.Ordinal));
                Controller.FrontEndController.DeploymentListScreenController.SetDeployments(deploymentDatas.ToArray());

                updatedDeploymentList = true;
            }
        }

        private void BrowseDeployments()
        {
            Owner.SetState(new ListDeploymentsState(Controller, Owner));
        }

        private void Connect()
        {
            Owner.SetState(new ConnectState(null, Controller, Owner));
        }

        private void FindDeployments()
        {
            foreach (var loginToken in loginTokens)
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
                    }
                }

                var deploymentData = new DeploymentData(loginToken.DeploymentName, playerCount, maxPlayerCount,
                    isAvailable);
                deploymentDatas.Add(deploymentData);
            }
        }
    }
}
