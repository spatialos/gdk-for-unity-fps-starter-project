using Improbable.Gdk.Movement;
using UnityEngine;

namespace Fps
{
    public class FpsAnimator : MonoBehaviour
    {
        [System.Serializable]
        private struct AnimationSettings
        {
            [Tooltip("The speed at which the animator changes pitch.")]
            public float InterpolatePitchSpeed;

            [Tooltip("The speed at which the animator changes movement speed.")]
            public float InterpolateMovementSpeed;

            public float JumpCrossFadeDuration;
        }

        private class AnimationFloatParameter
        {
            public string Name;
            public float CurrentValue;
            public float TargetValue;
        }

        private class AnimationBoolParameter
        {
            public string Name;
            public bool LastValue;
        }

        [SerializeField] private AnimationSettings animationSettings = new AnimationSettings
        {
            InterpolatePitchSpeed = 100.0f,
            InterpolateMovementSpeed = 10.0f,
            JumpCrossFadeDuration = 0.1f
        };

        [SerializeField] private Animator[] animators;

        private Vector3 latestPosition;

        // For the edge case of moving slowly down a wall (flickers between 0 and a very small amount)
        private const float MinClampThreshold = 0.3f, MaxClampThreshold = 0.5f;
        private const int RequiredFrames = 10;
        private int framesInThresholdFor;

        private readonly AnimationBoolParameter aimingParameter = new AnimationBoolParameter { Name = "Aiming" };
        private readonly AnimationBoolParameter groundedParameter = new AnimationBoolParameter { Name = "Grounded" };
        private readonly AnimationBoolParameter sprintingParameter = new AnimationBoolParameter { Name = "Sprinting" };

        private const string JumpParameter = "Jump";
        private readonly AnimationFloatParameter movementParameter = new AnimationFloatParameter { Name = "Movement" };
        private readonly AnimationFloatParameter pitchParameter = new AnimationFloatParameter { Name = "Pitch" };
        private readonly AnimationFloatParameter xParameter = new AnimationFloatParameter { Name = "X" };
        private readonly AnimationFloatParameter zParameter = new AnimationFloatParameter { Name = "Z" };

        private void Awake()
        {
            latestPosition = transform.position;
        }

        public void SetAiming(bool aiming)
        {
            SetBoolParameter(aimingParameter, aiming);
        }

        public void Jump()
        {
            foreach (var animator in animators)
            {
                animator.CrossFadeInFixedTime(JumpParameter, animationSettings.JumpCrossFadeDuration);
            }
        }

        public void SetGrounded(bool grounded)
        {
            SetBoolParameter(groundedParameter, grounded);
        }

        public void SetPitch(float pitch)
        {
            if (pitch > 180)
            {
                pitch -= 360;
            }

            pitchParameter.TargetValue = pitch;
        }

        public void StopMovement()
        {
            SetXAndZ(Vector2.zero);
            SetMoving(0);
        }

        public void SetMovement(Vector3 newPosition, float time)
        {
            var movementDirection = newPosition - latestPosition;
            movementDirection.y = 0;
            latestPosition = newPosition;

            movementDirection /= time;

            var diffAngle = CalculateAngle(transform, movementDirection);
            var angleRadian = Mathf.Deg2Rad * diffAngle;

            var directionMagnitude = movementDirection.magnitude;

            var magnitudeToUse = 0f;
            if (directionMagnitude > MovementSpeedSettings.SharedSettings.RunSpeed)
            {
                directionMagnitude -= MovementSpeedSettings.SharedSettings.RunSpeed;
                magnitudeToUse = 2 + directionMagnitude / (MovementSpeedSettings.SharedSettings.SprintSpeed -
                    MovementSpeedSettings.SharedSettings.RunSpeed);
            }
            else if (directionMagnitude > MovementSpeedSettings.SharedSettings.WalkSpeed)
            {
                directionMagnitude -= MovementSpeedSettings.SharedSettings.WalkSpeed;
                magnitudeToUse = 1 + directionMagnitude / (MovementSpeedSettings.SharedSettings.RunSpeed -
                    MovementSpeedSettings.SharedSettings.WalkSpeed);
            }
            else
            {
                magnitudeToUse = directionMagnitude / MovementSpeedSettings.SharedSettings.WalkSpeed;
            }

            var delta = Vector2.zero;
            if (magnitudeToUse != 0)
            {
                delta = new Vector2(Mathf.Sin(angleRadian), Mathf.Cos(angleRadian)) * magnitudeToUse;
            }

            SetXAndZ(delta);

            ThresholdMoving(ref magnitudeToUse);
            SetMoving(magnitudeToUse);
        }

        private void SetMoving(float moving)
        {
            movementParameter.TargetValue = moving;
            SetBoolParameter(sprintingParameter, moving >= 2.4f);
        }

        private void ThresholdMoving(ref float moving)
        {
            // If above the threshold, ignore this.
            if (moving > MaxClampThreshold)
            {
                return;
            }

            // If below the threshold, reset the counter.
            if (moving < MinClampThreshold)
            {
                if (framesInThresholdFor > 0)
                {
                    framesInThresholdFor = 0;
                }

                moving = 0;
                return;
            }

            // Otherwise, clamp to 0 if not within the threshold for the required number of frames.
            if (framesInThresholdFor < RequiredFrames)
            {
                framesInThresholdFor++;
                moving = 0;
            }
        }

        private void SetXAndZ(Vector2 position)
        {
            xParameter.TargetValue = position.x;
            zParameter.TargetValue = position.y;
        }

        private static float CalculateAngle(Transform from, Vector3 direction)
        {
            var targetAngle = Vector3.Angle(from.forward, direction);
            targetAngle *= Mathf.Sign(Vector3.Dot(from.right, direction));
            return targetAngle;
        }

        private void SetBoolParameter(AnimationBoolParameter parameter, bool newValue)
        {
            if (parameter.LastValue != newValue)
            {
                parameter.LastValue = newValue;
                foreach (var animator in animators)
                {
                    animator.SetBool(parameter.Name, newValue);
                }
            }
        }

        private void Update()
        {
            LerpFloatParameter(pitchParameter, animationSettings.InterpolatePitchSpeed);
            LerpFloatParameter(xParameter, animationSettings.InterpolateMovementSpeed);
            LerpFloatParameter(zParameter, animationSettings.InterpolateMovementSpeed);
            LerpFloatParameter(movementParameter, animationSettings.InterpolateMovementSpeed);
        }

        private void LerpFloatParameter(AnimationFloatParameter parameter, float interpolateSpeed)
        {
            if (parameter.CurrentValue == parameter.TargetValue)
            {
                return;
            }

            parameter.CurrentValue = Mathf.MoveTowards(parameter.CurrentValue, parameter.TargetValue,
                interpolateSpeed * Time.deltaTime);
            foreach (var animator in animators)
            {
                animator.SetFloat(parameter.Name, parameter.CurrentValue);
            }
        }
    }
}
