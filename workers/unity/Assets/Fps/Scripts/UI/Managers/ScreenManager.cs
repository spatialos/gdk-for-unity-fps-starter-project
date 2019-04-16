using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Fps
{
    public class ScreenManager : MonoBehaviour
    {
        [Header("General")]
        public Button QuitButton;
        [SerializeField] private GameObject FrontEndCamera;

        [Header("Screens")]
        public DefaultConnectScreenManager DefaultConnectScreenManager;
        public StartScreenManager StartScreenManager;
        public DeploymentListScreenManager DeploymentListScreenManager;
        public ResultsScreenManager ResultsScreenManager;
        public LobbyScreenManager LobbyScreenManager;

        private Component currentScreenManager;

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
            QuitButton.onClick.AddListener(UIManager.Quit);
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
