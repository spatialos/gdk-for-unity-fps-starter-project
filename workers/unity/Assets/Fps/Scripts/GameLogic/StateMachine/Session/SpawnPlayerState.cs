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

            Blackboard.ClientConnector.SpawnPlayer(Manager.ScreenManager.PlayerNameInputField.text, OnCreatePlayerResponse);
        }

        private void OnCreatePlayerResponse(PlayerCreator.CreatePlayer.ReceivedResponse response)
        {
            if (response.StatusCode == StatusCode.Success)
            {
                Owner.SetState(new PlayState(Manager, Owner));
            }
            else
            {
                Manager.ScreenManager.LobbyStatus.ShowSpawningFailedText(response.Message);
                UnityObjectDestroyer.Destroy(Blackboard.ClientConnector);
                Blackboard.ClientConnector = null;
                Owner.SetState(Owner.StartState);
            }
        }
    }
}
