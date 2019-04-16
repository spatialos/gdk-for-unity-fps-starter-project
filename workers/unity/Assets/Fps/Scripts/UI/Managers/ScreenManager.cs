using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Fps
{
    public class ScreenManager : MonoBehaviour
    {
        private enum ScreenType
        {
            None,
            DefaultConnect,
            SessionScreen,
            DeploymentList,
            Lobby,
            Results
        }

        [Header("General")] public Button QuitButton;
        [SerializeField] public GameObject FrontEndCamera;

        [Header("Screens")] public DefaultConnectScreenManager defaultConnectScreenManager;
        public StartScreenManager startScreenManager;
        public DeploymentListScreenManager deploymentListScreenManager;
        public ResultsScreenManager resultsScreenManager;
        public LobbyScreenManager lobbyScreenManager;

        private ScreenType currentScreen = ScreenType.None;

        [SerializeField] private ScreenType EditorCurrentScreen;

        public void SwitchToDeploymentListScreen()
        {
            SetScreenTo(ScreenType.DeploymentList);
        }

        public void SwitchToSessionScreen()
        {
            SetScreenTo(ScreenType.SessionScreen);
        }

        public void SwitchToResultsScreen()
        {
            SetScreenTo(ScreenType.Results);
        }

        public void SwitchToLobbyScreen()
        {
            SetScreenTo(ScreenType.Lobby);
        }

        public void SwitchToDefaultConnectScreen()
        {
            SetScreenTo(ScreenType.DefaultConnect);
        }

        private void OnValidate()
        {
            EditorSwitchScreens();
        }

        private void Awake()
        {
            defaultConnectScreenManager.gameObject.SetActive(false);
            startScreenManager.gameObject.SetActive(false);
            deploymentListScreenManager.gameObject.SetActive(false);
            resultsScreenManager.gameObject.SetActive(false);
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

        private void SetScreenTo(ScreenType screenType)
        {
            var currentController = GetControllerFromScreen(currentScreen);
            if (currentController != null)
            {
                currentController.gameObject.SetActive(false);
            }

            currentScreen = screenType;
            currentController = GetControllerFromScreen(currentScreen);
            currentController.gameObject.SetActive(true);
        }

        private Component GetControllerFromScreen(ScreenType screenType)
        {
            switch (screenType)
            {
                case ScreenType.None:
                    return null;
                case ScreenType.DefaultConnect:
                    return defaultConnectScreenManager;
                case ScreenType.SessionScreen:
                    return startScreenManager;
                case ScreenType.DeploymentList:
                    return deploymentListScreenManager;
                case ScreenType.Results:
                    return resultsScreenManager;
                case ScreenType.Lobby:
                    return lobbyScreenManager;
                default:
                    throw new ArgumentOutOfRangeException(nameof(screenType), screenType, null);
            }
        }

        /// <summary>
        ///     Utility for controlling visible screen in editor using TestScreenType dropdown.
        /// </summary>
        private void EditorSwitchScreens()
        {
#if UNITY_EDITOR
            if (!gameObject.scene.isLoaded)
            {
                return;
            }

            if (UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() != null
                && UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetPrefabStage(gameObject) == null)
            {
                // Don't switch screens as this is not the prefab being edited in the stage
                return;
            }

            if (EditorCurrentScreen == currentScreen)
            {
                return;
            }

            SetScreenTo(EditorCurrentScreen);
#endif
        }
    }
}
