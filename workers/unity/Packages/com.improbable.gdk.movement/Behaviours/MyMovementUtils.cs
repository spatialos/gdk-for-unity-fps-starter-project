using Improbable.Gdk.Movement;
using Improbable.Gdk.StandardTypes;
using UnityEngine;

public class MyMovementUtils
{
    public static bool ShowDebug;

    public static readonly MovementSettings movementSettings = new MovementSettings
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

    public const int ConstantInputBufferExtra = 2;

    public static class TerminalVelocity
    {
        public static void Apply(ref StandardMovementState standardMovementState)
        {
            standardMovementState.Velocity = Vector3
                .ClampMagnitude(standardMovementState.Velocity.ToVector3(), movementSettings.TerminalVelocity)
                .ToIntAbsolute();
        }
    }

    public static class Gravity
    {
        public static void Apply(ref StandardMovementState standardMovementState, float deltaTime)
        {
            var velocity = standardMovementState.Velocity.ToVector3();

            velocity += Vector3.down * movementSettings.Gravity * deltaTime;

            standardMovementState.Velocity = velocity.ToIntAbsolute();
        }
    }

    public static class SprintCooldown
    {
        public static void Reset(out float newCooldown)
        {
            newCooldown = 0;
        }

        public static void Update(bool sprintPressed, float previousCooldown, out float newCooldown, float deltaTime)
        {
            if (sprintPressed)
            {
                newCooldown = movementSettings.SprintCooldown;
            }
            else
            {
                newCooldown = Mathf.Max(previousCooldown - deltaTime, 0);
            }
        }
    }

    public static class CharacterControllerMovement
    {
        public static void Move(CharacterController controller, ref StandardMovementState standardMovementState,
            float deltaTime)
        {
            if (controller.enabled)
            {
                controller.Move(standardMovementState.Velocity.ToVector3() * deltaTime);
            }

            standardMovementState.Position = controller.transform.position.ToIntAbsolute();
        }

        public static void Teleport(CharacterController controller, ref StandardMovementState standardMovementState)
        {
            controller.transform.position = standardMovementState.Position.ToVector3();
        }
    }

    public static class RemoveWorkerOrigin
    {
        public static void Remove(ref StandardMovementState standardMovementState, Vector3 origin)
        {
            standardMovementState.Position = (standardMovementState.Position.ToVector3() - origin).ToIntAbsolute();
        }
    }

    public static class AdjustVelocity
    {
        public static void Apply(StandardMovementState previousState,
            ref StandardMovementState newState, float deltaTime)
        {
            var oldPosition = previousState.Position.ToVector3();
            var newPosition = newState.Position.ToVector3();
            newState.Velocity = ((newPosition - oldPosition) / deltaTime).ToIntAbsolute();
        }
    }

    public class TeleportMovement
    {
        public Vector3 Origin { get; set; }

        private Vector3 teleportPosition = Vector3.zero;
        private bool hasTeleport;

        public void Teleport(Vector3 position)
        {
            hasTeleport = true;
            teleportPosition = position + Origin;
        }

        public bool ApplyTeleport(ref StandardMovementState standardMovement)
        {
            if (hasTeleport)
            {
                standardMovement.Position = teleportPosition.ToIntAbsolute();

                hasTeleport = false;
                teleportPosition = Vector3.zero;

                return true;
            }

            return false;
        }
    }

    public const float FrameLength = CommandFrameSystem.FrameLength;

    public static MovementState ApplyCustomInput(
        ClientRequest input, MovementState previousState, IMovementProcessor customProcessor)
    {
        return ApplyPartialCustomInput(input, previousState, customProcessor, CommandFrameSystem.FrameLength);
    }

    public static MovementState ApplyPartialCustomInput(
        ClientRequest input, MovementState previousState, IMovementProcessor customProcessor, float deltaTime)
    {
        var state = new MovementState
        {
            RawState = customProcessor.Process(input.InputRaw, previousState.RawState, deltaTime)
        };
        return state;
    }

    public static bool GetProxyState<TInput, TState>(out float t, out TState from, out TState to,
        MyProxyMovementDriver proxyDriver, AbstractMovementProcessor<TInput, TState> processor) where TInput : new() where TState : new()
    {
        if (proxyDriver.GetInterpState(out t, out var fromRaw, out var toRaw))
        {
            from = processor.DeserializeState(fromRaw);
            to = processor.DeserializeState(toRaw);
            return true;
        }
        else
        {
            from = default(TState);
            to = default(TState);
            return false;
        }
    }

    public static TState GetLatestState<TInput, TState>(MyClientMovementDriver driver,
        AbstractMovementProcessor<TInput, TState> processor) where TInput : new() where TState : new()
    {
        return processor.DeserializeState(driver.GetLatestState().RawState);
    }

    public static int CalculateInputBufferSize(float rtt)
    {
        return Mathf.CeilToInt(rtt / (2 * CommandFrameSystem.FrameLength)) + ConstantInputBufferExtra;
    }

    public static float GetMovmentSpeedVelocity(MovementSpeed speed)
    {
        switch (speed)
        {
            case MovementSpeed.Walk:
                return movementSettings.MovementSpeed.WalkSpeed;
            case MovementSpeed.Run:
                return movementSettings.MovementSpeed.RunSpeed;
            case MovementSpeed.Sprint:
                return movementSettings.MovementSpeed.SprintSpeed;
            default:
                return -1;
        }
    }

    public class PidController
    {
        public float Kp;
        public float Ki;
        public float Kd;

        public float lastError;
        public float integral;
        public float value;

        public PidController(float kp, float ki, float kd)
        {
            Kp = kp;
            Ki = ki;
            Kd = kd;
        }

        public PidController(float kp, float ki, float kd, float initialValue, float initialIntegral)
        {
            Kp = kp;
            Ki = ki;
            Kd = kd;
            value = initialValue;
            integral = initialIntegral;
        }

        public float Update(float error, float dt)
        {
            float derivative = (error - lastError) / dt;
            integral += error * dt;
            lastError = error;

            value = Kp * error + Ki * integral + Kd * derivative;
            return value;
        }
    }
}
