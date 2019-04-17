namespace Fps
{
    public class DefaultInitState : DefaultState
    {
        public DefaultInitState(UIManager manager, ConnectionStateMachine owner) : base(manager, owner)
        {
        }

        public override void StartState()
        {
            Manager.ShowFrontEnd();
            Manager.ScreenManager.SwitchToDefaultConnectScreen();
            Owner.SetState(new DefaultConnectState(Manager, Owner));
        }

        public override void ExitState()
        {
        }
    }
}
