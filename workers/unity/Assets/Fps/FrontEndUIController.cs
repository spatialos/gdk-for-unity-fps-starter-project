using System;
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
            SetScreenTo(TestScreenType);
            TestScreenType = CurrentScreenType;
        }

        private void OnEnable()
        {
            Debug.Log("Enabled front end controller");

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

        public GameObject SetScreenTo(ScreenType screenType)
        {
            if (screenType == CurrentScreenType
                || screenType == PreviousScreenType && PreviousScreenType == CurrentScreenType)
            {
                return GetGOFromScreen(CurrentScreenType);
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

            ConnectScreenController.gameObject.SetActive(ConnectScreenController.gameObject == GetGOFromScreen(CurrentScreenType));
            MatchmakingScreen.SetActive(MatchmakingScreen == GetGOFromScreen(CurrentScreenType));
            BrowseScreen.SetActive(BrowseScreen == GetGOFromScreen(CurrentScreenType));
            ResultsScreen.SetActive(ResultsScreen == GetGOFromScreen(CurrentScreenType));
            return GetGOFromScreen(CurrentScreenType);
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
