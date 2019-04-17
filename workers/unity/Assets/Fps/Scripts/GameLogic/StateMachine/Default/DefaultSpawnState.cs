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
            Animator.SetTrigger("Ready");
            DefaultConnectScreenManager.ConnectButton.onClick.AddListener(SpawnPlayer);
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
            Owner.Blackboard.ClientConnector.SpawnPlayer("Local Player", OnPlayerResponse);
            Animator.SetTrigger("Connecting");
        }
    }
}
