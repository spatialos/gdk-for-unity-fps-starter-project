using System;
using System.Collections;
using System.Collections.Generic;
using Improbable.Gdk.Core;
using Improbable.Worker.CInterop;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = System.Random;

namespace Fps
{
    public class MapBuilder
    {
        private int layers;

        private const string SmallLevelFlag = "small";
        private const string LargeLevelFlag = "large";

        private MapBuilderSettings mapBuilderSettings;

        // Store the half-value as many calculations are simplified by going from -halfNumGroundLayers to halfNumGroundLayers.
        private int halfNumGroundLayers => (layers - 1) / mapBuilderSettings.TilesPerGroundLayer + 1;

        private GameObject groundTile;
        private GameObject groundEdge;
        private GameObject surroundWall;
        private GameObject cornerPiece;

        private Transform tileParentTransform;
        private Transform groundParentTransform;
        private Transform surroundParentTransform;
        private Transform spawnPointSystemTransform;

        private const string TileParentName = "TileParent";
        private const string GroundParentName = "GroundParent";
        private const string SurroundParentName = "SurroundParent";
        private const string SpawnPointSystemName = "SpawnPointSystem";

        private const string GroundTilePath = "Prefabs/Level/Ground/Ground4x4";
        private const string GroundEdgePath = "Prefabs/Level/Ground/Edge";
        private const string SurroundPath = "Prefabs/Level/Surround/Wall";
        private const string CornerPath = "Prefabs/Level/Surround/Corner";

        private GameObject gameObject;

        private Random random;

        public bool InvalidMapBuilder => gameObject == null;

        public MapBuilder(MapBuilderSettings mapBuilderSettings, GameObject gameObject)
        {
            this.mapBuilderSettings = mapBuilderSettings;
            this.gameObject = gameObject;
        }

        public IEnumerator CleanAndBuild(int worldLayers = 4, string seed = "SpatialOS GDK for Unity")
        {
            if (mapBuilderSettings == null)
            {
                Debug.LogError("MapBuilderSettings has not been set.");
                yield break;
            }

            layers = worldLayers;
            random = new Random(seed.GetHashCode());

            if (!TryLoadResources())
            {
                Debug.LogError("Generation aborted (See previous message)");
                yield break;
            }

            Clean();

            InitializeGroupsAndComponents();

            var originalPosition = gameObject.transform.position;
            var originalRotation = gameObject.transform.rotation;
            gameObject.transform.position = Vector3.zero;
            gameObject.transform.rotation = Quaternion.identity;

            // Split the following operations over a number of frames to allow the connection to the SpatialOS runtime
            // to stay alive.
            yield return null;
            yield return PlaceTiles();

            yield return null;
            PlaceGround();

            yield return null;
            FillSurround();

            yield return null;
            CollapseTileMeshes();

            yield return null;
            MakeLevelObjectStatic();

            spawnPointSystemTransform.gameObject.GetComponent<SpawnPoints>()?.SetSpawnPoints();

            gameObject.transform.position = originalPosition;
            gameObject.transform.rotation = originalRotation;

            var numPlayableTilesWide = layers * 2;

            // four tiles per groundLayer,
            // number of ground tiles is 2 * groundLayers
            // This value gives total tile-space including empty tiles around edge.
            var numTotalTilesWide = halfNumGroundLayers * 2 * mapBuilderSettings.TilesPerGroundLayer;

            Debug.Log("Finished building world\nClick for details..." +
                "\n\tPlayable space" +
                $"\n\t\t{numPlayableTilesWide}x{numPlayableTilesWide} tiles" +
                $"\n\t\t{numPlayableTilesWide * mapBuilderSettings.UnitsPerTile + mapBuilderSettings.UnitsPerBlock}" +
                $"x{numPlayableTilesWide * mapBuilderSettings.UnitsPerTile + mapBuilderSettings.UnitsPerBlock} units" +
                "\n\tTOTAL space" +
                $"\n\t\t{numTotalTilesWide}x{numTotalTilesWide} tiles" +
                $"\n\t\t{numTotalTilesWide * mapBuilderSettings.UnitsPerTile + mapBuilderSettings.UnitsPerBlock}" +
                $"x{numTotalTilesWide * mapBuilderSettings.UnitsPerTile + mapBuilderSettings.UnitsPerBlock} units\n");
        }

