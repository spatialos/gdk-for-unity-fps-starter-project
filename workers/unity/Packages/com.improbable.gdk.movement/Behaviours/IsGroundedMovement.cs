using Improbable.Fps.Custommovement;
using Improbable.Gdk.Movement;
using Improbable.Gdk.StandardTypes;
using UnityEngine;

// Checks newstate velocity vs actual character movement, sets IsGrounded = true not all desired vertical movement
// happened (because we hit the ground).
public class IsGroundedMovement : MyMovementUtils.IMovementProcessorOLD
{
    private const float DeltaThreshold = 0.1f;

    public bool Process(CustomInput input, CustomState previousState,
        ref CustomState newState, float deltaTime)
    {
        var desiredMovement = (newState.StandardMovement.Velocity.ToVector3() * deltaTime).y;
        var actualMovement = (newState.StandardMovement.Position.ToVector3() - previousState.StandardMovement.Position.ToVector3()).y;
        var delta = actualMovement - desiredMovement;

        newState.IsGrounded = (Mathf.Abs(delta / actualMovement) > DeltaThreshold && delta > 0);

        return true;
    }
}
