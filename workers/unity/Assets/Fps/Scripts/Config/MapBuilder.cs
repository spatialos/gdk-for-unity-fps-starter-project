using System;
using System.Collections;
using System.Collections.Generic;
using Improbable.Gdk.Core;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using Random = System.Random;

namespace Fps
{
    public class MapBuilder
    {
        private readonly MapTemplate mapTemplate;
        private readonly GameObject parentGameObject;

        private int layers;
        private Random random;
        private Scene tempScene;
        private Scene activeScene;

        private GameObject groundTile;
        private GameObject groundEdge;
        private GameObject wallStraight;
        private GameObject wallCorner;

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
        private const string WallStraightPath = "Prefabs/Level/Surround/Wall";
        private const string WallCornerPath = "Prefabs/Level/Surround/Corner";

        private int HalfNumGroundLayers => (layers - 1) / 4 + 1;
        private const int UnitsPerBlock = 4; // One textured square on the ground is a 'block'.
        private static int UnitsPerTile => 9 * UnitsPerBlock;

        private const int TilesPerGroundLayer = 4; // Ground layers are large quads that encompass 4x4 tiles.
        private const int BoundaryCollisionHeight = 16;

        private int UnitsPerGroundLayer => TilesPerGroundLayer * UnitsPerTile;

        public bool InvalidMapBuilder => parentGameObject == null;

        public MapBuilder(MapTemplate mapTemplate, GameObject parentGameObject)
        {
            this.mapTemplate = mapTemplate.Copy();
            this.parentGameObject = parentGameObject;
        }

        public IEnumerator CleanAndBuild(int worldLayers = 4, string seed = "SpatialOS GDK for Unity")
        {
            if (mapTemplate == null)
            {
                Debug.LogError("MapTemplate has not been set.");
                yield break;
            }

            layers = worldLayers;
            random = new Random(seed.GetHashCode());
            tempScene = SceneManager.CreateScene(parentGameObject.name, new CreateSceneParameters(LocalPhysicsMode.None));
            activeScene = SceneManager.GetActiveScene();

            if (!TryLoadResources())
            {
                Debug.LogError("Generation aborted (See previous message)");
                yield break;
            }

            Clean();

            InitializeGroupsAndComponents();

            // Initialize tiles
            SceneManager.SetActiveScene(tempScene);
            foreach (var tileCollection in mapTemplate.tileCollections)
            {
                tileCollection.LoadAndOptimizeTiles();
            }

            mapTemplate.defaultTileCollection.LoadAndOptimizeTiles();
            SceneManager.SetActiveScene(activeScene);

            yield return null;

            var originalPosition = parentGameObject.transform.position;
            var originalRotation = parentGameObject.transform.rotation;
            parentGameObject.transform.position = Vector3.zero;
            parentGameObject.transform.rotation = Quaternion.identity;

            yield return PlaceTiles();

            SceneManager.SetActiveScene(tempScene);

            PlaceGround();
            FillSurround();

            spawnPointSystemTransform.gameObject.GetComponent<SpawnPoints>()?.SetSpawnPoints();

            parentGameObject.transform.position = originalPosition;
            parentGameObject.transform.rotation = originalRotation;

            // Cleanup
            foreach (var tileCollection in mapTemplate.tileCollections)
            {
                tileCollection.Clear();
            }

            mapTemplate.defaultTileCollection.Clear();

            // Merge building temp scene into active scene
            SceneManager.SetActiveScene(activeScene);
            SceneManager.MergeScenes(tempScene, activeScene);
        }

        private void InitializeGroupsAndComponents()
        {
            if (parentGameObject.GetComponentInChildren<SpawnPoints>() == null)
            {
                spawnPointSystemTransform = MakeChildGroup(SpawnPointSystemName);
                spawnPointSystemTransform.gameObject.AddComponent<SpawnPoints>();
            }

            groundParentTransform = MakeChildGroup(GroundParentName);

            surroundParentTransform = MakeChildGroup(SurroundParentName);

            tileParentTransform = MakeChildGroup(TileParentName);
        }

