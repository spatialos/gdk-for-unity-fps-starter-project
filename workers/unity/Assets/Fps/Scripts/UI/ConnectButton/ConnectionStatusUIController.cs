using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Fps
{
    public class ConnectionStatusUIController : MonoBehaviour
    {
        public GameObject SpinnerSymbol;
        public GameObject ErrorSymbol;
        public GameObject SuccessSymbol;
        public Text StatusText;

        public string DeploymentListAvailableText = "Deployments available";
        public string GettingDeploymentListText = "Getting deployment list...";
        public string FailedToGetDeploymentListText = "Failed to get deployment list!";
        public string ConnectingText = "Searching for deployment to join...";
        public string ConnectionFailedText = "Failed to join deployment!";
        public string ConnectedText = "Connected";
        public string WaitingForGameStartText = "Game starting in {0:#} SECONDS";
        public string WaitingForGameToStartText_NoNumber = "Game starting...";
        public string GameReadyText = "Game begun! Ready to spawn.";
        public string SpawningText = "Joining deployment...";
        public string SpawningFailedText = "Failed to spawn player!";
        public string WorkerDisconnectedText = "Worker was disconnected";

        private void Awake()
        {
            ConnectionStateReporter.OnConnectionStateChange += OnConnectionStateChange;
        }

        public void OnEnable()
        {
            OnConnectionStateChange(ConnectionStateReporter.CurrentState, ConnectionStateReporter.CurrentInformation);
        }

        public void OnConnectionStateChange(ConnectionStateReporter.State state, string information)
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }

            switch (state)
            {
                case ConnectionStateReporter.State.None:
                    State_None();
                    break;
                case ConnectionStateReporter.State.GettingDeploymentList:
                    State_GettingDeploymentList();
                    break;
                case ConnectionStateReporter.State.DeploymentListAvailable:
                    State_DeploymentListAvailable();
                    break;
                case ConnectionStateReporter.State.FailedToGetDeploymentList:
                    State_FailedToGetDeploymentList(information);
                    break;
                case ConnectionStateReporter.State.Connecting:
                    State_Connecting();
                    break;
                case ConnectionStateReporter.State.Connected:
                    State_Connected();
                    break;
                case ConnectionStateReporter.State.ConnectionFailed:
                    State_ConnectionFailed(information);
                    break;
                case ConnectionStateReporter.State.WaitingForGameStart:
                    State_WaitingForGameStart();
                    break;
                case ConnectionStateReporter.State.GameReady:
                    State_GameReady();
                    break;
                case ConnectionStateReporter.State.Spawning:
                    State_Spawning();
                    break;
                case ConnectionStateReporter.State.Spawned:
                    break;
                case ConnectionStateReporter.State.SpawningFailed:
                    State_SpawningFailed(information);
                    break;
                case ConnectionStateReporter.State.WorkerDisconnected:
                    State_WorkerDisconnected();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private void State_None()
        {
            StatusText.text = string.Empty;
            SetSymbol(null);
        }

        private void State_GettingDeploymentList()
        {
            StatusText.text = GettingDeploymentListText;
            SetSymbol(SpinnerSymbol);
        }

        private void State_DeploymentListAvailable()
        {
            StatusText.text = DeploymentListAvailableText;
            SetSymbol(SuccessSymbol);
        }

        private void State_FailedToGetDeploymentList(string error)
        {
            StatusText.text = $"{FailedToGetDeploymentListText}\n Error: {error}";
            SetSymbol(ErrorSymbol);
        }

        private void State_Connecting()
        {
            StatusText.text = ConnectingText;
            SetSymbol(SpinnerSymbol);
        }

        private void State_Connected()
        {
            StatusText.text = ConnectedText;
            SetSymbol(SuccessSymbol);
        }

        private void State_ConnectionFailed(string error)
        {
            StatusText.text = $"{ConnectionFailedText}\nError: {error}";
            SetSymbol(ErrorSymbol);
        }

        private void State_WaitingForGameStart()
        {
            SetSymbol(SpinnerSymbol);
            StartCoroutine(nameof(UpdateGameStartTime));
        }

        private void State_GameReady()
        {
            SetSymbol(SuccessSymbol);
            StatusText.text = GameReadyText;
        }

        private void State_Spawning()
        {
            StatusText.text = SpawningText;
        }

        private void State_SpawningFailed(string error)
        {
            StatusText.text = $"{SpawningFailedText}\nError: {error}";
            SetSymbol(ErrorSymbol);
        }

        private void State_WorkerDisconnected()
        {
            StatusText.text = WorkerDisconnectedText;
            SetSymbol(ErrorSymbol);
        }

        private void SetSymbol(GameObject symbol)
        {
            Debug.Assert(symbol == null
                || symbol == ErrorSymbol
                || symbol == SpinnerSymbol
                || symbol == SuccessSymbol);

            if (ErrorSymbol != null)
            {
                ErrorSymbol.SetActive(symbol == ErrorSymbol);
            }

            if (SpinnerSymbol != null)
            {
                SpinnerSymbol.SetActive(symbol == SpinnerSymbol);
            }

            if (SuccessSymbol != null)
            {
                SuccessSymbol.SetActive(symbol == SuccessSymbol);
            }
        }

        private IEnumerator UpdateGameStartTime()
        {
            while (ConnectionStateReporter.CurrentState == ConnectionStateReporter.State.WaitingForGameStart)
            {
                var timeUntilStart = ConnectionStateReporter.TimeUntilGameStart;
                StatusText.text = timeUntilStart < 1
                    ? WaitingForGameToStartText_NoNumber
                    : string.Format(WaitingForGameStartText, ConnectionStateReporter.TimeUntilGameStart);

                yield return new WaitForSeconds(.5f);
            }
        }
    }
}
