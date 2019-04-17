namespace Fps
{
    public abstract class SessionState : State
    {
        protected readonly UIManager Manager;
        protected readonly ConnectionStateMachine Owner;

        protected SessionState(UIManager manager, ConnectionStateMachine owner)
        {
            Manager = manager;
            Owner = owner;
        }
    }
}
