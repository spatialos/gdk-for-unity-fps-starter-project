using UnityEngine;

namespace Fps.Movement
{
    public class GroundCheckingDriver : CharacterControllerMotor
    {
        // Ground checking
        public bool IsGrounded { get; private set; }

        [Tooltip("Uses an overlap sphere of this radius from the character's feet to check for collision.")]
        [SerializeField]
        private float groundedRadius = 0.05f;

        [SerializeField] private LayerMask groundLayerMask = ~0;
        private readonly Collider[] groundedOverlapSphereArray = new Collider[1];

        private void CheckGrounded()
        {
            IsGrounded = Physics.OverlapSphereNonAlloc(transform.position, groundedRadius, groundedOverlapSphereArray,
                groundLayerMask) > 0;
        }

        protected override void Move(Vector3 toMove)
        {
            base.Move(toMove);
            CheckGrounded();
        }

        protected void CheckExtensionsForOverrides()
        {
            if (MotorExtension.IsOverrideAir())
            {
                IsGrounded = false;
            }
        }
    }
}
