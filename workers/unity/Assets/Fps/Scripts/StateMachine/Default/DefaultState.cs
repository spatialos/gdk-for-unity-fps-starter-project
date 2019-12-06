using Fps.UI;
using UnityEngine;

namespace Fps.StateMachine
{
    public abstract class DefaultState : State
    {
        protected readonly ConnectionStateMachine Owner;
        protected readonly UIManager Manager;
        protected readonly ScreenManager ScreenManager;
        protected readonly Animator Animator;

        protected DefaultState(UIManager manager, ConnectionStateMachine owner)
        {
            Manager = manager;
            ScreenManager = Manager.ScreenManager;
            Owner = owner;

            Animator = manager.ScreenManager.DefaultConnectButton.GetComponent<Animator>();
        }

        public override void Tick()
        {
            if (Input.GetKeyDown(KeyCode.Space) && ScreenManager.DefaultConnectButton.enabled)
            {
                ScreenManager.DefaultConnectButton.onClick.Invoke();
            }
        }
    }
}
