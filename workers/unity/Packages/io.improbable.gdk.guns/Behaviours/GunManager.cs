using Improbable.Gdk.Subscriptions;
using UnityEngine;

namespace Improbable.Gdk.Guns
{
    public class GunManager : MonoBehaviour
    {
        [Require] private GunComponentReader gun;

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
