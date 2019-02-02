using System;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;

namespace Fps
{
    public class FrontEndUIController : MonoBehaviour
    {
        public ConnectScreenController ConnectScreenController;
        public GameObject MatchmakingScreen;
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
            DebugPrint($"Scene name: {gameObject.scene.name}\n" +
                $"Scene is loaded: {gameObject.scene.isLoaded}\n" +
                $"GetCurrentPrefabStage: {PrefabStageUtility.GetCurrentPrefabStage()}\n" +
                $"PrefabStage: {PrefabStageUtility.GetPrefabStage(gameObject)}\n");
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

        private void OnEnable()
        {
            FrontEndCamera.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;

            ConnectScreenController.gameObject.SetActive(false);
            MatchmakingScreen.SetActive(false);
            LobbyScreenController.gameObject.SetActive(false);
            ResultsScreen.SetActive(false);


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
                CurrentScreenType = ScreenType.Matchmaking;
                TestScreenType = CurrentScreenType;
                PreviousScreenType = CurrentScreenType;
                MatchmakingScreen.SetActive(true);
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

        private void DebugPrint(string status)
        {
            var output = "";

            output += $"{status} for object {gameObject.name}\n" +
                $"instanceId: {gameObject.GetInstanceID()}\n" +
                $"prefabAssetType: {PrefabUtility.GetPrefabAssetType(gameObject)}\n" +
                $"path: {PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(gameObject)}\n" +
                $"prefabInstanceStatus: {PrefabUtility.GetPrefabInstanceStatus(gameObject)}\n" +
                $"prefabStage: {PrefabStageUtility.GetPrefabStage(gameObject)}";
        }

        private void RefreshActiveScreen()
        {
            ConnectScreenController.gameObject.SetActive(ConnectScreenController.gameObject ==
                GetGOFromScreen(CurrentScreenType));
            MatchmakingScreen.SetActive(MatchmakingScreen == GetGOFromScreen(CurrentScreenType));
            LobbyScreenController.gameObject.SetActive(LobbyScreenController.gameObject ==
                GetGOFromScreen(CurrentScreenType));
            ResultsScreen.SetActive(ResultsScreen == GetGOFromScreen(CurrentScreenType));
        }

        public void SwitchToLobbyScreen()
        {
            SetScreenTo(ScreenType.Lobby);
        }

        public void SwitchToMatchmakingScreen()
        {
            SetScreenTo(ScreenType.Matchmaking);
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
                case ScreenType.Matchmaking:
                    return MatchmakingScreen;
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
            Matchmaking,
            Lobby,
            Results,
            Previous
        }
    }
}
