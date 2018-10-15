using Improbable.Gdk.Core;
using Unity.Collections;
using Unity.Entities;

namespace Improbable.Gdk.Guns
{
    [UpdateInGroup(typeof(SpatialOSUpdateGroup))]
    public class PlayerScoreTrackerSystem : ComponentSystem
    {
        private struct PlayersWithScoreRequests
        {
            public readonly int Length;
            [ReadOnly] public ComponentDataArray<Authoritative<ScoreComponent.Component>> DenotesAuthority;
            [ReadOnly] public ComponentDataArray<ScoreComponent.CommandRequests.AddScore> AddScoreRequests;
            public ComponentDataArray<ScoreComponent.Component> ScoreComponent;
            public ComponentDataArray<ScoreComponent.CommandResponders.AddScore> AddScoreResponders;
        }

        [Inject] private PlayersWithScoreRequests players;

        protected override void OnUpdate()
        {
            for (var i = 0; i < players.Length; i++)
            {
                var scoreComponent = players.ScoreComponent[i];
                foreach (var scoreRequest in players.AddScoreRequests[i].Requests)
                {
                    scoreComponent.Kills++;
                }

                players.ScoreComponent[i] = scoreComponent;
            }
        }
    }
}
