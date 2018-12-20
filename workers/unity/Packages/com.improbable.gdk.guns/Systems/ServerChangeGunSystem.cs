using Improbable.Gdk.Core;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Improbable.Gdk.Guns
{
    /// <summary>
    ///     Respond to change gun slot, or gun id requests.
    /// </summary>
    [UpdateInGroup(typeof(SpatialOSUpdateGroup))]
    public class ServerChangeGunSystem : ComponentSystem
    {
        private struct GunSlotChangeRequests
        {
            public readonly int Length;
            public ComponentDataArray<GunComponent.Component> GunComponents;
            [ReadOnly] public ComponentDataArray<GunComponent.CommandRequests.ChangeSlot> ChangeSlotRequests;
            [ReadOnly] public ComponentDataArray<Authoritative<GunComponent.Component>> DenotesAuthority;
        }

        private struct GunPickupRequests
        {
            public readonly int Length;
            public ComponentDataArray<GunComponent.Component> GunComponents;
            [ReadOnly] public ComponentDataArray<GunComponent.CommandRequests.PickUpGun> PickUpGunRequests;
            [ReadOnly] public ComponentDataArray<Authoritative<GunComponent.Component>> DenotesAuthority;
        }

        [Inject] private GunSlotChangeRequests slotChangeRequests;
        [Inject] private GunPickupRequests pickupRequests;

        protected override void OnUpdate()
        {
            for (var i = 0; i < slotChangeRequests.Length; i++)
            {
                var gunComponent = slotChangeRequests.GunComponents[i];
                var requests = slotChangeRequests.ChangeSlotRequests[i];

                foreach (var request in requests.Requests)
                {
                    var newSlot = request.Payload.NewGunSlot;

                    if (newSlot == gunComponent.CurrentSlot)
                    {
                        continue;
                    }

                    // Only allow a switch if there is a gun in that slot.
                    if (gunComponent.GunSlots[newSlot] >= 0)
                    {
                        gunComponent.CurrentSlot = newSlot;
                        slotChangeRequests.GunComponents[i] = gunComponent;
                    }
                }
            }

            for (var i = 0; i < pickupRequests.Length; i++)
            {
                var gunComponent = pickupRequests.GunComponents[i];
                var requests = pickupRequests.PickUpGunRequests[i];

                foreach (var request in requests.Requests)
                {
                    var newId = request.Payload.NewGunId;
                    var gunList = gunComponent.GunSlots;
                    var handledPickup = false;

                    // If the new Id is < 0, do nothing.
                    if (newId < 0)
                    {
                        continue;
                    }

                    // If you already have that gun in a slot, switch to it.
                    for (var n = 0; n < gunList.Count; n++)
                    {
                        if (newId == gunList[n])
                        {
                            gunComponent.CurrentSlot = n;
                            handledPickup = true;
                            break;
                        }
                    }

                    // Otherwise, if you have a spare slot, put the gun in that, and switch to it.
                    if (!handledPickup)
                    {
                        for (var n = 0; n < gunList.Count; n++)
                        {
                            if (gunList[n] < 0)
                            {
                                gunComponent.CurrentSlot = n;
                                gunList[n] = newId;
                                gunComponent.GunSlots = gunList;
                                handledPickup = true;
                                break;
                            }
                        }
                    }

                    // Otherwise, replace your current gun with it.
                    if (!handledPickup)
                    {
                        gunList[gunComponent.CurrentSlot] = newId;
                        gunComponent.GunSlots = gunList;
                    }

                    pickupRequests.GunComponents[i] = gunComponent;
                }
            }
        }
    }
}
