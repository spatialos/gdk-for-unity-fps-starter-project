using UnityEngine;

namespace Fps
{
    public abstract class DefaultState : State
    {
        protected readonly ConnectionStateMachine Owner;
        protected readonly UIManager Manager;
        protected readonly ScreenManager ScreenManager;
        protected readonly Animator Animator;

        protected Blackboard Blackboard;

        protected DefaultState(UIManager manager, ConnectionStateMachine owner)
        {
            Manager = manager;
            ScreenManager = Manager.ScreenManager;
            Owner = owner;
            Blackboard = Owner.Blackboard;

            Animator = manager.GetComponentInChildren<Animator>(true);
        }
    }
}
