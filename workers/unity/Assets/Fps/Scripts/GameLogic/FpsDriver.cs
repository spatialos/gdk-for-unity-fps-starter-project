using System.Collections;
using System.Security.Authentication;
using Improbable.Common;
using Improbable.Fps.Custommovement;
using Improbable.Gdk.Core;
using Improbable.Gdk.GameObjectRepresentation;
using Improbable.Gdk.Guns;
using Improbable.Gdk.Health;
using Improbable.Gdk.Movement;
using Improbable.Gdk.StandardTypes;
using Improbable.Worker.CInterop;
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

        [Require] private GunStateComponent.Requirable.Writer gunState;
        [Require] private HealthComponent.Requirable.Reader health;
        [Require] private HealthComponent.Requirable.CommandRequestSender commandSender;

        [SerializeField] public GameObject ControllerProxy;

        private MyClientMovementDriver movement;
        private FpsMovement fpsMovement;
        private CharacterController controller;
        private ClientShooting shooting;
        private ShotRayProvider shotRayProvider;
        private FpsAnimator fpsAnimator;
        private GunManager currentGun;
        private SpatialOSComponent spatialComponent;

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

        private CustomState state;

        private void Awake()
        {
            movement = GetComponent<MyClientMovementDriver>();
            controller = ControllerProxy.GetComponent<CharacterController>();
            shooting = GetComponent<ClientShooting>();
            shotRayProvider = GetComponent<ShotRayProvider>();
            fpsAnimator = GetComponent<FpsAnimator>();
            currentGun = GetComponent<GunManager>();
        }

        private void Start()
        {
            ControllerProxy.transform.parent = null;
            ControllerProxy.name = $"{name} Controller Proxy";
            movement.Controller = ControllerProxy.GetComponent<CharacterController>();
        }

        private void OnEnable()
        {
            spatialComponent = GetComponent<SpatialOSComponent>();

            fpsMovement = new FpsMovement(controller, spatialComponent.Worker.Origin);
            movement.SetCustomProcessor(fpsMovement);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            health.OnRespawn += OnRespawn;
        }

        private void OnDestroy()
        {
            if (ControllerProxy != null)
            {
                Debug.Log($"Destroying Proxy for {name}");
                Destroy(ControllerProxy);
            }
        }

        private void Update()
        {
            // Don't allow controls if in the menu.
            if (ScreenUIController.InEscapeMenu)
            {
                Animations(new CustomState());
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

            // Debug
            if (Input.GetKeyDown(KeyCode.K))
            {
                MyMovementUtils.ShowDebug = !MyMovementUtils.ShowDebug;
            }

            // Movement
            var forward = Input.GetKey(KeyCode.W);
            var backward = Input.GetKey(KeyCode.S);
            var left = Input.GetKey(KeyCode.A);
            var right = Input.GetKey(KeyCode.D);
            var onlyForward = forward && !(backward || left || right);

            // Rotation
            var yawDelta = Input.GetAxis("Mouse X");
            var pitchDelta = Input.GetAxis("Mouse Y");

            // Modifiers
            var isAiming = Input.GetMouseButton(1);
            var isSprinting = Input.GetKey(KeyCode.LeftShift) && onlyForward;
            var isJumpPressed = Input.GetKey(KeyCode.Space);

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

            // Mediator
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
            var rotation = transform.eulerAngles;
            rotation.y += yawChange;
            transform.rotation = Quaternion.Euler(rotation);

            fpsMovement.AddInput(
                forward,
                backward,
                left,
                right,
                isJumpPressed,
                isSprinting,
                isAiming,
                rotation.y,
                newPitch);

            state = MyMovementUtils.GetLatestState(movement, fpsMovement);

            // Check for sprint cooldown
            if (isAiming || (state.SprintCooldown <= 0 && !isSprinting))
            {
                HandleShooting(shootPressed, shootHeld);
            }

            Aiming(isAiming);

            Animations(state);

            transform.position = ControllerProxy.transform.position;
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

        private void Animations(CustomState state)
        {
            fpsAnimator.SetAiming(gunState.Data.IsAiming);
            fpsAnimator.SetGrounded(state.IsGrounded);
            fpsAnimator.SetMovement(state.StandardMovement.Velocity.ToVector3());
            fpsAnimator.SetPitch(pitchTransform.transform.localEulerAngles.x);

            if (state.DidJump)
            {
                fpsAnimator.Jump();
            }
        }
    }
}
