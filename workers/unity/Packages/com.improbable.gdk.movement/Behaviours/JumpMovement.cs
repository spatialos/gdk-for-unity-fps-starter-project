using Improbable.Fps.Custommovement;
using Improbable.Gdk.Movement;
using Improbable.Gdk.StandardTypes;
using UnityEngine;

public class JumpMovement : MyMovementUtils.IMovementProcessorOLD
{
    public bool Process(CustomInput input, CustomState previousState,
        ref CustomState newState, float deltaTime)
    {
        if (newState.DidTeleport)
        {
            return true;
        }

        var grounded = previousState.IsGrounded;
        var canJump = previousState.CanJump;
        var jumpPressed = input.JumpPressed;

        if (grounded && canJump && jumpPressed)
        {
            newState.StandardMovement.Velocity =
                (newState.StandardMovement.Velocity.ToVector3() + Vector3.up * MyMovementUtils.movementSettings.StartingJumpSpeed)
                .ToIntAbsolute();
            newState.DidJump = true;
        }
        else
        {
            newState.DidJump = false;
        }

        if (!jumpPressed && grounded)
        {
            newState.CanJump = true;
        }
        else
        {
            newState.CanJump = false;
        }

        return true;
    }
}
