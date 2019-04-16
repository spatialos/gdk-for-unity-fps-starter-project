using Improbable.Gdk.Core;
using Improbable.PlayerLifecycle;
using Improbable.Worker.CInterop;
using UnityEngine;

namespace Fps
{
    public abstract class DefaultState : State
    {
        protected readonly ConnectionStateMachine Owner;
        protected readonly UIManager Manager;
        protected readonly DefaultConnectScreenManager DefaultConnectScreenManager;
        protected readonly Animator Animator;

        protected DefaultState(UIManager manager, ConnectionStateMachine owner)
        {
            Manager = manager;
            DefaultConnectScreenManager = manager.FrontEndController.defaultConnectScreenManager;
            Owner = owner;

            Animator = DefaultConnectScreenManager.GetComponentInChildren<Animator>(true);
        }
    }
}
