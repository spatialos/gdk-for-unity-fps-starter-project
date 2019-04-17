using System.Resources;
using UnityEngine;

namespace Fps
{
    public class SessionInitState : SessionState
    {
        private readonly StartScreenManager startScreenManager;

        public SessionInitState(UIManager manager, ConnectionStateMachine owner) : base(manager, owner)
        {
            startScreenManager = manager.ScreenManager.StartScreenManager;
        }

        public override void StartState()
        {
            startScreenManager.BrowseButton.enabled = false;
            startScreenManager.QuickJoinButton.enabled = false;
            Manager.ShowFrontEnd();
            Manager.ScreenManager.SwitchToSessionScreen();

            var listDeploymentsState = new PrepareDeploymentsListState(Manager, Owner);
            var getPitState = new GetPlayerIdentityTokenState(listDeploymentsState, Manager, Owner);

            var textAsset = Resources.Load<TextAsset>("DevAuthToken");
            if (textAsset != null)
            {
                Owner.Blackboard.DevAuthToken = textAsset.text.Trim();
            }
            else
            {
                throw new MissingManifestResourceException(
                    "Unable to find DevAuthToken.txt inside your Resources folder. Ensure to generate one.");
            }

            Manager.ScreenManager.StartScreenManager.ShowGetDeploymentListText();
            Owner.SetState(getPitState);
        }

        public override void ExitState()
        {
        }
    }
}
