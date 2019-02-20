using System;
using System.Collections.Generic;
<<<<<<< HEAD
using System.Linq;
using Improbable.Gdk.Core;
using Improbable.Worker.CInterop;
=======
>>>>>>> MapBuilder uses texture lookup to place tiles. MaptileCollectionAsset holds list of prefabs to spawn for associated colour found in texture lookup.
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace Fps
{
    public class MapBuilder
    {
        private int Layers;
        private string Seed;
        private float EmptyTileChance;

        private const string SmallLevelFlag = "small";
        private const string LargeLevelFlag = "large";

        private MapBuilderSettings mapBuilderSettings;

<<<<<<< HEAD
=======
        public Texture2D MapTileLookupTexture;
        public MapTileCollectionAsset DefaultTileCollection;
        public MapTileCollectionAsset[] ExtraTileCollections;

        // Measurements.
        // All sizes are 1:1 ratio in X/Z, so we just define one value to represent both axis.
        public const int UnitsPerBlock = 4; // One textured square on the ground is a 'block'.
        public const int UnitsPerTile = 9 * UnitsPerBlock;
        public const int TilesPerGroundLayer = 4; // Ground layers are large quads that encompass 4x4 tiles.
        public const int BoundaryCollisionHeight = 16;


#if UNITY_EDITOR
>>>>>>> MapBuilder uses texture lookup to place tiles. MaptileCollectionAsset holds list of prefabs to spawn for associated colour found in texture lookup.
        // Store the half-value as many calculations are simplified by going from -halfNumGroundLayers to halfNumGroundLayers.
        private int halfNumGroundLayers => (Layers - 1) / mapBuilderSettings.TilesPerGroundLayer + 1;

        private GameObject[] levelTiles;
        private GameObject groundTile;
        private GameObject groundEdge;
        private GameObject surroundWall;
        private GameObject cornerPiece;

        private Transform tileParentTransform;
        private Transform groundParentTransform;
        private Transform surroundParentTransform;
        private Transform spawnPointSystemTransform;

        private const string TileParentName = "TileParent (auto-generated)";
        private const string GroundParentName = "GroundParent (auto-generated)";
        private const string SurroundParentName = "SurroundParent (auto-generated)";
        private const string SpawnPointSystemName = "SpawnPointSystem";

        private const string LevelTilePath = "Prefabs/Level/Tiles";
        private const string GroundTilePath = "Prefabs/Level/Ground/Ground4x4";
        private const string GroundEdgePath = "Prefabs/Level/Ground/Edge";
        private const string SurroundPath = "Prefabs/Level/Surround/Wall";
        private const string CornerPath = "Prefabs/Level/Surround/Corner";

        // Central tiles are hardcoded.
        private const string CentreTile0 = "Prefabs/Level/Tiles/Centre0";
        private const string CentreTile1 = "Prefabs/Level/Tiles/Centre1";
        private const string CentreTile2 = "Prefabs/Level/Tiles/Centre2";
        private const string CentreTile3 = "Prefabs/Level/Tiles/Centre3";

        private GameObject gameObject;

        public bool InvalidMapBuilder => gameObject == null;

        public MapBuilder(MapBuilderSettings mapBuilderSettings, GameObject gameObject)
        {
            this.mapBuilderSettings = mapBuilderSettings;
            this.gameObject = gameObject;
        }

        public void CleanAndBuild(
            int worldLayers = 4,
            string seed = "SpatialOS GDK for Unity",
            float emptyTileChance = 0.2f)
        {
            if (mapBuilderSettings == null)
            {
                Debug.LogError("MapBuilderSettings has not been set.");
                return;
            }

            Layers = worldLayers;
            Seed = seed;
            EmptyTileChance = emptyTileChance;

            if (!TryLoadResources())
            {
                Debug.LogError("Generation aborted (See previous message)");
                return;
            }

            if (DefaultTileCollection == null)
            {
                Debug.LogWarning("Must have at least a Default Tile Collection asset reference to build the map");
                return;
            }


            Clean();

            InitializeGroupsAndComponents();
            Random.InitState(Seed.GetHashCode());

            var originalPosition = gameObject.transform.position;
            var originalRotation = gameObject.transform.rotation;
            gameObject.transform.position = Vector3.zero;
            gameObject.transform.rotation = Quaternion.identity;
            PlaceTiles();
            PlaceGround();
            FillSurround();
            CollapseTileMeshes();
            MakeLevelObjectStatic();

            spawnPointSystemTransform.gameObject.GetComponent<SpawnPoints>()?.SetSpawnPoints();

            gameObject.transform.position = originalPosition;
            gameObject.transform.rotation = originalRotation;

            var numPlayableTilesWide = Layers * 2;

            // four tiles per groundLayer,
            // number of ground tiles is 2 * groundLayers
            // This value gives total tile-space including empty tiles around edge.
            var numTotalTilesWide = halfNumGroundLayers * 2 * mapBuilderSettings.TilesPerGroundLayer;

            Debug.Log("Finished building world (Expand for details...)\n" +
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

        private void PlaceTestTiles()
        {
            var numTotalTilesWide = halfNumGroundLayers * 2 * TilesPerGroundLayer;

            var tileIndex = 0;
            for (var i = 0; i < numTotalTilesWide * numTotalTilesWide; i++)
            {
                if (i >= levelTiles.Length)
                {
                    break;
                }

                PlaceTile(
                    new Vector2Int(i % numTotalTilesWide - numTotalTilesWide / 2 + 1,
                        i / numTotalTilesWide - numTotalTilesWide / 2 + 1), levelTiles[i], 0);
            }
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
            if (!TryLoadResource(GroundTilePath, out groundTile))
            {
                return false;
            }

            if (!TryLoadResource(GroundEdgePath, out groundEdge))
            {
                return false;
            }

            if (!TryLoadResource(SurroundPath, out surroundWall))
            {
                return false;
            }

            if (!TryLoadResource(CornerPath, out cornerPiece))
            {
                return false;
            }

            levelTiles = Resources.LoadAll<GameObject>(LevelTilePath);

            if (levelTiles.Length <= 0)
            {
                Debug.LogError($"Failed to load any resources at Resources/{LevelTilePath}");
                return false;
            }

            return true;
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
                offset += mapBuilderSettings.unitsPerGroundLayer *
                    .5f; // centre is half-distance across the ground layer
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

            for (var i = 0; i < 360; i += 90)
            {
                MakeCorner(i, cornerOffset);
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

            if (Application.isPlaying)
            {
                Object.Destroy(collision.GetComponent<MeshRenderer>());
                Object.Destroy(collision.GetComponent<MeshFilter>());
            }
            else
            {
                Object.DestroyImmediate(collision.GetComponent<MeshRenderer>());
                Object.DestroyImmediate(collision.GetComponent<MeshFilter>());
            }
        }

        private void MakeCorner(int angle, Vector3 cornerOffset)
        {
            var rotation = Quaternion.Euler(0, angle, 0);
            Object.Instantiate(cornerPiece,
                rotation * cornerOffset,
                rotation,
                surroundParentTransform);
        }

        private void PlaceTiles()
        {
            var blockers = GetComponentsInChildren<MapBuilderTileBlocker>();

            var tileCoord = new Vector2Int();
            var diff = new Vector2Int(0, -1);

            var tileCount = Math.Pow(2 * Layers, 2);


            // Tiles are built in a spiral manner from the centre outward to ensure increasing the # of tile layers doesn't
            // alter the existing tile types.
            for (var i = 0; i < tileCount; i++)
            {
                if (!TileCoordIsBlocked(tileCoord, blockers))
                {
                    // -layers < x <= layers AND -layers < y <= layers
                    if (-Layers < tileCoord.x && tileCoord.x <= Layers
                        && -Layers < tileCoord.y && tileCoord.y <= Layers)
                    {
                        PlaceTile(tileCoord);
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

        private bool TileCoordIsBlocked(Vector2Int tileCoord, MapBuilderTileBlocker[] blockers)
        {
            var worldPos = GetWorldPositionFromTileCoord(tileCoord);

            foreach (var blocker in blockers)
            {
                var top = blocker.transform.position.z + blocker.transform.localScale.z * .5f;
                var bot = blocker.transform.position.z - blocker.transform.localScale.z * .5f;
                var rht = blocker.transform.position.x + blocker.transform.localScale.x * .5f;
                var lft = blocker.transform.position.x - blocker.transform.localScale.x * .5f;

                if (worldPos.x >= lft && worldPos.x <= rht && worldPos.z >= bot && worldPos.z <= top)
                {
                    return true;
                }
            }

            return false;
        }

        private void PlaceTile(Vector2Int tileCoord)
        {
            GameObject tile = null;
        if(MapTileLookupTexture!=null){

            var textureCentre = Vector2Int.one * -1 +
                new Vector2Int(MapTileLookupTexture.width / 2, MapTileLookupTexture.height / 2);
            var texturePixelCoord = textureCentre + tileCoord;

            var pixel = MapTileLookupTexture.GetPixel(texturePixelCoord.x, texturePixelCoord.y);


            if (pixel == DefaultTileCollection.PixelRepresentationColor)
            {
                tile = DefaultTileCollection.GetRandomTile();
            }
            else
            {
                foreach (var tileCollection in ExtraTileCollections)
                {
                    if (pixel == tileCollection.PixelRepresentationColor)
                    {
                        tile = tileCollection.GetRandomTile();
                        break;
                    }
                }
            }
        }else{
            tile = DefaultTileCollection.GetRandomTile();
            if (Random.value < EmptyTileChance)
            {
                tile = null;
            }
        }


            if (tile == null)
            {
                //Debug.Log($"<b><color=#{ColorUtility.ToHtmlStringRGBA(pixel)}>{pixel}</color></b> did not match anything");
                return;
            }


            //var tile = levelTiles[Random.Range(0, levelTiles.Length)];
            float rotation = 90 * Random.Range(0, 4);

            PlaceTile(tileCoord, tile, rotation);
        }

        private void PlaceTile(Vector2Int tileCoord, GameObject tile, float rotation)
        {
            var tileOffset = mapBuilderSettings.UnitsPerTile / 2;

            Object.Instantiate(
                tile,
                new Vector3
                {
                    x = (tileCoord.x - 1) * mapBuilderSettings.UnitsPerTile + tileOffset,
                    z = (tileCoord.y - 1) * mapBuilderSettings.UnitsPerTile + tileOffset
                },
                new Quaternion
                {
                    y = rotation
                }
            };
            newTile.transform.parent = tileParentTransform.transform;
        }

        private Vector3 GetWorldPositionFromTileCoord(Vector2Int tileCoord)
        {
            return new Vector3
            {
                x = (tileCoord.x - 1) * UnitsPerTile + UnitsPerTile * .5f,
                z = (tileCoord.y - 1) * UnitsPerTile + UnitsPerTile * .5f
            };
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
        public static GameObject GenerateMap(
            MapBuilderSettings mapBuilderSettings,
            Transform workerTransform,
            Connection connection,
            string workerType,
            ILogDispatcher logDispatcher)
        {
            var levelInstance = new GameObject();
            var worldSize = GetWorldSizeFlag(connection);

            if (TryGetWorldLayerCount(mapBuilderSettings, worldSize, out var worldLayerCount))
            {
                levelInstance.name = $"FPS-Level_{worldLayerCount}({workerType})";
                levelInstance.transform.position = workerTransform.position;
                levelInstance.transform.rotation = workerTransform.rotation;

                var mapBuilder = new MapBuilder(mapBuilderSettings, levelInstance);
                mapBuilder.CleanAndBuild(worldLayerCount);
            }
            else
            {
                logDispatcher.HandleLog(LogType.Error,
                    new LogEvent("Invalid world_size worker flag. Make sure that it is either small or large,")
                        .WithField("world_size", worldSize));
            }

            return levelInstance;
        }
    }
}
