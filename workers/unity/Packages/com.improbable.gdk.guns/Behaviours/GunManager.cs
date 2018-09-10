using Improbable.Gdk.GameObjectRepresentation;
using UnityEngine;

namespace Improbable.Gdk.Guns
{
    public class GunManager : MonoBehaviour
    {
        [Require] private GunComponent.Requirable.Reader gun;

        public GunSettings CurrentGunSettings { get; private set; }
        public int GunSlots => gun != null ? gun.Data.GunSlots.Count : 0;

        private void OnEnable()
        {
            gun.CurrentSlotUpdated += OnGunChanged;
            OnGunChanged(gun.Data.CurrentSlot);
        }

        private void OnGunChanged(int gunId)
        {
            CurrentGunSettings = GunDictionary.GetCurrentGun(gun.Data);
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
