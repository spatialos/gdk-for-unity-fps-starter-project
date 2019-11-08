using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fps.UI
{
    public class ScreenManager : MonoBehaviour
    {
        [Header("Screens")]
        public GameObject DefaultScreen;
        public GameObject StartScreen;
        public GameObject DeploymentListScreen;
        public GameObject LobbyScreen;
        public GameObject ResultsScreen;

        [Header("Player Input")]
        public Text PlayerNameHintText;
        public InputField PlayerNameInputField;

        [Header("Session Status")]
        public ConnectionStatusManager StartStatus;
        public ConnectionStatusManager DeploymentListStatus;
        public ConnectionStatusManager LobbyStatus;

        [Header("Buttons")]
        public Button QuitButton;
        public Button DefaultConnectButton;
        public Button ResultsScreenDoneButton;
        public Button JoinDeploymentButton;
        public Button BackButton;
        public Button StartGameButton;
        public Button CancelLobbyButton;
        public Button QuickJoinButton;
        public Button ListDeploymentsButton;

        [SerializeField] private GameObject FrontEndCamera;

        private GameObject currentScreen;

        private void OnValidate()
        {
            if (DefaultScreen == null)
            {
                throw new MissingReferenceException("Missing reference to the default screen.");
            }

            if (StartScreen == null)
            {
                throw new MissingReferenceException("Missing reference to the start screen.");
            }

            if (DeploymentListScreen == null)
            {
                throw new MissingReferenceException("Missing reference to the deployment list screen.");
            }

            if (LobbyScreen == null)
            {
                throw new MissingReferenceException("Missing reference to the lobby screen.");
            }

            if (ResultsScreen == null)
            {
                throw new MissingReferenceException("Missing reference to the result screen.");
            }

            if (FrontEndCamera == null)
            {
                throw new MissingReferenceException("Missing reference to the front end camera.");
            }

            if (QuitButton == null)
            {
                throw new MissingReferenceException("Missing reference to the quit button.");
            }

            if (ResultsScreenDoneButton == null)
            {
                throw new MissingReferenceException("Missing reference to the done button.");
            }

            if (DefaultConnectButton == null)
            {
                throw new MissingReferenceException("Missing reference to the connect button.");
            }

            if (JoinDeploymentButton == null)
            {
                throw new MissingReferenceException("Missing reference to the join button.");
            }

            if (BackButton == null)
            {
                throw new MissingReferenceException("Missing reference to the back button.");
            }

            if (StartGameButton == null)
            {
                throw new MissingReferenceException("Missing reference to the start button.");
            }

            if (CancelLobbyButton == null)
            {
                throw new MissingReferenceException("Missing reference to the cancel button.");
            }

            if (QuickJoinButton == null)
            {
                throw new MissingReferenceException("Missing reference to the quick join button.");
            }

            if (ListDeploymentsButton == null)
            {
                throw new MissingReferenceException("Missing reference to the list deployments button.");
            }

            if (StartStatus == null)
            {
                throw new MissingReferenceException("Missing reference to the start screen status manager.");
            }

            if (DeploymentListStatus == null)
            {
                throw new MissingReferenceException("Missing reference to the deployment list screen status manager.");
            }

            if (LobbyStatus == null)
            {
                throw new MissingReferenceException("Missing reference to the lobby screen status manager.");
            }

            if (PlayerNameHintText == null)
            {
                throw new MissingReferenceException("Missing reference to the hint text for the player name.");
            }

            if (PlayerNameInputField == null)
            {
                throw new MissingReferenceException("Missing reference to the input field for the player name.");
            }
        }

        public void InformOfDeployments(List<DeploymentData> deployments)
        {
            var deploymentListScreenManager = DeploymentListScreen.GetComponent<DeploymentListScreenManager>();
            deploymentListScreenManager.SetDeployments(deployments);
        }

        public void InformOfResults(List<ResultsData> results, int playerRank)
        {
            var resultsScreenManager = ResultsScreen.GetComponent<ResultsScreenManager>();
            resultsScreenManager.SetResults(results, playerRank);
        }

        public void SwitchToDeploymentListScreen()
        {
            SetScreenTo(DeploymentListScreen);
        }

        public void SwitchToStartScreen()
        {
            SetScreenTo(StartScreen);
        }

        public void SwitchToResultsScreen()
        {
            SetScreenTo(ResultsScreen);
        }

        public void SwitchToLobbyScreen()
        {
            SetScreenTo(LobbyScreen);
        }

        public void SwitchToDefaultScreen()
        {
            SetScreenTo(DefaultScreen);
        }

        private void Awake()
        {
            DefaultScreen.SetActive(false);
            StartScreen.SetActive(false);
            DeploymentListScreen.SetActive(false);
            LobbyScreen.SetActive(false);
            ResultsScreen.SetActive(false);
        }

        private void OnEnable()
        {
            FrontEndCamera.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        private void OnDisable()
        {
            FrontEndCamera.SetActive(false);
        }

        private void SetScreenTo(GameObject screenObject)
        {
            if (currentScreen != null)
            {
                currentScreen.gameObject.SetActive(false);
            }

            currentScreen = screenObject;
            currentScreen.gameObject.SetActive(true);
        }
    }
}
