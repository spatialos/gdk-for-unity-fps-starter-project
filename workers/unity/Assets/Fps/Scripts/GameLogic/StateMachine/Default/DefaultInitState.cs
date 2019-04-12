using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Improbable.Worker.CInterop.Alpha;

namespace Fps
{
    public class DefaultInitState : DefaultState
    {
        public DefaultInitState(ScreenUIController controller, ConnectionStateMachine owner) : base(controller, owner)
        {
        }

        public override void StartState()
        {
            Controller.FrontEndController.SwitchToDefaultConnectScreen();
            Owner.SetState(new DefaultConnectState(Controller, Owner));
        }

        public override void ExitState()
        {
        }

        public override void Tick()
        {
        }
    }
}
