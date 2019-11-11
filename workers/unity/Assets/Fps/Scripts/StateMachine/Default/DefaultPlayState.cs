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
            Blackboard.ClientConnector.Worker.OnDisconnect += WorkerOnDisconnect;
            Blackboard.ClientConnector.OnLostPlayerEntity += LostPlayerEntity;
            Manager.InGameManager.Timer.SetActive(false);
            Manager.ShowGameView();
        }

        public override void ExitState()
        {
            Blackboard.ClientConnector.Worker.OnDisconnect -= WorkerOnDisconnect;
            Blackboard.ClientConnector.OnLostPlayerEntity -= LostPlayerEntity;
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
