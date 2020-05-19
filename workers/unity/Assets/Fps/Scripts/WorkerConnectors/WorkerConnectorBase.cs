using System;
using System.Collections;
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
        protected virtual IEnumerator LoadWorld()
        {
            // Defer a frame to allow worker flags to propagate.
            yield return null;

            var worldSize = GetWorldSize();
            if (worldSize <= 0)
            {
                yield break;
            }

            yield return MapBuilder.GenerateMap(
                mapTemplate,
                worldSize,
                transform,
                Worker.WorkerType,
                this);
        }

        protected int GetWorldSize()
        {
            var flagValue = Worker.GetWorkerFlag("world_size");

            if (!int.TryParse(flagValue, out var worldSize))
            {
                Worker.LogDispatcher.HandleLog(LogType.Error,
                    new LogEvent($"Invalid world_size worker flag. Expected an integer, got \"{flagValue}\""));
                return 0;
            }

            return worldSize;
        }
    }
}
