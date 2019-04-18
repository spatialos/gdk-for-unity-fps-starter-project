namespace Fps
{
    public abstract class SessionState : State
    {
        protected readonly UIManager Manager;
        protected readonly ScreenManager ScreenManager;
        protected readonly ConnectionStateMachine Owner;
        protected Blackboard Blackboard;

        protected SessionState(UIManager manager, ConnectionStateMachine owner)
        {
            Manager = manager;
            ScreenManager = manager.ScreenManager;
            Owner = owner;
            Blackboard = Owner.Blackboard;
        }
    }
}
