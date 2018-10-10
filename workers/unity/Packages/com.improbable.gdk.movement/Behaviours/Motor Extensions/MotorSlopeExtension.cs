using UnityEngine;

namespace Improbable.Gdk.Movement
{
    [RequireComponent(typeof(CharacterController), typeof(CharacterControllerMotor))]
    public class MotorSlopeExtension : MonoBehaviour, IMotorExtension
    {
        [Range(0, 1)] [SerializeField] private float slideFriction;

        private CharacterController characterController;
        private CharacterControllerMotor motor;
        private Vector3 hitNormal;
        private bool isGrounded;
        private bool overrideInAir;

        private const float ErrorTolerance = 0.1f;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            motor = GetComponent<CharacterControllerMotor>();
        }

        // Record the hits of the CharacterController
        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            hitNormal = hit.normal;
            var angleToGround = Vector3.Angle(Vector3.up, hitNormal);

            // Ignore sliding etc. if colliding with walls.
            if (angleToGround >= 90 - ErrorTolerance)
            {
                overrideInAir = false;
                return;
            }

            // Consider as in-air if beyond the slope limit.
            isGrounded = angleToGround <= characterController.slopeLimit + ErrorTolerance;
            if (isGrounded == overrideInAir)
            {
                overrideInAir = !isGrounded;
            }
        }

        void IMotorExtension.BeforeMove(Vector3 toMove)
        {
            //Character sliding off surfaces. - push away from the surface before moving down.
            // Scale by toMove.y, such that you slide faster down as you pick up fall speed.
            if (!isGrounded && toMove.y < 0)
            {
                var toMoveSpeed = -toMove.y;
                var pushDistance = (1f - hitNormal.y) * (1f - slideFriction) * toMoveSpeed;
                characterController.Move(new Vector3(
                    hitNormal.x * pushDistance,
                    0,
                    hitNormal.z * pushDistance
                ));
            }
        }

        bool IMotorExtension.IsOverrideAir()
        {
            return overrideInAir;
        }
    }
}
