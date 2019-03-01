using Improbable.Gdk.Core;
using Improbable.Gdk.Health;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Improbable.Gdk.Health
{
    [AlwaysUpdateSystem]
    [UpdateInGroup(typeof(SpatialOSUpdateGroup))]
    public class ServerHealthModifierSystem : ComponentSystem
    {
        [Inject] private CommandSystem commandSystem;
        [Inject] private ComponentUpdateSystem updateSystem;
        [Inject] private WorkerSystem workerSystem;

        protected override void OnUpdate()
        {
            var requests = commandSystem.GetRequests<HealthComponent.ModifyHealth.ReceivedRequest>();
            for (int i = 0; i < requests.Count; ++i)
            {
                var entityId = requests[i].EntityId;
                ref readonly var modifier = ref requests[i].Payload;
                workerSystem.TryGetEntity(entityId, out var entity);
                var health = EntityManager.GetComponentData<HealthComponent.Component>(entity);

                if (health.Health <= 0)
                {
                    continue;
                }


                var healthModifiedInfo = new HealthModifiedInfo
                {
                    Modifier = modifier,
                    HealthBefore = health.Health
                };

                health.Health = Mathf.Clamp(health.Health + modifier.Amount, 0, health.MaxHealth);
                healthModifiedInfo.HealthAfter = health.Health;


                if (health.Health <= 0)
                {
                    healthModifiedInfo.Died = true;
                }

                updateSystem.SendEvent(new HealthComponent.HealthModified.Event(healthModifiedInfo), entityId);

                EntityManager.SetComponentData(entity, health);
            }
        }
    }
}
