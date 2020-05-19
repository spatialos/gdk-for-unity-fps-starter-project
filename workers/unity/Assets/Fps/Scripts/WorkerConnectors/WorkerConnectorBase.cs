using System;
using System.Collections;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fps.WorldTiles;
using Improbable.Gdk.Core;
using UnityEngine;

namespace Fps.WorkerConnectors
{
    public abstract class WorkerConnectorBase : WorkerConnector
    {
        [SerializeField] protected MapTemplate mapTemplate;

        [NonSerialized] internal GameObject LevelInstance;

        public override void Dispose()
        {
            if (LevelInstance != null)
            {
                Destroy(LevelInstance);
                LevelInstance = null;
            }

            base.Dispose();
        }

        // Get the world size from the config, and use it to generate the correct-sized level
        protected async Task LoadWorld()
        {
            var worldSize = await GetWorldSize();

            if (worldSize <= 0)
            {
                throw new ArgumentException("Received a world size of 0 or less.");
            }

            LevelInstance = await MapBuilder.GenerateMap(mapTemplate, worldSize, transform, Worker.WorkerType);
        }

        protected async Task WaitForWorkerFlags(CancellationToken token, params string[] flagKeys)
        {
            while (flagKeys.Any(key => string.IsNullOrEmpty(Worker.GetWorkerFlag(key))))
            {
                if (token.IsCancellationRequested)
                {
                    throw new TaskCanceledException();
                }

                Worker.LogDispatcher.HandleLog(LogType.Log, new LogEvent("Waiting for required worker flags.."));
                await Task.Yield();
            }
        }

        protected async Task<int> GetWorldSize()
        {
            await WaitForWorkerFlags(CancellationToken.None, "world_size");

            var flagValue = Worker.GetWorkerFlag("world_size");

            if (!int.TryParse(flagValue, out var worldSize))
            {
                Worker.LogDispatcher.HandleLog(LogType.Error,
                    new LogEvent($"Invalid world_size worker flag. Expected an integer, got \"{flagValue}\""));
            }

            return worldSize;
        }
    }
}
