using System.Collections.Generic;
using Improbable.Gdk.Core;
using Improbable.Gdk.Core.Commands;
using Improbable.Gdk.Session;
using Improbable.Worker.CInterop;
using Improbable.Worker.CInterop.Query;
using Unity.Entities;
using UnityEngine;
using EntityQuery = Improbable.Worker.CInterop.Query.EntityQuery;

namespace Fps
{
    [DisableAutoCreation]
    public class TrackPlayerSystem : ComponentSystem
    {
        private long? sentPlayerStateQueryId;
        private Unity.Entities.EntityQuery sessionGroup;
        private CommandSystem commandSystem;
        private ILogDispatcher logDispatcher;

        private readonly EntityQuery playerStateQuery = new EntityQuery
        {
            Constraint = new ComponentConstraint(PlayerState.ComponentId),
            ResultType = new SnapshotResultType(new List<uint> { PlayerState.ComponentId })
        };

        public Status? SessionStatus { get; private set; }
        public List<ResultsData> PlayerResults { get; private set; }

        protected override void OnCreate()
        {
            base.OnCreate();

            commandSystem = World.GetExistingSystem<CommandSystem>();
            logDispatcher = World.GetExistingSystem<WorkerSystem>().LogDispatcher;
            sessionGroup = GetEntityQuery(ComponentType.ReadOnly<Session.Component>());

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
                Entities.With(sessionGroup).ForEach((ref Session.Component session) =>
                {
                    SessionStatus = session.Status;
                });
            }

            if (SessionStatus != Status.STOPPING)
            {
                return;
            }

            if (sentPlayerStateQueryId == null && PlayerResults.Count == 0)
            {
                sentPlayerStateQueryId =
                    commandSystem.SendCommand(new WorldCommands.EntityQuery.Request(playerStateQuery));
            }

            var entityQueryResponses = commandSystem.GetResponses<WorldCommands.EntityQuery.ReceivedResponse>();
            for (var i = 0; i < entityQueryResponses.Count; i++)
            {
                ref readonly var response = ref entityQueryResponses[i];
                if (response.RequestId != sentPlayerStateQueryId)
                {
                    continue;
                }

                sentPlayerStateQueryId = null;

                if (response.StatusCode == StatusCode.Success)
                {
                    PlayerResults.Clear();
                    foreach (var responseValue in response.Result.Values)
                    {
                        var playerState = responseValue.GetComponentSnapshot<PlayerState.Snapshot>().Value;
                        var result = new ResultsData(playerState.Name, playerState.Kills, playerState.Deaths);
                        PlayerResults.Add(result);
                    }

                    PlayerResults.Sort((x, y) => y.KillDeathRatio.CompareTo(x.KillDeathRatio));

                    for (var j = 0; j < PlayerResults.Count; j++)
                    {
                        var result = PlayerResults[j];
                        result.Rank = j + 1;
                        PlayerResults[j] = result;
                    }
                }
                else
                {
                    logDispatcher.HandleLog(LogType.Error, new LogEvent(
                        $"Failed to receive results..."
                    ));
                }
            }
        }
    }
}
