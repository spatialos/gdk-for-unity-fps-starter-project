using System.Collections.Generic;
using Improbable.Gdk.Movement;
using Improbable.Gdk.StandardTypes;
using UnityEngine;

public class MyMovementUtils
{
    public interface IMovementProcessor
    {
        bool Process(CharacterController controller, ClientRequest input, MovementState previousState, ref MovementState newState);
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

    public class RestoreStateProcessor : IMovementProcessor
    {
        public Vector3 Origin = Vector3.zero;

        public bool Process(CharacterController controller, ClientRequest input, MovementState previousState,
            ref MovementState newState)
        {
            controller.transform.position = previousState.Position.ToVector3() + Origin;

            return true;
        }
    }

    public class TerminalVelocity : IMovementProcessor
    {
        public bool Process(CharacterController controller, ClientRequest input, MovementState previousState,
            ref MovementState newState)
        {
            newState.Velocity = Vector3.ClampMagnitude(newState.Velocity.ToVector3(), movementSettings.TerminalVelocity)
                .ToIntAbsolute();
            return true;
        }
    }

    public class Gravity : IMovementProcessor
    {
        public bool Process(CharacterController controller, ClientRequest input, MovementState previousState,
            ref MovementState newState)
        {
            var velocity = newState.Velocity.ToVector3();
            if (IsGrounded(controller) && velocity.y <= 0)
            {
                velocity.y = -movementSettings.GroundedFallSpeed * FrameLength;
            }
            else
            {
                velocity += Vector3.down * movementSettings.Gravity * FrameLength;
            }

            newState.Velocity = velocity.ToIntAbsolute();

            return true;
        }
    }

    public class SprintCooldown : IMovementProcessor
    {
        private Dictionary<int, float> cooldown = new Dictionary<int, float>();

        public bool Process(CharacterController controller, ClientRequest input, MovementState previousState,
            ref MovementState newState)
        {
            if (input.SprintPressed)
            {
                newState.SprintCooldown = movementSettings.SprintCooldown;
            }
            else
            {
                newState.SprintCooldown = Mathf.Max(previousState.SprintCooldown - FrameLength, 0);
            }

            return true;
        }

        public float GetCooldown(MovementState state)
        {
            return state.SprintCooldown;
        }
    }

    public class ApplyMovementProcessor : IMovementProcessor
    {
        public bool Process(CharacterController controller, ClientRequest input, MovementState previousState,
            ref MovementState newState)
        {
            if (controller.enabled)
            {
                controller.Move(newState.Velocity.ToVector3() * FrameLength);
            }

            newState.Position = controller.transform.position.ToIntAbsolute();

            return true;
        }
    }

    public class RemoveWorkerOrigin : IMovementProcessor
    {
        public Vector3 Origin;

        public bool Process(CharacterController controller, ClientRequest input, MovementState previousState,
            ref MovementState newState)
        {
            newState.Position = (newState.Position.ToVector3() - Origin).ToIntAbsolute();
            return true;
        }
    }

    public class AdjustVelocity : IMovementProcessor
    {
        public bool Process(CharacterController controller, ClientRequest input, MovementState previousState,
            ref MovementState newState)
        {
            var oldPosition = previousState.Position.ToVector3();
            var newPosition = newState.Position.ToVector3();
            newState.Velocity = ((newPosition - oldPosition) / FrameLength).ToIntAbsolute();

            return true;
        }
    }

    public const float FrameLength = CommandFrameSystem.FrameLength;

    public static MovementState ApplyInput(
        CharacterController controller, ClientRequest input, MovementState previousState, IMovementProcessor[] processors)
    {
        MovementState newState = new MovementState();
        for (int i = 0; i < processors.Length; i++)
        {
            if (!processors[i].Process(controller, input, previousState, ref newState))
            {
                break;
            }
        }

        return newState;
    }

    public static bool IsGrounded(CharacterController controller)
    {
        return Physics.CheckSphere(controller.transform.position, 0.1f, LayerMask.GetMask("Default"));
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
