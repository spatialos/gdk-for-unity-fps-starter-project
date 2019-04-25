﻿using Improbable.Gdk.Subscriptions;
using Improbable.Gdk.Session;
using UnityEngine;

namespace Fps
{
    public class SessionObserver : MonoBehaviour
    {
        [Require] private SessionWriter sessionWriter;
        [Require] private TimerWriter timerWriter;

        private const float LobbyTime = 60f;
        private const float CooldownTime = 60f;

        private float timer;

        private float maxSessionTime;
        private float currentSessionTime;
        private float lastUpdate;

        private void OnEnable()
        {
            maxSessionTime = timerWriter.Data.MaxTimeSeconds;
        }

        private void Update()
        {
            if (timer > (maxSessionTime + CooldownTime + LobbyTime) && sessionWriter.Data.Status == Status.STOPPING)
            {
                sessionWriter.SendUpdate(new Session.Update { Status = Status.STOPPED });
            }
            else if (timer > (maxSessionTime + LobbyTime) && sessionWriter.Data.Status == Status.RUNNING)
            {
                sessionWriter.SendUpdate(new Session.Update { Status = Status.STOPPING });
            }
            else if (timer > LobbyTime && sessionWriter.Data.Status == Status.LOBBY)
            {
                sessionWriter.SendUpdate(new Session.Update { Status = Status.RUNNING });
                currentSessionTime = 0;
            }

            timer += Time.deltaTime;
            currentSessionTime += Time.deltaTime;
            if (currentSessionTime - lastUpdate > 1f && sessionWriter.Data.Status == Status.RUNNING)
            {
                timerWriter.SendUpdate(new Timer.Update { CurrentTimeSeconds = (uint) Mathf.RoundToInt(currentSessionTime) });
                lastUpdate = currentSessionTime;
            }
        }
    }
}
