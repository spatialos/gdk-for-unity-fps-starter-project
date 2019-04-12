using Improbable.Gdk.Core;
using Improbable.PlayerLifecycle;
using Improbable.Worker.CInterop;

namespace Fps
{
    public class SpawnPlayerState : SessionState
    {
        private readonly LobbyScreenController lobbyScreenController;

        public SpawnPlayerState(ScreenUIController controller, ConnectionStateMachine owner) : base(controller, owner)
        {
            lobbyScreenController = controller.FrontEndController.LobbyScreenController;
        }

        public override void StartState()
        {
            lobbyScreenController.ConnectionStatusUIController.ShowSpawningText();
            lobbyScreenController.startButton.enabled = false;

            Owner.ClientConnector.SpawnPlayerAction(Controller.FrontEndController.LobbyScreenController.GetPlayerName(), CreatedPlayer);
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
                Controller.ShowGameView();
                Owner.SetState(new PlayState(Controller, Owner));
            }
            else
            {
                Controller.FrontEndController.LobbyScreenController.ConnectionStatusUIController.ShowSpawningFailedText(response.Message);
                Owner.DestroyClientWorker();
                Owner.SetState(Owner.StartState);
            }
        }
    }
}
