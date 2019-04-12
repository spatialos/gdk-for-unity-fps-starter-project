using System.Diagnostics;
using Improbable.Gdk.Core;
using Improbable.Gdk.Session;
using Improbable.PlayerLifecycle;
using Improbable.Worker.CInterop;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Fps
{
    public class PlayState : SessionState
    {
        private readonly TrackPlayerSystem trackPlayerSystem;

        public PlayState(ScreenUIController controller, ConnectionStateMachine owner) : base(controller, owner)
        {
            trackPlayerSystem = Owner.ClientConnector.Worker.World.GetExistingManager<TrackPlayerSystem>();
        }

        public override void StartState()
        {
        }

        public override void ExitState()
        {
        }

        public override void Tick()
        {
            Status? status = trackPlayerSystem.SessionStatus;
            if (status == Status.STOPPING)
            {
                Owner.SetState(new ResultsState(Controller, Owner));
            }
        }
    }
}