        private bool TryLoadResources()
        {
            return TryLoadResource(GroundTilePath, out groundTile)
                && TryLoadResource(GroundEdgePath, out groundEdge)
                && TryLoadResource(WallStraightPath, out wallStraight)
                && TryLoadResource(WallCornerPath, out wallCorner);
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
            group.parent = parentGameObject.transform;
            group.localPosition = Vector3.zero;
            group.localRotation = Quaternion.identity;
            group.localScale = Vector3.one;
            return group;
        }

        private void FillSurround()
        {
            for (var groundLayerIndex = -HalfNumGroundLayers; groundLayerIndex < HalfNumGroundLayers; groundLayerIndex++)
            {
                float offset = groundLayerIndex * UnitsPerGroundLayer;
                offset += UnitsPerGroundLayer * .5f; // centre is half-distance across the ground layer

                MakeEdge(offset, 0);
                MakeEdge(offset, 90);
                MakeEdge(offset, 180);
                MakeEdge(offset, 270);
            }

            var cornerOffset =
                new Vector3
                {
                    x = HalfNumGroundLayers * -UnitsPerGroundLayer,
                    z = HalfNumGroundLayers * UnitsPerGroundLayer
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
                    HalfNumGroundLayers * UnitsPerGroundLayer + UnitsPerBlock * 0.25f),
                rotation * Quaternion.Euler(90, 0, 0),
                groundParentTransform);

            floor.transform.localScale = new Vector3(
                UnitsPerGroundLayer,
                UnitsPerBlock * .5f,
                1);

            var wall = Object.Instantiate(wallStraight,
                rotation * new Vector3(
                    offset,
                    UnitsPerBlock * .5f,
                    HalfNumGroundLayers * UnitsPerGroundLayer + UnitsPerBlock * .5f),
                rotation,
                surroundParentTransform);

            wall.transform.localScale = new Vector3(
                UnitsPerGroundLayer,
                UnitsPerBlock,
                1);

            var wallFloor = Object.Instantiate(groundEdge,
                rotation * new Vector3(
                    offset,
                    UnitsPerBlock,
                    HalfNumGroundLayers * UnitsPerGroundLayer + UnitsPerBlock),
                rotation * Quaternion.Euler(90, 0, 0),
                surroundParentTransform);

            wallFloor.transform.localScale = new Vector3(
                UnitsPerGroundLayer,
                UnitsPerBlock,
                1);

            // Collision
            var collision = Object.Instantiate(wallStraight,
                rotation * new Vector3(
                    offset,
                    UnitsPerBlock + BoundaryCollisionHeight * .5f,
                    HalfNumGroundLayers * UnitsPerGroundLayer +
                    UnitsPerBlock * .5f),
                rotation,
                surroundParentTransform);

            collision.transform.localScale =
                new Vector3(
                    UnitsPerGroundLayer +
                    UnitsPerBlock, // Collisions overlap to fill corners
                    BoundaryCollisionHeight,
                    1);

            collision.name = "Collision";

            UnityObjectDestroyer.Destroy(collision.GetComponent<MeshRenderer>());
            UnityObjectDestroyer.Destroy(collision.GetComponent<MeshFilter>());
        }

        private void MakeCorner(int angle, Vector3 cornerOffset)
        {
            var rotation = Quaternion.Euler(0, angle, 0);
            Object.Instantiate(wallCorner,
                rotation * cornerOffset,
                rotation,
                surroundParentTransform);
        }

