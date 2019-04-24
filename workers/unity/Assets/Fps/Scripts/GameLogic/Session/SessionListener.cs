using System;
using Improbable.Gdk.Subscriptions;
using Improbable.Gdk.Session;
using UnityEngine;

namespace Fps
{
    [WorkerType(WorkerUtils.UnityClient, WorkerUtils.iOSClient, WorkerUtils.AndroidClient)]
    public class SessionListener : MonoBehaviour
    {
        [Require] private SessionReader sessionReader;
        [Require] private TimerReader timerReader;

        private GameUITimer gameUITimer;

        private void OnEnable()
        {
            var uiManager = GetComponent<UIManager>()?.GetComponent<UIManager>();
            if (uiManager == null)
            {
                throw new NullReferenceException("Was not able to find the OnScreenUI prefab in the scene.");
            }

            uiManager.ShowGameView();
            var timerGameObject = uiManager.InGameManager.Timer;
            timerGameObject.SetActive(true);
            gameUITimer = timerGameObject.GetComponent<GameUITimer>();
            gameUITimer.SetMaxTime(timerReader.Data.MaxTimeSeconds);
            gameUITimer.SynchronizeTime(timerReader.Data.CurrentTimeSeconds);
            timerReader.OnCurrentTimeSecondsUpdate += OnCurrentTimeSecondsUpdated;
        }

        private void OnCurrentTimeSecondsUpdated(uint currentTimeSeconds)
        {
            gameUITimer.SynchronizeTime(currentTimeSeconds);
        }
    }
}
