namespace Fps
{
    public abstract class State
    {
        public abstract void StartState();

        public abstract void Tick();
        public abstract void ExitState();
    }
}
