using Fps.UI;
using UnityEngine;

namespace Fps.StateMachine
{
    public class ListDeploymentsState : SessionState
    {
        public ListDeploymentsState(UIManager manager, ConnectionStateMachine owner) : base(manager, owner)
        {
        }

        public override void StartState()
        {
            ScreenManager.DeploymentListStatus.ShowDeploymentListAvailableText();
            ScreenManager.JoinDeploymentButton.onClick.AddListener(Connect);
            ScreenManager.BackButton.onClick.AddListener(ResetToStart);
            ScreenManager.SwitchToDeploymentListScreen();
        }

        public override void ExitState()
        {
            ScreenManager.JoinDeploymentButton.onClick.RemoveListener(Connect);
            ScreenManager.BackButton.onClick.RemoveListener(ResetToStart);
        }

        public override void Tick()
        {
            ScreenManager.JoinDeploymentButton.enabled = !string.IsNullOrEmpty(Blackboard.Deployment);

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ScreenManager.BackButton.onClick.Invoke();
            }

            if ((Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return)) && ScreenManager.JoinDeploymentButton.enabled)
            {
                ScreenManager.JoinDeploymentButton.onClick.Invoke();
            }
        }

        private void Connect()
        {
            Owner.SetState(new SessionConnectState(Blackboard.Deployment, Manager, Owner));
        }

        private void ResetToStart()
        {
            Owner.SetState(Owner.StartState);
        }
    }
}
