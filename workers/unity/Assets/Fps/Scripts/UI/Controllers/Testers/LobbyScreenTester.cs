using System.Collections;
using System.Collections.Generic;
using Fps;
using UnityEngine;

public class LobbyScreenTester : MonoBehaviour
{
    public float TimeUntilStart = 60;
    private float timeUntilStart;

    void Awake()
    {
        ConnectionStateReporter.OnConnectionStateChange += ConnectionStateChanged;
    }

    private void ConnectionStateChanged(ConnectionStateReporter.State state, string information)
    {
        if (state == ConnectionStateReporter.State.Connected)
        {
            ConnectionStateReporter.SetTimeUntilGameStart(timeUntilStart);
            ConnectionStateReporter.SetState(ConnectionStateReporter.State.WaitingForGameStart);
            timeUntilStart = TimeUntilStart;
            StartCoroutine(CountDownTime());
        }
    }

    private IEnumerator CountDownTime()
    {
        while (timeUntilStart > 0)
        {
            ConnectionStateReporter.SetTimeUntilGameStart(timeUntilStart);
            yield return new WaitForEndOfFrame();
            timeUntilStart -= Time.deltaTime;
        }

        ConnectionStateReporter.SetState(ConnectionStateReporter.State.GameReady);
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }
}
