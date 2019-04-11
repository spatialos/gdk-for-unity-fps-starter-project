using System.Collections.Generic;
using Improbable.Gdk.Session;
using Unity.Entities;

namespace Fps
{
    [DisableAutoCreation]
    public class TrackPlayerSystem : ComponentSystem
    {
        private ComponentGroup ownPlayerGroup;
        private ComponentGroup playersGroup;
        private ComponentGroup sessionGroup;

        public string PlayerName { get; private set; }
        public Status? SessionStatus { get; private set; }
        public List<ResultsData> PlayerResults { get; private set; }

        protected override void OnCreateManager()
        {
            base.OnCreateManager();
            Enabled = false;

            ownPlayerGroup = GetComponentGroup(
                ComponentType.ReadOnly<PlayerState.Component>(),
                ComponentType.ReadOnly<PlayerState.ComponentAuthority>()
            );
            ownPlayerGroup.SetFilter(PlayerState.ComponentAuthority.Authoritative);

            playersGroup = GetComponentGroup(ComponentType.ReadOnly<PlayerState.Component>());
            sessionGroup = GetComponentGroup(ComponentType.ReadOnly<Session.Component>());
        }

        protected override void OnUpdate()
        {
            if (sessionGroup.IsEmptyIgnoreFilter)
            {
                SessionStatus = null;
            }

            SessionStatus = sessionGroup.GetComponentDataArray<Session.Component>()[0].Status;

            var playerStateData = ownPlayerGroup.GetComponentDataArray<PlayerState.Component>();
            if (playerStateData.Length == 0)
            {
                PlayerName = null;
            }

            PlayerName = playerStateData[0].Name;

            var results = new List<ResultsData>();
            var playerStates = playersGroup.GetComponentDataArray<PlayerState.Component>();
            for (var i = 0; i < playerStates.Length; i++)
            {
                var playerState = playerStates[i];
                var result = new ResultsData(playerState.Name, playerState.Kills, playerState.Deaths);
                results.Add(result);
            }

            results.Sort((x, y) => y.KillDeathRatio.CompareTo(x.KillDeathRatio));
            PlayerResults = results;
        }
    }
}
