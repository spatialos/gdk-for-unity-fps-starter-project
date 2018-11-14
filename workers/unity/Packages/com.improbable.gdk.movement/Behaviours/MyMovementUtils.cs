using System;
using System.Collections.Generic;
using System.Reflection;
using Improbable.Gdk.Movement;
using Improbable.Gdk.StandardTypes;
using UnityEngine;

public class MyMovementUtils
{
    public interface IMovementProcessor
    {
        Vector3 GetMovement(CharacterController controller, ClientRequest input, int frame, Vector3 velocity,
            Vector3 previous);

        void Clean(int frame);
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

    public class TerminalVelocity : IMovementProcessor
    {
        public Vector3 GetMovement(CharacterController controller, ClientRequest input, int frame, Vector3 velocity,
            Vector3 previous)
        {
            return Vector3.ClampMagnitude(previous, movementSettings.TerminalVelocity);
        }

        public void Clean(int frame)
        {
            // Do nothing, not storing any state that needs cleaning.
        }
    }

    public class Gravity : IMovementProcessor
    {
        public Vector3 GetMovement(CharacterController controller, ClientRequest input, int frame, Vector3 velocity,
            Vector3 previous)
        {
            return previous + Vector3.down * movementSettings.Gravity * FrameLength;
        }

        public void Clean(int frame)
        {
            // Do nothing, not storing any state that needs cleaning.
        }
    }

    public class SprintCooldown : IMovementProcessor
    {
        private Dictionary<int, float> cooldown = new Dictionary<int, float>();

        public Vector3 GetMovement(CharacterController controller, ClientRequest input, int frame, Vector3 velocity,
            Vector3 previous)
        {
            if (input.IncludesSprint)
            {
                cooldown[frame] = movementSettings.SprintCooldown;
            }
            else
            {
                cooldown.TryGetValue(frame - 1, out var last);
                cooldown[frame] = Mathf.Max(last - FrameLength, 0);
            }

            return previous;
        }

        public void Clean(int frame)
        {
            cooldown.Remove(frame);
        }

        public float GetCooldown(int frame)
        {
            cooldown.TryGetValue(frame, out var result);
            return result;
        }
    }

    public const float FrameLength = CommandFrameSystem.FrameLength;

    public static void ApplyInput(CharacterController controller, ClientRequest input, int frame, Vector3 velocity,
        IMovementProcessor[] processors)
    {
        var toMove = Vector3.zero;
        for (var i = 0; i < processors.Length; i++)
        {
            toMove = processors[i].GetMovement(controller, input, frame, velocity, toMove);
        }

        ApplyMovement(controller, toMove);
    }

    public static void ApplyMovement(CharacterController controller, Vector3 movement)
    {
        controller.Move(movement * FrameLength);
        // controller.transform.position = controller.transform.position.ToIntAbsolute().ToVector3();
    }

    public static bool IsGrounded(CharacterController controller)
    {
        return Physics.CheckSphere(controller.transform.position, 0.1f, LayerMask.GetMask("Default"));
    }

    public static void CleanProcessors(IMovementProcessor[] processors, int frame)
    {
        for (var i = 0; i < processors.Length; i++)
        {
            processors[i].Clean(frame);
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
