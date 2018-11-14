using System.Collections.Generic;
using Improbable.Gdk.Movement;
using UnityEngine;

public class JumpMovement : MyMovementUtils.IMovementProcessor
{
    private Dictionary<int, bool> jumpState = new Dictionary<int, bool>();
    private Dictionary<int, bool> jumped = new Dictionary<int, bool>();

    public Vector3 GetMovement(CharacterController controller, ClientRequest input, int frame, Vector3 velocity,
        Vector3 previous)
    {
        var result = previous;

        var grounded = MyMovementUtils.IsGrounded(controller);
        jumpState.TryGetValue(frame - 1, out var canJump);
        var jumpPressed = input.JumpPressed;

        if (grounded && canJump && jumpPressed)
        {
            result.y = MyMovementUtils.movementSettings.StartingJumpSpeed;
            jumped[frame] = true;
        }
        else
        {
            jumped[frame] = false;
        }

        if (!jumpPressed && grounded)
        {
            jumpState[frame] = true;
        }
        else
        {
            jumpState[frame] = false;
        }

        return result;
    }

    public void Clean(int frame)
    {
        // remove old jump state.
        jumpState.Remove(frame);
    }

    public bool DidJump(int frame)
    {
        jumped.TryGetValue(frame, out var result);
        return result;
    }
}
