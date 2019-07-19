using Improbable.Gdk.Core;
using Improbable.Gdk.Subscriptions;
using Improbable.Gdk.Guns;
using Improbable.Gdk.Movement;
using Improbable.Gdk.StandardTypes;
using Improbable.Gdk.TransformSynchronization;
using UnityEngine;

namespace Fps
{
    [RequireComponent(typeof(FpsAnimator), typeof(ProxyMovementDriver))]
    public class ProxyAnimation : MonoBehaviour
    {
        [Require] private GunStateComponentReader gunState;
        [Require] private ServerMovementReader serverMovementReader;
        [Require] private ClientRotationReader clientRotationReader;

        [Tooltip(
            "If proxy players do not receive ServerMovement component updates for this many Unity updates, they will return to idle.")]
        [SerializeField]
        private float stopAnimatingAfterUpdates = 5.0f;

        private ProxyMovementDriver driver;
        private FpsAnimator fpsAnimator;

        private float movementTimeout;
        private bool isMoving;

        private void Awake()
        {
            driver = GetComponent<ProxyMovementDriver>();
            fpsAnimator = GetComponent<FpsAnimator>();
        }

        private void OnEnable()
        {
            gunState.OnIsAimingUpdate += SetAiming;
            serverMovementReader.OnMovementUpdate += MovementUpdate;
            clientRotationReader.OnRotationUpdate += RotationUpdate;

            SetAiming(gunState.Data.IsAiming);
            RotationUpdate(clientRotationReader.Data.Rotation);
        }

        private void Update()
        {
            fpsAnimator.SetGrounded(driver.IsGrounded);
            if (!isMoving)
            {
                return;
            }

            movementTimeout -= Time.deltaTime;
            if (movementTimeout > 0)
            {
                return;
            }

            fpsAnimator.StopMovement();
            isMoving = false;
        }

        private void SetAiming(bool isAiming)
        {
            fpsAnimator.SetAiming(isAiming);
        }

        private void RotationUpdate(RotationInfo rotationInfo)
        {
            var pitch = TransformUtils.ToUnityQuaternion(rotationInfo.Rotation).eulerAngles.x;

            if (pitch < -180)
            {
                pitch += 360;
            }

            if (pitch > 180)
            {
                pitch -= 360;
            }

            // AnimController treats negative pitch as pointing downwards
            fpsAnimator.SetPitch(-pitch);
        }

        private void MovementUpdate(MovementInfo movementInfo)
        {
            fpsAnimator.SetMovement(
                TransformUtils.ToUnityVector3(movementInfo.Position),
                movementInfo.TimeDelta);

            if (movementInfo.IncludesJump)
            {
                fpsAnimator.Jump();
            }

            movementTimeout = movementInfo.TimeDelta * stopAnimatingAfterUpdates;
            isMoving = true;
        }
    }
}
