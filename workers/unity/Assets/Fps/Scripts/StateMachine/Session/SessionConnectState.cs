using Fps.UI;
using Fps.WorkerConnectors;
using UnityEngine;

namespace Fps.StateMachine
{
    public class SessionConnectState : SessionState
    {
        private string deployment;
        private readonly Color hintTextColor = new Color(1f, .4f, .4f);

        public SessionConnectState(string deployment, UIManager manager, ConnectionStateMachine owner) : base(manager, owner)
        {
            this.deployment = deployment;
        }

        public override void StartState()
        {
            if (string.IsNullOrEmpty(deployment))
            {
                deployment = SelectFirstDeployment();
            }

            if (string.IsNullOrEmpty(deployment))
            {
                Owner.SetState(Owner.StartState);
            }

            Manager.ScreenManager.SwitchToLobbyScreen();

            Manager.ScreenManager.LobbyStatus.ShowConnectingText();
            ScreenManager.StartGameButton.enabled = false;

            var clientWorker = Object.Instantiate(Owner.ClientWorkerConnectorPrefab, Owner.transform.position, Quaternion.identity);
            Blackboard.ClientConnector = clientWorker.GetComponent<ClientWorkerConnector>();
            Blackboard.ClientConnector.Connect(deployment);

            ScreenManager.PlayerNameHintText.text = string.Empty;
            ScreenManager.PlayerNameHintText.color = hintTextColor;
            ScreenManager.PlayerNameInputField.Select();
            ScreenManager.PlayerNameInputField.ActivateInputField();
        }

        public override void Tick()
        {
            if (!Blackboard.ClientConnector.HasConnected)
            {
                return;
            }

            var state = new WaitForGameState(Manager, Owner);
            var nextState = new QuerySessionStatusState(state, Manager, Owner);
            ScreenManager.LobbyStatus.ShowWaitForGameText();
            Owner.SetState(nextState);
        }

        private string SelectFirstDeployment()
        {
            foreach (var loginToken in Blackboard.LoginTokens)
            {
                if (loginToken.Tags.Contains("status_lobby") || loginToken.Tags.Contains("status_running"))
                {
                    return loginToken.DeploymentName;
                }
            }

            return string.Empty;
        }
    }
}
