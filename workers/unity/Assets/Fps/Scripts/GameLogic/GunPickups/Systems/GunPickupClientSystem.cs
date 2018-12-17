using System.Collections.Generic;
using Fps.Schema.Shooting;
using Improbable;
using Improbable.Gdk.Core;
using Improbable.Gdk.GameObjectRepresentation;
using Improbable.Gdk.Interaction;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Fps.GunPickups
{
    [UpdateBefore(typeof(InteractableInitializationSystem))]
    public class GunPickupClientSystem : ComponentSystem
    {
        private struct NewPickups
        {
            public readonly int Length;
            [ReadOnly] public EntityArray Entities;
            [ReadOnly] public ComponentDataArray<SpatialEntityId> SpatialEntityIds;
            [ReadOnly] public ComponentDataArray<GunPickupComponent.Component> GunPickupComponents;
            [ReadOnly] public ComponentDataArray<InteractableComponent.Component> InteractableComponents;
            [ReadOnly] public ComponentDataArray<Position.Component> PositionComponents;
            [ReadOnly] public ComponentDataArray<NewlyAddedSpatialOSEntity> NewlyCreatedEntities;
        }

        private struct EnabledChanges
        {
            public readonly int Length;
            [ReadOnly] public ComponentDataArray<GunPickupComponent.Component> GunPickupComponents;
            [ReadOnly] public ComponentDataArray<GunPickupComponent.ReceivedUpdates> DenotesUpdated;
            [ReadOnly] public ComponentArray<GunPickupDisplay> PickupDisplays;
        }

        [Inject] private NewPickups newPickups;
        [Inject] private EnabledChanges enabledChanges;
        [Inject] private EntityGameObjectLinkerSystem linkerSystem;
        [Inject] private WorkerSystem workerSystem;

        private ViewCommandBuffer viewCommandBuffer;
        private readonly Dictionary<int, GameObject> instantiatedParents = new Dictionary<int, GameObject>();

        protected override void OnCreateManager()
        {
            base.OnCreateManager();

            viewCommandBuffer = new ViewCommandBuffer(EntityManager, workerSystem.LogDispatcher);
        }

        protected override void OnUpdate()
        {
            for (var i = 0; i < newPickups.Length; i++)
            {
                var pickupComponent = newPickups.GunPickupComponents[i];
                var interactableComponent = newPickups.InteractableComponents[i];
                var positionCoords = newPickups.PositionComponents[i].Coords;
                var position = new Vector3(
                    (float) positionCoords.X,
                    (float) positionCoords.Y,
                    (float) positionCoords.Z);
                var entity = newPickups.Entities[i];
                var spatialEntityId = newPickups.SpatialEntityIds[i].EntityId;

                var gameObject = InstantiateAndLinkGameObject(ref entity, ref position, spatialEntityId);
                var pickupDisplay = gameObject.GetComponent<GunPickupDisplay>();
                pickupDisplay.SetGunId(pickupComponent.GunId);
                pickupDisplay.SetEnabled(pickupComponent.IsEnabled);
                viewCommandBuffer.AddComponent(entity, typeof(GunPickupDisplay), pickupDisplay);

                var interactableTag = gameObject.GetComponent<InteractableTag>();
                if (interactableTag == null)
                {
                    interactableTag = gameObject.AddComponent<InteractableTag>();
                }

                viewCommandBuffer.AddComponent(entity, typeof(InteractableTag), interactableTag);
            }

            for (var i = 0; i < enabledChanges.Length; i++)
            {
                var pickupComponent = enabledChanges.GunPickupComponents[i];
                var pickupDisplay = enabledChanges.PickupDisplays[i];

                pickupDisplay.SetEnabled(pickupComponent.IsEnabled);
            }

            viewCommandBuffer.FlushBuffer();
        }

        private GameObject InstantiateAndLinkGameObject(ref Entity entity,
            ref Vector3 position,
            Improbable.Worker.EntityId spatialEntityId)
        {
            var prefab = GunPickupSettings.GunPickupPrefab;
            if (prefab == null)
            {
                Debug.LogErrorFormat("Prefab not found");
                return null;
            }

            var spawningPosition = position + workerSystem.Origin;
            var spawningRotation = Quaternion.identity;
            var gameObject = Object.Instantiate(prefab, spawningPosition, spawningRotation);
            gameObject.name = $"{prefab.name}(SpatialOS: {spatialEntityId.Id}, Unity: {entity.Index}/{World.Name})";

            // Add a reference to the GameObject, such that it can be accessed after the entity has been deleted.
            // instantiatedParents.Add(entity.Index, gameObject);

            // // Add a persistent handle, to detect if the entity has been deleted.
            // var parentReferenceHandle = new ParentReferenceHandle();
            // PostUpdateCommands.AddComponent(entity, parentReferenceHandle);

            viewCommandBuffer.AddComponent(entity, typeof(Transform), gameObject.transform);
            linkerSystem.Linker.LinkGameObjectToEntity(gameObject, entity, spatialEntityId, viewCommandBuffer);

            return gameObject;
        }
    }
}
