using Fps.UI;

namespace Fps.StateMachine
{
    public abstract class SessionState : State
    {
        protected readonly UIManager Manager;
        protected readonly ScreenManager ScreenManager;
        protected readonly ConnectionStateMachine Owner;
        protected readonly Blackboard Blackboard;

        protected SessionState(UIManager manager, ConnectionStateMachine owner)
        {
            Manager = manager;
            ScreenManager = manager.ScreenManager;
            Owner = owner;
            Blackboard = Owner.Blackboard;
        }
    }
}
