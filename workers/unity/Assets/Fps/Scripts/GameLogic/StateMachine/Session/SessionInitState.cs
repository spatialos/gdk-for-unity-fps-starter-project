using System.Resources;
using UnityEngine;

namespace Fps
{
    public class SessionInitState : SessionState
    {
        public SessionInitState(UIManager manager, ConnectionStateMachine owner) : base(manager, owner)
        {
        }

        public override void StartState()
        {
            ScreenManager.ListDeploymentsButton.enabled = false;
            ScreenManager.QuickJoinButton.enabled = false;
            Manager.ShowFrontEnd();
            Manager.ScreenManager.SwitchToStartScreen();

            var listDeploymentsState = new PrepareDeploymentsListState(Manager, Owner);
            var getPitState = new GetPlayerIdentityTokenState(listDeploymentsState, Manager, Owner);

            var textAsset = Resources.Load<TextAsset>("DevAuthToken");
            if (textAsset != null)
            {
                Blackboard.DevAuthToken = textAsset.text.Trim();
            }
            else
            {
                throw new MissingManifestResourceException(
                    "Unable to find DevAuthToken.txt inside your Resources folder. Ensure to generate one.");
            }

            Manager.ScreenManager.StartStatus.ShowGetDeploymentListText();
            Owner.SetState(getPitState);
        }
    }
}
