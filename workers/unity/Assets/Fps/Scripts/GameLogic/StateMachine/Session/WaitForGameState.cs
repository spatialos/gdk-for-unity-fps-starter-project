using System;
using Improbable.Gdk.Core;
using Improbable.Gdk.Session;

namespace Fps
{
    public class WaitForGameState : SessionState
    {
        private readonly LobbyScreenManager lobbyScreenManager;

        public WaitForGameState(UIManager manager, ConnectionStateMachine owner) : base(manager, owner)
        {
            lobbyScreenManager = manager.FrontEndController.lobbyScreenManager;
        }

        public override void StartState()
        {
            lobbyScreenManager.startButton.onClick.AddListener(StartSession);
            lobbyScreenManager.cancelButton.onClick.AddListener(CancelSession);
            lobbyScreenManager.ShowWaitForGameText();
        }

        public override void ExitState()
        {
            lobbyScreenManager.startButton.onClick.RemoveListener(StartSession);
            lobbyScreenManager.cancelButton.onClick.RemoveListener(CancelSession);
        }

        public override void Tick()
        {
            var status = GetStatus();
            switch (status)
            {
                case Status.LOBBY:
                    var state = new WaitForGameState(Manager, Owner);
                    var nextState = new GetPlayerIdentityTokenState(state, Manager, Owner);
                    Owner.SetState(nextState);
                    break;
                case Status.RUNNING:
                    lobbyScreenManager.UpdateHintText(true);
                    var isValid = lobbyScreenManager.IsValidName();
                    lobbyScreenManager.startButton.enabled = status == Status.RUNNING && isValid;
                    lobbyScreenManager.ShowGameReadyText();
                    break;
                case Status.STOPPING:
                case Status.STOPPED:
                    Owner.DestroyClientWorker();
                    Owner.SetState(Owner.StartState);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void StartSession()
        {
            Owner.SetState(new SpawnPlayerState(Manager, Owner));
        }

        private void CancelSession()
        {
            Owner.DestroyClientWorker();
            Owner.SetState(Owner.StartState);
        }

        private Status GetStatus()
        {
            foreach (var loginToken in Owner.LoginTokens)
            {
                if (loginToken.DeploymentName == Owner.ClientConnector.Deployment)
                {
                    foreach (var tag in loginToken.Tags)
                    {
                        if (!tag.Contains("status"))
                        {
                            continue;
                        }

                        switch (tag)
                        {
                            case "status_lobby":
                                return Status.LOBBY;
                            case "status_running":
                                return Status.RUNNING;
                            case "status_stopping":
                                return Status.STOPPING;
                            default:
                                return Status.STOPPED;
                        }
                    }
                }
            }

            return Status.STOPPED;
        }
    }
}
