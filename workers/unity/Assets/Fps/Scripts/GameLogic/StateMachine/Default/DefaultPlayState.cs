namespace Fps
{
    public class DefaultPlayState : DefaultState
    {
        public DefaultPlayState(UIManager manager, ConnectionStateMachine owner) : base(manager, owner)
        {
        }

        public override void StartState()
        {
            Owner.ClientConnector.Worker.OnDisconnect += WorkerOnDisconnect;
            Manager.ShowGameView();
        }

        public override void Tick()
        {
        }

        public override void ExitState()
        {
        }

        private void WorkerOnDisconnect(string obj)
        {
            Owner.ClientConnector = null;
            Owner.SetState(Owner.StartState);
        }
    }
}