        private void CollapseTileMeshes()
        {
            tileParentTransform.GetComponent<TileCollapser>().CollapseMeshes();
        }

        private void InitializeGroupsAndComponents()
        {
            if (gameObject.GetComponentInChildren<SpawnPoints>() == null)
            {
                spawnPointSystemTransform = MakeChildGroup(SpawnPointSystemName);
                spawnPointSystemTransform.gameObject.AddComponent<SpawnPoints>();
            }

            groundParentTransform = MakeChildGroup(GroundParentName);

            surroundParentTransform = MakeChildGroup(SurroundParentName);

            tileParentTransform = MakeChildGroup(TileParentName);
            tileParentTransform.gameObject.AddComponent<TileCollapser>();
        }

        private bool TryLoadResources()
        {
            return TryLoadResource(GroundTilePath, out groundTile)
                && TryLoadResource(GroundEdgePath, out groundEdge)
                && TryLoadResource(SurroundPath, out surroundWall)
                && TryLoadResource(CornerPath, out cornerPiece);
        }

        private bool TryLoadResource(string resourcePath, out GameObject resource)
        {
            resource = Resources.Load<GameObject>(resourcePath);
            if (resource != null)
            {
                return true;
            }

            Debug.LogError($"Failed to load resource at Resources/{resourcePath}");
            return false;
        }

        private Transform MakeChildGroup(string groupName)
        {
            var group = new GameObject(groupName).transform;
            group.parent = gameObject.transform;
            group.localPosition = Vector3.zero;
            group.localRotation = Quaternion.identity;
            group.localScale = Vector3.one;
            return group;
        }

        private void FillSurround()
        {
            for (var groundLayerIndex = -halfNumGroundLayers;
                groundLayerIndex < halfNumGroundLayers;
                groundLayerIndex++)
            {
                float offset = groundLayerIndex * mapBuilderSettings.unitsPerGroundLayer;
                offset += mapBuilderSettings.unitsPerGroundLayer * .5f; // centre is half-distance across the ground layer

                MakeEdge(offset, 0);
                MakeEdge(offset, 90);
                MakeEdge(offset, 180);
                MakeEdge(offset, 270);
            }

            var cornerOffset =
                new Vector3
                {
                    x = halfNumGroundLayers * -mapBuilderSettings.unitsPerGroundLayer,
                    z = halfNumGroundLayers * mapBuilderSettings.unitsPerGroundLayer
                };

            for (var i = 0; i < 4; i++)
            {
                MakeCorner(90 * i, cornerOffset);
            }
        }

        private void MakeEdge(float offset, int angle)
        {
            var rotation = Quaternion.Euler(0, angle, 0);

            var floor = Object.Instantiate(groundEdge,
                rotation * new Vector3(
                    offset,
                    0,
                    halfNumGroundLayers * mapBuilderSettings.unitsPerGroundLayer +
                    mapBuilderSettings.UnitsPerBlock * 0.25f),
                rotation * Quaternion.Euler(90, 0, 0),
                groundParentTransform);
            floor.transform.localScale = new Vector3(
                mapBuilderSettings.unitsPerGroundLayer,
                mapBuilderSettings.UnitsPerBlock * .5f,
                1);

            var wall = Object.Instantiate(surroundWall,
                rotation * new Vector3(
                    offset,
                    mapBuilderSettings.UnitsPerBlock * .5f,
                    halfNumGroundLayers * mapBuilderSettings.unitsPerGroundLayer +
                    mapBuilderSettings.UnitsPerBlock * .5f),
                rotation,
                surroundParentTransform);
            wall.transform.localScale = new Vector3(
                mapBuilderSettings.unitsPerGroundLayer,
                mapBuilderSettings.UnitsPerBlock,
                1);

            var wallFloor = Object.Instantiate(groundEdge,
                rotation * new Vector3(
                    offset,
                    mapBuilderSettings.UnitsPerBlock,
                    halfNumGroundLayers * mapBuilderSettings.unitsPerGroundLayer + mapBuilderSettings.UnitsPerBlock),
                rotation * Quaternion.Euler(90, 0, 0),
                surroundParentTransform);
            wallFloor.transform.localScale = new Vector3(
                mapBuilderSettings.unitsPerGroundLayer,
                mapBuilderSettings.UnitsPerBlock,
                1);

            // Collision
            var collision = Object.Instantiate(surroundWall,
                rotation * new Vector3(
                    offset,
                    mapBuilderSettings.UnitsPerBlock + mapBuilderSettings.BoundaryCollisionHeight * .5f,
                    halfNumGroundLayers * mapBuilderSettings.unitsPerGroundLayer +
                    mapBuilderSettings.UnitsPerBlock * .5f),
                rotation,
                surroundParentTransform);
            collision.transform.localScale =
                new Vector3(
                    mapBuilderSettings.unitsPerGroundLayer +
                    mapBuilderSettings.UnitsPerBlock, // Collisions overlap to fill corners
                    mapBuilderSettings.BoundaryCollisionHeight,
                    1);
            collision.gameObject.name = "Collision";

            UnityObjectDestroyer.Destroy(collision.GetComponent<MeshRenderer>());
            UnityObjectDestroyer.Destroy(collision.GetComponent<MeshFilter>());
        }

