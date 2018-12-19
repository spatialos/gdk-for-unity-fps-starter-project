using Improbable.Fps.Custommovement;
using Improbable.Gdk.GameObjectRepresentation;
using Improbable.Gdk.Movement;
using Improbable.Worker.CInterop;
using UnityEngine;

public class FpsMovement : AbstractMovementProcessor<CustomInput>
{
    private readonly GameObject go;
    private readonly CharacterController controller;

    private bool initialized;
    private MyMovementUtils.IMovementProcessorOLD[] processors;
    private readonly JumpMovement jumpMovement = new JumpMovement();
    public readonly MyMovementUtils.SprintCooldown SprintCooldown = new MyMovementUtils.SprintCooldown();
    private readonly MyMovementUtils.RemoveWorkerOrigin removeOrigin = new MyMovementUtils.RemoveWorkerOrigin();

    public FpsMovement(GameObject go, CharacterController controller)
    {
        this.go = go;
        this.controller = controller;
    }

    public void AddInput(bool forward = false, bool back = false, bool left = false, bool right = false,
        bool jump = false, bool sprint = false, bool aim = false, float yaw = -1, float pitch = -1)
    {
        Input.ForwardPressed |= forward;
        Input.BackPressed |= back;
        Input.LeftPressed |= left;
        Input.RightPressed |= right;
        Input.JumpPressed |= jump;
        Input.SprintPressed |= sprint;
        Input.AimPressed |= aim;
        if (yaw >= 0)
        {
            Input.Yaw = (int) yaw * 100000;
        }

        if (pitch >= 0)
        {
            Input.Pitch = (int) pitch * 100000;
        }
    }

    public override byte[] Serialize(CustomInput input)
    {
        var sco = new SchemaComponentData(0);
        CustomInput.Serialization.Serialize(input, sco.GetFields());
        return sco.GetFields().Serialize();
    }

    public override CustomInput Deserialize(byte[] raw)
    {
        var sco = new SchemaComponentData(0);
        sco.GetFields().MergeFromBuffer(raw);
        return CustomInput.Serialization.Deserialize(sco.GetFields());
    }

    public override MovementState Process(CustomInput input, MovementState previousState, float deltaTime)
    {
        if (!initialized)
        {
            Init();
        }

        var newState = new MovementState();
        for (var i = 0; i < processors.Length; i++)
        {
            if (!processors[i].Process(input, previousState, ref newState, deltaTime))
            {
                break;
            }
        }

        return newState;
    }

    private void Init()
    {
        removeOrigin.Origin = go.GetComponent<SpatialOSComponent>().Worker.Origin;

        processors = new MyMovementUtils.IMovementProcessorOLD[]
        {
            new StandardMovement(),
            SprintCooldown,
            jumpMovement,
            new MyMovementUtils.Gravity(),
            new MyMovementUtils.TerminalVelocity(),
            new MyMovementUtils.CharacterControllerMovement(controller),
            removeOrigin,
            new IsGroundedMovement(),
            new MyMovementUtils.AdjustVelocity()
        };

        initialized = true;
    }
}