        private IEnumerator PlaceTiles()
        {
            var tileCoord = new Vector2Int(0, 0);
            var diff = new Vector2Int(0, -1);

            var tileCount = Math.Pow(2 * layers, 2);

            var targetFramerate = 10.0f;
            var timeLimit = TimeSpan.FromSeconds(1.0f / targetFramerate);
            var timeStart = DateTime.UtcNow;

            // Tiles are built in a spiral manner from the centre outward to ensure increasing the # of tile layers doesn't
            // alter the existing tile types.
            SceneManager.SetActiveScene(tempScene);
            for (var i = 0; i < tileCount; i++)
            {
                // -layers < x <= layers AND -layers < y <= layers
                if (-layers < tileCoord.x && tileCoord.x <= layers
                    && -layers < tileCoord.y && tileCoord.y <= layers)
                {
                    PlaceTemplatedTile(tileCoord);
                }

                if (tileCoord.x == tileCoord.y ||
                    tileCoord.x < 0 && tileCoord.x == -tileCoord.y ||
                    tileCoord.x > 0 && tileCoord.x == 1 - tileCoord.y)
                {
                    diff = new Vector2Int(-diff.y, diff.x);
                }

                tileCoord += diff;

                if (DateTime.UtcNow.Subtract(timeStart) >= timeLimit)
                {
                    SceneManager.SetActiveScene(activeScene);
                    yield return null;
                    timeStart = DateTime.UtcNow;
                    SceneManager.SetActiveScene(tempScene);
                }
            }

            SceneManager.SetActiveScene(activeScene);

            tileParentTransform.position = new Vector3
            {
                x = tileParentTransform.position.x,
                z = tileParentTransform.position.z
            };
        }

        private void PlaceTemplatedTile(Vector2Int tileCoord)
        {
            var tile = mapTemplate.GetTileForLocation(new Vector2(tileCoord.x, tileCoord.y), random);

            if (tile == null)
            {
                return;
            }

            float rotation = 90 * random.Next(0, 4);

            PlaceTile(tileCoord, tile, rotation);
        }

        private Vector3 GetWorldLocationFromCoordinate(Vector2Int coordinate)
        {
            var tileOffset = UnitsPerTile / 2;
            return new Vector3
            {
                x = (coordinate.x - 1) * UnitsPerTile + tileOffset,
                z = (coordinate.y - 1) * UnitsPerTile + tileOffset
            };
        }

        private void PlaceTile(Vector2Int tileCoord, GameObject tile, float rotation)
        {
            var tileOffset = UnitsPerTile / 2;

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
            for (var x = -HalfNumGroundLayers; x < HalfNumGroundLayers; x++)
            {
                for (var z = -HalfNumGroundLayers; z < HalfNumGroundLayers; z++)
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
                    x = groundX * UnitsPerGroundLayer + UnitsPerGroundLayer * .5f,
                    z = groundZ * UnitsPerGroundLayer + UnitsPerGroundLayer * .5f
                },
                Quaternion.identity,
                groundParentTransform.transform);
        }

        public void Clean()
        {
            if (parentGameObject == null)
            {
                return;
            }

            var childrenToDestroy = new Queue<GameObject>();

            foreach (var child in parentGameObject.GetComponentsInChildren<Transform>())
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

        public static Vector3 GetWorldDimensions(int layers)
        {
            var dimensions = layers * UnitsPerTile * 2 + UnitsPerBlock;
            return new Vector3(dimensions, 100f, dimensions);
        }

        // Get the world size from the config, and use it to generate the correct-sized level
        public static IEnumerator GenerateMap(
            MapTemplate mapTemplate,
            int mapSize,
            Transform parentTransform,
            string workerType,
            WorkerConnectorBase worker)
        {
            var levelInstance = new GameObject($"FPS-Level_{mapSize}({workerType})");
            levelInstance.transform.position = parentTransform.position;
            levelInstance.transform.rotation = parentTransform.rotation;

            var mapBuilder = new MapBuilder(mapTemplate, levelInstance);

            yield return mapBuilder.CleanAndBuild(mapSize);

            worker.LevelInstance = levelInstance;
        }
    }
}
