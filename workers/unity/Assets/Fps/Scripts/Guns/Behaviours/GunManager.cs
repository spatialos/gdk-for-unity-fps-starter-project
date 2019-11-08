using Fps.Guns;
using Improbable.Gdk.Subscriptions;
using UnityEngine;

namespace Fps.Guns
{
    public class GunManager : MonoBehaviour
    {
#pragma warning disable 649
        [Require] private GunComponentReader gun;
#pragma warning restore 649

        public GunSettings CurrentGunSettings { get; private set; }

        private void OnEnable()
        {
            gun.OnGunIdUpdate += OnGunChanged;
            OnGunChanged(gun.Data.GunId);
        }

        private void OnGunChanged(int gunId)
        {
            CurrentGunSettings = GunDictionary.Get(gunId);
            SetGunSettings();
        }

        private void SetGunSettings()
        {
            foreach (var gunUser in GetComponentsInChildren<IRequiresGun>())
            {
                gunUser.InformOfGun(CurrentGunSettings);
            }
        }
    }
}
