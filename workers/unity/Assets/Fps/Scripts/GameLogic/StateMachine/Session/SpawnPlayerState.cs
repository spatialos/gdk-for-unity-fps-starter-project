using Improbable.Gdk.Core;
using Improbable.PlayerLifecycle;
using Improbable.Worker.CInterop;

namespace Fps
{
    public class SpawnPlayerState : SessionState
    {
        private readonly LobbyScreenManager lobbyScreenManager;

        public SpawnPlayerState(UIManager manager, ConnectionStateMachine owner) : base(manager, owner)
        {
            lobbyScreenManager = manager.FrontEndController.lobbyScreenManager;
        }

        public override void StartState()
        {
            lobbyScreenManager.ShowSpawningText();
            lobbyScreenManager.startButton.enabled = false;

            Owner.ClientConnector.SpawnPlayerAction(Manager.FrontEndController.lobbyScreenManager.GetPlayerName(), CreatedPlayer);
        }

        public override void ExitState()
        {
        }

        public override void Tick()
        {
        }

        private void CreatedPlayer(PlayerCreator.CreatePlayer.ReceivedResponse response)
        {
            if (response.StatusCode == StatusCode.Success)
            {
                Manager.ShowGameView();
                Owner.SetState(new PlayState(Manager, Owner));
            }
            else
            {
                Manager.FrontEndController.lobbyScreenManager.ShowSpawningFailedText(response.Message);
                Owner.DestroyClientWorker();
                Owner.SetState(Owner.StartState);
            }
        }
    }
}
