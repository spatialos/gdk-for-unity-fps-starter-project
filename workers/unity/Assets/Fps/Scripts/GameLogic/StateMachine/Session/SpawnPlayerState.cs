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
            lobbyScreenManager = manager.ScreenManager.LobbyScreenManager;
        }

        public override void StartState()
        {
            lobbyScreenManager.ShowSpawningText();
            lobbyScreenManager.StartButton.enabled = false;

            Owner.Blackboard.ClientConnector.SpawnPlayer(Manager.ScreenManager.LobbyScreenManager.GetPlayerName(), OnCreatePlayerResponse);
        }

        public override void ExitState()
        {
        }

        private void OnCreatePlayerResponse(PlayerCreator.CreatePlayer.ReceivedResponse response)
        {
            if (response.StatusCode == StatusCode.Success)
            {
                Owner.SetState(new PlayState(Manager, Owner));
            }
            else
            {
                Manager.ScreenManager.LobbyScreenManager.ShowSpawningFailedText(response.Message);
                UnityObjectDestroyer.Destroy(Owner.Blackboard.ClientConnector);
                Owner.Blackboard.ClientConnector = null;
                Owner.SetState(Owner.StartState);
            }
        }
    }
}
