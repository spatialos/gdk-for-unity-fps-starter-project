using Improbable.Gdk.Core;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

namespace Improbable.Gdk.Interaction
{
    /// <summary>
    ///     Proccess the assorted interact types, to check who to send the interact command to.
    /// </summary>
    [UpdateInGroup(typeof(SpatialOSUpdateGroup))]
    [UpdateAfter(typeof(Update))]
    public class InteractionSystem : ComponentSystem
    {
        public struct RaycastInteracts
        {
            public readonly int Length;
            public ComponentDataArray<InteractableComponent.CommandSenders.Interact> InteractRequestSenders;
            [ReadOnly] public ComponentDataArray<AttemptInteractRaycast> RaycastAttempts;
            [ReadOnly] public ComponentDataArray<SpatialEntityId> SpatialEntityIds;
            [ReadOnly] public EntityArray Entities;
        }

        public struct ProximityInteracts
        {
            public readonly int Length;
            public ComponentDataArray<InteractableComponent.CommandSenders.Interact> InteractRequestSenders;
            [ReadOnly] public ComponentDataArray<AttemptInteractProximity> ProximityAttempts;
            [ReadOnly] public ComponentDataArray<SpatialEntityId> SpatialEntityIds;
            [ReadOnly] public EntityArray Entities;
        }

        public struct TriggerInteracts
        {
            public readonly int Length;
            public ComponentDataArray<InteractableComponent.CommandSenders.Interact> InteractRequestSenders;
            [ReadOnly] public ComponentDataArray<TriggeredInteract> InteractTriggers;
            [ReadOnly] public ComponentDataArray<SpatialEntityId> SpatialEntityIds;
            [ReadOnly] public EntityArray Entities;
        }

        [Inject] private RaycastInteracts raycastInteracts;
        [Inject] private ProximityInteracts proximityInteracts;
        [Inject] private TriggerInteracts triggerInteracts;

        protected override void OnUpdate()
        {
            // Perform a raycast. Send an interact request to the hit object, if it is interactable via raycast.
            for (var i = 0; i < raycastInteracts.Length; i++)
            {
                var entity = raycastInteracts.Entities[i];
                var raycastDetails = raycastInteracts.RaycastAttempts[i];

                RaycastHit rayHit;
                var hitObject = Physics.Raycast(raycastDetails.Position, raycastDetails.RaycastDirection, out rayHit,
                    raycastDetails.Radius);

                if (hitObject)
                {
                    var interactEntityId = GetAndCheckInteractiveObject(rayHit.collider, InteractionType.Raycast);
                    if (interactEntityId != -1)
                    {
                        var commandSender = raycastInteracts.InteractRequestSenders[i];
                        var interactRequest = InteractableComponent.Interact.CreateRequest(
                            new EntityId(interactEntityId),
                            new InteractRequest(raycastInteracts.SpatialEntityIds[i].EntityId.Id)
                        );
                        commandSender.RequestsToSend.Add(interactRequest);
                        raycastInteracts.InteractRequestSenders[i] = commandSender;
                    }
                }

                PostUpdateCommands.RemoveComponent<AttemptInteractRaycast>(entity);
            }

            // If there are any objects nearby that are interactable via proximity, send an interact request to the closest one.
            for (var i = 0; i < proximityInteracts.Length; i++)
            {
                var entity = proximityInteracts.Entities[i];
                var proximityDetails = proximityInteracts.ProximityAttempts[i];

                var callerPosition = proximityDetails.Position;
                var hitObjects = Physics.OverlapSphere(callerPosition, proximityDetails.Radius);

                // Check all objects, keep track of the current closest object interactable via proximity.
                var closestEntityId = -1L;
                var closestSqrDistance = -1f;
                foreach (var collider in hitObjects)
                {
                    var interactEntityId = GetAndCheckInteractiveObject(collider, InteractionType.Proximity);
                    if (interactEntityId != -1)
                    {
                        var sqrDistance = (collider.transform.position - callerPosition).sqrMagnitude;
                        if (closestSqrDistance < 0 || sqrDistance < closestSqrDistance)
                        {
                            closestEntityId = interactEntityId;
                            closestSqrDistance = sqrDistance;
                        }
                    }
                }

                if (closestEntityId != -1)
                {
                    var commandSender = proximityInteracts.InteractRequestSenders[i];
                    var interactRequest = InteractableComponent.Interact.CreateRequest(
                        new EntityId(closestEntityId),
                        new InteractRequest(proximityInteracts.SpatialEntityIds[i].EntityId.Id)
                    );
                    commandSender.RequestsToSend.Add(interactRequest);
                    proximityInteracts.InteractRequestSenders[i] = commandSender;
                }

                PostUpdateCommands.RemoveComponent<AttemptInteractProximity>(entity);
            }

            // Send an interact request to the entity that was the trigger.
            for (var i = 0; i < triggerInteracts.Length; i++)
            {
                var entity = triggerInteracts.Entities[i];
                var triggerDetails = triggerInteracts.InteractTriggers[i];

                var commandSender = triggerInteracts.InteractRequestSenders[i];
                var interactRequest = InteractableComponent.Interact.CreateRequest(
                    new EntityId(triggerDetails.TargetEntityId),
                    new InteractRequest(triggerInteracts.SpatialEntityIds[i].EntityId.Id)
                );
                commandSender.RequestsToSend.Add(interactRequest);
                triggerInteracts.InteractRequestSenders[i] = commandSender;

                PostUpdateCommands.RemoveComponent<TriggeredInteract>(entity);
            }
        }

        public static long GetAndCheckInteractiveObject(Collider collider, InteractionType type)
        {
            var interactiveTarget = collider.GetComponentInParent<InteractableTag>();
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
