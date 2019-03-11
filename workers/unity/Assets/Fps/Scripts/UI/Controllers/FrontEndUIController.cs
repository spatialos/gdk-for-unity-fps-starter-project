using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Fps
{
    public class FrontEndUIController : MonoBehaviour
    {
        public enum ScreenType
        {
            None,
            DefaultConnect,
            SessionScreen,
            DeploymentList,
            Lobby,
            Results
        }

        [Header("General")] public Button QuitButton;
        public GameObject FrontEndCamera;

        [Header("Screens")] public ConnectScreenController ConnectScreenController;
        public SessionScreenController SessionScreenController;
        public DeploymentListScreenController DeploymentListScreenController;
        public ResultsScreenController ResultsScreenController;
        [FormerlySerializedAs("PregameScreenController")] public LobbyScreenController LobbyScreenController;

        public ScreenType CurrentScreen { get; private set; }

        [Header("Editor / Testing")] public bool testResultsAvailable;
        [SerializeField] private ScreenType EditorCurrentScreen;

        private void OnValidate()
        {
            EditorSwitchScreens();
        }

        private void Awake()
        {
            CurrentScreen = ScreenType.None;
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

            SetScreenTo(ClientWorkerHandler.IsInSessionBasedGame
                ? ScreenType.SessionScreen
                : ScreenType.DefaultConnect);
        }

        private void OnDisable()
        {
            FrontEndCamera.SetActive(false);
        }

        public void SetScreenTo(ScreenType screenType)
        {
            CurrentScreen = screenType;
            RefreshActiveScreen();
        }

        private void RefreshActiveScreen()
        {
            var currentScreenGO = GetGOFromScreen(CurrentScreen);
            ConnectScreenController.gameObject.SetActive(ConnectScreenController.gameObject == currentScreenGO);
            SessionScreenController.gameObject.SetActive(SessionScreenController.gameObject == currentScreenGO);
            DeploymentListScreenController.gameObject.SetActive(DeploymentListScreenController.gameObject ==
                currentScreenGO);
            ResultsScreenController.gameObject.SetActive(ResultsScreenController.gameObject == currentScreenGO);
            LobbyScreenController.gameObject.SetActive(LobbyScreenController.gameObject == currentScreenGO);
        }

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

        private GameObject GetGOFromScreen(ScreenType screenType)
        {
            switch (screenType)
            {
                case ScreenType.None:
                    return null;
                case ScreenType.DefaultConnect:
                    return ConnectScreenController.gameObject;
                case ScreenType.SessionScreen:
                    return SessionScreenController.gameObject;
                case ScreenType.DeploymentList:
                    return DeploymentListScreenController.gameObject;
                case ScreenType.Results:
                    return ResultsScreenController.gameObject;
                case ScreenType.Lobby:
                    return LobbyScreenController.gameObject;
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

            if (EditorCurrentScreen == CurrentScreen)
            {
                return;
            }

            SetScreenTo(EditorCurrentScreen);
#endif
        }
    }
}
