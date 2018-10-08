using UnityEngine;

namespace Improbable.Gdk.Movement
{
    public enum MovementSpeed
    {
        Walk,
        Run,
        Sprint
    }

    [System.Serializable]
    public struct MovementSpeedSettings
    {
        public static MovementSpeedSettings SharedSettings = new MovementSpeedSettings();

        public float WalkSpeed;
        public float RunSpeed;
        public float SprintSpeed;
    }

    [System.Serializable]
    public struct MovementSettings
    {
        public MovementSpeedSettings MovementSpeed;
        public float SprintCooldown;
        public float Gravity;
        public float StartingJumpSpeed;
        public float TerminalVelocity;

        [Tooltip("There should always be some downwards speed when grounded, to handle going down ramps.")]
        [SerializeField]
        public float GroundedFallSpeed;

        [Tooltip(
            "If 1, movement in the air is the same as when grounded (but no sprinting). If 0, there is no air control.")]
        [Range(0, 1)]
        [SerializeField]
        public float AirControlModifier;

        [Tooltip("When in the air, the player's movement/momentum is decelerated/damped using this value.")]
        [SerializeField]
        public float InAirDamping;
    }

    [System.Serializable]
    public struct RotationConstraints
    {
        [Tooltip("Implement rotation on the X-axis.")] [SerializeField]
        public bool XAxisRotation;

        [Tooltip("Implement rotation on the Y-axis.")] [SerializeField]
        public bool YAxisRotation;

        [Tooltip("Implement rotation on the Z-axis.")] [SerializeField]
        public bool ZAxisRotation;
    }
}
