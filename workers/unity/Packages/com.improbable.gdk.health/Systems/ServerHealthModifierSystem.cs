using Improbable.Gdk.Core;
using Improbable.Gdk.Health;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Improbable.Gdk.Health
{
    [UpdateInGroup(typeof(SpatialOSUpdateGroup))]
    public class ServerHealthModifierSystem : ComponentSystem
    {
        private struct EntitiesWithModifiedHealth
        {
            public readonly int Length;
            public ComponentDataArray<HealthComponent.Component> Health;
            [ReadOnly] public SharedComponentDataArray<HealthComponent.ComponentAuthority> Authority;
            [ReadOnly] public ComponentDataArray<SpatialEntityId> EntityIds;
            [ReadOnly] public EntityArray Entity;
        }

        [Inject] private EntitiesWithModifiedHealth entitiesWithModifiedHealth;
        [Inject] private ComponentUpdateSystem updateSystem;
        [Inject] private CommandSystem commandSystem;

        protected override void OnUpdate()
        {
            for (var i = 0; i < entitiesWithModifiedHealth.Length; i++)
            {
                if (!entitiesWithModifiedHealth.Authority[i].HasAuthority)
                {
                    continue;
                }

                var update = new HealthComponent.Update();
                var health = entitiesWithModifiedHealth.Health[i];

                if (health.Health <= 0)
                {
                    continue;
                }

                var entityId = entitiesWithModifiedHealth.EntityIds[i].EntityId;
                var requests = commandSystem.GetRequests<HealthComponent.ModifyHealth.ReceivedRequest>(entityId);
                for (int j = 0; j < requests.Count; ++j)
                {
                    ref readonly var request = ref requests[j];
                    var modifier = request.Payload;

                    var healthModifiedInfo = new HealthModifiedInfo
                    {
                        Modifier = modifier,
                        HealthBefore = health.Health
                    };

                    health.Health = Mathf.Clamp(health.Health + modifier.Amount, 0, health.MaxHealth);
                    update.Health = health.Health;
                    healthModifiedInfo.HealthAfter = health.Health;


                    if (health.Health <= 0)
                    {
                        healthModifiedInfo.Died = true;
                        updateSystem.SendEvent(new HealthComponent.HealthModified.Event(healthModifiedInfo), entityId);
                        break;
                    }

                    updateSystem.SendEvent(new HealthComponent.HealthModified.Event(healthModifiedInfo), entityId);
                }

                entitiesWithModifiedHealth.Health[i] = health;
                updateSystem.SendUpdate(update, entityId);
            }
        }
    }
}
