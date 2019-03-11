using Improbable.Gdk.Core;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Improbable.Gdk.Session
{
    [UpdateInGroup(typeof(SpatialOSUpdateGroup))]
    public class PlayerStateServerSystem : ComponentSystem
    {
        private struct PlayerStateData
        {
            public readonly int Length;
            [ReadOnly] public ComponentDataArray<SpatialEntityId> EntityId;
            [ReadOnly] public ComponentDataArray<PlayerState.CommandRequests.GainedKill> GainedKillRequest;
            public ComponentDataArray<PlayerState.Component> PlayerState;
        }

        [Inject] private PlayerStateData entitiesWithState;

        protected override void OnUpdate()
        {
            for (var i = 0; i < entitiesWithState.Length; i++)
            {
                var gainedKillRequests = entitiesWithState.GainedKillRequest[i];

                if (gainedKillRequests.Requests.Count == 0)
                {
                    continue;
                }
                
                var playerState = entitiesWithState.PlayerState[i];

                playerState.Kills += gainedKillRequests.Requests.Count;

                entitiesWithState.PlayerState[i] = playerState;
            }
        }
    }
}
