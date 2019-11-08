using Fps.Config;
using Fps.UI;
using Improbable.Gdk.Session;
using Improbable.Gdk.Subscriptions;
using UnityEngine;

namespace Fps.Session
{
    [WorkerType(WorkerUtils.UnityClient, WorkerUtils.MobileClient)]
    public class SessionListener : MonoBehaviour
    {
        [Require] private SessionReader sessionReader;
        [Require] private TimerReader timerReader;

        private GameUITimer gameUITimer;

        private void OnEnable()
        {
            var uiManager = GameObject.FindGameObjectWithTag("OnScreenUI")?.GetComponent<UIManager>();
            if (uiManager == null)
            {
                Debug.LogWarning("Was not able to find the OnScreenUI prefab in the scene.");
                enabled = false;
                return;
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
