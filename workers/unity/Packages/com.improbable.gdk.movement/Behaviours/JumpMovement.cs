using Improbable.Gdk.Movement;
using Improbable.Gdk.StandardTypes;
using UnityEngine;

public class JumpMovement : MyMovementUtils.IMovementProcessor
{
    public bool Process(CharacterController controller, ClientRequest input, MovementState previousState,
        ref MovementState newState, float deltaTime)
    {
        if (newState.DidTeleport)
        {
            return true;
        }

        var grounded = MyMovementUtils.IsGrounded(controller);
        var canJump = previousState.CanJump;
        var jumpPressed = input.JumpPressed;

        if (grounded && canJump && jumpPressed)
        {
            newState.Velocity =
                (newState.Velocity.ToVector3() + Vector3.up * MyMovementUtils.movementSettings.StartingJumpSpeed)
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

    public bool DidJump(MovementState state)
    {
        return state.DidJump;
    }
}
