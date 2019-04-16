namespace Fps
{
    public abstract class SessionState : State
    {
        protected readonly UIManager Manager;
        protected readonly ConnectionStateMachine Owner;

        protected SessionState(UIManager manager, ConnectionStateMachine owner)
        {
            Manager = manager;
            Owner = owner;
        }

        protected void ShowErrorMessage(string errorMessage)
        {
            Manager.ScreenManager.StartScreenManager.ShowFailedToGetDeploymentsText(errorMessage);
            Manager.ScreenManager.LobbyScreenManager.ShowFailedToGetDeploymentsText(errorMessage);
        }
    }
}
