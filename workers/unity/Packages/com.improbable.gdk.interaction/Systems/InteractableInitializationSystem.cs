using Improbable.Gdk.Core;
using Unity.Collections;
using Unity.Entities;

namespace Improbable.Gdk.Interaction
{
    /// <summary>
    ///
    /// </summary>
    [UpdateInGroup(typeof(SpatialOSReceiveGroup.GameObjectInitializationGroup))]
    public class InteractableInitializationSystem : ComponentSystem
    {
        public struct Interactables
        {
            public readonly int Length;
            [ReadOnly] public EntityArray Entities;
            [ReadOnly] public ComponentDataArray<SpatialEntityId> SpatialEntityIds;
            [ReadOnly] public ComponentDataArray<InteractableComponent.Component> InteractableComponents;
            [ReadOnly] public ComponentArray<InteractableTag> Tags;
            [ReadOnly] public ComponentDataArray<NewlyAddedSpatialOSEntity> NewlyCreatedEntities;
        }

        [Inject] private Interactables interactables;

        protected override void OnUpdate()
        {
            for (int i = 0; i < interactables.Length; i++)
            {
                var entity = interactables.Entities[i];
                var spatialId = interactables.SpatialEntityIds[i];
                var component = interactables.InteractableComponents[i];
                var tag = interactables.Tags[i];
                
                InteractionType type = (InteractionType) component.InteractionType;
                
                tag.InteractionType = type;
                tag.EntityId = spatialId.EntityId.Id;
            }
        }
    }
}
