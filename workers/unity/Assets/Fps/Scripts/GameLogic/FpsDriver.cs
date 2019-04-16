using System.Collections;
using Improbable.Common;
using Improbable.Gdk.Core;
using Improbable.Gdk.Subscriptions;
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

        [Require] private ClientMovementWriter authority;
        [Require] private ServerMovementReader serverMovement;
        [Require] private GunStateComponentWriter gunState;
        [Require] private HealthComponentReader health;
        [Require] private HealthComponentCommandSender commandSender;
        [Require] private EntityId entityId;

        private ClientMovementDriver movement;
        private ClientShooting shooting;
        private ShotRayProvider shotRayProvider;
        private FpsAnimator fpsAnimator;
        private GunManager currentGun;

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

        private IControlProvider controller;
        private InGameScreenManager inGameManager;

        private void Awake()
        {
            movement = GetComponent<ClientMovementDriver>();
            shooting = GetComponent<ClientShooting>();
            shotRayProvider = GetComponent<ShotRayProvider>();
            fpsAnimator = GetComponent<FpsAnimator>();
            currentGun = GetComponent<GunManager>();
            controller = GetComponent<IControlProvider>();

            inGameManager = GameObject.FindGameObjectWithTag("OnScreenUI")?.GetComponentInChildren<InGameScreenManager>(true);
        }

        private void OnEnable()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            serverMovement.OnForcedRotationEvent += OnForcedRotation;
            health.OnRespawnEvent += OnRespawn;
        }

        private void Update()
        {
            if (controller.MenuPressed)
            {
                inGameManager.TryOpenSettingsMenu();
            }

            // Don't allow controls if in the menu.
            if (InGameScreenManager.InEscapeMenu)
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
                if (controller.RespawnPressed)
                {
                    isRequestingRespawn = true;
                    requestingRespawnCoroutine = StartCoroutine(RequestRespawn());
                }

                return;
            }

            // Movement
            var toMove = transform.rotation * controller.Movement;

            // Rotation
            var yawDelta = controller.YawDelta;
            var pitchDelta = controller.PitchDelta;

            // Modifiers
            var isAiming = controller.IsAiming;
            var isSprinting = controller.AreSprinting;

            var isJumpPressed = controller.JumpPressed;

            // Events
            var shootPressed = controller.ShootPressed;
            var shootHeld = controller.ShootHeld;


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
                commandSender?.SendRequestRespawnCommand(entityId, new Empty());
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
                gunState.SendUpdate(update);
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
            var newPitch = Mathf.Clamp(forcedRotation.Pitch.ToFloat1k(), -cameraSettings.MaxPitch,
                -cameraSettings.MinPitch);
            pitchTransform.localRotation = Quaternion.Euler(newPitch, 0, 0);
        }
    }
}
