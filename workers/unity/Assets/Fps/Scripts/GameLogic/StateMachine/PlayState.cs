using System.Diagnostics;
using Improbable.Gdk.Core;
using Improbable.Gdk.Session;
using Improbable.PlayerLifecycle;
using Improbable.Worker.CInterop;
using UnityEngine;

namespace Fps
{
    public class PlayState : SessionState
    {
        private readonly TrackPlayerSystem trackPlayerSystem;
        private readonly LobbyScreenController lobbyScreenController;

        private bool queryDeploymentStatus;
        private bool hasGameStarted;
        private bool hasValidName;
        private float time;
        private ClientWorkerConnector connector;

        public PlayState(ClientWorkerConnector connector, ScreenUIController controller, ConnectionStateMachine owner) : base(controller, owner)
        {
            this.connector = connector;
            trackPlayerSystem = connector.Worker.World.GetExistingManager<TrackPlayerSystem>();
            lobbyScreenController = controller.FrontEndController.LobbyScreenController;
        }

        public override void StartState()
        {
            Controller.FrontEndController.LobbyScreenController.playerNameInputController.OnNameChanged += OnNameUpdated;
            lobbyScreenController.startButton.onClick.AddListener(StartSession);
            lobbyScreenController.cancelButton.onClick.AddListener(CancelSession);
            lobbyScreenController.ConnectionStatusUIController.ShowWaitForGameText();
            queryDeploymentStatus = true;
        }

        public override void ExitState()
        {
            lobbyScreenController.startButton.onClick.RemoveListener(StartSession);
            lobbyScreenController.cancelButton.onClick.RemoveListener(CancelSession);
        }

        public override void Tick()
        {
            time += Time.deltaTime;

            if (!queryDeploymentStatus || time < 1f)
            {
                return;
            }

            time = 0;

            Status? status;
            if (hasGameStarted)
            {
                status = trackPlayerSystem.SessionStatus;
            }
            else
            {
                status = GetStatus();
                if (status == Status.RUNNING)
                {
                    lobbyScreenController.startButton.enabled = status == Status.RUNNING && hasValidName;
                    lobbyScreenController.ConnectionStatusUIController.ShowGameReadyText();
                }
            }

            if (status == Status.STOPPING)
            {
                Owner.SetState(new ResultsState(connector, Controller, Owner));
            }
        }

        private void OnNameUpdated(bool isValid)
        {
            hasValidName = isValid;
        }

        private void StartSession()
        {
            queryDeploymentStatus = false;
            lobbyScreenController.startButton.enabled = false;
            connector.SpawnPlayerAction(Controller.FrontEndController.LobbyScreenController.GetPlayerName(), CreatedPlayer);
            lobbyScreenController.ConnectionStatusUIController.ShowSpawningText();
        }

        private void CreatedPlayer(PlayerCreator.CreatePlayer.ReceivedResponse obj)
        {
            if (obj.StatusCode == StatusCode.Success)
            {
                Controller.ShowGameView();
                hasGameStarted = true;
                queryDeploymentStatus = true;
            }
            else
            {
                UnityEngine.Debug.LogWarning(obj.StatusCode);
                UnityObjectDestroyer.Destroy(connector.gameObject);
                connector = null;
                Owner.SetState(new InitState(Controller, Owner));
            }
        }

        private void CancelSession()
        {
            UnityObjectDestroyer.Destroy(connector.gameObject);
            connector = null;
            Owner.SetState(new InitState(Controller, Owner));
        }

        private Status GetStatus()
        {
            var pit = GetDevelopmentPlayerIdentityToken();
            var loginTokens = GetDevelopmentLoginTokens(WorkerUtils.UnityClient, pit);
            foreach (var loginToken in loginTokens)
            {
                if (loginToken.DeploymentName == connector.Deployment)
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
