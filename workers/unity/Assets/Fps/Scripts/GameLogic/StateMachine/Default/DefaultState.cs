using Improbable.Gdk.Core;
using Improbable.PlayerLifecycle;
using Improbable.Worker.CInterop;
using UnityEngine;

namespace Fps
{
    public abstract class DefaultState : State
    {
        protected readonly ConnectionStateMachine Owner;
        protected readonly ScreenUIController Controller;
        protected readonly ConnectScreenController ConnectScreenController;
        protected readonly Animator Animator;

        protected DefaultState(ScreenUIController controller, ConnectionStateMachine owner)
        {
            Controller = controller;
            ConnectScreenController = controller.FrontEndController.ConnectScreenController;
            Owner = owner;

            Animator = ConnectScreenController.GetComponentInChildren<Animator>(true);
        }
    }
}
