using Improbable.Gdk.Movement;
using UnityEngine;

public class MyMovementUtils
{
    public const float Gravity = 10f;
    public const float Jump = 5f;
    public const float FrameLength = 1 / 30f;
    public const float NormalSpeed = 3f;
    public const float SprintSpeed = 6f;

    public const float AirControlMultiplier = 0.01f;

    public static Vector3 ApplyMovement(CharacterController controller, ClientRequest input, Vector3 velocity)
    {
        var newVelocity = velocity / FrameLength;
        var speed = input.IncludesSprint ? SprintSpeed : NormalSpeed;
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

        if (IsGrounded(controller))
        {
            newVelocity.z = inputVector.z;
            newVelocity.x = inputVector.x;

            if (input.IncludesJump)
            {
                newVelocity.y = Jump;
            }
        }
        else
        {
            newVelocity.z += (AirControlMultiplier * inputVector.z);
            newVelocity.x += (AirControlMultiplier * inputVector.x);
        }

        newVelocity.y = newVelocity.y - Gravity * FrameLength;

        newVelocity *= FrameLength;

        controller.Move(newVelocity);

        return newVelocity;
    }

    public static bool IsGrounded(CharacterController controller)
    {
        var start = controller.transform.position + controller.center;
        var maxFloorDistance = controller.center.y + 0.3f;

        if (Physics.SphereCast(new Ray(start, Vector3.down), 0.45f, out var hit, maxFloorDistance, Physics.AllLayers))
        {
            Debug.DrawLine(controller.transform.position, hit.point, Color.green);
            return true;
        }

        Debug.DrawLine(controller.transform.position,
            controller.transform.position + Vector3.down * maxFloorDistance, Color.red);

        return false;
    }
}
