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

        public PlayState(UIManager manager, ConnectionStateMachine owner) : base(manager, owner)
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
                Owner.SetState(new ResultsState(Manager, Owner));
            }
        }
    }
}
