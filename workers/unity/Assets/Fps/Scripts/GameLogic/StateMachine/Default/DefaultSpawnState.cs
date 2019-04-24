using Improbable.PlayerLifecycle;
using Improbable.Worker.CInterop;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            ScreenManager.DefaultConnectButton.onClick.AddListener(SpawnPlayer);
            ScreenManager.DefaultConnectButton.enabled = true;
        }

        public override void Tick()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ScreenManager.DefaultConnectButton.onClick.Invoke();
            }
        }

        public override void ExitState()
        {
            ScreenManager.DefaultConnectButton.onClick.RemoveListener(SpawnPlayer);
        }

        private void OnPlayerResponse(PlayerCreator.CreatePlayer.ReceivedResponse response)
        {
            if (response.StatusCode == StatusCode.Success)
            {
                Owner.SetState(new DefaultPlayState(Manager, Owner));
            }
            else
            {
                ScreenManager.DefaultConnectButton.enabled = true;
                Animator.SetTrigger("FailedToSpawn");
            }
        }

        private void SpawnPlayer()
        {
            ScreenManager.DefaultConnectButton.enabled = false;
            Blackboard.ClientConnector.SpawnPlayer("Local Player", OnPlayerResponse);
            Animator.SetTrigger("Connecting");
        }
    }
}
