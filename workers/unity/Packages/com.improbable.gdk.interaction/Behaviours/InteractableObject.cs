using System.Collections;
using System.Collections.Generic;
using Improbable.Gdk.GameObjectRepresentation;
using Improbable.Gdk.Interaction;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class InteractableObject : MonoBehaviour
{
    [Require] private InteractableComponent.Requirable.Reader denotesInteractable;

    public InteractionType InteractionType = InteractionType.Proximity;
    public long EntityId => spatialOSComponent != null ? spatialOSComponent.SpatialEntityId.Id : -1;

    private SpatialOSComponent spatialOSComponent;

    private void OnEnable()
    {
        spatialOSComponent = GetComponent<SpatialOSComponent>();
    }

    // Ensure that a trigger interactable object is indeed a trigger.
    private void OnValidate()
    {
        if (InteractionType == InteractionType.Trigger)
        {
            var collider = GetComponent<Collider>();
            collider.isTrigger = true;
        }
    }
}
