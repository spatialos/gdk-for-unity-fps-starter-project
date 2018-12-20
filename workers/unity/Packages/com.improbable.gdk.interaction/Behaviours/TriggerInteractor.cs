using UnityEngine;

namespace Improbable.Gdk.Interaction
{
    // Needs to be added to the character capable of interacting, somewhere that receives OnTriggerEnter calls.
    [RequireComponent(typeof(InteractionHandler))]
    public class TriggerInteractor : MonoBehaviour
    {
        private InteractionHandler interactionHandler;

        private void Awake()
        {
            interactionHandler = GetComponent<InteractionHandler>();
        }

        void OnTriggerEnter(Collider collider)
        {
            long targetEntityId = InteractionHandler.GetAndCheckInteractiveObject(collider, InteractionType.Trigger);
            if (targetEntityId != -1)
            {
                interactionHandler.TriggeredInteract(targetEntityId);
            }
        }
    }
}
