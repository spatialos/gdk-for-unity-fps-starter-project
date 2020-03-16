using System;
using UnityEngine;

namespace Fps.UI
{
    [RequireComponent(typeof(UnityEngine.Animation))]
    public class TouchscreenButtonAnimator : MonoBehaviour
    {
        // START These properties are manually drawn in StandardButtonAnimatorInspector

        public AnimationClip IdleAnimation;
        public AnimationClip OnDownAnimation;
        public AnimationClip PressedAnimation;
        public AnimationClip OnUpAnimation;

        public float IdleAnimationTimeScale = 1;
        public float OnDownAnimationTimeScale = 1;
        public float PressedAnimationTimeScale = 1;
        public float OnUpAnimationTimeScale = 1;

        // END
        // Add new properties after here (unless if manually drawing them)

        private UnityEngine.Animation anim;

        public enum AnimType
        {
            Idle,
            OnDown,
            Pressed,
            OnUp
        }

        private void Awake()
        {
            anim = GetComponent<UnityEngine.Animation>();
        }

        private void SetAnimationSpeed(AnimationClip clip, float speed)
        {
            if (!clip)
            {
                return;
            }

            anim[clip.name].speed = speed;
        }

        public void PlayAnimation(AnimType animType)
        {
            var clip = GetClip(animType);
            var speed = GetSpeed(animType);

            if (!clip)
            {
                return;
            }

            if (anim.isPlaying)
            {
                anim.Stop();
            }

            SetClipToStart(animType);
            SetAnimationSpeed(clip, speed);
            anim.Play(clip.name);
        }

        public void QueueAnimation(AnimType animType)
        {
            var clip = GetClip(animType);
            var speed = GetSpeed(animType);

            if (!clip)
            {
                return;
            }

            SetClipToStart(animType);
            SetAnimationSpeed(clip, speed);
            anim.PlayQueued(clip.name);
        }

        private AnimationClip GetClip(AnimType animType)
        {
            switch (animType)
            {
                case AnimType.Idle:
                    return IdleAnimation;
                case AnimType.OnDown:
                    return OnDownAnimation;
                case AnimType.Pressed:
                    return PressedAnimation;
                case AnimType.OnUp:
                    return OnUpAnimation;
                default:
                    throw new ArgumentOutOfRangeException(nameof(animType), animType, null);
            }
        }

        private float GetSpeed(AnimType animType)
        {
            switch (animType)
            {
                case AnimType.Idle:
                    return IdleAnimationTimeScale;
                case AnimType.OnDown:
                    return OnDownAnimationTimeScale;
                case AnimType.Pressed:
                    return PressedAnimationTimeScale;
                case AnimType.OnUp:
                    return OnUpAnimationTimeScale;
                default:
                    throw new ArgumentOutOfRangeException(nameof(animType), animType, null);
            }
        }

        public void SetClipToStart(AnimType animType)
        {
            var clip = GetClip(animType);
            if (!clip)
            {
                return;
            }

            anim[clip.name].normalizedTime = anim[clip.name].speed >= 0f ? 0f : 1f;
        }

        public void SetClipToEnd(AnimType animType)
        {
            var clip = GetClip(animType);
            if (!clip)
            {
                return;
            }

            anim[clip.name].normalizedTime = anim[clip.name].speed >= 0f ? 1f : 0f;
        }
    }
}
