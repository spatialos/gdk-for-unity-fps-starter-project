using Improbable.Gdk.Core;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Improbable.Gdk.Session
{
    [UpdateInGroup(typeof(SpatialOSUpdateGroup))]
    public class PlayerStateServerSystem : ComponentSystem
    {
        private CommandSystem commandSystem;
        private WorkerSystem workerSystem;

        protected override void OnCreateManager()
        {
            base.OnCreateManager();

            commandSystem = World.GetExistingManager<CommandSystem>();
            workerSystem = World.GetExistingManager<WorkerSystem>();
        }

        protected override void OnUpdate()
        {
            var requests = commandSystem.GetRequests<PlayerState.GainedKill.ReceivedRequest>();
            if (requests.Count == 0)
            {
                return;
            }

            var playerStateData = GetComponentDataFromEntity<PlayerState.Component>();

            for (var i = 0; i < requests.Count; i++)
            {
                ref readonly var request = ref requests[i];
                if (workerSystem.TryGetEntity(request.EntityId, out var entity))
                {
                    continue;
                }

                var playerState = playerStateData[entity];
                playerState.Kills++;
                playerStateData[entity] = playerState;
            }
        }
    }
}
