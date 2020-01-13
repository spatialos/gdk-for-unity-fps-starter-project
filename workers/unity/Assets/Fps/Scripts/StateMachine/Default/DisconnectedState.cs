using Fps.UI;
using UnityEngine;

namespace Fps.StateMachine
{
    public class DisconnectedState : DefaultState
    {
        public DisconnectedState(UIManager manager, ConnectionStateMachine owner) : base(manager, owner)
        {
        }

        public override void StartState()
        {
            Object.Destroy(Owner.ClientConnector.gameObject);
            Owner.ClientConnector = null;
            ScreenManager.DefaultConnectButton.enabled = true;
            ScreenManager.DefaultConnectButton.onClick.AddListener(Connect);
            Manager.ShowFrontEnd();
            ScreenManager.SwitchToDefaultScreen();
            Animator.SetTrigger("Disconnected");
        }

        public override void ExitState()
        {
            ScreenManager.DefaultConnectButton.onClick.RemoveListener(Connect);
        }

        private void Connect()
        {
            Animator.SetTrigger("Retry");
            Owner.SetState(new DefaultConnectState(Manager, Owner));
        }
    }
}
