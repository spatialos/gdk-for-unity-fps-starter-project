using Improbable.Gdk.Core;
using Improbable.Gdk.GameObjectRepresentation;
using UnityEngine;

namespace Improbable.Gdk.Interaction
{
    public class InteractionHandler : MonoBehaviour
    {
        [Require] private InteractableComponent.Requirable.CommandRequestSender interactRequestSender;
        private EntityId entityId;

        private void OnEnable()
        {
            entityId = GetComponent<SpatialOSComponent>().SpatialEntityId;
        }

        public void TriggeredInteract(long triggerEntityId)
        {
            SendInteractRequest(triggerEntityId);
        }

        public void InteractRaycast(Vector3 position, Vector3 direction, float range)
        {
            if (Physics.Raycast(position, direction, out var rayHit, range))
            {
                var interactEntityId = GetAndCheckInteractiveObject(rayHit.collider, InteractionType.Raycast);
                if (interactEntityId != -1)
                {
                    SendInteractRequest(interactEntityId);
                }
            }
        }

        public void InteractProximity(Vector3 position, float radius)
        {
            var hitObjects = Physics.OverlapSphere(position, radius, ~0, QueryTriggerInteraction.Collide);

            // Check all objects, keep track of the current closest object interactable via proximity.
            var closestEntityId = -1L;
            var closestSqrDistance = -1f;
            foreach (var collider in hitObjects)
            {
                var interactEntityId = GetAndCheckInteractiveObject(collider, InteractionType.Proximity);
                if (interactEntityId != -1)
                {
                    var sqrDistance = (collider.transform.position - position).sqrMagnitude;
                    if (closestSqrDistance < 0 || sqrDistance < closestSqrDistance)
                    {
                        closestEntityId = interactEntityId;
                        closestSqrDistance = sqrDistance;
                    }
                }
            }

            if (closestEntityId != -1)
            {
                SendInteractRequest(closestEntityId);
            }
        }

        private void SendInteractRequest(long targetEntity)
        {
            if (!enabled)
            {
                return;
            }

            interactRequestSender.SendInteractRequest(
                new EntityId(targetEntity),
                new InteractRequest
                {
                    UserEntityId = entityId.Id
                });
        }


        public static long GetAndCheckInteractiveObject(Collider collider, InteractionType type)
        {
            var interactiveTarget = collider.GetComponentInParent<InteractableObject>();
            if (interactiveTarget == null)
            {
                return -1;
            }

            if (type != interactiveTarget.InteractionType)
            {
                return -1;
            }

            // The collider may be on a child of the interactive object, so get the actual object from the interface.
            return interactiveTarget.EntityId;
        }
    }
}
