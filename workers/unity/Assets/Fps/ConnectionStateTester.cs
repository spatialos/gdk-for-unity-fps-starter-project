using UnityEngine;
using UnityEngine.UI;

public class ConnectionStateTester : MonoBehaviour
{
    private Text text;

    private void Awake()
    {
        ConnectionStateReporter.OnConnectionStateChange += StateChanged;
        text = GetComponent<Text>();
    }

    private void StateChanged(ConnectionStateReporter.State state, string information)
    {
        text.text = $"State: {state.ToString()}\n" +
            $"Info: {information}";
    }
}
