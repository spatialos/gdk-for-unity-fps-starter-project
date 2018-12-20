using Improbable.Gdk.Movement;
using Improbable.Gdk.StandardTypes;
using UnityEngine;

public class JumpMovement
{
    public static void Process(bool jumpPressed, bool grounded, bool canJump,
        ref StandardMovementState standardMovementState, out bool didJump, out bool newCanJump)
    {
        if (grounded && canJump && jumpPressed)
        {
            standardMovementState.Velocity =
                (standardMovementState.Velocity.ToVector3() + Vector3.up * MyMovementUtils.movementSettings.StartingJumpSpeed)
                .ToIntAbsolute();
            didJump = true;
        }
        else
        {
            didJump = false;
        }

        if (!jumpPressed && grounded)
        {
            newCanJump = true;
        }
        else
        {
            newCanJump = false;
        }
    }
}
