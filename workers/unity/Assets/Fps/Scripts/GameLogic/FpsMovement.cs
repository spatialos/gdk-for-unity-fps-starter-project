using Improbable.Fps.Custommovement;
using Improbable.Gdk.Movement;
using Improbable.Gdk.StandardTypes;
using Improbable.Worker.CInterop;
using UnityEngine;

public class FpsMovement : AbstractMovementProcessor<CustomInput, CustomState>
{
    public static readonly MovementSettings MovementSettings = new MovementSettings
    {
        MovementSpeed = new MovementSpeedSettings
        {
            WalkSpeed = 2f,
            RunSpeed = 3.5f,
            SprintSpeed = 6f
        },
        SprintCooldown = 0.2f,
        Gravity = 25f,
        StartingJumpSpeed = 8f,
        TerminalVelocity = 40f,
        GroundedFallSpeed = 3.5f,
        AirControlModifier = 0.5f,
        InAirDamping = 0.05f
    };

    private readonly CharacterController controller;
    private readonly Vector3 origin;

    public readonly MyMovementUtils.TeleportMovement TeleportProcessor = new MyMovementUtils.TeleportMovement();

    public FpsMovement(CharacterController controller, Vector3 origin)
    {
        this.controller = controller;
        this.origin = origin;

        TeleportProcessor.Origin = origin;
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
                ref newState.StandardMovement,
                MovementSettings.AirControlModifier, MovementSettings.InAirDamping);

            MyMovementUtils.SprintCooldown.Update(
                input.SprintPressed, previousState.SprintCooldown,
                out newState.SprintCooldown, deltaTime, MovementSettings.SprintCooldown);

            JumpMovement.Process(input.JumpPressed, previousState.IsGrounded, previousState.CanJump,
                ref newState.StandardMovement, out var didJump, out var canJump, MovementSettings.StartingJumpSpeed);

            newState.DidJump = didJump;
            newState.CanJump = canJump;

            // Maintain jetpack charge.
            newState.JetpackCharge = previousState.JetpackCharge;

            // If we're in the air and pressing jump, accelerate us 1.5 * gravity upwards, up to StartingJumpSpeed.
            if (!previousState.IsGrounded && input.JumpPressed && previousState.JetpackCharge > 0)
            {
                var velocity = newState.StandardMovement.Velocity.ToVector3();
                velocity += Vector3.up * MovementSettings.Gravity * 1.5f * deltaTime;
                velocity.y = Mathf.Min(velocity.y, MovementSettings.StartingJumpSpeed);
                newState.StandardMovement.Velocity = velocity.ToIntAbsolute();

                // 100 charge gives 2 seconds of jetpack.
                const int chargePerSecond = 10000 / 2;

                var newCharge = Mathf.Floor(previousState.JetpackCharge - chargePerSecond * deltaTime);
                newState.JetpackCharge = (uint) Mathf.Clamp(newCharge, 0, 10000);
            }
            else if (!didJump && previousState.IsGrounded && previousState.JetpackCharge < 10000)
            {
                // Takes 1 seconds to recharge jetpack.
                const int rechargePerSecond = 10000 / 1;

                var newCharge = Mathf.Floor(previousState.JetpackCharge + rechargePerSecond * deltaTime);
                newState.JetpackCharge = (uint) Mathf.Clamp(newCharge, 0, 10000);
            }

            MyMovementUtils.Gravity.Apply(ref newState.StandardMovement, deltaTime, MovementSettings.Gravity);

            MyMovementUtils.TerminalVelocity.Apply(ref newState.StandardMovement, MovementSettings.TerminalVelocity);

            MyMovementUtils.CharacterControllerMovement.Move(controller, ref newState.StandardMovement, deltaTime);
        }
        else
        {
            newState.DidTeleport = true;

            MyMovementUtils.SprintCooldown.Reset(out newState.SprintCooldown);

            MyMovementUtils.CharacterControllerMovement.Teleport(controller, ref newState.StandardMovement);
        }

        MyMovementUtils.RemoveWorkerOrigin.Remove(ref newState.StandardMovement, origin);

        newState.IsGrounded =
            IsGroundedMovement.Get(newState.StandardMovement, previousState.StandardMovement, deltaTime);

        MyMovementUtils.AdjustVelocity.Apply(previousState.StandardMovement, ref newState.StandardMovement, deltaTime);

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

    public static float GetSpeed(bool isAiming, bool isSprinting)
    {
        var speed = MovementSettings.MovementSpeed.RunSpeed;

        if (isAiming)
        {
            speed = MovementSettings.MovementSpeed.WalkSpeed;
        }
        else if (isSprinting)
        {
            speed = MovementSettings.MovementSpeed.SprintSpeed;
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
