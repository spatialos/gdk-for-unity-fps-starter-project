using Improbable.Gdk.GameObjectRepresentation;
using Improbable.Gdk.Interaction;
using UnityEngine;

namespace Improbable.Gdk.Guns
{
    public class ClientGunChanging : MonoBehaviour
    {
        [Require] private GunComponent.Requirable.CommandRequestSender gunRequestSender;
        [Require] private GunComponent.Requirable.Reader gunComponent;
        private SpatialOSComponent spatialOSComponent;
        private InteractionHandler interactionHandler;

        [SerializeField] [Tooltip("The maximum distance away that gun pickups can be picked up from")]
        private float gunPickupRange = 1f;

        private void OnEnable()
        {
            spatialOSComponent = GetComponent<SpatialOSComponent>();
            interactionHandler = GetComponent<InteractionHandler>();
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

        // Uses the interaction system, assuming proximity.
        public void AttemptPickUpGun()
        {
            Debug.DrawLine(transform.position, transform.position + transform.forward * gunPickupRange, Color.red, 1);
            interactionHandler.InteractProximity(transform.position, gunPickupRange);
        }
    }
}
