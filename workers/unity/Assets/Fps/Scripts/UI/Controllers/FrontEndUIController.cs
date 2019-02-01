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
        public GameObject BrowseScreen;
        public GameObject ResultsScreen;
        public ScreenType PreviousScreenType { get; private set; }
        public ScreenType CurrentScreenType { get; private set; }
        public GameObject FrontEndCamera;

        public bool sessionBasedGame;
        public bool resultsAvailable;

        public ScreenType TestScreenType;

        private void OnValidate()
        {
            /*DebugPrint($"Scene name: {gameObject.scene.name}\n" +
                $"Scene is loaded: {gameObject.scene.isLoaded}\n" +
                $"GetCurrentPrefabStage: {PrefabStageUtility.GetCurrentPrefabStage()}\n" +
                $"PrefabStage: {PrefabStageUtility.GetPrefabStage(gameObject)}\n");*/
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
            BrowseScreen.SetActive(false);
            ResultsScreen.SetActive(false);


            if (resultsAvailable)
            {
                CurrentScreenType = ScreenType.Results;
                PreviousScreenType = CurrentScreenType;
                ResultsScreen.SetActive(true);
                return;
            }

            if (sessionBasedGame)
            {
                CurrentScreenType = ScreenType.Matchmaking;
                PreviousScreenType = CurrentScreenType;
                MatchmakingScreen.SetActive(true);
            }
            else
            {
                CurrentScreenType = ScreenType.DefaultConnect;
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

            if (pendingRefresh)
            {
                Debug.Log("Already pending a refresh when entering SetScreenTo())");
                return;
            }

            DebugPrint("SetScreenTo()");

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

            pendingRefresh = true;
            UnityEditor.EditorApplication.update += RefreshActiveScreen;
        }

        private bool pendingRefresh;


        private void DebugPrint(string status)
        {
            var output = "";

            output += $"{status} for object {gameObject.name}\n" +
                $"instanceId: {gameObject.GetInstanceID()}\n" +
                $"prefabAssetType: {PrefabUtility.GetPrefabAssetType(gameObject)}\n" +
                $"path: {PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(gameObject)}\n" +
                $"prefabInstanceStatus: {PrefabUtility.GetPrefabInstanceStatus(gameObject)}\n" +
                $"prefabStage: {PrefabStageUtility.GetPrefabStage(gameObject)}";

            Debug.Log(output);
        }

        private void RefreshActiveScreen()
        {
            UnityEditor.EditorApplication.update -= RefreshActiveScreen;
            if (gameObject == null)
            {
                return;
            }

            if (!pendingRefresh)
            {
                Debug.Log("Won't execute RefreshActiveScreen() because there is no refresh pending");
                return;
            }

            DebugPrint("RefreshActiveScreen()");

            pendingRefresh = false;
            ConnectScreenController.gameObject.SetActive(ConnectScreenController.gameObject ==
                GetGOFromScreen(CurrentScreenType));
            MatchmakingScreen.SetActive(MatchmakingScreen == GetGOFromScreen(CurrentScreenType));
            BrowseScreen.SetActive(BrowseScreen == GetGOFromScreen(CurrentScreenType));
            ResultsScreen.SetActive(ResultsScreen == GetGOFromScreen(CurrentScreenType));
        }

        public void SwitchToBrowseScreen()
        {
            SetScreenTo(ScreenType.Browse);
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
                case ScreenType.Browse:
                    return BrowseScreen;
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
            Browse,
            Results,
            Previous
        }
    }
}
