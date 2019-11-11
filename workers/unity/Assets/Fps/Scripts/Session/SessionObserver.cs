using Improbable.Gdk.Session;
using Improbable.Gdk.Subscriptions;
using UnityEngine;

namespace Fps.Session
{
    public class SessionObserver : MonoBehaviour
    {
        [Require] private SessionWriter sessionWriter;
        [Require] private TimerWriter timerWriter;

        [SerializeField] private float lobbyTime = 60f;
        [SerializeField] private float cooldownTime = 60f;

        private float sessionTimer;
        private float maxSessionTime;
        private float lastUpdate;

        private void OnEnable()
        {
            maxSessionTime = timerWriter.Data.MaxTimeSeconds;
        }

        private void Update()
        {
            var status = sessionWriter.Data.Status;
            if (status == Status.STOPPING && sessionTimer > cooldownTime)
            {
                sessionWriter.SendUpdate(new Improbable.Gdk.Session.Session.Update { Status = Status.STOPPED });
                sessionTimer = 0;
            }
            else if (status == Status.RUNNING)
            {
                if (sessionTimer > maxSessionTime)
                {
                    sessionWriter.SendUpdate(new Improbable.Gdk.Session.Session.Update { Status = Status.STOPPING });
                    sessionTimer = 0;
                }

                if (sessionTimer - lastUpdate > 1f)
                {
                    timerWriter.SendUpdate(new Timer.Update { CurrentTimeSeconds = (uint) Mathf.RoundToInt(sessionTimer) });
                    lastUpdate = sessionTimer;
                }
            }
            else if (status == Status.LOBBY && sessionTimer > lobbyTime)
            {
                sessionWriter.SendUpdate(new Improbable.Gdk.Session.Session.Update { Status = Status.RUNNING });
                sessionTimer = 0;
                lastUpdate = 0;
            }

            sessionTimer += Time.deltaTime;
        }
    }
}
