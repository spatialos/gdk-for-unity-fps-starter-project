using System.Diagnostics;

namespace Fps
{
    public class DefaultPlayState : DefaultState
    {
        public DefaultPlayState(UIManager manager, ConnectionStateMachine owner) : base(manager, owner)
        {
        }

        public override void StartState()
        {
            Blackboard.ClientConnector.Worker.OnDisconnect += WorkerOnDisconnect;
            Manager.InGameManager.Timer.SetActive(false);
            Manager.ShowGameView();
        }

        public override void ExitState()
        {
            Blackboard.ClientConnector.Worker.OnDisconnect -= WorkerOnDisconnect;
        }

        private void WorkerOnDisconnect(string reason)
        {
            Owner.SetState(new DisconnectedState(Manager, Owner));
        }
    }
}
