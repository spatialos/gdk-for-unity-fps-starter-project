using System.Collections;
using Improbable.Common;
using Improbable.Gdk.Core;
using Improbable.Gdk.GameObjectRepresentation;
using Improbable.Gdk.Guns;
using Improbable.Gdk.Health;
using Improbable.Gdk.Movement;
using Improbable.Gdk.StandardTypes;
using UnityEngine;

namespace Fps
{
    public class FpsDriver : MonoBehaviour
    {
        [System.Serializable]
        private struct CameraSettings
        {
            public float PitchSpeed;
            public float YawSpeed;
            public float MinPitch;
            public float MaxPitch;
        }

        [Require] private ClientMovement.Requirable.Writer authority;
        [Require] private ServerMovement.Requirable.Reader serverMovement;
        [Require] private GunStateComponent.Requirable.Writer gunState;
        [Require] private HealthComponent.Requirable.Reader health;
        [Require] private HealthComponent.Requirable.CommandRequestSender commandSender;

        private ClientMovementDriver movement;
        private ClientShooting shooting;
        private ShotRayProvider shotRayProvider;
        private FpsAnimator fpsAnimator;
        private GunManager currentGun;
        private SpatialOSComponent spatialComponent;

        private readonly Vector3[] cachedDirectionVectors = new Vector3[16];
        [SerializeField] private Transform pitchTransform;
        [SerializeField] private new Camera camera;

        [SerializeField] private CameraSettings cameraSettings = new CameraSettings
        {
            PitchSpeed = 1.0f,
            YawSpeed = 1.0f,
            MinPitch = -80.0f,
            MaxPitch = 60.0f
        };

        private bool isRequestingRespawn;
        private Coroutine requestingRespawnCoroutine;

        private void Awake()
        {
            movement = GetComponent<ClientMovementDriver>();
            shooting = GetComponent<ClientShooting>();
            shotRayProvider = GetComponent<ShotRayProvider>();
            fpsAnimator = GetComponent<FpsAnimator>();
            currentGun = GetComponent<GunManager>();
            CreateDirectionCache();
        }

        private void OnEnable()
        {
            spatialComponent = GetComponent<SpatialOSComponent>();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            serverMovement.OnForcedRotation += OnForcedRotation;
            health.OnRespawn += OnRespawn;
        }

        private void Update()
        {
            // Don't allow controls if in the menu.
            if (ScreenUIController.InEscapeMenu)
            {
                // Still apply physics.
                movement.ApplyMovement(Vector3.zero, transform.rotation, MovementSpeed.Run, false);
                Animations(false);
                return;
            }

            if (isRequestingRespawn)
            {
                return;
            }

            if (health.Data.Health == 0)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    isRequestingRespawn = true;
                    requestingRespawnCoroutine = StartCoroutine(RequestRespawn());
                }

                return;
            }

            // Movement
            var forward = Input.GetKey(KeyCode.W);
            var backward = Input.GetKey(KeyCode.S);
            var left = Input.GetKey(KeyCode.A);
            var right = Input.GetKey(KeyCode.D);

            var toMove = transform.rotation * GetDirectionFromCache(forward, backward, left, right);
            var onlyForward = forward && !(backward || left || right);

            // Rotation
            var yawDelta = Input.GetAxis("Mouse X");
            var pitchDelta = Input.GetAxis("Mouse Y");

            // Modifiers
            var isAiming = Input.GetMouseButton(1);
            var isSprinting = Input.GetKey(KeyCode.LeftShift) && onlyForward;
            var isJumpPressed = Input.GetKeyDown(KeyCode.Space);

            // Events
            var shootPressed = Input.GetMouseButtonDown(0);
            var shootHeld = Input.GetMouseButton(0);

            // Update the pitch speed with that of the gun if aiming.
            var yawSpeed = cameraSettings.YawSpeed;
            var pitchSpeed = cameraSettings.PitchSpeed;
            if (isAiming)
            {
                yawSpeed = currentGun.CurrentGunSettings.AimYawSpeed;
                pitchSpeed = currentGun.CurrentGunSettings.AimPitchSpeed;
            }

            //Mediator
            var movementSpeed = isAiming
                ? MovementSpeed.Walk
                : isSprinting
                    ? MovementSpeed.Sprint
                    : MovementSpeed.Run;
            var yawChange = yawDelta * yawSpeed;
            var pitchChange = pitchDelta * -pitchSpeed;
            var currentPitch = pitchTransform.transform.localEulerAngles.x;
            var newPitch = currentPitch + pitchChange;
            if (newPitch > 180)
            {
                newPitch -= 360;
            }