        private void MakeCorner(int angle, Vector3 cornerOffset)
        {
            var rotation = Quaternion.Euler(0, angle, 0);
            Object.Instantiate(cornerPiece,
                rotation * cornerOffset,
                rotation,
                surroundParentTransform);
        }

        private IEnumerator PlaceTiles()
        {
            const int tilesPerFrame = 50;

            var tileCoord = new Vector2Int();
            var diff = new Vector2Int(0, -1);

            var tileCount = Math.Pow(2 * layers, 2);

            // Tiles are built in a spiral manner from the centre outward to ensure increasing the # of tile layers doesn't
            // alter the existing tile types.
            for (var i = 0; i < tileCount; i++)
            {
                // -layers < x <= layers AND -layers < y <= layers
                if (-layers < tileCoord.x && tileCoord.x <= layers
                    && -layers < tileCoord.y && tileCoord.y <= layers)
                {
                    PlaceTile(tileCoord);
                    if (i % tilesPerFrame == 0)
                    {
                        yield return null;
                    }
                }

                if (tileCoord.x == tileCoord.y ||
                    tileCoord.x < 0 && tileCoord.x == -tileCoord.y ||
                    tileCoord.x > 0 && tileCoord.x == 1 - tileCoord.y)
                {
                    diff = new Vector2Int(-diff.y, diff.x);
                }

                tileCoord += diff;
            }

            tileParentTransform.position = new Vector3
            {
                x = tileParentTransform.position.x,
                z = tileParentTransform.position.z
            };
        }

        private void PlaceTile(Vector2Int tileCoord)
        {
            var tile = GetTileObjectAtCoordinate(tileCoord);

            if (tile == null)
            {
                return;
            }

            float rotation = 90 * random.Next(0, 4);

            PlaceTile(tileCoord, tile, rotation);
        }

        private Vector3 GetWorldLocationFromCoordinate(Vector2Int coordinate)
        {
            var tileOffset = mapBuilderSettings.UnitsPerTile / 2;
            return new Vector3
            {
                x = (coordinate.x - 1) * mapBuilderSettings.UnitsPerTile + tileOffset,
                z = (coordinate.y - 1) * mapBuilderSettings.UnitsPerTile + tileOffset
            };
        }

        private void PlaceTile(Vector2Int tileCoord, GameObject tile, float rotation)
        {
            var tileOffset = mapBuilderSettings.UnitsPerTile / 2;

            Object.Instantiate(
                tile,
                GetWorldLocationFromCoordinate(tileCoord),
                new Quaternion
                {
                    eulerAngles = new Vector3
                    {
                        y = rotation
                    }
                },
                tileParentTransform.transform);
        }

        private void PlaceGround()
        {
            for (var x = -halfNumGroundLayers; x < halfNumGroundLayers; x++)
            {
                for (var z = -halfNumGroundLayers; z < halfNumGroundLayers; z++)
                {
                    PlaceGroundTile(x, z);
                }
            }
        }

