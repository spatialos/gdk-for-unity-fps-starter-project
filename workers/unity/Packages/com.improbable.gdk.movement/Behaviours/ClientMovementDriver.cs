using Improbable.Gdk.Core;
using Improbable.Gdk.Subscriptions;
using Improbable.Gdk.StandardTypes;
using UnityEngine;

namespace Improbable.Gdk.Movement
{
    public class ClientMovementDriver : GroundCheckingDriver
    {
        [Require] private ClientMovementWriter client;
        [Require] private ClientRotationWriter rotation;
        [Require] private ServerMovementReader server;

        [SerializeField] private float transformUpdateHz = 15.0f;
        [SerializeField] private float rotationUpdateHz = 15.0f;
        [SerializeField] [HideInInspector] private float transformUpdateDelta;
        [SerializeField] [HideInInspector] private float rotationUpdateDelta;

        [SerializeField] private MovementSettings movementSettings = new MovementSettings
        {
            MovementSpeed = new MovementSpeedSettings
            {
                WalkSpeed = 2.5f,
                RunSpeed = 4.0f,
                SprintSpeed = 6.0f
            },
            SprintCooldown = 0.1f,
            Gravity = 9.81f,
            StartingJumpSpeed = 10.0f,
            TerminalVelocity = 100.0f,
            GroundedFallSpeed = 1.0f,
            AirControlModifier = 0.0f,
            InAirDamping = 1.0f
        };

        [SerializeField] private RotationConstraints rotationConstraints = new RotationConstraints
        {
            XAxisRotation = true,
            YAxisRotation = true,
            ZAxisRotation = true
        };

        private const float FloatErrorTolerance = 0.01f;

        private Vector3 origin;

        private float verticalVelocity;
        private Vector3 lastDirection;
        private bool jumpedThisFrame;
        private bool didJump;
        private bool lastMovementStationary;

        private float cumulativeRotationTimeDelta;
        private float currentPitch;
        private float currentYaw;
        private float currentRoll;
        private bool yawDirty;
        private bool rollDirty;
        private bool pitchDirty;
        private float sprintCooldownExpires;

        public float CurrentYaw
        {
            set
            {
                if (value != currentYaw)
                {
                    currentYaw = value;
                    yawDirty = true;
                }
            }
            get => currentYaw;
        }

        public float CurrentPitch
        {
            set
            {
                if (value != currentPitch)
                {
                    currentPitch = value;
                    pitchDirty = true;
                }
            }
            get => currentPitch;
        }

        public float CurrentRoll
        {
            set
            {
                if (value != currentRoll)
                {
                    currentRoll = value;
                    rollDirty = true;
                }
            }
            get => currentRoll;
        }

        public bool HasSprintedRecently => Time.time < sprintCooldownExpires;

        private LinkedEntityComponent LinkedEntityComponent;

        // Cache the update delta values.
        private void OnValidate()
        {
            if (transformUpdateHz > 0.0f)
            {
                transformUpdateDelta = 1.0f / transformUpdateHz;
            }
            else
            {
                transformUpdateDelta = 1.0f;
                Debug.LogError("The Transform Update Hz must be greater than 0.");
            }

            if (rotationUpdateHz > 0.0f)
            {
                rotationUpdateDelta = 1.0f / rotationUpdateHz;
            }
            else
            {
                rotationUpdateDelta = 1.0f;
                Debug.LogError("The Rotation Update Hz must be greater than 0.");
            }
        }

        protected override void Awake()
        {
            base.Awake();
            // There will only be one client movement driver, but there will always be one.
            // Therefore it should be safe to set shared movement settings here.
            MovementSpeedSettings.SharedSettings = movementSettings.MovementSpeed;
        }

        private void OnEnable()
        {
            LinkedEntityComponent = GetComponent<LinkedEntityComponent>();
            origin = LinkedEntityComponent.Worker.Origin;
            server.OnLatestUpdate += OnServerUpdate;
            server.OnForcedRotationEvent += OnForcedRotation;
        }

        private void OnForcedRotation(RotationUpdate forcedRotation)
        {
            var rotationUpdate = new RotationUpdate
            {
                Pitch = forcedRotation.Pitch,
                Roll = forcedRotation.Roll,
                Yaw = forcedRotation.Yaw,
                TimeDelta = forcedRotation.TimeDelta
            };
            var update = new ClientRotation.Update { Latest = new Option<RotationUpdate>(rotationUpdate) };
            rotation.SendUpdate(update);

            cumulativeRotationTimeDelta = 0;
            pitchDirty = rollDirty = yawDirty = false;

            //Apply the forced rotation
            var x = rotationConstraints.XAxisRotation ? forcedRotation.Pitch.ToFloat1k() : 0;
            var y = rotationConstraints.YAxisRotation ? forcedRotation.Yaw.ToFloat1k() : 0;
            var z = rotationConstraints.ZAxisRotation ? forcedRotation.Roll.ToFloat1k() : 0;
            transform.rotation = Quaternion.Euler(x, y, z);
        }

        private void OnServerUpdate(ServerResponse update)
        {
            Reconcile(update.Position.ToVector3() + origin, update.Timestamp);
        }

        public void ApplyMovement(Vector3 movement, Quaternion rotation, MovementSpeed movementSpeed, bool startJump)
        {
            ProcessInput(movement, rotation, movementSpeed, startJump);
            SendPositionUpdate();
            SendRotationUpdate();
        }

