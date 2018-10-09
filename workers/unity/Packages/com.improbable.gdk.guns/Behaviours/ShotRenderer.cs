using System.Collections;
using System.Collections.Generic;
using Improbable.Gdk.GameObjectRepresentation;
using Improbable.Gdk.ObjectPooling;
using Improbable.Gdk.StandardTypes;
using UnityEngine;

namespace Improbable.Gdk.Guns
{
    [RequireComponent(typeof(GunSocket))]
    public class ShotRenderer : MonoBehaviour, IRequiresGun
    {
        // Details about the bullet when it is in flight.
        private struct BulletDetails
        {
            public float Start;
            public float End;
        }

        [Require] private ShootingComponent.Requirable.Reader shooting;
        [Require] private GunStateComponent.Requirable.Reader gunState;

        [Tooltip(
            "Over this distance, the bullet's actual position (in a straight line to the target) is lerped with the direction directly out from the gun (to smooth out firing shots from a moving barrel).")]
        [SerializeField]
        private float barrelLerpThreshold = 50f;

        [Tooltip(
            "The bullet's length upon exiting the gun will be half as long as when past this threshold. (It takes this much distance to grow to full length.")]
        [SerializeField]
        private float fullLengthShotThreshold = 50f;

        [Tooltip("If false, the BarrelLerpThreshold will not be used.")] [SerializeField]
        private bool barrelSnap = true;

        private bool muzzleFlashNeedsRefresh;
        private GameObject muzzleFlashObject;
        private ParticleSystem[] muzzleEffects;


        private ObjectPool<PoolableParticleEffect> hitPool;
        private ObjectPool<PoolableLineRenderer> bulletPool;

        private readonly Dictionary<PoolableLineRenderer, BulletDetails> activeBulletDetails =
            new Dictionary<PoolableLineRenderer, BulletDetails>();

        private IRecoil[] recoilComponents;

        private GunSettings gunSettings;
        private GunSocket gunSocket;

        private UnityEngine.Transform GunBarrel => gunSocket.Gun.Barrel != null
            ? gunSocket.Gun.Barrel
            : gunSocket.Gun.transform;

        private void Awake()
        {
            recoilComponents = GetComponentsInChildren<IRecoil>();
            gunSocket = GetComponent<GunSocket>();
        }

        private void OnEnable()
        {
            shooting.OnShots += ShotFired;
        }

        private void OnDisable()
        {
            foreach (var bullet in activeBulletDetails.Keys)
            {
                bullet.ReturnToPool();
            }

            activeBulletDetails.Clear();
        }

        private void ShotFired(ShotInfo shotInfo)
        {
            var isAiming = gunState.Data.IsAiming;
            var hitLocation = shotInfo.HitLocation.ToVector3();
            var hitSomething = shotInfo.HitSomething;

            PlayRecoil(isAiming);
            VisualGunShot(hitLocation, hitSomething);
        }

        void IRequiresGun.InformOfGun(GunSettings settings)
        {
            gunSettings = settings;

            hitPool = ObjectPooler.GetOrCreateObjectPool<PoolableParticleEffect>(gunSettings.ImpactEffect, 10);
            bulletPool = ObjectPooler.GetOrCreateObjectPool<PoolableLineRenderer>(gunSettings.BulletLineRenderer, 10);

            foreach (var component in recoilComponents)
            {
                component.SetRecoilSettings(gunSettings.HipRecoil, gunSettings.AimRecoil);
            }

            // The gun has changed, so reset the cached muzzleflash. (May not be ready yet, so get later)
            muzzleFlashNeedsRefresh = true;
        }

        private void PlayRecoil(bool aiming)
        {
            foreach (var component in recoilComponents)
            {
                component.Recoil(aiming);
            }
        }

        private void VisualImpact(Vector3 position)
        {
            if (hitPool == null)
            {
                return;
            }

            var hitEffect = hitPool.Get();
            if (hitEffect == null)
            {
                return;
            }

            hitEffect.transform.position = position;
            hitEffect.transform.rotation = Quaternion.Inverse(gunSocket.Gun.Barrel.rotation);
            if (hitEffect.Particles != null)
            {
                var mainEmitter = hitEffect.Particles.main;
                mainEmitter.startColor = gunSettings.ShotColour;
            }
        }

        public void VisualGunShot(Vector3 hitPoint, bool hit)
        {
            PlayMuzzleFlash();
            VisualBullet(hitPoint, hit);
        }

