using System;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;
using UnityEngine.Serialization;

namespace Fps
{
    public class FrontEndUIController : MonoBehaviour
    {
        public ConnectScreenController ConnectScreenController;
        public GameObject SessionScreen;
        public LobbyScreenController LobbyScreenController;
        public GameObject ResultsScreen;
        public ScreenType PreviousScreenType { get; private set; }
        public ScreenType CurrentScreenType { get; private set; }
        public GameObject FrontEndCamera;

        public bool sessionBasedGame;
        public bool resultsAvailable;

        public ScreenType TestScreenType;

        private void OnValidate()
        {
            if (!gameObject.scene.isLoaded)
            {
                return;
            }

            if (PrefabStageUtility.GetCurrentPrefabStage() != null
                && PrefabStageUtility.GetPrefabStage(gameObject) == null)
            {
                // Don't switch screens as this is not the prefab being edited in the stage
                return;
            }

            SetScreenTo(TestScreenType);
            TestScreenType = CurrentScreenType;
        }

        void Awake()
        {
            ConnectScreenController.gameObject.SetActive(false);
            SessionScreen.SetActive(false);
            LobbyScreenController.gameObject.SetActive(false);
            ResultsScreen.SetActive(false);
        }

        private void OnEnable()
        {
            FrontEndCamera.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;

            if (resultsAvailable)
            {
                CurrentScreenType = ScreenType.Results;
                TestScreenType = CurrentScreenType;
                PreviousScreenType = CurrentScreenType;
                ResultsScreen.SetActive(true);
                return;
            }

            if (sessionBasedGame)
            {
                CurrentScreenType = ScreenType.SessionScreen;
                TestScreenType = CurrentScreenType;
                PreviousScreenType = CurrentScreenType;
                SessionScreen.SetActive(true);
            }
            else
            {
                CurrentScreenType = ScreenType.DefaultConnect;
                TestScreenType = CurrentScreenType;
                PreviousScreenType = CurrentScreenType;
                ConnectScreenController.gameObject.SetActive(true);
            }
        }

        private void OnDisable()
        {
            FrontEndCamera.SetActive(false);
        }

        public void SetScreenTo(ScreenType screenType)
        {
            var screenUnmodified = screenType == CurrentScreenType
                || screenType == PreviousScreenType && PreviousScreenType == CurrentScreenType;
            if (screenUnmodified)
            {
                return;
            }

            if (screenType == ScreenType.Previous)
            {
                var s = CurrentScreenType;
                CurrentScreenType = PreviousScreenType;
                PreviousScreenType = s;
            }
            else
            {
                PreviousScreenType = CurrentScreenType;
                CurrentScreenType = screenType;
            }

            Invoke(nameof(RefreshActiveScreen), 0);
        }

        private void RefreshActiveScreen()
        {
            ConnectScreenController.gameObject.SetActive(ConnectScreenController.gameObject ==
                GetGOFromScreen(CurrentScreenType));
            SessionScreen.SetActive(SessionScreen == GetGOFromScreen(CurrentScreenType));
            LobbyScreenController.gameObject.SetActive(LobbyScreenController.gameObject ==
                GetGOFromScreen(CurrentScreenType));
            ResultsScreen.SetActive(ResultsScreen == GetGOFromScreen(CurrentScreenType));
        }

        public void SwitchToLobbyScreen()
        {
            SetScreenTo(ScreenType.Lobby);
        }

        public void SwitchToSessionScreen()
        {
            SetScreenTo(ScreenType.SessionScreen);
        }

        public void SwitchToResultsScreen()
        {
            SetScreenTo(ScreenType.Results);
        }

        public void BackButton()
        {
            SetScreenTo(ScreenType.Previous);
        }


        private GameObject GetGOFromScreen(ScreenType screenType)
        {
            switch (screenType)
            {
                case ScreenType.DefaultConnect:
                    return ConnectScreenController.gameObject;
                case ScreenType.SessionScreen:
                    return SessionScreen;
                case ScreenType.Lobby:
                    return LobbyScreenController.gameObject;
                case ScreenType.Results:
                    return ResultsScreen;
                case ScreenType.Previous:
                    if (PreviousScreenType == ScreenType.Previous)
                    {
                        throw new ArgumentException();
                    }

                    return GetGOFromScreen(PreviousScreenType);
                default:
                    throw new ArgumentOutOfRangeException(nameof(screenType), screenType, null);
            }
        }

        public enum ScreenType
        {
            DefaultConnect,
            SessionScreen,
            Lobby,
            Results,
            Previous
        }
    }
}
