using Fps.Guns;
using Improbable.Gdk.Guns;
using UnityEngine;

namespace Fps.GunPickups
{
    public class GunPickupDisplay : MonoBehaviour
    {
        [SerializeField] Transform gunSocket;
        private GameObject gun;
        private Collider pickupCollider;

        private void Awake()
        {
            pickupCollider = GetComponent<Collider>();
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

        public void SetEnabled(bool enabled)
        {
            gun.SetActive(enabled);
            pickupCollider.enabled = enabled;
        }
    }
}
