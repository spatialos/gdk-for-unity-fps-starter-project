using Improbable.Gdk.Core;
using Improbable.PlayerLifecycle;
using Improbable.Worker.CInterop;
using UnityEngine;

namespace Fps
{
    public class DefaultSpawnState : DefaultState
    {
        public DefaultSpawnState(ScreenUIController controller, ConnectionStateMachine owner) : base(controller, owner)
        {
        }

        public override void StartState()
        {
            ConnectScreenController.ConnectButton.onClick.AddListener(SpawnPlayer);
            SpawnPlayer();
        }

        public override void Tick()
        {
            ConnectScreenController.ConnectButton.onClick.AddListener(SpawnPlayer);
        }

        public override void ExitState()
        {
        }

        private void OnPlayerResponse(PlayerCreator.CreatePlayer.ReceivedResponse obj)
        {
            if (obj.StatusCode == StatusCode.Success)
            {
                Controller.ShowGameView();
            }
            else
            {
                ConnectScreenController.ConnectButton.enabled = true;
                Animator.SetTrigger("FailedToSpawn");
            }
        }

        private void SpawnPlayer()
        {
            ConnectScreenController.ConnectButton.enabled = false;
            Owner.ClientConnector.SpawnPlayerAction("Local Player", OnPlayerResponse);
            Animator.SetTrigger("Connecting");
        }
    }
}
