using Improbable.Gdk.Core;
using Improbable.Gdk.GameObjectRepresentation;
using Improbable.Gdk.Guns;
using Improbable.Gdk.Movement;
using Improbable.Gdk.StandardTypes;
using UnityEngine;

namespace Fps
{
    [RequireComponent(typeof(FpsAnimator), typeof(GroundChecker))]
    public class ProxyAnimation : MonoBehaviour
    {
        [Require] private GunStateComponent.Requirable.Reader gunState;
        [Require] private ServerMovement.Requirable.Reader serverMovement;
        [Require] private ClientRotation.Requirable.Reader clientRotation;

        [Tooltip(
            "If proxy players do not receive ServerMovement component updates for this many Unity updates, they will return to idle.")]
        [SerializeField]
        private float stopAnimatingAfterUpdates = 5.0f;

        private GroundChecker groundChecker;
        private FpsAnimator fpsAnimator;

        private float movementTimeout;
        private bool isMoving;

        private void Awake()
        {
            groundChecker = GetComponent<GroundChecker>();
            fpsAnimator = GetComponent<FpsAnimator>();
        }

        private void OnEnable()
        {
            gunState.IsAimingUpdated += OnAiming;
            serverMovement.LatestUpdated += OnMovement;
            clientRotation.LatestUpdated += OnRotation;

            OnAiming(gunState.Data.IsAiming);
            OnRotation(clientRotation.Data.Latest);
        }

        private void Update()
        {
            fpsAnimator.SetGrounded(groundChecker.Grounded);
            fpsAnimator.SetNearGround(groundChecker.NearGround);
            if (isMoving)
            {
                movementTimeout -= Time.deltaTime;
                if (movementTimeout <= 0)
                {
                    fpsAnimator.StopMovement();
                    isMoving = false;
                }
            }
        }

        private void OnAiming(BlittableBool isAiming)
        {
            fpsAnimator.SetAiming(isAiming);
        }

        private void OnRotation(RotationUpdate rotation)
        {
            fpsAnimator.SetPitch(rotation.Pitch.ToFloat1k());
        }

        private void OnMovement(ServerResponse movement)
        {
            fpsAnimator.SetMovement(movement.Position.ToVector3(), movement.TimeDelta);
            if (movement.IncludesJump)
            {
                fpsAnimator.Jump();
            }

            movementTimeout = movement.TimeDelta * stopAnimatingAfterUpdates;
            isMoving = true;
        }
    }
}
