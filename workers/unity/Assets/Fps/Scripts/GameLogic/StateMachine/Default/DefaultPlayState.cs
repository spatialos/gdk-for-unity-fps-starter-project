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

        private void WorkerOnDisconnect(string obj)
        {
            Blackboard.ClientConnector = null;
            Owner.SetState(Owner.StartState);
        }
    }
}
