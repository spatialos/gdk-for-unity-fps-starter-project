using Improbable.Fps.Custommovement;
using Improbable.Gdk.StandardTypes;
using Improbable.Worker.CInterop;
using UnityEngine;

public class FpsMovement : AbstractMovementProcessor<CustomInput, CustomState>
{
    private readonly CharacterController controller;
    private readonly Vector3 origin;

    private readonly MyMovementUtils.IMovementProcessorOLD[] processors;
    private readonly MyMovementUtils.RemoveWorkerOrigin removeOrigin = new MyMovementUtils.RemoveWorkerOrigin();
    public readonly MyMovementUtils.TeleportMovement TeleportProcessor = new MyMovementUtils.TeleportMovement();

    public FpsMovement(CharacterController controller, Vector3 origin)
    {
        this.controller = controller;
        this.origin = origin;

        TeleportProcessor.Origin = origin;
        removeOrigin.Origin = origin;

        processors = new MyMovementUtils.IMovementProcessorOLD[]
        {
            new MyMovementUtils.Gravity(),
            new MyMovementUtils.TerminalVelocity(),
            new MyMovementUtils.CharacterControllerMovement(controller),
            removeOrigin,
            new IsGroundedMovement(),
            new MyMovementUtils.AdjustVelocity()
        };
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

    public override CustomState Process(CustomInput input, CustomState previousState, float deltaTime)
    {
        var newState = new CustomState { IsAiming = input.AimPressed, Pitch = input.Pitch, Yaw = input.Yaw };

        if (!TeleportProcessor.ApplyTeleport(ref newState.StandardMovement))
        {
            var speed = GetSpeed(input.AimPressed, input.SprintPressed);

            var direction = StandardCharacterMovement.GetDesiredDirectionFromWasd(input.ForwardPressed, input.BackPressed,
                input.LeftPressed, input.RightPressed);

            direction = StandardCharacterMovement.RotateToFaceYaw(direction, input.Yaw / 100000f);

            StandardCharacterMovement.ApplyMovement(speed, direction,
                previousState.StandardMovement,
                previousState.IsGrounded,
                ref newState.StandardMovement);

            MyMovementUtils.SprintCooldown.Update(
                input.SprintPressed, previousState.SprintCooldown,
                out newState.SprintCooldown, deltaTime);

            JumpMovement.Process(input.JumpPressed, previousState.IsGrounded, previousState.CanJump,
                ref newState.StandardMovement, out var didJump, out var canJump);

            newState.DidJump = didJump;
            newState.CanJump = canJump;
        }
        else
        {
            newState.DidTeleport = true;

            MyMovementUtils.SprintCooldown.Reset(out newState.SprintCooldown);
        }

        for (var i = 0; i < processors.Length; i++)
        {
            if (!processors[i].Process(input, previousState, ref newState, deltaTime))
            {
                break;
            }
        }

        return newState;
    }

    public override bool ShouldReplay(CustomState predicted, CustomState actual)
    {
        var predictionPosition = predicted.StandardMovement.Position.ToVector3();
        var actualPosition = actual.StandardMovement.Position.ToVector3();
        var distance = Vector3.Distance(predictionPosition, actualPosition);
        return (distance > 0.1f);
    }

    public override Vector3 GetPosition(CustomState state)
    {
        return state.StandardMovement.Position.ToVector3();
    }

    public override void RestoreToState(CustomState state)
    {
        controller.transform.position = state.StandardMovement.Position.ToVector3() + origin;
    }

    private static float GetSpeed(bool isAiming, bool isSprinting)
    {
        var speed = MyMovementUtils.movementSettings.MovementSpeed.RunSpeed;

        if (isAiming)
        {
            speed = MyMovementUtils.movementSettings.MovementSpeed.WalkSpeed;
        }
        else if (isSprinting)
        {
            speed = MyMovementUtils.movementSettings.MovementSpeed.SprintSpeed;
        }

        return speed;
    }

    #region Serialization

    public override byte[] SerializeInput(CustomInput input)
    {
        var sco = new SchemaComponentData(0);
        CustomInput.Serialization.Serialize(input, sco.GetFields());
        return sco.GetFields().Serialize();
    }

    public override CustomInput DeserializeInput(byte[] raw)
    {
        var sco = new SchemaComponentData(0);
        sco.GetFields().MergeFromBuffer(raw);
        return CustomInput.Serialization.Deserialize(sco.GetFields());
    }

    public static byte[] SerializeStateStatic(CustomState state)
    {
        var sco = new SchemaComponentData(0);
        CustomState.Serialization.Serialize(state, sco.GetFields());
        return sco.GetFields().Serialize();
    }

    public override byte[] SerializeState(CustomState state)
    {
        return SerializeStateStatic(state);
    }

    public static CustomState DeserializeStateStatic(byte[] raw)
    {
        if (raw == null)
        {
            return default(CustomState);
        }

        var sco = new SchemaComponentData(0);
        sco.GetFields().MergeFromBuffer(raw);
        return CustomState.Serialization.Deserialize(sco.GetFields());
    }

    public override CustomState DeserializeState(byte[] raw)
    {
        return DeserializeStateStatic(raw);
    }

    #endregion
}
