using Improbable.Gdk.Core;
using Improbable.PlayerLifecycle;
using Improbable.Worker.CInterop;
using UnityEngine;

namespace Fps
{
    public class DefaultSpawnState : DefaultState
    {
        public DefaultSpawnState(UIManager manager, ConnectionStateMachine owner) : base(manager, owner)
        {
        }

        public override void StartState()
        {
            DefaultConnectScreenManager.ConnectButton.enabled = true;
            DefaultConnectScreenManager.ConnectButton.onClick.AddListener(SpawnPlayer);
            SpawnPlayer();
        }

        public override void Tick()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                DefaultConnectScreenManager.ConnectButton.onClick.Invoke();
            }
        }

        public override void ExitState()
        {
            DefaultConnectScreenManager.ConnectButton.onClick.RemoveListener(SpawnPlayer);
        }

        private void OnPlayerResponse(PlayerCreator.CreatePlayer.ReceivedResponse response)
        {
            if (response.StatusCode == StatusCode.Success)
            {
                Owner.SetState(new DefaultPlayState(Manager, Owner));
            }
            else
            {
                DefaultConnectScreenManager.ConnectButton.enabled = true;
                Animator.SetTrigger("FailedToSpawn");
            }
        }

        private void SpawnPlayer()
        {
            DefaultConnectScreenManager.ConnectButton.enabled = false;
            Owner.ClientConnector.SpawnPlayerAction("Local Player", OnPlayerResponse);
            Animator.SetTrigger("Connecting");
        }
    }
}
