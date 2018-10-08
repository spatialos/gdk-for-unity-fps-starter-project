using Improbable.Gdk.GameObjectRepresentation;
using UnityEngine;

namespace Improbable.Gdk.Guns
{
    public class GunManager : MonoBehaviour
    {
        [Require] private GunComponent.Requirable.Reader gun;

        public GunSettings CurrentGunSettings { get; private set; }

        private void OnEnable()
        {
            gun.GunIdUpdated += OnGunChanged;
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
