using System.Resources;
using Improbable.Gdk.Movement;
using UnityEngine.Experimental.PlayerLoop;

public interface IMovementProcessor
{
    MovementState Process(byte[] input, MovementState previousState, float deltaTime);

    byte[] ConsumeInput();
}

public abstract class AbstractMovementProcessor<TInput> : IMovementProcessor where TInput : new()
{
    protected TInput Input;

    public byte[] ConsumeInput()
    {
        var raw = Serialize(Input);
        Input = new TInput();
        return raw;
    }

    public MovementState Process(byte[] input, MovementState previousState, float deltaTime)
    {
        return Process(Deserialize(input), previousState, deltaTime);
    }

    public abstract byte[] Serialize(TInput input);

    public abstract TInput Deserialize(byte[] raw);

    public abstract MovementState Process(TInput input, MovementState previousState, float deltaTime);
}
