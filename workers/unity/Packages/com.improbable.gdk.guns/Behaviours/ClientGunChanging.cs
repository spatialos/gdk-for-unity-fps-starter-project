using Improbable.Gdk.GameObjectRepresentation;
using UnityEngine;

namespace Improbable.Gdk.Guns
{
    public class ClientGunChanging : MonoBehaviour
    {
        [Require] private GunComponent.Requirable.CommandRequestSender gunRequestSender;
        [Require] private GunComponent.Requirable.Reader gunComponent;
        private SpatialOSComponent spatialOSComponent;

        private void OnEnable()
        {
            spatialOSComponent = GetComponent<SpatialOSComponent>();
        }

        public void AttemptChangeSlot(int slot)
        {
            // Ensure the slot is within bounds.
            if (slot < 0 || slot >= gunComponent.Data.GunSlots.Count)
            {
                return;
            }

            // Don't bother if request is for current slot.
            if (slot == gunComponent.Data.CurrentSlot)
            {
                return;
            }

            gunRequestSender.SendChangeSlotRequest(spatialOSComponent.SpatialEntityId,
                new ChangeCurrentSlotRequest(slot));
        }

        public void AttemptPickUpGun(int gunId)
        {
            gunRequestSender.SendPickUpGunRequest(spatialOSComponent.SpatialEntityId, new PickUpGunRequest(gunId));
        }
    }
}
