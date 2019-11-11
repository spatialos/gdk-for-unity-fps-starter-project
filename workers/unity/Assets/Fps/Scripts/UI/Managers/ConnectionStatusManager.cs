using UnityEngine;
using UnityEngine.UI;

namespace Fps.UI
{
    public class ConnectionStatusManager : MonoBehaviour
    {
        [SerializeField] private GameObject SpinnerSymbol;
        [SerializeField] private GameObject ErrorSymbol;
        [SerializeField] private GameObject SuccessSymbol;
        [SerializeField] private Text StatusText;
        [SerializeField] private GameObject activeSymbol;

        private const string DeploymentListAvailableText = "Deployments available";
        private const string GettingDeploymentListText = "Getting deployment list...";
        private const string FailedToGetDeploymentListText = "Failed to get deployment list!";
        private const string WaitingForGameStartText = "Waiting for game to start...";
        private const string WorkerDisconnectedText = "Worker has disconnected...";
        private const string SpawningText = "Spawning player...";
        private const string GameReadyText = "Press start to play";
        private const string ConnectingText = "Joining deployment...";
        private const string SpawningFailedText = "Failed to spawn player!";

        private void OnValidate()
        {
            if (SpinnerSymbol == null)
            {
                throw new MissingReferenceException("Missing reference to the spinner symbol.");
            }

            if (ErrorSymbol == null)
            {
                throw new MissingReferenceException("Missing reference to the error symbol.");
            }

            if (SuccessSymbol == null)
            {
                throw new MissingReferenceException("Missing reference to the success symbol.");
            }

            if (StatusText == null)
            {
                throw new MissingReferenceException("Missing reference to the status text.");
            }

            activeSymbol = SpinnerSymbol;
        }

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
            activeSymbol.SetActive(false);
            symbol.SetActive(true);
            activeSymbol = symbol;
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }
    }
}