        private void PlaceGroundTile(int groundX, int groundZ)
        {
            Object.Instantiate(
                groundTile,
                new Vector3
                {
                    x = groundX * mapBuilderSettings.unitsPerGroundLayer + mapBuilderSettings.unitsPerGroundLayer * .5f,
                    z = groundZ * mapBuilderSettings.unitsPerGroundLayer + mapBuilderSettings.unitsPerGroundLayer * .5f
                },
                Quaternion.identity,
                groundParentTransform.transform);
        }

        private void MakeLevelObjectStatic()
        {
            gameObject.isStatic = true;
            foreach (var childTransform in gameObject.GetComponentsInChildren<Transform>(true))
            {
                childTransform.gameObject.isStatic = true;
            }
        }

        private GameObject GetTileObjectAtCoordinate(Vector2Int coordinate)
        {
            var hits = Physics.OverlapSphere(GetWorldLocationFromCoordinate(coordinate), .5f);

            foreach (var hit in hits)
            {
                var volume = hit.gameObject.GetComponent<TileTypeVolume>();
                if (volume == null)
                {
                    continue;
                }

                return volume.TypeCollection.GetRandomTile(random);
            }

            return mapBuilderSettings.DefaultTileTypeCollection?.GetRandomTile(random);
        }

        public void Clean()
        {
            var childrenToDestroy = new Queue<GameObject>();

            foreach (var child in gameObject.GetComponentsInChildren<Transform>())
            {
                if (child.gameObject.name.Contains(TileParentName))
                {
                    childrenToDestroy.Enqueue(child.gameObject);
                    continue;
                }

                if (child.gameObject.name.Contains(GroundParentName))
                {
                    childrenToDestroy.Enqueue(child.gameObject);
                    continue;
                }

                if (child.gameObject.name.Contains(SurroundParentName))
                {
                    childrenToDestroy.Enqueue(child.gameObject);
                }
            }

            while (childrenToDestroy.Count != 0)
            {
                Object.DestroyImmediate(childrenToDestroy.Dequeue());
            }
        }

        public static Vector3 GetWorldDimensions(MapBuilderSettings mapBuilderSettings, int worldLayerCount)
        {
            var dimensions = worldLayerCount * mapBuilderSettings.UnitsPerTile * 2 + mapBuilderSettings.UnitsPerBlock;
            return new Vector3(dimensions, 100f, dimensions);
        }

        public static bool TryGetWorldLayerCount(MapBuilderSettings mapBuilderSettings,
            string worldSize, out int worldLayerCount)
        {
            switch (worldSize)
            {
                case SmallLevelFlag:
                    worldLayerCount = mapBuilderSettings.SmallWorldLayerCount;
                    break;
                case LargeLevelFlag:
                    worldLayerCount = mapBuilderSettings.LargeWorldLayerCount;
                    break;
                default:
                    return int.TryParse(worldSize, out worldLayerCount);
            }

            return true;
        }

        public static string GetWorldSizeFlag(Connection connection)
        {
            return connection.GetWorkerFlag("world_size");
        }

        // Get the world size from the config, and use it to generate the correct-sized level
        public static IEnumerator GenerateMap(
            MapBuilderSettings mapBuilderSettings,
            Transform workerTransform,
            Connection connection,
            string workerType,
            ILogDispatcher logDispatcher,
            WorkerConnectorBase worker)
        {
            var worldSize = GetWorldSizeFlag(connection);

            if (!TryGetWorldLayerCount(mapBuilderSettings, worldSize, out var worldLayerCount))
            {
                logDispatcher.HandleLog(LogType.Error,
                    new LogEvent("Invalid world_size worker flag. Make sure that it is either small or large,")
                        .WithField("world_size", worldSize));
                yield break;
            }

            var levelInstance = worker.LevelInstance;
            levelInstance.name = $"FPS-Level_{worldLayerCount}({workerType})";
            levelInstance.transform.position = workerTransform.position;
            levelInstance.transform.rotation = workerTransform.rotation;

            var mapBuilder = new MapBuilder(mapBuilderSettings, levelInstance);

            var volumesPrefab = mapBuilderSettings.WorldTileVolumes == null
                ? null
                : Object.Instantiate(mapBuilderSettings.WorldTileVolumes);

            yield return mapBuilder.CleanAndBuild(worldLayerCount);

            if (volumesPrefab != null)
            {
                UnityObjectDestroyer.Destroy(volumesPrefab);
            }
        }
    }
}
