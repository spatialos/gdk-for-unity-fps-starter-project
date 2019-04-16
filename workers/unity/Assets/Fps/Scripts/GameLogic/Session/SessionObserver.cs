using Improbable.Gdk.Subscriptions;
using Improbable.Gdk.Session;
using UnityEngine;

namespace Fps
{
    public class SessionObserver : MonoBehaviour
    {
        [Require] private SessionWriter sessionWriter;
        [Require] private TimerWriter timerWriter;

        public float LobbyTime = 60f;
        public float CooldownTime = 60f;

        private float sessionTimer;

        private float maxSessionTime;
        private float currentSessionTime;
        private float lastUpdate;

        private void OnEnable()
        {
            maxSessionTime = timerWriter.Data.MaxTimeSeconds;
        }

        private void Update()
        {
            var status = sessionWriter.Data.Status;
            if (status == Status.STOPPING && sessionTimer > (maxSessionTime + CooldownTime + LobbyTime))
            {
                sessionWriter.SendUpdate(new Session.Update { Status = Status.STOPPED });
            }
            else if (status == Status.RUNNING && sessionTimer > (maxSessionTime + LobbyTime))
            {
                sessionWriter.SendUpdate(new Session.Update { Status = Status.STOPPING });
            }
            else if (status == Status.LOBBY && sessionTimer > LobbyTime)
            {
                sessionWriter.SendUpdate(new Session.Update { Status = Status.RUNNING });
                currentSessionTime = 0;
            }

            sessionTimer += Time.deltaTime;
            currentSessionTime += Time.deltaTime;
            if (currentSessionTime - lastUpdate > 1f && status == Status.RUNNING)
            {
                timerWriter.SendUpdate(new Timer.Update { CurrentTimeSeconds = (uint) Mathf.RoundToInt(currentSessionTime) });
                lastUpdate = currentSessionTime;
            }
        }
    }
}
