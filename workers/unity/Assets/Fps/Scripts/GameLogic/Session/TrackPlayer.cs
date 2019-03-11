using System.Collections.Generic;
using Improbable.Gdk.Core;
using Improbable.Gdk.Movement;
using Improbable.Gdk.ReactiveComponents;
using Improbable.Gdk.Session;
using Unity.Entities;
using UnityEngine;

namespace Fps
{
    [DisableAutoCreation]
    public class TrackPlayerSystem : ComponentSystem
    {
        private ResultsScreenController resultsScreenController;

        protected override void OnCreateManager()
        {
            base.OnCreateManager();
            var controller = Object.FindObjectOfType<ScreenUIController>();
            resultsScreenController = controller.FrontEndController.ResultsScreenController;
            ConnectionStateReporter.OnConnectionStateChange += OnConnectionStateChange;
            Enabled = false;
        }

        private void OnConnectionStateChange(ConnectionStateReporter.State state, string information)
        {
            if (state == ConnectionStateReporter.State.GatherResults)
            {
                var results = new List<ResultsData>();
                var ownPlayerFilter = new[]
                {
                    ComponentType.ReadOnly<PlayerState.Component>(),
                    ComponentType.ReadOnly<Authoritative<ClientMovement.Component>>()
                };

                var ownPlayerGroup = GetComponentGroup(ownPlayerFilter);

                var playerName = ownPlayerGroup.GetComponentDataArray<PlayerState.Component>()[0].Name;

                var playersFilter = new[]
                {
                    ComponentType.ReadOnly<PlayerState.Component>(),
                };

                var playersGroup = GetComponentGroup(playersFilter);
                var playerStates = playersGroup.GetComponentDataArray<PlayerState.Component>();

                for (var i = 0; i < playerStates.Length; i++)
                {
                    var playerState = playerStates[i];
                    var result = new ResultsData(playerState.Name, playerState.Kills, playerState.Deaths);
                    results.Add(result);
                }


                results.Sort((x, y) => y.KillDeathRatio.CompareTo(x.KillDeathRatio));
                var playerRank = -1;

                for (var i = 0; i < results.Count; i++)
                {
                    var result = results[i];
                    result.Rank = i + 1;
                    results[i] = result;

                    if (playerName == result.PlayerName)
                    {
                        playerRank = i;
                    }
                }

                resultsScreenController.gameObject.SetActive(true);
                resultsScreenController.SetResults(results.ToArray(), playerRank);
                ConnectionStateReporter.SetState(ConnectionStateReporter.State.ShowResults);
            }
        }

        protected override void OnUpdate()
        {
        }

        protected override void OnDestroyManager()
        {
            ConnectionStateReporter.OnConnectionStateChange -= OnConnectionStateChange;
            base.OnDestroyManager();
        }
    }
}
