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
            InitTimer();
            timerReader.OnCurrentTimeSecondsUpdate += OnCurrentTimeSecondsUpdated;
        }

        private void OnCurrentTimeSecondsUpdated(uint currentTimeSeconds)
        {
            if (gameUITimer == null)
            {
                InitTimer();
            }

            if (gameUITimer != null)
            {
                gameUITimer.SynchronizeTime(currentTimeSeconds);
            }
        }

        private void InitTimer()
        {
            var timerGameObject = GameObject.FindGameObjectWithTag("Timer");
            if (timerGameObject == null)
            {
                return;
            }

            timerGameObject.SetActive(true);
            gameUITimer = timerGameObject.GetComponent<GameUITimer>();
            gameUITimer.SetMaxTime(timerReader.Data.MaxTimeSeconds);
            gameUITimer.SynchronizeTime(timerReader.Data.CurrentTimeSeconds);
        }
    }
}
