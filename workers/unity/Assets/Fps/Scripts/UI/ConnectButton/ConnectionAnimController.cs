using System;
using Fps;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionAnimController : MonoBehaviour
{
    public FpsUIButton ConnectButton;
    public GameObject SpinnerSymbol;
    public GameObject ErrorSymbol;
    public GameObject SearchingText;
    public GameObject SearchFailedText;
    public GameObject JoiningText;
    public GameObject SpawnFailedText;
    public GameObject WorkerDisconnectedText;

    public Selectable[] SearchingObjsToDisable;
    public Selectable[] JoiningObjsToDisable;

    public ConnectionState CurrentState
    {
        get;
        private set;
    }

    public void SetState(ConnectionState state)
    {
        switch (state)
        {
            case ConnectionState.Initializing:
                Initialize();
                CurrentState = state;
                break;
            case ConnectionState.Searching:
                SearchForServer();
                CurrentState = state;
                break;
            case ConnectionState.SearchFailed:
                SearchFailed();
                CurrentState = state;
                break;
            case ConnectionState.Joining:
                Spawning();
                CurrentState = state;
                break;
            case ConnectionState.JoinFailed:
                SpawnFailed();
                CurrentState = state;
                break;
            case ConnectionState.WorkerDisconnected:
                WorkerDisconnected();
                CurrentState = state;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    private void Initialize()
    {
        ConnectButton.SetText((int) ConnectButtonStates.Ready);
        SetStatusText(null);
        SetSymbol(null);

        foreach (var obj in JoiningObjsToDisable)
        {
            obj.interactable = true;
        }
    }

    private void SearchForServer()
    {
        ConnectButton.SetText((int) ConnectButtonStates.Cancel);
        SetStatusText(SearchingText);
        SetSymbol(SpinnerSymbol);

        foreach (var obj in SearchingObjsToDisable)
        {
            obj.interactable = false;
        }
    }

    private void SearchFailed()
    {
        ConnectButton.SetText((int) ConnectButtonStates.Ready);
        SetStatusText(SearchFailedText);
        SetSymbol(ErrorSymbol);

        foreach (var obj in SearchingObjsToDisable)
        {
            obj.interactable = true;
        }
    }

    private void Spawning()
    {
        SetStatusText(JoiningText);

        foreach (var obj in JoiningObjsToDisable)
        {
            obj.interactable = false;
        }
    }

    private void SpawnFailed()
    {
        SetStatusText(SpawnFailedText);
        SetSymbol(ErrorSymbol);

        foreach (var obj in JoiningObjsToDisable)
        {
            obj.interactable = true;
        }
    }

    private void WorkerDisconnected()
    {
        SetSymbol(WorkerDisconnectedText);
        SetSymbol(ErrorSymbol);

        foreach (var obj in JoiningObjsToDisable)
        {
            obj.interactable = true;
        }
    }

    private void SetSymbol(GameObject symbolGO)
    {
        Debug.Assert(symbolGO == null
            || symbolGO == ErrorSymbol
            || symbolGO == SpinnerSymbol);
        ErrorSymbol.SetActive(symbolGO == ErrorSymbol);
        SpinnerSymbol.SetActive(symbolGO == SpinnerSymbol);
    }

    private void SetStatusText(GameObject textGO)
    {
        Debug.Assert(textGO == null
            || textGO == SearchingText
            || textGO == SearchFailedText
            || textGO == JoiningText
            || textGO == SpawnFailedText
            || textGO == WorkerDisconnectedText);

        SearchingText.SetActive(textGO == SearchingText);
        SearchFailedText.SetActive(textGO == SearchFailedText);
        JoiningText.SetActive(textGO == JoiningText);
        SpawnFailedText.SetActive(textGO == SpawnFailedText);
        WorkerDisconnectedText.SetActive(textGO == WorkerDisconnectedText);
    }

    private enum ConnectButtonStates
    {
        Ready,
        Cancel
    }

    public enum ConnectionState
    {
        Initializing,
        Searching,
        SearchFailed,
        Joining,
        JoinFailed,
        WorkerDisconnected
    }
}
