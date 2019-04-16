using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Improbable.Worker.CInterop.Alpha;

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
                startScreenManager.browseButton.onClick.AddListener(BrowseDeployments);
                startScreenManager.quickJoinButton.onClick.AddListener(Connect);

                startScreenManager.browseButton.enabled = true;
                startScreenManager.quickJoinButton.enabled = true;
            }
        }

        public override void ExitState()
        {
            startScreenManager.browseButton.onClick.RemoveListener(BrowseDeployments);
            startScreenManager.quickJoinButton.onClick.RemoveListener(Connect);
        }

        public override void Tick()
        {
        }

        private void BrowseDeployments()
        {
            Owner.SetState(new ListDeploymentsState(Manager, Owner));
        }

        private void Connect()
        {
            Owner.SetState(new ConnectState(null, Manager, Owner));
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
            Manager.ScreenManager.DeploymentListScreenManager.SetDeployments(deploymentDatas.ToArray());

            if (!hasAvailableDeployment)
            {
                ShowErrorMessage("No deployment is currently available.");
                Owner.SetState(Owner.StartState, 2f);
            }

            return hasAvailableDeployment;
        }
    }
}
