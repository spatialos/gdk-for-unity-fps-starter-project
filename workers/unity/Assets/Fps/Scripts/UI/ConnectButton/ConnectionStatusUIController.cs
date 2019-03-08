using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
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
        public string GameStartingImminentText = "Game starting...";
        public string GameReadyText = "Game begun! Ready to spawn.";
        public string SpawningText = "Joining deployment...";
        public string SpawningFailedText = "Failed to spawn player!";
        public string WorkerDisconnectedText = "Worker was disconnected";

        private void Awake()
        {
            ConnectionStateReporter.OnConnectionStateChange += OnConnectionStateChange;
        }

        private void OnEnable()
        {
            OnConnectionStateChange(ConnectionStateReporter.CurrentState, ConnectionStateReporter.CurrentInformation);
        }

        private void OnDisable()
        {
            StopAllCoroutines();
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
                    StateNone();
                    break;
                case ConnectionStateReporter.State.GettingDeploymentList:
                    StateGettingDeploymentList();
                    break;
                case ConnectionStateReporter.State.DeploymentListAvailable:
                    StateDeploymentListAvailable();
                    break;
                case ConnectionStateReporter.State.FailedToGetDeploymentList:
                    StateFailedToGetDeploymentList(information);
                    break;
                case ConnectionStateReporter.State.Connecting:
                    StateConnecting();
                    break;
                case ConnectionStateReporter.State.Connected:
                    StateConnected();
                    break;
                case ConnectionStateReporter.State.ConnectionFailed:
                    StateConnectionFailed(information);
                    break;
                case ConnectionStateReporter.State.WaitingForGameStart:
                    StateWaitingForGameStart();
                    break;
                case ConnectionStateReporter.State.GameReady:
                    StateGameReady();
                    break;
                case ConnectionStateReporter.State.Spawning:
                    StateSpawning();
                    break;
                case ConnectionStateReporter.State.Spawned:
                    break;
                case ConnectionStateReporter.State.SpawningFailed:
                    StateSpawningFailed(information);
                    break;
                case ConnectionStateReporter.State.WorkerDisconnected:
                    StateWorkerDisconnected();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private void StateNone()
        {
            StatusText.text = string.Empty;
            SetSymbol(null);
        }

        private void StateGettingDeploymentList()
        {
            StatusText.text = GettingDeploymentListText;
            SetSymbol(SpinnerSymbol);
        }

        private void StateDeploymentListAvailable()
        {
            StatusText.text = DeploymentListAvailableText;
            SetSymbol(SuccessSymbol);
        }

        private void StateFailedToGetDeploymentList(string error)
        {
            StatusText.text = $"{FailedToGetDeploymentListText}\n Error: {error}";
            SetSymbol(ErrorSymbol);
        }

        private void StateConnecting()
        {
            StatusText.text = ConnectingText;
            SetSymbol(SpinnerSymbol);
        }

        private void StateConnected()
        {
            StatusText.text = ConnectedText;
            SetSymbol(SuccessSymbol);
        }

        private void StateConnectionFailed(string error)
        {
            StatusText.text = $"{ConnectionFailedText}\nError: {error}";
            SetSymbol(ErrorSymbol);
        }

        private void StateWaitingForGameStart()
        {
            SetSymbol(SpinnerSymbol);
            StartCoroutine(UpdateGameStartTime());
        }

        private void StateGameReady()
        {
            SetSymbol(SuccessSymbol);
            StatusText.text = GameReadyText;
        }

        private void StateSpawning()
        {
            StatusText.text = SpawningText;
        }

        private void StateSpawningFailed(string error)
        {
            StatusText.text = $"{SpawningFailedText}\nError: {error}";
            SetSymbol(ErrorSymbol);
        }

        private void StateWorkerDisconnected()
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
                    ? GameStartingImminentText
                    : string.Format(WaitingForGameStartText, ConnectionStateReporter.TimeUntilGameStart);

                yield return new WaitForSeconds(.5f);
            }
        }
    }
}
