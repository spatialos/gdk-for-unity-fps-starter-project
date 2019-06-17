using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace Improbable.Gdk.ObjectPooling
{
    [RequireComponent(typeof(ParticleSystem))]
    public class PoolableParticleEffect : BasePoolableObject
    {
        private ParticleSystem particleEffect;

        public ParticleSystem Particles
        {
            get
            {
                if (particleEffect == null)
                {
                    particleEffect = GetComponentInChildren<ParticleSystem>();
                }

                return particleEffect;
            }
        }

        [Tooltip("Time before the object is returned to the pool. If <= 0, will use the particle system's duration.")]
        [SerializeField]
        private float lifetime = -1;

        private Coroutine effectFinished;

        private void OnDisable()
        {
            if (effectFinished != null)
            {
                StopCoroutine(effectFinished);
            }
        }

        private void Begin()
        {
            Particles.Play();
            var main = Particles.main;
            var duration = lifetime >= 0 ? lifetime : main.duration;

            effectFinished = StartCoroutine(Cleanup(duration));
        }

        private void End()
        {
            Particles.Stop();
        }

        private IEnumerator Cleanup(float timeToWait)
        {
            yield return new WaitForSeconds(timeToWait);
            ReturnToPool();
        }

        public override void OnPoolEnable(Action returnToPoolMethod)
        {
            base.OnPoolEnable(returnToPoolMethod);
            Begin();
        }

        public override void OnPoolDisable()
        {
            End();
            base.OnPoolDisable();
        }
    }
}
