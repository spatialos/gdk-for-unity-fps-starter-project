namespace Fps
{
    public abstract class State
    {
        public abstract void StartState();

        public virtual void ExitState()
        {
        }

        public virtual void Tick()
        {
        }
    }
}
