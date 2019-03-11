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
            gameUITimer = GameObject.FindGameObjectWithTag("Timer").GetComponent<GameUITimer>();
            gameUITimer.SetMaxTime(timerReader.Data.MaxTimeSeconds);
            gameUITimer.SynchronizeTime(timerReader.Data.CurrentTimeSeconds);
            
            
            sessionReader.OnStatusUpdate += OnSessionStatusUpdated;
            timerReader.OnCurrentTimeSecondsUpdate += OnCurrentTimeSecondsUpdated;

            var status = sessionReader.Data.Status;
            OnSessionStatusUpdated(status);
        }

        private void OnCurrentTimeSecondsUpdated(uint currentTimeSeconds)
        {
            gameUITimer.SynchronizeTime(currentTimeSeconds);
        }

        private void OnSessionStatusUpdated(Status status)
        {
            if (status == Status.STOPPING && ConnectionStateReporter.CurrentState == ConnectionStateReporter.State.Spawned)
            {
                ConnectionStateReporter.SetState(ConnectionStateReporter.State.ShowResults);
            }
            else if (status == Status.STOPPED && ConnectionStateReporter.CurrentState != ConnectionStateReporter.State.EndSession)
            {
                ConnectionStateReporter.SetState(ConnectionStateReporter.State.EndSession);
            }
        }
    }
}
