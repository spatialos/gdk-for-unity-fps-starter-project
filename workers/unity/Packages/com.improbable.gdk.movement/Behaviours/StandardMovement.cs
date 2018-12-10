using Improbable.Gdk.Movement;
using Improbable.Gdk.StandardTypes;
using UnityEngine;

public class StandardMovement : MyMovementUtils.IMovementProcessor
{
    public bool Process(CharacterController controller, ClientRequest input, MovementState previousState,
        ref MovementState newState, float deltaTime)
    {
        if (newState.DidTeleport)
        {
            return true;
        }

        var newVelocity = previousState.Velocity.ToVector3();
        var speed = MyMovementUtils.movementSettings.MovementSpeed.RunSpeed;
        if (input.AimPressed)
        {
            speed = MyMovementUtils.movementSettings.MovementSpeed.WalkSpeed;
        }
        else if (input.SprintPressed)
        {
            speed = MyMovementUtils.movementSettings.MovementSpeed.SprintSpeed;
        }

        var inputVector = new Vector3(
            input.RightPressed ? 1 : (input.LeftPressed) ? -1 : 0,
            0,
            input.ForwardPressed ? 1 : (input.BackPressed ? -1 : 0));

        inputVector.Normalize();

        var rot = Quaternion.Euler(0, input.CameraYaw / 100000f, 0);

        // rotate to face yaw in input.
        inputVector = rot * inputVector;

        // multiply by speed determined by sprint.
        inputVector *= speed;

        if (MyMovementUtils.IsGrounded(controller))
        {
            newVelocity.z = inputVector.z;
            newVelocity.x = inputVector.x;
        }
        else
        {
            // Apply movement with air control damping.
            var controlledMovement = new Vector2(newVelocity.x, newVelocity.z);
            var input2D = new Vector2(inputVector.x, inputVector.z);

            controlledMovement = controlledMovement * (1 - MyMovementUtils.movementSettings.InAirDamping)
                + input2D * MyMovementUtils.movementSettings.AirControlModifier;

            controlledMovement = Vector2.ClampMagnitude(controlledMovement, speed);

            newVelocity.x = controlledMovement.x;
            newVelocity.z = controlledMovement.y;
        }

        newState.Velocity = newVelocity.ToIntAbsolute();

        return true;
    }
}
