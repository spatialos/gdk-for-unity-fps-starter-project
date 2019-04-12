using System;
using Improbable.Gdk.Core;
using Improbable.Gdk.Session;

namespace Fps
{
    public class WaitForGameState : SessionState
    {
        private readonly LobbyScreenController lobbyScreenController;

        private bool hasValidName;

        public WaitForGameState(ScreenUIController controller, ConnectionStateMachine owner) : base(controller, owner)
        {
            lobbyScreenController = controller.FrontEndController.LobbyScreenController;
        }

        public override void StartState()
        {
            Controller.FrontEndController.LobbyScreenController.playerNameInputController.OnNameChanged += OnNameUpdated;
            lobbyScreenController.startButton.onClick.AddListener(StartSession);
            lobbyScreenController.cancelButton.onClick.AddListener(CancelSession);
            lobbyScreenController.ConnectionStatusUIController.ShowWaitForGameText();

            hasValidName = Controller.FrontEndController.LobbyScreenController.playerNameInputController.NameIsValid;
        }

        public override void ExitState()
        {
            lobbyScreenController.startButton.onClick.RemoveListener(StartSession);
            lobbyScreenController.cancelButton.onClick.RemoveListener(CancelSession);
        }

        public override void Tick()
        {
            var status = GetStatus();
            switch (status)
            {
                case Status.LOBBY:
                    var state = new WaitForGameState(Controller, Owner);
                    var nextState = new GetPlayerIdentityTokenState(state, Controller, Owner);
                    Owner.SetState(nextState);
                    break;
                case Status.RUNNING:
                    lobbyScreenController.startButton.enabled = status == Status.RUNNING && hasValidName;
                    lobbyScreenController.ConnectionStatusUIController.ShowGameReadyText();
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

        private void OnNameUpdated(bool isValid)
        {
            hasValidName = isValid;
        }

        private void StartSession()
        {
            Owner.SetState(new SpawnPlayerState(Controller, Owner));
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
