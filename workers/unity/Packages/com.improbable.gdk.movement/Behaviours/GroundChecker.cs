using UnityEngine;

namespace Improbable.Gdk.Movement
{
    [RequireComponent(typeof(CharacterController))]
    public class GroundChecker : MonoBehaviour
    {
        [SerializeField] private float nearToGroundDistance = 2f;

        [SerializeField] private LayerMask walkableLayer = ~0;

        private CharacterController characterController;
        private bool grounded;

        public bool OverrideInAir { get; set; }

        public bool Grounded => grounded || characterController.isGrounded;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
        }

        // Update is called once per frame
        private void Update()
        {
            var upOffset = characterController.radius + 0.1f;
            var ray = new Ray(transform.position + Vector3.up * upOffset, Vector3.down);
            var hit = Physics.SphereCast(ray, characterController.radius, out var hitInfo,
                nearToGroundDistance + upOffset, walkableLayer);
            grounded = hit && hitInfo.distance < upOffset + 0.1f;
        }
    }
}
