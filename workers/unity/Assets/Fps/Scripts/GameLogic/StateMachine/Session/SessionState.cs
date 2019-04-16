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
        protected readonly UIManager Manager;
        protected readonly ConnectionStateMachine Owner;

        protected SessionState(UIManager manager, ConnectionStateMachine owner)
        {
            Manager = manager;
            Owner = owner;
        }

        protected void ShowErrorMessage(string errorMessage)
        {
            Manager.FrontEndController.startScreenManager.ShowFailedToGetDeploymentsText(errorMessage);
            Manager.FrontEndController.lobbyScreenManager.ShowFailedToGetDeploymentsText(errorMessage);
        }
    }
}
