using Improbable.PlayerLifecycle;
using Improbable.Worker.CInterop;

namespace Fps
{
    public class SpawnPlayerState : SessionState
    {
        private readonly LobbyScreenManager lobbyScreenManager;

        public SpawnPlayerState(UIManager manager, ConnectionStateMachine owner) : base(manager, owner)
        {
            lobbyScreenManager = manager.ScreenManager.LobbyScreenManager;
        }

        public override void StartState()
        {
            lobbyScreenManager.ShowSpawningText();
            lobbyScreenManager.StartButton.enabled = false;

            Owner.ClientConnector.SpawnPlayerAction(Manager.ScreenManager.LobbyScreenManager.GetPlayerName(), CreatedPlayer);
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
                Manager.ScreenManager.LobbyScreenManager.ShowSpawningFailedText(response.Message);
                Owner.DestroyClientWorker();
                Owner.SetState(Owner.StartState);
            }
        }
    }
}
