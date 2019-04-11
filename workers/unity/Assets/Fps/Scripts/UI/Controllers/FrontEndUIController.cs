using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Fps
{
    public class FrontEndUIController : MonoBehaviour
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

        [Header("Screens")] public ConnectScreenController ConnectScreenController;
        public SessionScreenController SessionScreenController;
        public DeploymentListScreenController DeploymentListScreenController;
        public ResultsScreenController ResultsScreenController;
        public LobbyScreenController LobbyScreenController;

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
            ConnectScreenController.gameObject.SetActive(false);
            SessionScreenController.gameObject.SetActive(false);
            DeploymentListScreenController.gameObject.SetActive(false);
            ResultsScreenController.gameObject.SetActive(false);
            QuitButton.onClick.AddListener(ScreenUIController.Quit);
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

        private void RefreshActiveScreen()
        {
            var currentController = GetControllerFromScreen(currentScreen);
            ConnectScreenController.gameObject.SetActive(ConnectScreenController == currentController);
            SessionScreenController.gameObject.SetActive(SessionScreenController == currentController);
            DeploymentListScreenController.gameObject.SetActive(DeploymentListScreenController == currentController);
            ResultsScreenController.gameObject.SetActive(ResultsScreenController == currentController);
            LobbyScreenController.gameObject.SetActive(LobbyScreenController == currentController);
        }

        private void SetScreenTo(ScreenType screenType)
        {
            currentScreen = screenType;
            RefreshActiveScreen();
        }

        private Component GetControllerFromScreen(ScreenType screenType)
        {
            switch (screenType)
            {
                case ScreenType.None:
                    return null;
                case ScreenType.DefaultConnect:
                    return ConnectScreenController;
                case ScreenType.SessionScreen:
                    return SessionScreenController;
                case ScreenType.DeploymentList:
                    return DeploymentListScreenController;
                case ScreenType.Results:
                    return ResultsScreenController;
                case ScreenType.Lobby:
                    return LobbyScreenController;
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
