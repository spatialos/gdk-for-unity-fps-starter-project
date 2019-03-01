using UnityEngine;

namespace Fps
{
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
            WaitingForGameStart,
            GameReady,
            Spawning,
            Spawned,
            SpawningFailed,
            WorkerDisconnected
        }

        public static float TimeUntilGameStart { get; private set; }

        public delegate void ConnectionStateChange(State state, string information);

        public static ConnectionStateChange OnConnectionStateChange;
        public static State CurrentState;
        public static string CurrentInformation;
        public static bool IsConnected { get; private set; }
        public static bool HaveDeployments { get; private set; }
        public static bool IsSpawned { get; private set; }

        private static ConnectionController connectionController;
        private static ScreenUIController screenUIController;

        public static void TryConnect()
        {
            if (IsConnected)
            {
                Debug.LogWarning("Tried connecting whilst already connected");
                return;
            }

            ClientWorkerHandler.CreateClient();
        }

        public static void TrySpawn()
        {
            if (!IsConnected)
            {
                Debug.LogWarning("Tried spawning whilst not connected");
                return;
            }

            if (IsSpawned)
            {
                Debug.LogWarning("Tried spawning whilst already spawned");
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

            IsConnected = CurrentState == State.Connected
                || CurrentState == State.WaitingForGameStart
                || CurrentState == State.GameReady
                || CurrentState == State.Spawned
                || CurrentState == State.Spawning
                || CurrentState == State.SpawningFailed;
            HaveDeployments = CurrentState != State.None
                && CurrentState != State.GettingDeploymentList
                && CurrentState != State.FailedToGetDeploymentList;
            IsSpawned = CurrentState == State.Spawned;

            OnConnectionStateChange?.Invoke(state, information);
        }

        public static void InformOfConnectionController(ConnectionController controller)
        {
            connectionController = controller;
        }

        public static void InformOfUIController(ScreenUIController controller)
        {
            screenUIController = controller;
        }

        public static void SetTimeUntilGameStart(float timeInSeconds)
        {
            TimeUntilGameStart = timeInSeconds;
        }
    }
}
