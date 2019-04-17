using System;
using System.Linq;
using Improbable.Gdk.Core;
using Improbable.Gdk.Core.Commands;
using Improbable.Gdk.Session;
using Improbable.Worker.CInterop.Query;

namespace Fps
{
    public class WaitForGameState : SessionState
    {
        private readonly LobbyScreenManager lobbyScreenManager;

        public WaitForGameState(UIManager manager, ConnectionStateMachine owner) : base(manager, owner)
        {
            lobbyScreenManager = manager.ScreenManager.LobbyScreenManager;
        }

        public override void StartState()
        {
            lobbyScreenManager.StartButton.onClick.AddListener(StartSession);
            lobbyScreenManager.CancelButton.onClick.AddListener(CancelSession);
            lobbyScreenManager.ShowWaitForGameText();
        }

        public override void ExitState()
        {
            lobbyScreenManager.StartButton.onClick.RemoveListener(StartSession);
            lobbyScreenManager.CancelButton.onClick.RemoveListener(CancelSession);
        }

        public override void Tick()
        {
            switch (Owner.Blackboard.SessionStatus)
            {
                case Status.LOBBY:
                    var state = new WaitForGameState(Manager, Owner);
                    var nextState = new QuerySessionStatusState(state, Manager, Owner);
                    Owner.SetState(nextState);
                    break;
                case Status.RUNNING:
                    lobbyScreenManager.UpdateHintText(hasGameBegun: true);
                    lobbyScreenManager.StartButton.enabled = Owner.Blackboard.SessionStatus == Status.RUNNING
                        && lobbyScreenManager.IsValidName();
                    lobbyScreenManager.ShowGameReadyText();
                    break;
                case Status.STOPPING:
                case Status.STOPPED:
                    CancelSession();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void StartSession()
        {
            Owner.SetState(new SpawnPlayerState(Manager, Owner));
        }

        private void CancelSession()
        {
            UnityObjectDestroyer.Destroy(Owner.Blackboard.ClientConnector);
            Owner.Blackboard.ClientConnector = null;
            Owner.SetState(Owner.StartState);
        }
    }
}