        public void ProcessInput(Vector3 movement, Quaternion rotation, MovementSpeed movementSpeed, bool startJump)
        {
            var isJumping = verticalVelocity > 0;

            verticalVelocity = Mathf.Clamp(verticalVelocity - movementSettings.Gravity * Time.deltaTime,
                -movementSettings.TerminalVelocity, verticalVelocity);

            if (IsGrounded && verticalVelocity < -movementSettings.GroundedFallSpeed)
            {
                verticalVelocity = -movementSettings.GroundedFallSpeed;
            }

            var inAir = isJumping || !IsGrounded;
            var isSprinting = movementSpeed == MovementSpeed.Sprint && movement.sqrMagnitude > 0 && IsGrounded;
            if (isSprinting)
            {
                sprintCooldownExpires = Time.time + movementSettings.SprintCooldown;
            }

            // Grounded motion
            // Strafe in the direction given.
            var speed = GetSpeed(movementSpeed);
            var toMove = movement * speed;

            // Aerial motion
            if (inAir)
            {
                // Keep your last direction, with some damping.
                var momentumMovement = Vector3.Lerp(lastDirection, Vector3.zero,
                    Time.deltaTime * movementSettings.InAirDamping);

                // Update the last direction (reduced by the air damping)
                lastDirection = momentumMovement;

                // Can only accelerate up to the movement speed.
                var maxAirSpeed = Mathf.Max(momentumMovement.magnitude, movementSettings.MovementSpeed.RunSpeed);

                var aerialMovement = Vector3.ClampMagnitude(
                    momentumMovement + toMove,
                    maxAirSpeed
                );
                // Lerp between just momentum, and the momentum with full movement
                toMove = Vector3.Lerp(momentumMovement, aerialMovement, movementSettings.AirControlModifier);
            }


            // Jumping
            if (jumpedThisFrame)
            {
                jumpedThisFrame = false;
            }

            if (startJump && IsGrounded && !isJumping)
            {
                verticalVelocity = movementSettings.StartingJumpSpeed;
                didJump = true;
                jumpedThisFrame = true;
            }

            // Record the (horizontal) direction when last on the ground (for jumping or falling off platforms)
            if (IsGrounded)
            {
                lastDirection = toMove;
            }

            toMove += Vector3.up * verticalVelocity;

            // Inform the motor.
            var oldPosition = transform.position;
            MoveFrame(toMove * Time.deltaTime);

            // Stop vertical velocity (upwards) if blocked
            if (verticalVelocity > 0 &&
                transform.position.y - oldPosition.y < toMove.y * Time.deltaTime - FloatErrorTolerance)
            {
                verticalVelocity = 0;
            }

            CheckExtensionsForOverrides();

            //Rotation
            var x = rotationConstraints.XAxisRotation ? rotation.eulerAngles.x : 0;
            var y = rotationConstraints.YAxisRotation ? rotation.eulerAngles.y : 0;
            var z = rotationConstraints.ZAxisRotation ? rotation.eulerAngles.z : 0;
            transform.rotation = Quaternion.Euler(x, y, z);
            CurrentPitch = rotation.eulerAngles.x;
            CurrentYaw = rotation.eulerAngles.y;
            CurrentRoll = rotation.eulerAngles.z;
        }

        public float GetSpeed(MovementSpeed requestedSpeed)
        {
            switch (requestedSpeed)
            {
                case MovementSpeed.Sprint:
                    return movementSettings.MovementSpeed.SprintSpeed;
                case MovementSpeed.Run:
                    return movementSettings.MovementSpeed.RunSpeed;
                case MovementSpeed.Walk:
                    return movementSettings.MovementSpeed.WalkSpeed;
                default:
                    return 0;
            }
        }

        // Returns true if an update is sent.
        private bool SendPositionUpdate()
        {
            //Send network data if required (If moved, or was still moving last update)
            var anyUpdate = false;
            if (HasEnoughMovement(transformUpdateDelta,
                out var movement,
                out var timeDelta,
                out var anyMovement,
                out var messageStamp))
            {
                Reset();
                if (anyMovement || !lastMovementStationary)
                {
                    var clientRequest = new ClientRequest
                    {
                        IncludesJump = didJump,
                        Movement = movement.ToIntDelta(),
                        TimeDelta = timeDelta,
                        Timestamp = messageStamp
                    };
                    var update = new ClientMovement.Update { Latest = new Option<ClientRequest>(clientRequest) };
                    client.SendUpdate(update);
                    lastMovementStationary = !anyMovement;
                    didJump = false;
                    anyUpdate = true;
                }
            }

            return anyUpdate;
        }

        // Returns true if an update needs to be sent.
        private bool SendRotationUpdate()
        {
            //Send network data if required (If moved, or was still moving last update)
            var anyUpdate = false;

            if (cumulativeRotationTimeDelta > rotationUpdateDelta)
            {
                if (pitchDirty || rollDirty || yawDirty)
                {
                    var rotationUpdate = new RotationUpdate
                    {
                        Pitch = currentPitch.ToInt1k(),
                        Roll = currentRoll.ToInt1k(),
                        Yaw = currentYaw.ToInt1k(),
                        TimeDelta = cumulativeRotationTimeDelta
                    };
                    var update = new ClientRotation.Update { Latest = new Option<RotationUpdate>(rotationUpdate) };
                    rotation.SendUpdate(update);
                    anyUpdate = true;
                }

                cumulativeRotationTimeDelta = 0;
                pitchDirty = rollDirty = yawDirty = false;
            }
            else
            {
                cumulativeRotationTimeDelta += Time.deltaTime;
            }

            return anyUpdate;
        }
    }
}
