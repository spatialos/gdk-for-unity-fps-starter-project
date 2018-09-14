using Generated.Improbable.Gdk.Interaction;
using Unity.Entities;
using UnityEngine;

namespace Improbable.Gdk.Interaction
{
    // Needs to be added to the character capable of interacting, somewhere that receives OnTriggerEnter calls.
    public class TriggerInteractor : MonoBehaviour
    {
        public Entity CommandingEntity;
        public EntityManager Manager;

        void OnTriggerEnter(Collider collider)
        {
            long targetEntityId = InteractionSystem.GetAndCheckInteractiveObject(collider, InteractionType.Trigger);
            if (targetEntityId != -1)
            {
                var triggeredInteract = new TriggeredInteract();
                triggeredInteract.TargetEntityId = targetEntityId;

                Manager.AddComponentData(CommandingEntity, triggeredInteract);
            }
        }
    }
}
