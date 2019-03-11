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
            QuickJoin,
            Connecting,
            Connected,
            ConnectionFailed,
            WaitingForGameStart,
            GameReady,
            Spawning,
            Spawned,
            SpawningFailed,
            WorkerDisconnected,
            GatherResults,
            ShowResults,
            EndSession,
        }

        public delegate void ConnectionStateChange(State state, string information);

        public static event ConnectionStateChange OnConnectionStateChange;
        public static State CurrentState;
        public static string CurrentInformation;
        public static bool IsConnected { get; private set; }
        public static bool HaveDeployments { get; private set; }
        public static bool IsSpawned { get; private set; }

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
    }
}
