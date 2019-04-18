using Improbable.Gdk.Core;
using Improbable.PlayerLifecycle;
using Improbable.Worker.CInterop;

namespace Fps
{
    public class SpawnPlayerState : SessionState
    {
        public SpawnPlayerState(UIManager manager, ConnectionStateMachine owner) : base(manager, owner)
        {
        }

        public override void StartState()
        {
            ScreenManager.LobbyStatus.ShowSpawningText();
            ScreenManager.StartGameButton.enabled = false;

            Blackboard.ClientConnector.SpawnPlayer(ScreenManager.PlayerNameInputField.text, OnCreatePlayerResponse);
            Blackboard.PlayerName = ScreenManager.PlayerNameInputField.text;
        }

        private void OnCreatePlayerResponse(PlayerCreator.CreatePlayer.ReceivedResponse response)
        {
            if (response.StatusCode == StatusCode.Success)
            {
                Owner.SetState(new PlayState(Manager, Owner));
            }
            else
            {
                ScreenManager.LobbyStatus.ShowSpawningFailedText(response.Message);
                UnityObjectDestroyer.Destroy(Blackboard.ClientConnector);
                Blackboard.ClientConnector = null;
                Owner.SetState(Owner.StartState);
            }
        }
    }
}
