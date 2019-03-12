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
            // sometimes this is still disabled...
            InitTimer();

            sessionReader.OnStatusUpdate += OnSessionStatusUpdated;
            timerReader.OnCurrentTimeSecondsUpdate += OnCurrentTimeSecondsUpdated;

            var status = sessionReader.Data.Status;
            OnSessionStatusUpdated(status);
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

        private void OnSessionStatusUpdated(Status status)
        {
            if (status == Status.STOPPING && ConnectionStateReporter.CurrentState == ConnectionStateReporter.State.Spawned)
            {
                ConnectionStateReporter.SetState(ConnectionStateReporter.State.GatherResults);
            }
        }

        private void InitTimer()
        {
            var timerGameObject = GameObject.FindGameObjectWithTag("Timer");
            if (timerGameObject == null)
            {
                return;
            }

            gameUITimer = timerGameObject.GetComponent<GameUITimer>();
            gameUITimer.SetMaxTime(timerReader.Data.MaxTimeSeconds);
            gameUITimer.SynchronizeTime(timerReader.Data.CurrentTimeSeconds);
        }
    }
}
