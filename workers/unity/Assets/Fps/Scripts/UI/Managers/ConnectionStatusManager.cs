using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Fps
{
    public class ConnectionStatusManager : MonoBehaviour
    {
        public GameObject SpinnerSymbol;
        public GameObject ErrorSymbol;
        public GameObject SuccessSymbol;
        public Text StatusText;

        private const string DeploymentListAvailableText = "Deployments available";
        private const string GettingDeploymentListText = "Getting deployment list...";
        private const string FailedToGetDeploymentListText = "Failed to get deployment list!";
        private const string WaitingForGameStartText = "Waiting for game to start...";
        private const string WorkerDisconnectedText = "Worker has disconnected...";
        private const string SpawningText = "Spawning player...";
        private const string GameReadyText = "Press start to play";
        private const string ConnectingText = "Joining deployment...";
        private const string SpawningFailedText = "Failed to spawn player!";

        public void ShowGetDeploymentListText()
        {
            StatusText.text = GettingDeploymentListText;
            SetSymbol(SpinnerSymbol);
        }

        public void ShowDeploymentListAvailableText()
        {
            StatusText.text = DeploymentListAvailableText;
            SetSymbol(SuccessSymbol);
        }

        public void ShowFailedToGetDeploymentsText(string error)
        {
            StatusText.text = $"{FailedToGetDeploymentListText}\n Error: {error}";
            SetSymbol(ErrorSymbol);
        }

        public void ShowConnectingText()
        {
            StatusText.text = ConnectingText;
            SetSymbol(SpinnerSymbol);
        }

        public void ShowWaitForGameText()
        {
            StatusText.text = WaitingForGameStartText;
            SetSymbol(SpinnerSymbol);
        }

        public void ShowGameReadyText()
        {
            SetSymbol(SuccessSymbol);
            StatusText.text = GameReadyText;
        }

        public void ShowSpawningText()
        {
            StatusText.text = SpawningText;
        }

        public void ShowWorkerDisconnectedText()
        {
            StatusText.text = WorkerDisconnectedText;
            SetSymbol(ErrorSymbol);
        }

        public void ShowSpawningFailedText(string error)
        {
            StatusText.text = $"{SpawningFailedText}\nError: {error}";
            SetSymbol(ErrorSymbol);
        }

        private void SetSymbol(GameObject symbol)
        {
            Debug.Assert(symbol == null
                || symbol == ErrorSymbol
                || symbol == SpinnerSymbol
                || symbol == SuccessSymbol);

            ErrorSymbol.SetActive(symbol == ErrorSymbol);
            SpinnerSymbol.SetActive(symbol == SpinnerSymbol);
            SuccessSymbol.SetActive(symbol == SuccessSymbol);
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }
    }
}
