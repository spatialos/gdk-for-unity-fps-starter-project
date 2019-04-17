namespace Fps
{
    public abstract class State
    {
        public abstract void StartState();
        public abstract void ExitState();

        public virtual void Tick()
        {
        }
    }
}
