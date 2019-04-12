using System.Collections.Generic;
using System.Resources;
using Improbable.Gdk.Core;
using Improbable.Worker.CInterop;
using Improbable.Worker.CInterop.Alpha;
using UnityEngine;

namespace Fps
{
    public abstract class SessionState : State
    {
        protected readonly ScreenUIController Controller;
        protected readonly ConnectionStateMachine Owner;

        protected SessionState(ScreenUIController controller, ConnectionStateMachine owner)
        {
            Controller = controller;
            Owner = owner;
        }

        protected void ShowErrorMessage(string errorMessage)
        {
            Controller.FrontEndController.SessionScreenController.ConnectionStatusUIController.ShowFailedToGetDeploymentsText(errorMessage);
            Controller.FrontEndController.LobbyScreenController.ConnectionStatusUIController.ShowFailedToGetDeploymentsText(errorMessage);
        }
    }
}
