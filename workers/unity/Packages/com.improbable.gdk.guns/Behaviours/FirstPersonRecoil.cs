using UnityEngine;

namespace Improbable.Gdk.Guns
{
    public class FirstPersonRecoil : MonoBehaviour, IRecoil
    {
        [SerializeField] private UnityEngine.Transform socketOffset;
        private Vector3 socketInitialOffset;

        private Vector3 aimOffset;
        private Vector3 hipOffset;
        private Vector3 aimVelocity;
        private Vector3 hipVelocity;

        private GunSettings.FirstPersonRecoilSettings hipRecoil;
        private GunSettings.FirstPersonRecoilSettings aimRecoil;

        private bool isAiming;

        private void Awake()
        {
            socketInitialOffset = socketOffset.localPosition;
        }

        public void Recoil(bool aiming)
        {
            isAiming = aiming;
            aimVelocity = CalculateNewVelocity(aimRecoil);
            hipVelocity = CalculateNewVelocity(hipRecoil);
        }

        private static Vector3 CalculateNewVelocity(GunSettings.FirstPersonRecoilSettings recoil)
        {
            var xOffset = Random.Range(recoil.MinXVariance, recoil.MaxXVariance);
            var yOffset = Random.Range(recoil.MinYVariance, recoil.MaxYVariance);
            var zOffset = Random.Range(recoil.MinZVariance, recoil.MaxZVariance);
            return new Vector3(xOffset, yOffset, zOffset);
        }

        private void LateUpdate()
        {
            ApplyVelocityAndDampening(aimRecoil, ref aimOffset, ref aimVelocity);
            ApplyVelocityAndDampening(hipRecoil, ref hipOffset, ref hipVelocity);
            socketOffset.localPosition = socketInitialOffset + (isAiming ? aimOffset : hipOffset);
        }

        private void ApplyVelocityAndDampening(GunSettings.FirstPersonRecoilSettings recoil, ref Vector3 offset,
            ref Vector3 velocity)
        {
            offset += velocity * Time.deltaTime;
            offset = Vector3.Lerp(offset, Vector2.zero, Time.deltaTime * recoil.OffsetDamper);
            velocity = Vector3.Lerp(velocity, Vector2.zero, Time.deltaTime * recoil.VelocityDamper);
        }

        public void SetRecoilSettings(GunSettings.RecoilSettings hipRecoilSettings, GunSettings.RecoilSettings aimRecoilSettings)
        {
            hipRecoil = hipRecoilSettings.FirstPersonRecoil;
            aimRecoil = aimRecoilSettings.FirstPersonRecoil;
        }
    }
}
