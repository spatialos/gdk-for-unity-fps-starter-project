using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Improbable.Worker.CInterop.Alpha;

namespace Fps
{
    public class DefaultInitState : DefaultState
    {
        public DefaultInitState(UIManager manager, ConnectionStateMachine owner) : base(manager, owner)
        {
        }

        public override void StartState()
        {
            Manager.FrontEndController.SwitchToDefaultConnectScreen();
            Owner.SetState(new DefaultConnectState(Manager, Owner));
        }

        public override void ExitState()
        {
        }

        public override void Tick()
        {
        }
    }
}
