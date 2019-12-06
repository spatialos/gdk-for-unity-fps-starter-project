using Fps.UI;

namespace Fps.StateMachine
{
    public class DefaultPlayState : DefaultState
    {
        public DefaultPlayState(UIManager manager, ConnectionStateMachine owner) : base(manager, owner)
        {
        }

        public override void StartState()
        {
            Owner.ClientConnector.Worker.OnDisconnect += WorkerOnDisconnect;
            Owner.ClientConnector.OnLostPlayerEntity += LostPlayerEntity;
            Manager.ShowGameView();
        }

        public override void ExitState()
        {
            Owner.ClientConnector.Worker.OnDisconnect -= WorkerOnDisconnect;
            Owner.ClientConnector.OnLostPlayerEntity -= LostPlayerEntity;
        }

        private void WorkerOnDisconnect(string reason)
        {
            Owner.SetState(new DisconnectedState(Manager, Owner));
        }

        private void LostPlayerEntity()
        {
            Manager.ShowFrontEnd();
            ScreenManager.SwitchToDefaultScreen();
            Animator.SetTrigger("Disconnected");
            Owner.SetState(new DisconnectedState(Manager, Owner));
        }
    }
}
