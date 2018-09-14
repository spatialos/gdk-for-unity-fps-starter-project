using Generated.Improbable.Gdk.Interaction;
using UnityEngine;

namespace Improbable.Gdk.Interaction
{
    // Must be added to any object that has the InteractableComponent, such that it can be interacted with.
    public class InteractableTag : MonoBehaviour
    {
        public InteractionType InteractionType { get; set; }
        public long EntityId { get; set; }
    }
}
