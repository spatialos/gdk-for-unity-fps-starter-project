using System;
using System.Linq;
using Improbable.Gdk.Core;
using Improbable.Gdk.Core.Commands;
using Improbable.Gdk.Session;
using Improbable.Worker.CInterop.Query;
using UnityEngine;

namespace Fps
{
    public class WaitForGameState : SessionState
    {
        public WaitForGameState(UIManager manager, ConnectionStateMachine owner) : base(manager, owner)
        {
        }

        public override void StartState()
        {
            ScreenManager.StartGameButton.onClick.AddListener(StartSession);
            ScreenManager.CancelLobbyButton.onClick.AddListener(CancelSession);
            ScreenManager.LobbyStatus.ShowWaitForGameText();
        }

        public override void ExitState()
        {
            ScreenManager.StartGameButton.onClick.RemoveListener(StartSession);
            ScreenManager.CancelLobbyButton.onClick.RemoveListener(CancelSession);
        }

        public override void Tick()
        {
            switch (Blackboard.SessionStatus)
            {
                case Status.LOBBY:
                    var state = new WaitForGameState(Manager, Owner);
                    var nextState = new QuerySessionStatusState(state, Manager, Owner);
                    Owner.SetState(nextState);
                    break;
                case Status.RUNNING:
                    ScreenManager.StartGameButton.enabled = IsValidName();
                    ScreenManager.LobbyStatus.ShowGameReadyText();
                    break;
                case Status.STOPPING:
                case Status.STOPPED:
                    CancelSession();
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
            UnityObjectDestroyer.Destroy(Blackboard.ClientConnector);
            Blackboard.ClientConnector = null;
            Owner.SetState(Owner.StartState);
        }

        private bool IsValidName()
        {
            UpdateHintText();
            return ScreenManager.PlayerNameInputField.text.Length >= 3;
        }

        private void UpdateHintText()
        {
            var nameLength = ScreenManager.PlayerNameInputField.text.Trim().Length;

            if (nameLength == 0)
            {
                ScreenManager.PlayerNameHintText.text = "You must enter a name to play";
            }
            else if (nameLength < 3)
            {
                ScreenManager.PlayerNameHintText.text = "Minimum 3 characters required";
            }
            else
            {
                ScreenManager.PlayerNameHintText.text = string.Empty;
            }
        }
    }
}
