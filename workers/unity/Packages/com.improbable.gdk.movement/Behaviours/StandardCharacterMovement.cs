using System;
using Improbable.Gdk.Movement;
using Improbable.Gdk.StandardTypes;
using UnityEngine;

public class StandardCharacterMovement
{
    private const float Root2Over2 = 0.70710678118f;

    private static readonly Vector3[] DirectionVectors =
    {
        Vector3.zero,
        Vector3.forward, // w
        Vector3.back, // s
        Vector3.zero, // ws
        Vector3.left, // a
        new Vector3(-Root2Over2, 0, Root2Over2), // wa
        new Vector3(-Root2Over2, 0, -Root2Over2), // as
        Vector3.left, // was
        Vector3.right, // d
        new Vector3(Root2Over2, 0, Root2Over2), // wd
        new Vector3(Root2Over2, 0, -Root2Over2), // sd
        Vector3.right, // wsd
        Vector3.zero, // ad
        Vector3.forward, // wad
        Vector3.back, // asd
        Vector3.zero, // wasd
    };

    [Flags]
    private enum DirectionEnum : byte
    {
        Forward = 1 << 0,
        Backward = 1 << 1,
        Left = 1 << 2,
        Right = 1 << 3,
    }

    public static Vector3 GetDesiredDirectionFromWasd(bool forward, bool backward, bool left, bool right)
    {
        DirectionEnum directionData = 0;
        if (forward)
        {
            directionData |= DirectionEnum.Forward;
        }

        if (backward)
        {
            directionData |= DirectionEnum.Backward;
        }

        if (left)
        {
            directionData |= DirectionEnum.Left;
        }

        if (right)
        {
            directionData |= DirectionEnum.Right;
        }

        return DirectionVectors[(int) directionData];
    }

    public static Vector3 RotateToFaceYaw(Vector3 direction, float yaw)
    {
        var rot = Quaternion.Euler(0, yaw, 0);

        return rot * direction;
    }

    public static void ApplyMovement(
        float speed,
        Vector3 unitDirection,
        StandardMovementState previousMovementState, bool isGrounded,
        ref StandardMovementState newMovementState)
    {
        var newVelocity = previousMovementState.Velocity.ToVector3();

        // multiply by speed.
        unitDirection *= speed;

        if (isGrounded)
        {
            ApplyGroundControl(unitDirection, ref newVelocity);
        }
        else
        {
            // Apply movement with air control damping.
            ApplyAirControl(unitDirection, ref newVelocity,
                MyMovementUtils.movementSettings.AirControlModifier,
                MyMovementUtils.movementSettings.InAirDamping, speed);
        }

        newMovementState.Velocity = newVelocity.ToIntAbsolute();
    }

    public static void ApplyGroundControl(Vector3 direction, ref Vector3 velocity)
    {
        velocity.z = direction.z;
        velocity.x = direction.x;
    }


    public static void ApplyAirControl(Vector3 direction, ref Vector3 velocity, float modifier, float damping, float maxSpeed)
    {
        var controlledMovement = new Vector2(velocity.x, velocity.z);
        var input2D = new Vector2(direction.x, direction.z);

        controlledMovement = controlledMovement * (1 - damping) + input2D * modifier;

        controlledMovement = Vector2.ClampMagnitude(controlledMovement, maxSpeed);

        velocity.x = controlledMovement.x;
        velocity.z = controlledMovement.y;
    }
}
