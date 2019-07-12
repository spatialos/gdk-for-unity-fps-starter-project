using UnityEngine;

namespace Improbable.Gdk.Guns
{
    [RequireComponent(typeof(GunSocket))]
    public class ShotRayProvider : MonoBehaviour, IRequiresGun
    {
        private static readonly Vector3 CameraCentre = new Vector3(0.5f, 0.5f, 0);
        private GunSettings gunSettings;
        private GunSocket gunSocket;

        private void Awake()
        {
            gunSocket = GetComponent<GunSocket>();
        }

        public Ray GetShotRay(bool aiming, Camera camera)
        {
            var viewportAimingPosition = CameraCentre;

            var gunScope = gunSocket.Gun.Scope;
            if (gunScope != null && aiming)
            {
                viewportAimingPosition = camera.WorldToViewportPoint(gunScope.transform.position);
            }

            var ray = camera.ViewportPointToRay(viewportAimingPosition);

            var inaccuracyAt50m = aiming
                ? gunSettings.InaccuracyWhileAiming
                : gunSettings.InaccuracyFromHip;
            var inaccuracyOffset = Random.insideUnitSphere * inaccuracyAt50m;

            var shotPositionAt50m = ray.GetPoint(50) + inaccuracyOffset;
            var direction = shotPositionAt50m - ray.origin;

            return new Ray(ray.origin, direction);
        }

        public void InformOfGun(GunSettings settings)
        {
            gunSettings = settings;
        }
    }
}
