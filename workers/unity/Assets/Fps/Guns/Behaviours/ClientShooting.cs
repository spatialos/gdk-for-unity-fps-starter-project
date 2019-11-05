using Improbable.Gdk.Core;
using Improbable.Gdk.Subscriptions;
using UnityEngine;

namespace Fps.Guns
{
    public class ClientShooting : MonoBehaviour, IRequiresGun
    {
#pragma warning disable 649
        [Require] private ShootingComponentWriter shooting;
#pragma warning restore 649

        [SerializeField] private LayerMask shootingLayerMask;

        [SerializeField] private float gunTriggerAllowance = 0.2f;
        private float nextShotTime;
        private GunSettings gunSettings;
        private Trigger shotTrigger;
        private Vector3 workerOrigin;

        private bool IsOnCooldown => nextShotTime > Time.time;

        private void OnEnable()
        {
            var linkedEntityComponent = GetComponent<LinkedEntityComponent>();
            workerOrigin = linkedEntityComponent.Worker.Origin;
        }

        private void Start()
        {
            shotTrigger = new Trigger(gunTriggerAllowance);
        }

        public void InitiateCooldown(float cooldown)
        {
            nextShotTime = Time.time + cooldown;
        }

        public void BufferShot()
        {
            shotTrigger.Fire();
        }

        // Returns true if the player can shoot.
        // (If shooting is held for an automatic gun, or if a shot has been buffered for a non-automatic.)
        // (And the gun is not cooling down.)
        public bool IsShooting(bool shootingHeld)
        {
            if (!IsOnCooldown && gunSettings != null)
            {
                if (gunSettings.IsAutomatic && shootingHeld)
                {
                    return true;
                }

                if (!gunSettings.IsAutomatic && shotTrigger.Peek())
                {
                    shotTrigger.Consume();
                    return true;
                }
            }

            return false;
        }

        public void InformOfGun(GunSettings settings)
        {
            gunSettings = settings;
        }

        public void FireShot(float range, Ray ray)
        {
            var hitLocation = ray.origin + ray.direction * range;
            var hitSomething = false;
            EntityId entityId = new EntityId(0);

            if (Physics.Raycast(ray, out var hit, range, shootingLayerMask))
            {
                hitSomething = true;
                hitLocation = hit.point;
                var spatialEntity = hit.transform.root.GetComponent<LinkedEntityComponent>();
                if (spatialEntity != null)
                {
                    entityId = spatialEntity.EntityId;
                }
            }

            var shotInfo = new ShotInfo()
            {
                EntityId = entityId,
                HitSomething = hitSomething,
                HitLocation = (hitLocation - workerOrigin).ToVector3Int(),
                HitOrigin = (ray.origin - workerOrigin).ToVector3Int(),
            };

            shooting.SendShotsEvent(shotInfo);
        }
    }
}
