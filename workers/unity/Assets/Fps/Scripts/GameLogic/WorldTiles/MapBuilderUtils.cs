using Improbable.Gdk.Core;
using Improbable.Worker.CInterop;
using UnityEngine;

namespace Fps
{
    public static class MapBuilderUtils
    {
        private const string Small = "small";
        private const string Large = "large";

        public const int SmallWorldLayerCount = 4;
        public const int LargeWorldLayerCount = 24;

        // Measurements.
        // All sizes are 1:1 ratio in X/Z, so we just define one value to represent both axis.
        public const int UnitsPerBlock = 4; // One textured square on the ground is a 'block'.
        public const int UnitsPerTile = 9 * UnitsPerBlock;
        public const int TilesPerGroundLayer = 4; // Ground layers are large quads that encompass 4x4 tiles.
        public const int BoundaryCollisionHeight = 16;

        public static int unitsPerGroundLayer => TilesPerGroundLayer * UnitsPerTile;

        public static Vector3 GetWorldDimensions(int worldLayerCount)
        {
            var dimensions = (worldLayerCount * UnitsPerTile * 2) + UnitsPerBlock;
            return new Vector3(dimensions, 100f, dimensions);
        }

        public static bool GetWorldLayerCount(string worldSize, out int worldLayerCount)
        {
            switch (worldSize)
            {
                case Small:
                    worldLayerCount = SmallWorldLayerCount;
                    break;
                case Large:
                    worldLayerCount = LargeWorldLayerCount;
                    break;
                default:
                    return int.TryParse(worldSize, out worldLayerCount);
            }

            return true;
        }

        public static string GetWorldSize(Connection connection)
        {
            return connection.GetWorkerFlag("world_size");
        }

        // Get the world size from the config, and use it to generate the correct-sized level
        public static GameObject GenerateMap(
            Transform workerTransform,
            Connection connection,
            string workerType,
            WorkerSystem workerSystem)
        {
            var levelInstance = new GameObject();
            var worldSize = GetWorldSize(connection);

            if (GetWorldLayerCount(worldSize, out var worldLayerCount))
            {
                levelInstance.name = $"FPS-Level_{worldLayerCount}({workerType})";
                levelInstance.transform.position = workerTransform.position;
                levelInstance.transform.rotation = workerTransform.rotation;

                var mapBuilder = new MapBuilder(levelInstance);
                mapBuilder.CleanAndBuild(worldLayerCount);
            }
            else
            {
                workerSystem.LogDispatcher.HandleLog(LogType.Error,
                    new LogEvent("Invalid world_size worker flag. Make sure that it is either small or large,")
                        .WithField("world_size", worldSize));
            }

            return levelInstance;
        }
    }
}
