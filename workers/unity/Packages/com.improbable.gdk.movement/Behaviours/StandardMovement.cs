using Improbable.Gdk.Movement;
using UnityEngine;

public class StandardMovement : MyMovementUtils.IMovementProcessor
{
    public Vector3 GetMovement(CharacterController controller, ClientRequest input, int frame, Vector3 velocity,
        Vector3 previous)
    {
        var newVelocity = velocity;
        var speed = input.IncludesSprint
            ? MyMovementUtils.movementSettings.MovementSpeed.SprintSpeed
            : MyMovementUtils.movementSettings.MovementSpeed.RunSpeed;
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

            controlledMovement = Vector2.ClampMagnitude(controlledMovement,
                MyMovementUtils.movementSettings.MovementSpeed.SprintSpeed);

            newVelocity.x = controlledMovement.x;
            newVelocity.z = controlledMovement.y;
        }

        return newVelocity;
    }

    public void Clean(int frame)
    {
        // Do nothing, no state stored.
    }
}
