using Fps;
using UnityEngine;

public static class ConnectionStateReporter
{
    public enum State
    {
        None,
        GettingDeploymentList,
        DeploymentListAvailable,
        FailedToGetDeploymentList,
        Connecting,
        Connected,
        ConnectionFailed,
        Spawning,
        Spawned,
        SpawningFailed,
        WorkerDisconnected
    }

    public static bool AreConnected { get; private set; }
    public static bool HaveDeployments { get; private set; }
    public static bool AreSpawned { get; private set; }

    public static string CurrentInformation;

    public static State CurrentState;

    public static void TryConnect()
    {
        if (AreConnected)
        {
            Debug.LogWarning("Tried connecting when already connected");
            return;
        }

        ClientWorkerHandler.CreateClient();
    }

    public static void TrySpawn()
    {
        if (!AreConnected)
        {
            Debug.LogWarning("Tried spawning when we weren't connected");
            return;
        }

        if (AreSpawned)
        {
            Debug.LogWarning("Tried spawning when we were already spawned");
            return;
        }

        connectionController.SpawnPlayerAction();
    }

    public static void TryDisconnect()
    {
        // TODO Implement some form of disconnection
    }

    public static void SetState(State state, string information = "")
    {
        CurrentState = state;
        CurrentInformation = information;

        AreConnected = CurrentState == State.Connected
            || CurrentState == State.Spawned
            || CurrentState == State.Spawning
            || CurrentState == State.SpawningFailed;
        HaveDeployments = CurrentState != State.None
            && CurrentState != State.GettingDeploymentList
            && CurrentState != State.FailedToGetDeploymentList;
        AreSpawned = CurrentState == State.Spawned;

        OnConnectionStateChange(state, information);
    }

    public delegate void ConnectionStateChange(State state, string information);

    public static ConnectionStateChange OnConnectionStateChange;


    private static ConnectionController connectionController;
    private static ScreenUIController screenUIController;

    public static void InformOfConnectionController(ConnectionController controller)
    {
        connectionController = controller;
    }

    public static void InformOfUIController(ScreenUIController controller)
    {
        screenUIController = controller;
    }
}