        private void PlayMuzzleFlash()
        {
            if (muzzleFlashNeedsRefresh)
            {
                RefreshMuzzleFlash();
            }

            if (muzzleEffects != null)
            {
                foreach (var effect in muzzleEffects)
                {
                    effect.Play();
                }
            }
        }

        private void RefreshMuzzleFlash()
        {
            if (muzzleFlashObject != null)
            {
                Destroy(muzzleFlashObject);
            }

            if (gunSettings.MuzzleFlashEffect != null)
            {
                muzzleFlashObject = Instantiate(gunSettings.MuzzleFlashEffect, GunBarrel);
                muzzleEffects = muzzleFlashObject.GetComponentsInChildren<ParticleSystem>();
                foreach (var muzzleFlash in muzzleEffects)
                {
                    muzzleFlash.Stop();
                    var main = muzzleFlash.main;
                    main.startColor = gunSettings.ShotColour;
                }
            }

            muzzleFlashNeedsRefresh = false;
        }

        private void VisualBullet(Vector3 hitPoint, bool hit)
        {
            if (bulletPool == null)
            {
                return;
            }

            var bullet = bulletPool.Get();
            bullet.Renderer.material.color = gunSettings.ShotColour;
            activeBulletDetails.Add(bullet, new BulletDetails());

            StartCoroutine(Shot(bullet, hitPoint - GunBarrel.position, hit));
        }

        private void LateUpdate()
        {
            if (!barrelSnap)
            {
                return;
            }

            // If using Barrel Snap, lerp between the actual bullet's trajectory, and the trajectory if coming from the barrel's current location.
            // Lerp based on the distance travelled by the bullet.
            foreach (var entry in activeBulletDetails)
            {
                var lineRenderer = entry.Key.Renderer;
                var bulletDetails = entry.Value;
                if (bulletDetails.Start < barrelLerpThreshold)
                {
                    var startValue = bulletDetails.Start / barrelLerpThreshold;
                    startValue = Mathf.Clamp(startValue, 0, 1);
                    lineRenderer.SetPosition(0,
                        Vector3.Lerp(GunBarrel.position + GunBarrel.forward * bulletDetails.Start,
                            lineRenderer.GetPosition(0), startValue));
                    lineRenderer.SetPosition(1,
                        Vector3.Lerp(GunBarrel.position + GunBarrel.forward * bulletDetails.End,
                            lineRenderer.GetPosition(1), startValue));
                }
            }
        }

        private IEnumerator Shot(PoolableLineRenderer renderer, Vector3 target, bool requiresImpactVfx)
        {
            var maxLength = gunSettings.ShotRange;
            var bulletLength = gunSettings.BulletRenderLength;
            var renderTime = gunSettings.ShotRenderTime;

            var distance = Mathf.Min(target.magnitude, maxLength);

            var speed = maxLength / renderTime;
            var fullRenderTime = distance / speed;
            var elapsedTime = 0f;
            renderer.enabled = true;

            var direction = target.normalized;
            var startingPosition = GunBarrel.position;
            while (elapsedTime < renderTime * 2)
            {
                var percentage = elapsedTime / fullRenderTime;

                // Calculate the position along the length.
                var position = percentage * distance;
                var shotLength = position > fullLengthShotThreshold
                    ? bulletLength
                    : Mathf.Lerp(bulletLength / 2, bulletLength, position / fullLengthShotThreshold);

                // Start from negative, to see the bullet exit the gun, rather than spawning already fully released.
                var startPos = position - shotLength;
                var endPos = position;

                var start = Mathf.Clamp(startPos, 0, distance);
                var end = Mathf.Clamp(endPos, 0, distance);

                if (requiresImpactVfx && end == distance)
                {
                    VisualImpact(startingPosition + direction * end);
                    requiresImpactVfx = false;
                }

                // Update the line's position.
                renderer.Renderer.SetPosition(0, startingPosition + direction * start);
                renderer.Renderer.SetPosition(1, startingPosition + direction * end);

                // Inform the dictionary of the bullet's current position.
                activeBulletDetails[renderer] = new BulletDetails
                {
                    Start = start,
                    End = end
                };

                yield return new WaitForEndOfFrame();
                elapsedTime += Time.deltaTime;
            }

            // Clean up the bullet once it has finished.
            activeBulletDetails.Remove(renderer);
            renderer.ReturnToPool();
        }
    }
}
