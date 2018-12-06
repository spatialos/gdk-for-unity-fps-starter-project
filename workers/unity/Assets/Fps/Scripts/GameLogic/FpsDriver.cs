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

        [Require] private GunStateComponent.Requirable.Writer gunState;
        [Require] private HealthComponent.Requirable.Reader health;
        [Require] private HealthComponent.Requirable.CommandRequestSender commandSender;

        [SerializeField] public GameObject ControllerProxy;

        private MyClientMovementDriver movement;
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
        private readonly JumpMovement jumpMovement = new JumpMovement();
        private readonly MyMovementUtils.SprintCooldown sprintCooldown = new MyMovementUtils.SprintCooldown();
        private readonly MyMovementUtils.RestoreStateProcessor restoreState = new MyMovementUtils.RestoreStateProcessor();
        private readonly MyMovementUtils.RemoveWorkerOrigin removeOrigin = new MyMovementUtils.RemoveWorkerOrigin();
        private CommandFrameSystem commandFrame;

        private Vector3 from;
        private Vector3 to;
        private Vector3 next;
        private float t;
        private bool nextAvailable = false;

        private void Awake()
        {
            movement = GetComponent<MyClientMovementDriver>();
            movement.SetMovementProcessors(new MyMovementUtils.IMovementProcessor[]
            {
                restoreState,
                new StandardMovement(),
                sprintCooldown,
                jumpMovement,
                new MyMovementUtils.Gravity(),
                new MyMovementUtils.TerminalVelocity(),
                new MyMovementUtils.ApplyMovementProcessor(),
                removeOrigin,
                new MyMovementUtils.AdjustVelocity(),
            });
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
            commandFrame = spatialComponent.World.GetExistingManager<CommandFrameSystem>();

            restoreState.Origin = spatialComponent.Worker.Origin;
            removeOrigin.Origin = spatialComponent.Worker.Origin;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            health.OnRespawn += OnRespawn;
        }

        private void Update()
        {
            // Don't allow controls if in the menu.
            if (ScreenUIController.InEscapeMenu)
            {
                Animations(new MovementState());
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

            // Manually fudge command frame length.
            if (Input.GetKeyDown(KeyCode.I))
            {
                commandFrame.ManualFudge -= 0.05f;
            }

            if (Input.GetKeyDown(KeyCode.O))
            {
                commandFrame.ManualFudge += 0.05f;
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

            movement.AddInput(
                forward, backward, left, right,
                isJumpPressed, isSprinting, isAiming,
                rotation.y, newPitch);

            var state = movement.GetLatestState();

            //Check for sprint cooldown
            if (sprintCooldown.GetCooldown(state) <= 0 && !isSprinting)
            {
                HandleShooting(shootPressed, shootHeld);
            }

            Aiming(isAiming);

            Animations(state);

            t += Time.deltaTime / CommandFrameSystem.FrameLength;
            if (t > 1.0f)
            {
                if (nextAvailable)
                {
                    t -= 1.0f;
                    from = to;
                    to = next;
                    nextAvailable = false;
                }
                else
                {
                    // go back by a frame?
                    // Debug.LogFormat("Next not ready, go back a frame.");
                    t -= Time.deltaTime / CommandFrameSystem.FrameLength;
                }
            }

            var oldPosition = transform.position;
            transform.position = Vector3.Lerp(from, to, t);
            fpsVelocity = (transform.position - oldPosition).magnitude / Time.deltaTime;
        }

        private float fpsVelocity = -1;

        private void LateUpdate()
        {
            if (commandFrame.NewFrame)
            {
                // Debug.LogFormat("Frame {0}, t:{1:00.00}, nextAvailable:{2}",
                //     commandFrame.CurrentFrame, t, nextAvailable);

                Debug.DrawLine(ControllerProxy.transform.position, ControllerProxy.transform.position + Vector3.up * 6f, Color.green, 2);
                var pos = movement.GetLatestState().Position.ToVector3();
                Debug.DrawLine(pos, pos + Vector3.up * 5f, Color.red, 2);

                next = ControllerProxy.transform.position;
                nextAvailable = true;

                if (t + Time.deltaTime / CommandFrameSystem.FrameLength < 1.0f)
                {
                    // Debug.LogFormat("More than a frame behind, fast forward a tiny bit");
                    t += 0.001f;
                }

                // from = to;
                // to = ControllerProxy.transform.position;
            }
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

        private void Animations(MovementState state)
        {
            fpsAnimator.SetAiming(gunState.Data.IsAiming);
            fpsAnimator.SetGrounded(MyMovementUtils.IsGrounded(movement.Controller));
            fpsAnimator.SetMovement(state.Velocity.ToVector3());
            fpsAnimator.SetPitch(pitchTransform.transform.localEulerAngles.x);

            if (state.DidJump)
            {
                fpsAnimator.Jump();
            }
        }

        private void OnGUI()
        {
            if (!MyMovementUtils.ShowDebug)
            {
                return;
            }

            GUI.Label(new Rect(10, 400, 700, 20), string.Format("fps vel: {0:00.00}", fpsVelocity));
        }
    }
}
