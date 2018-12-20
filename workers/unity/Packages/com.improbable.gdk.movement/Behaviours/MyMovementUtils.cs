using System.Net.NetworkInformation;
using Improbable.Fps.Custommovement;
using Improbable.Gdk.Movement;
using Improbable.Gdk.StandardTypes;
using UnityEngine;

public class MyMovementUtils
{
    public interface IMovementProcessorOLD
    {
        bool Process(CustomInput input, CustomState previousState,
            ref CustomState newState, float deltaTime);
    }

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

    public class TerminalVelocity : IMovementProcessorOLD
    {
        public bool Process(CustomInput input, CustomState previousState,
            ref CustomState newState, float deltaTime)
        {
            newState.StandardMovement.Velocity = Vector3.ClampMagnitude(newState.StandardMovement.Velocity.ToVector3(), movementSettings.TerminalVelocity)
                .ToIntAbsolute();
            return true;
        }
    }

    public class Gravity : IMovementProcessorOLD
    {
        public bool Process(CustomInput input, CustomState previousState,
            ref CustomState newState, float deltaTime)
        {
            if (newState.DidTeleport)
            {
                return true;
            }

            var velocity = newState.StandardMovement.Velocity.ToVector3();

            velocity += Vector3.down * movementSettings.Gravity * deltaTime;

            newState.StandardMovement.Velocity = velocity.ToIntAbsolute();

            return true;
        }
    }

    public class SprintCooldown : IMovementProcessorOLD
    {
        public bool Process(CustomInput input, CustomState previousState,
            ref CustomState newState, float deltaTime)
        {
            if (newState.DidTeleport)
            {
                newState.SprintCooldown = 0;
                return true;
            }

            if (input.SprintPressed)
            {
                newState.SprintCooldown = movementSettings.SprintCooldown;
            }
            else
            {
                newState.SprintCooldown = Mathf.Max(previousState.SprintCooldown - deltaTime, 0);
            }

            return true;
        }

        public float GetCooldown(CustomState state)
        {
            return state.SprintCooldown;
        }
    }

    public class CharacterControllerMovement : IMovementProcessorOLD
    {
        private readonly CharacterController controller;

        public CharacterControllerMovement(CharacterController controller)
        {
            this.controller = controller;
        }

        public bool Process(CustomInput input, CustomState previousState,
            ref CustomState newState, float deltaTime)
        {
            if (newState.DidTeleport)
            {
                Debug.LogFormat("Found Teleport Request, teleporting to: {0}", newState.StandardMovement.Position.ToVector3());
                controller.transform.position = newState.StandardMovement.Position.ToVector3();

                return true;
            }

            if (controller.enabled)
            {
                controller.Move(newState.StandardMovement.Velocity.ToVector3() * deltaTime);
            }

            newState.StandardMovement.Position = controller.transform.position.ToIntAbsolute();

            return true;
        }
    }

    public class RemoveWorkerOrigin : IMovementProcessorOLD
    {
        public Vector3 Origin;

        public bool Process(CustomInput input, CustomState previousState,
            ref CustomState newState, float deltaTime)
        {
            newState.StandardMovement.Position = (newState.StandardMovement.Position.ToVector3() - Origin).ToIntAbsolute();
            return true;
        }
    }

    public class AdjustVelocity : IMovementProcessorOLD
    {
        public bool Process(CustomInput input, CustomState previousState,
            ref CustomState newState, float deltaTime)
        {
            // Don't adjust velocity if we teleported, since that would not produce a valid velocity.
            if (newState.DidTeleport)
            {
                return true;
            }

            var oldPosition = previousState.StandardMovement.Position.ToVector3();
            var newPosition = newState.StandardMovement.Position.ToVector3();
            newState.StandardMovement.Velocity = ((newPosition - oldPosition) / deltaTime).ToIntAbsolute();

            return true;
        }
    }

    public class TeleportProcessorOld : IMovementProcessorOLD
    {
        public Vector3 Origin { get; set; }

        private Vector3 teleportPosition = Vector3.zero;
        private bool hasTeleport;

        public void Teleport(Vector3 position)
        {
            Debug.LogFormat("TeleportProcessor: hasTeleport, position: {0}", position + Origin);
            hasTeleport = true;
            teleportPosition = position + Origin;
        }

        public bool Process(CustomInput input, CustomState previousState,
            ref CustomState newState, float deltaTime)
        {
            if (hasTeleport)
            {
                Debug.LogFormat("Procssing, hasTeleport: {0}", teleportPosition);
                newState.StandardMovement.Position = teleportPosition.ToIntAbsolute();
                newState.DidTeleport = true;

                Debug.LogFormat("NewState.DidTeleport: {0}, NewState.Position: {1}", newState.DidTeleport, newState.StandardMovement.Position.ToVector3());

                hasTeleport = false;
                teleportPosition = Vector3.zero;
            }

            return true;
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
