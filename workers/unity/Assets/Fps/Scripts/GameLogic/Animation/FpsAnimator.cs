using Improbable.Fps.Custommovement;
using Improbable.Gdk.Movement;
using JetBrains.Annotations;
using UnityEngine;

namespace Fps
{
    public class FpsAnimator : MonoBehaviour
    {
        [System.Serializable]
        private struct AnimationSettings
        {
            [Tooltip("The speed at which the animator changes pitch.")]
            public float InterpolatePitchDuration;

            [Tooltip("The speed at which the animator changes movement speed.")]
            public float InterpolateMovementDuration;

            public float JumpCrossFadeDuration;
        }

        private class AnimationFloatParameter
        {
            public string Name;
            public float OldValue;
            public float CurrentValue;
            public float LerpTimeElapsed;
            private float targetValue;

            public float TargetValue
            {
                get => targetValue;
                set
                {
                    OldValue = CurrentValue;
                    targetValue = value;
                    LerpTimeElapsed = 0;
                }
            }
        }

        private class AnimationBoolParameter
        {
            public string Name;
            public bool LastValue;
        }

        [SerializeField] private AnimationSettings animationSettings = new AnimationSettings
        {
            InterpolatePitchDuration = 0.1f,
            InterpolateMovementDuration = 0.1f,
            JumpCrossFadeDuration = 0.1f
        };

        [SerializeField] private Animator[] animators;

        // For the edge case of moving slowly down a wall (flickers between 0 and a very small amount)
        private const float MinClampThreshold = 0.3f, MaxClampThreshold = 0.5f;
        private const int RequiredFrames = 10;
        private int framesInThresholdFor;

        private readonly AnimationBoolParameter aimingParameter = new AnimationBoolParameter { Name = "Aiming" };
        private readonly AnimationBoolParameter groundedParameter = new AnimationBoolParameter { Name = "Grounded" };
        private readonly AnimationBoolParameter sprintingParameter = new AnimationBoolParameter { Name = "Sprinting" };

        private const string JumpParameter = "Jump";
        private readonly AnimationFloatParameter normalizedSpeedParamter = new AnimationFloatParameter { Name = "NormalizedSpeed" };
        private readonly AnimationFloatParameter pitchParameter = new AnimationFloatParameter { Name = "Pitch" };

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
            SetVelocity(Vector3.zero);
        }

        public void SetVelocity(Vector3 velocity)
        {
            velocity.y = 0;
            var speed = velocity.magnitude;
            normalizedSpeedParamter.TargetValue = Mathf.Clamp(speed / FpsMovement.GetSpeed(false, false), 0, 1);
        }

        public void SetSprinting(bool sprinting)
        {
            SetBoolParameter(sprintingParameter, sprinting);
        }

        private void SetBoolParameter(AnimationBoolParameter parameter, bool newValue)
        {
            if (parameter.LastValue == newValue)
            {
                return;
            }

            parameter.LastValue = newValue;
            foreach (var animator in animators)
            {
                animator.SetBool(parameter.Name, newValue);
            }
        }

        private void Update()
        {
            LerpFloatParameter(pitchParameter, animationSettings.InterpolatePitchDuration);
            LerpFloatParameter(normalizedSpeedParamter, animationSettings.InterpolateMovementDuration);
        }

        private void LerpFloatParameter(AnimationFloatParameter parameter, float lerpTime)
        {
            if (parameter.CurrentValue == parameter.TargetValue)
            {
                return;
            }

            // If lerpTime has passed (e.g. if lerpTime is 0), snap to the target value.
            if (parameter.LerpTimeElapsed >= lerpTime)
            {
                parameter.CurrentValue = parameter.TargetValue;

                foreach (var animator in animators)
                {
                    animator.SetFloat(parameter.Name, parameter.TargetValue);
                }

                return;
            }

            parameter.LerpTimeElapsed = Mathf.Clamp(parameter.LerpTimeElapsed + Time.deltaTime, 0, lerpTime);

            parameter.CurrentValue = Mathf.Lerp(parameter.OldValue, parameter.TargetValue,
                parameter.LerpTimeElapsed / lerpTime);

            foreach (var animator in animators)
            {
                animator.SetFloat(parameter.Name, parameter.CurrentValue);
            }
        }
    }
}