            newPitch = Mathf.Clamp(newPitch, -cameraSettings.MaxPitch, -cameraSettings.MinPitch);
            pitchTransform.localRotation = Quaternion.Euler(newPitch, 0, 0);
            var currentYaw = transform.eulerAngles.y;
            var newYaw = currentYaw + yawChange;
            var rotation = Quaternion.Euler(newPitch, newYaw, 0);

            //Check for sprint cooldown
            if (!movement.HasSprintedRecently)
            {
                HandleShooting(shootPressed, shootHeld);
            }

            Aiming(isAiming);

            var wasGroundedBeforeMovement = movement.IsGrounded;
            movement.ApplyMovement(toMove, rotation, movementSpeed, isJumpPressed);
            Animations(isJumpPressed && wasGroundedBeforeMovement);
        }

        private IEnumerator RequestRespawn()
        {
            while (true)
            {
                commandSender?.SendRequestRespawnRequest(spatialComponent.SpatialEntityId, new Empty());
                yield return new WaitForSeconds(2);
            }
        }

        private void OnRespawn(Empty _)
        {
            StopCoroutine(requestingRespawnCoroutine);
            isRequestingRespawn = false;
        }

        private void HandleShooting(bool shootingPressed, bool shootingHeld)
        {
            if (shootingPressed)
            {
                shooting.BufferShot();
            }

            var isShooting = shooting.IsShooting(shootingHeld);
            if (isShooting)
            {
                FireShot(currentGun.CurrentGunSettings);
            }
        }

        private void FireShot(GunSettings gunSettings)
        {
            var ray = shotRayProvider.GetShotRay(gunState.Data.IsAiming, camera);
            shooting.FireShot(gunSettings.ShotRange, ray);
            shooting.InitiateCooldown(gunSettings.ShotCooldown);
        }

        private void Aiming(bool shouldBeAiming)
        {
            if (shouldBeAiming != gunState.Data.IsAiming)
            {
                var update = new GunStateComponent.Update
                {
                    IsAiming = new Option<BlittableBool>(shouldBeAiming)
                };
                gunState.Send(update);
            }
        }

        private void Animations(bool isJumping)
        {
            fpsAnimator.SetAiming(gunState.Data.IsAiming);
            fpsAnimator.SetGrounded(movement.IsGrounded);
            fpsAnimator.SetMovement(transform.position, Time.deltaTime);
            fpsAnimator.SetPitch(pitchTransform.transform.localEulerAngles.x);

            if (isJumping)
            {
                fpsAnimator.Jump();
            }
        }

        private void OnForcedRotation(RotationUpdate forcedRotation)
        {
            var newPitch = Mathf.Clamp(forcedRotation.Pitch.ToFloat1k(), -cameraSettings.MaxPitch, -cameraSettings.MinPitch);
            pitchTransform.localRotation = Quaternion.Euler(newPitch, 0, 0);
        }

        // Cache the direction vectors to avoid having to normalize every time.
        private void CreateDirectionCache()
        {
            for (var i = 0; i < cachedDirectionVectors.Length; i++)
            {
                cachedDirectionVectors[i] = Vector3.zero;
            }

            var forwardRight = new Vector3(1, 0, 1).normalized;
            var forwardLeft = new Vector3(-1, 0, 1).normalized;
            var backwardRight = new Vector3(1, 0, -1).normalized;
            var backwardLeft = new Vector3(-1, 0, -1).normalized;

            cachedDirectionVectors[1] = Vector3.forward;
            cachedDirectionVectors[2] = Vector3.back;
            cachedDirectionVectors[4] = Vector3.right;
            cachedDirectionVectors[5] = forwardRight;
            cachedDirectionVectors[6] = backwardRight;
            cachedDirectionVectors[8] = Vector3.left;
            cachedDirectionVectors[9] = forwardLeft;
            cachedDirectionVectors[10] = backwardLeft;
        }

        private Vector3 GetDirectionFromCache(
            bool forward,
            bool backward,
            bool left,
            bool right)
        {
            var directionIndex = forward & !backward ? 1 : 0;
            directionIndex += backward & !forward ? 2 : 0;
            directionIndex += right & !left ? 4 : 0;
            directionIndex += left & !right ? 8 : 0;
            return cachedDirectionVectors[directionIndex];
        }
    }
}
