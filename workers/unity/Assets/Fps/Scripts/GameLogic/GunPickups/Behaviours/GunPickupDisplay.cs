using Fps.Schema.Shooting;
using Improbable.Gdk.Core;
using Improbable.Gdk.GameObjectRepresentation;
using Improbable.Gdk.Guns;
using UnityEngine;

namespace Fps.GunPickups
{
    public class GunPickupDisplay : MonoBehaviour
    {
        [Require] private GunPickupComponent.Requirable.Reader pickupComponent;

        [SerializeField] private Transform gunSocket;
        private GameObject gun;
        private Collider pickupCollider;

        private void OnEnable()
        {
            pickupCollider = GetComponent<Collider>();
            pickupComponent.GunIdUpdated += SetGunId;
            pickupComponent.IsEnabledUpdated += SetEnabled;
            SetGunId(pickupComponent.Data.GunId);
            SetEnabled(pickupComponent.Data.IsEnabled);
        }

        public void SetGunId(int id)
        {
            var gunSettings = GunDictionary.Get(id);
            gun = Instantiate(gunSettings.GunModel, gunSocket);

            // Turn off any collision on the instantiated gun.
            var collider = gun.GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = false;
            }
        }

        public void SetEnabled(BlittableBool enabled)
        {
            gun.SetActive(enabled);
            pickupCollider.enabled = enabled;
        }
    }
}
