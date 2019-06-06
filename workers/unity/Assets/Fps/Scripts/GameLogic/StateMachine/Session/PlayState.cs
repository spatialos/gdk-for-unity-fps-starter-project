using Improbable.Gdk.Session;

namespace Fps
{
    public class PlayState : SessionState
    {
        private readonly TrackPlayerSystem trackPlayerSystem;

        public PlayState(UIManager manager, ConnectionStateMachine owner) : base(manager, owner)
        {
            trackPlayerSystem = Blackboard.ClientConnector.Worker.World.GetExistingSystem<TrackPlayerSystem>();
        }

        public override void StartState()
        {
            Blackboard.ClientConnector.Worker.OnDisconnect += OnDisconnect;
        }

        private void OnDisconnect(string disconnectReason)
        {
            Blackboard.ClientConnector = null;
            Manager.ScreenManager.StartStatus.ShowWorkerDisconnectedText();
            Owner.SetState(Owner.StartState, 2f);
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
