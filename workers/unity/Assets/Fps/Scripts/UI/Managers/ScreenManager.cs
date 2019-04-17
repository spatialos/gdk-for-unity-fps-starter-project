using System;
using UnityEngine;
using UnityEngine.UI;

namespace Fps
{
    public class ScreenManager : MonoBehaviour
    {
        [Header("General")]
        public Button QuitButton;

        [Header("Screens")]
        public DefaultConnectScreenManager DefaultConnectScreenManager;
        public StartScreenManager StartScreenManager;
        public DeploymentListScreenManager DeploymentListScreenManager;
        public ResultsScreenManager ResultsScreenManager;
        public LobbyScreenManager LobbyScreenManager;

        [SerializeField] private GameObject FrontEndCamera;

        private Component currentScreenManager;

        private void OnValidate()
        {
            if (DefaultConnectScreenManager == null)
            {
                throw new NullReferenceException("Missing reference to the default connect screen manager.");
            }

            if (StartScreenManager == null)
            {
                throw new NullReferenceException("Missing reference to the start screen manager.");
            }

            if (DeploymentListScreenManager == null)
            {
                throw new NullReferenceException("Missing reference to the deployment list screen manager.");
            }

            if (ResultsScreenManager == null)
            {
                throw new NullReferenceException("Missing reference to the results screen manager.");
            }

            if (LobbyScreenManager == null)
            {
                throw new NullReferenceException("Missing reference to the lobby screen manager.");
            }

            if (FrontEndCamera == null)
            {
                throw new NullReferenceException("Missing reference to the front end camera.");
            }

            if (QuitButton == null)
            {
                throw new NullReferenceException("Missing reference to the quit button.");
            }
        }

        public void SwitchToDeploymentListScreen()
        {
            SetScreenTo(DeploymentListScreenManager);
        }

        public void SwitchToSessionScreen()
        {
            SetScreenTo(StartScreenManager);
        }

        public void SwitchToResultsScreen()
        {
            SetScreenTo(ResultsScreenManager);
        }

        public void SwitchToLobbyScreen()
        {
            SetScreenTo(LobbyScreenManager);
        }

        public void SwitchToDefaultConnectScreen()
        {
            SetScreenTo(DefaultConnectScreenManager);
        }

        private void Awake()
        {
            DefaultConnectScreenManager.gameObject.SetActive(false);
            StartScreenManager.gameObject.SetActive(false);
            DeploymentListScreenManager.gameObject.SetActive(false);
            LobbyScreenManager.gameObject.SetActive(false);
            ResultsScreenManager.gameObject.SetActive(false);
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

        private void SetScreenTo(Component screenManager)
        {
            if (currentScreenManager != null)
            {
                currentScreenManager.gameObject.SetActive(false);
            }

            currentScreenManager = screenManager;
            currentScreenManager.gameObject.SetActive(true);
        }
    }
}
