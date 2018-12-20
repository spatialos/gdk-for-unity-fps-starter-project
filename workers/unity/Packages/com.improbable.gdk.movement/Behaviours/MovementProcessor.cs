using UnityEngine;

public interface IMovementProcessor
{
    byte[] Process(byte[] input, byte[] previousState, float deltaTime);

    byte[] ConsumeInput();

    bool ShouldReplay(byte[] predicted, byte[] actual);

    Vector3 GetPosition(byte[] state);

    void RestoreToState(byte[] state);
}

public abstract class AbstractMovementProcessor<TInput, TState> : IMovementProcessor where TInput : new() where TState : new()
{
    protected TInput Input;

    public byte[] ConsumeInput()
    {
        var raw = SerializeInput(Input);
        Input = new TInput();
        return raw;
    }

    public byte[] Process(byte[] input, byte[] previousState, float deltaTime)
    {
        return SerializeState(Process(DeserializeInput(input), DeserializeState(previousState), deltaTime));
    }

    public bool ShouldReplay(byte[] predicted, byte[] actual)
    {
        return ShouldReplay(DeserializeState(predicted), DeserializeState(actual));
    }

    public Vector3 GetPosition(byte[] state)
    {
        return GetPosition(DeserializeState(state));
    }

    public void RestoreToState(byte[] state)
    {
        RestoreToState(DeserializeState(state));
    }

    public abstract byte[] SerializeInput(TInput input);

    public abstract TInput DeserializeInput(byte[] raw);

    public abstract byte[] SerializeState(TState state);

    public abstract TState DeserializeState(byte[] raw);

    public abstract TState Process(TInput input, TState previousState, float deltaTime);

    public abstract bool ShouldReplay(TState predicted, TState actual);

    public abstract Vector3 GetPosition(TState state);

    public abstract void RestoreToState(TState state);
}
