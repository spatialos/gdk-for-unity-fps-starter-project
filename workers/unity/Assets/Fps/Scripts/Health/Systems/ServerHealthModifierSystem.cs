using Improbable.Gdk.Core;
using Unity.Entities;
using UnityEngine;

namespace Fps.Health
{
    [UpdateInGroup(typeof(SpatialOSUpdateGroup))]
    public class ServerHealthModifierSystem : ComponentSystem
    {
        private WorkerSystem workerSystem;
        private CommandSystem commandSystem;
        private ComponentUpdateSystem componentUpdateSystem;

        protected override void OnCreate()
        {
            base.OnCreate();

            workerSystem = World.GetExistingSystem<WorkerSystem>();
            commandSystem = World.GetExistingSystem<CommandSystem>();
            componentUpdateSystem = World.GetExistingSystem<ComponentUpdateSystem>();
        }

        protected override void OnUpdate()
        {
            var requests = commandSystem.GetRequests<HealthComponent.ModifyHealth.ReceivedRequest>();
            if (requests.Count == 0)
            {
                return;
            }

            var healthComponentData = GetComponentDataFromEntity<HealthComponent.Component>();
            for (var i = 0; i < requests.Count; i++)
            {
                ref readonly var request = ref requests[i];
                var entityId = request.EntityId;
                if (!workerSystem.TryGetEntity(entityId, out var entity))
                {
                    continue;
                }

                var health = healthComponentData[entity];

                // Skip if already dead
                if (health.Health <= 0)
                {
                    continue;
                }

                var modifier = request.Payload;
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

                componentUpdateSystem.SendEvent(new HealthComponent.HealthModified.Event(healthModifiedInfo), entityId);
                healthComponentData[entity] = health;
            }
        }
    }
}
