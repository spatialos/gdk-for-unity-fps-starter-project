using Improbable.Gdk.GameObjectRepresentation;
using Improbable.Gdk.StandardTypes;
using UnityEngine;

namespace Improbable.Gdk.Guns
{
    public class ClientShooting : MonoBehaviour, IRequiresGun
    {
        [Require] private ShootingComponent.Requirable.Writer shooting;

        [SerializeField] private LayerMask shootingLayerMask;

        [SerializeField] private float gunTriggerAllowance = 0.2f;
        private float nextShotTime;
        private GunSettings gunSettings;
        private Trigger shotTrigger;
        private SpatialOSComponent spatial;

        public bool IsOnCooldown => nextShotTime > Time.time;

        private void OnEnable()
        {
            spatial = GetComponent<SpatialOSComponent>();
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
            var entityId = 0L;

            if (Physics.Raycast(ray, out var hit, range, shootingLayerMask))
            {
                hitSomething = true;
                hitLocation = hit.point;
                var spatialEntity = hit.transform.root.GetComponent<SpatialOSComponent>();
                if (spatialEntity != null)
                {
                    entityId = spatialEntity.SpatialEntityId.Id;
                }
            }

            var shotInfo = new ShotInfo()
            {
                EntityId = entityId,
                HitSomething = hitSomething,
                HitLocation = (hitLocation - spatial.Worker.Origin).ToIntAbsolute(),
                HitOrigin = (ray.origin - spatial.Worker.Origin).ToIntAbsolute()
            };

            shooting.SendShots(shotInfo);
        }
    }
}
