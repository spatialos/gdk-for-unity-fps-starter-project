using System;
using System.Collections.Generic;
using Improbable.Gdk.Movement;
using Improbable.Gdk.StandardTypes;
using UnityEngine;

public class MyMovementUtils
{
    public interface IMovementProcessor
    {
        bool Process(CharacterController controller, ClientRequest input, MovementState previousState,
            ref MovementState newState, float deltaTime);
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

    public class RestoreStateProcessor : IMovementProcessor
    {
        public Vector3 Origin = Vector3.zero;

        public bool Process(CharacterController controller, ClientRequest input, MovementState previousState,
            ref MovementState newState, float deltaTime)
        {
            RestoreToState(controller, previousState, Origin);
            return true;
        }
    }

    public static void RestoreToState(CharacterController controller, MovementState state, Vector3 origin)
    {
        controller.transform.position = state.Position.ToVector3() + origin;
    }

    public class TerminalVelocity : IMovementProcessor
    {
        public bool Process(CharacterController controller, ClientRequest input, MovementState previousState,
            ref MovementState newState, float deltaTime)
        {
            newState.Velocity = Vector3.ClampMagnitude(newState.Velocity.ToVector3(), movementSettings.TerminalVelocity)
                .ToIntAbsolute();
            return true;
        }
    }

    public class Gravity : IMovementProcessor
    {
        public bool Process(CharacterController controller, ClientRequest input, MovementState previousState,
            ref MovementState newState, float deltaTime)
        {
            if (newState.DidTeleport)
            {
                return true;
            }

            var velocity = newState.Velocity.ToVector3();

            velocity += Vector3.down * movementSettings.Gravity * deltaTime;

            // if (previousState.IsGrounded && velocity.y <= 0)
            // {
            //     velocity.y = -movementSettings.GroundedFallSpeed * deltaTime;
            // }
            // else
            // {
            //
            // }

            newState.Velocity = velocity.ToIntAbsolute();

            return true;
        }
    }

    public class SprintCooldown : IMovementProcessor
    {
        public bool Process(CharacterController controller, ClientRequest input, MovementState previousState,
            ref MovementState newState, float deltaTime)
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

        public float GetCooldown(MovementState state)
        {
            return state.SprintCooldown;
        }
    }

    public class ApplyMovementProcessor : IMovementProcessor
    {
        public bool Process(CharacterController controller, ClientRequest input, MovementState previousState,
            ref MovementState newState, float deltaTime)
        {
            if (newState.DidTeleport)
            {
                Debug.LogFormat("Found Teleport Request, teleporting to: {0}", newState.Position.ToVector3());
                controller.transform.position = newState.Position.ToVector3();

                return true;
            }

            if (controller.enabled)
            {
                controller.Move(newState.Velocity.ToVector3() * deltaTime);
            }

            newState.Position = controller.transform.position.ToIntAbsolute();

            return true;
        }
    }

    public class RemoveWorkerOrigin : IMovementProcessor
    {
        public Vector3 Origin;

        public bool Process(CharacterController controller, ClientRequest input, MovementState previousState,
            ref MovementState newState, float deltaTime)
        {
            newState.Position = (newState.Position.ToVector3() - Origin).ToIntAbsolute();
            return true;
        }
    }

    public class AdjustVelocity : IMovementProcessor
    {
        public bool Process(CharacterController controller, ClientRequest input, MovementState previousState,
            ref MovementState newState, float deltaTime)
        {
            // Don't adjust velocity if we teleported, since that would not produce a valid velocity.
            if (newState.DidTeleport)
            {
                return true;
            }

            var oldPosition = previousState.Position.ToVector3();
            var newPosition = newState.Position.ToVector3();
            newState.Velocity = ((newPosition - oldPosition) / deltaTime).ToIntAbsolute();

            return true;
        }
    }

    public class TeleportProcessor : IMovementProcessor
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

        public bool Process(CharacterController controller, ClientRequest input, MovementState previousState,
            ref MovementState newState, float deltaTime)
        {
            if (hasTeleport)
            {
                Debug.LogFormat("Procssing, hasTeleport: {0}", teleportPosition);
                newState.Position = teleportPosition.ToIntAbsolute();
                newState.DidTeleport = true;

                Debug.LogFormat("NewState.DidTeleport: {0}, NewState.Position: {1}", newState.DidTeleport, newState.Position.ToVector3());

                hasTeleport = false;
                teleportPosition = Vector3.zero;
            }

            return true;
        }
    }

    public const float FrameLength = CommandFrameSystem.FrameLength;

    public static MovementState ApplyInput(
        CharacterController controller, ClientRequest input, MovementState previousState, IMovementProcessor[] processors)
    {
        return ApplyPartialInput(controller, input, previousState, processors, CommandFrameSystem.FrameLength);
    }

    public static MovementState ApplyPartialInput(CharacterController controller, ClientRequest input,
        MovementState previousState, IMovementProcessor[] processors, float deltaTime)
    {
        var newState = new MovementState();
        for (var i = 0; i < processors.Length; i++)
        {
            if (!processors[i].Process(controller, input, previousState, ref newState, deltaTime))
            {
                break;
            }
        }

        return newState;
    }

    // public static bool IsGrounded(CharacterController controller)
    // {
    //     return Physics.CheckSphere(controller.transform.position, 0.1f, LayerMask.GetMask("Default"));
    // }

    public static int CalculateInputBufferSize(float rtt)
    {
        return Mathf.CeilToInt(rtt / (2 * CommandFrameSystem.FrameLength)) + ConstantInputBufferExtra;
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
