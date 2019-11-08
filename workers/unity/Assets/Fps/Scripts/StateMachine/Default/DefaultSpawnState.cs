using Fps.UI;
using Improbable.Gdk.PlayerLifecycle;
using Improbable.Worker.CInterop;

namespace Fps.StateMachine
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
            Blackboard.ClientConnector.Worker.OnDisconnect += WorkerOnDisconnect;
        }

        public override void ExitState()
        {
            ScreenManager.DefaultConnectButton.onClick.RemoveListener(SpawnPlayer);
            Blackboard.ClientConnector.Worker.OnDisconnect -= WorkerOnDisconnect;
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

        private void WorkerOnDisconnect(string reason)
        {
            Owner.SetState(new DisconnectedState(Manager, Owner));
        }
    }
}
