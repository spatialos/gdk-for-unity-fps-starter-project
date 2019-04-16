using System.Collections.Generic;
using Improbable.Gdk.Movement;
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

            ownPlayerGroup = GetComponentGroup(
                ComponentType.ReadOnly<PlayerState.Component>(),
                ComponentType.ReadOnly<ClientMovement.ComponentAuthority>()
            );
            ownPlayerGroup.SetFilter(ClientMovement.ComponentAuthority.Authoritative);

            playersGroup = GetComponentGroup(ComponentType.ReadOnly<PlayerState.Component>());
            sessionGroup = GetComponentGroup(ComponentType.ReadOnly<Session.Component>());

            PlayerResults = new List<ResultsData>();
        }

        protected override void OnUpdate()
        {
            if (sessionGroup.IsEmptyIgnoreFilter)
            {
                SessionStatus = null;
            }
            else
            {
                SessionStatus = sessionGroup.GetComponentDataArray<Session.Component>()[0].Status;
            }

            var playerStateData = ownPlayerGroup.GetComponentDataArray<PlayerState.Component>();
            if (playerStateData.Length == 0)
            {
                PlayerName = null;
            }
            else
            {
                PlayerName = playerStateData[0].Name;
            }

            PlayerResults.Clear();
            var playerStates = playersGroup.GetComponentDataArray<PlayerState.Component>();
            for (var i = 0; i < playerStates.Length; i++)
            {
                var playerState = playerStates[i];
                var result = new ResultsData(playerState.Name, playerState.Kills, playerState.Deaths);
                PlayerResults.Add(result);
            }

            PlayerResults.Sort((x, y) => y.KillDeathRatio.CompareTo(x.KillDeathRatio));
        }
    }
}
