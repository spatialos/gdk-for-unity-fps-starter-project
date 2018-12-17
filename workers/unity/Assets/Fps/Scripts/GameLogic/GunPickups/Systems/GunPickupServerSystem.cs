using Fps.Schema.Shooting;
using Improbable.Gdk.Core;
using Improbable.Gdk.Guns;
using Improbable.Gdk.Interaction;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Fps
{
    public class GunPickupServerSystem : ComponentSystem
    {
        private struct InteractRequests
        {
            public readonly int Length;
            public ComponentDataArray<GunPickupComponent.Component> GunPickupComponent;
            public ComponentDataArray<GunComponent.CommandSenders.PickUpGun> PickUpGunSenders;
            [ReadOnly] public ComponentDataArray<InteractableComponent.CommandRequests.Interact> InteractionRequests;
            [ReadOnly] public EntityArray Entities;
            [ReadOnly] public ComponentDataArray<Authoritative<GunPickupComponent.Component>> DenotesGunPickupAuthority;
            [ReadOnly] public SubtractiveComponent<PickupRespawnDelay> DenotesNotRecharging;
        }

        private struct NewPickups
        {
            public readonly int Length;
            [ReadOnly] public EntityArray Entities;
            [ReadOnly] public ComponentDataArray<GunPickupComponent.Component> GunPickupComponent;
            [ReadOnly] public ComponentDataArray<Authoritative<GunPickupComponent.Component>> DenotesGunPickupAuthority;
            [ReadOnly] public ComponentDataArray<NewlyAddedSpatialOSEntity> NewlyCreatedEntities;
        }

        private struct RespawningPickups
        {
            public readonly int Length;
            public ComponentDataArray<PickupRespawnDelay> RespawnDelays;
            public ComponentDataArray<GunPickupComponent.Component> GunPickupComponent;
            [ReadOnly] public EntityArray Entities;
            [ReadOnly] public ComponentDataArray<Authoritative<GunPickupComponent.Component>> DenotesGunPickupAuthority;
        }

        [Inject] private NewPickups newPickups;
        [Inject] private InteractRequests interactCommands;
        [Inject] private RespawningPickups respawningPickups;

        protected override void OnUpdate()
        {
            for (var i = 0; i < newPickups.Length; i++)
            {
                var gunPickupComponent = newPickups.GunPickupComponent[i];
                if (!gunPickupComponent.IsEnabled)
                {
                    // Add a recharge time.
                    PostUpdateCommands.AddComponent(newPickups.Entities[i], new PickupRespawnDelay
                    {
                        RechargeTime = GunPickupSettings.PickupRespawnTime
                    });
                }
            }

            for (var i = 0; i < interactCommands.Length; i++)
            {
                var requests = interactCommands.InteractionRequests[i];
                var gunPickupComponent = interactCommands.GunPickupComponent[i];

                // Only allow the interaction if the pickup is enabled. Disable after interaction.
                if (!gunPickupComponent.IsEnabled)
                {
                    continue;
                }

                gunPickupComponent.IsEnabled = false;
                interactCommands.GunPickupComponent[i] = gunPickupComponent;

                // Only respond to the first request.
                var request = requests.Requests[0];
                var interactRequest = request.Payload;
                var commandSender = interactCommands.PickUpGunSenders[i];
                var pickUpGunRequest = GunComponent.PickUpGun.CreateRequest(
                    new EntityId(interactRequest.UserEntityId),
                    new PickUpGunRequest(gunPickupComponent.GunId)
                );
                commandSender.RequestsToSend.Add(pickUpGunRequest);
                interactCommands.PickUpGunSenders[i] = commandSender;

                // Add a recharge time.
                PostUpdateCommands.AddComponent(interactCommands.Entities[i], new PickupRespawnDelay
                {
                    RechargeTime = GunPickupSettings.PickupRespawnTime
                });
            }

            for (var i = 0; i < respawningPickups.Length; i++)
            {
                var respawnDelay = respawningPickups.RespawnDelays[i];
                respawnDelay.RechargeTime -= Time.deltaTime;

                if (respawnDelay.RechargeTime <= 0)
                {
                    PostUpdateCommands.RemoveComponent<PickupRespawnDelay>(respawningPickups.Entities[i]);

                    var gunPickupComponent = respawningPickups.GunPickupComponent[i];
                    gunPickupComponent.IsEnabled = true;
                    respawningPickups.GunPickupComponent[i] = gunPickupComponent;
                }
                else
                {
                    respawningPickups.RespawnDelays[i] = respawnDelay;
                }
            }
        }
    }
}
