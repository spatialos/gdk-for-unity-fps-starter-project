using Improbable.Common;
using Improbable.Gdk.GameObjectRepresentation;
using Improbable.Gdk.Health;
using Improbable.Gdk.Ragdoll;
using UnityEngine;

namespace Fps
{
    [RequireComponent(typeof(RagdollSpawner))]
    public class LookAtRagdoll : MonoBehaviour
    {
        [Require] private HealthComponent.Requirable.Reader health;
        private RagdollSpawner ragdollSpawner;
        private PoolableRagdoll ragdoll;

        [SerializeField] private Camera playerCamera;

        [SerializeField] private float cameraHeightAboveRagdoll = 2f;
        [SerializeField] private float lookAtRagdollMoveSpeed = 5;
        [SerializeField] private float lookAtRagdollRotateSpeed = 50;

        private void Awake()
        {
            ragdollSpawner = GetComponent<RagdollSpawner>();
        }

        private void OnEnable()
        {
            ragdollSpawner.OnRagdollSpawned += SetRagdoll;
            health.OnRespawn += OnRespawn;
        }

        private void OnDisable()
        {
            ragdollSpawner.OnRagdollSpawned -= SetRagdoll;
            health.OnRespawn -= OnRespawn;
        }

        private void SetRagdoll(GameObject ragdollObject)
        {
            ragdoll = ragdollObject.GetComponent<PoolableRagdoll>();
            ragdoll.OnCleanup += StopLooking;
        }

        private void OnRespawn(Empty empty)
        {
            StopLooking();
            playerCamera.transform.localPosition = Vector3.zero;
            playerCamera.transform.localRotation = Quaternion.identity;
        }

        private void StopLooking()
        {
            if (ragdoll == null)
            {
                return;
            }

            ragdoll = null;
        }

        private void Update()
        {
            if (ragdoll == null)
            {
                return;
            }

            // If the ragdoll is returned to the pool, stop tracking it.
            if (!ragdoll.isActiveAndEnabled)
            {
                StopLooking();
                return;
            }

            var focusPosition = ragdoll.CentreBone != null
                ? ragdoll.CentreBone.transform.position
                : ragdoll.transform.position;

            TurnToFace(focusPosition);
        }

        private void TurnToFace(Vector3 position)
        {
            var cameraTransform = playerCamera.transform;

            // Glide to above the ragdoll
            var cameraTargetPosition = position + Vector3.up * cameraHeightAboveRagdoll;
            var toPosition = cameraTargetPosition - cameraTransform.position;

            var moveDistance = lookAtRagdollMoveSpeed * Time.deltaTime;
            if (toPosition.magnitude <= moveDistance)
            {
                cameraTransform.position = cameraTargetPosition;
            }
            else
            {
                cameraTransform.position += toPosition.normalized * moveDistance;
            }

            // Turn to look down at the ragdoll
            var targetRotation = Quaternion.LookRotation(position - cameraTransform.position, Vector3.up);
            var rotationDelta = lookAtRagdollRotateSpeed * Time.deltaTime;

            var newEulerAngles = new Vector3(
                Mathf.MoveTowardsAngle(cameraTransform.eulerAngles.x, targetRotation.eulerAngles.x, rotationDelta),
                Mathf.MoveTowardsAngle(cameraTransform.eulerAngles.y, targetRotation.eulerAngles.y, rotationDelta),
                0
            );

            // Don't rotate the yaw when directly above the ragdoll.
            if (Vector3.Dot((position - cameraTransform.position).normalized, Vector3.down) > 0.9f)
            {
                newEulerAngles.y = cameraTransform.eulerAngles.y;
            }

            cameraTransform.eulerAngles = newEulerAngles;
        }
    }
}
