namespace Fps
{
    public class DefaultPlayState : DefaultState
    {
        public DefaultPlayState(UIManager manager, ConnectionStateMachine owner) : base(manager, owner)
        {
        }

        public override void StartState()
        {
            Owner.Blackboard.ClientConnector.Worker.OnDisconnect += WorkerOnDisconnect;
            Manager.InGameManager.Timer.SetActive(false);
            Manager.ShowGameView();
        }

        public override void ExitState()
        {
        }

        private void WorkerOnDisconnect(string obj)
        {
            Owner.Blackboard.ClientConnector = null;
            Owner.SetState(Owner.StartState);
        }
    }
}
