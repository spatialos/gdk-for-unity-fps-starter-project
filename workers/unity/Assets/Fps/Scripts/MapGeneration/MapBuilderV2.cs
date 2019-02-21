using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace Fps
{
    [ExecuteInEditMode]
    public class MapBuilderV2 : MonoBehaviour
    {
        public int Layers = 3;

        [Tooltip("Enter \"Test\" to place 1 of each tile type down")]
        public string Seed = "SpatialOS GDK for Unity";

        public float EmptyTileChance = 0.2f;

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
        // Store the half-value as many calculations are simplified by going from -halfNumGroundLayers to halfNumGroundLayers.
        private int halfNumGroundLayers => (Layers - 1) / TilesPerGroundLayer + 1;
        private int unitsPerGroundLayer => TilesPerGroundLayer * UnitsPerTile;

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

        public void CleanAndBuild()
        {
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

            var originalPosition = transform.position;
            var originalRotation = transform.rotation;
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;


            if (Seed == "Test")
            {
                PlaceTestTiles();
            }
            else
            {
                PlaceTiles();
            }

            PlaceGround();
            FillSurround();
            MakeLevelObjectStatic();

            transform.position = originalPosition;
            transform.rotation = originalRotation;

            var numPlayableTilesWide = Layers * 2;

            // four tiles per groundLayer,
            // number of ground tiles is 2 * groundLayers
            // This value gives total tile-space including empty tiles around edge.
            var numTotalTilesWide = halfNumGroundLayers * 2 * TilesPerGroundLayer;

            Debug.Log("Finished building world (Expand for details...)\n" +
                "\n\tPlayable space" +
                $"\n\t\t{numPlayableTilesWide}x{numPlayableTilesWide} tiles" +
                $"\n\t\t{numPlayableTilesWide * UnitsPerTile + UnitsPerBlock}x{numPlayableTilesWide * UnitsPerTile + UnitsPerBlock} units" +
                "\n\tTOTAL space" +
                $"\n\t\t{numTotalTilesWide}x{numTotalTilesWide} tiles" +
                $"\n\t\t{numTotalTilesWide * UnitsPerTile + UnitsPerBlock}x{numTotalTilesWide * UnitsPerTile + UnitsPerBlock} units\n");
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
            if (GetComponentInChildren<SpawnPoints>() == null)
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
            group.parent = transform;
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
                float offset = groundLayerIndex * unitsPerGroundLayer;
                offset += unitsPerGroundLayer * .5f; // centre is half-distance across the ground layer
                MakeEdge(offset, 0);
                MakeEdge(offset, 90);
                MakeEdge(offset, 180);
                MakeEdge(offset, 270);
            }

            var cornerOffset =
                new Vector3
                {
                    x = halfNumGroundLayers * -unitsPerGroundLayer,
                    z = halfNumGroundLayers * unitsPerGroundLayer
                };

            for (var i = 0; i < 360; i += 90)
            {
                MakeCorner(i, cornerOffset);
            }
        }

        private void MakeEdge(float offset, int angle)
        {
            var rotation = Quaternion.Euler(0, angle, 0);

            var floor = (GameObject) PrefabUtility.InstantiatePrefab(groundEdge);
            floor.transform.position =
                rotation * new Vector3(
                    offset,
                    0,
                    halfNumGroundLayers * unitsPerGroundLayer + UnitsPerBlock * 0.25f);
            floor.transform.rotation = rotation * Quaternion.Euler(90, 0, 0);
            floor.transform.parent = groundParentTransform;
            floor.transform.localScale = new Vector3(
                unitsPerGroundLayer,
                UnitsPerBlock * .5f,
                1);

            var wall = (GameObject) PrefabUtility.InstantiatePrefab(surroundWall);
            wall.transform.position = rotation * new Vector3(offset,
                UnitsPerBlock * .5f,
                halfNumGroundLayers * unitsPerGroundLayer + UnitsPerBlock * .5f);
            wall.transform.rotation = rotation;
            wall.transform.parent = surroundParentTransform;
            wall.transform.localScale = new Vector3(
                unitsPerGroundLayer,
                UnitsPerBlock,
                1);

            var wallFloor = (GameObject) PrefabUtility.InstantiatePrefab(groundEdge);
            wallFloor.transform.position = rotation * new Vector3(
                offset,
                UnitsPerBlock,
                halfNumGroundLayers * unitsPerGroundLayer + UnitsPerBlock);
            wallFloor.transform.rotation = rotation * Quaternion.Euler(90, 0, 0);
            wallFloor.transform.parent = surroundParentTransform;
            wallFloor.transform.localScale = new Vector3(
                unitsPerGroundLayer,
                UnitsPerBlock,
                1);

            // Collision
            var collision = (GameObject) PrefabUtility.InstantiatePrefab(surroundWall);
            collision.transform.position = rotation * new Vector3(
                offset,
                UnitsPerBlock + BoundaryCollisionHeight * .5f,
                halfNumGroundLayers * unitsPerGroundLayer + UnitsPerBlock * .5f);
            collision.transform.rotation = rotation;
            collision.transform.parent = surroundParentTransform;
            collision.transform.localScale =
                new Vector3(
                    unitsPerGroundLayer + UnitsPerBlock, // Collisions overlap to fill corners
                    BoundaryCollisionHeight,
                    1);
            collision.gameObject.name = "Collision";

            if (Application.isPlaying)
            {
                Destroy(collision.GetComponent<MeshRenderer>());
                Destroy(collision.GetComponent<MeshFilter>());
            }
            else
            {
                DestroyImmediate(collision.GetComponent<MeshRenderer>());
                DestroyImmediate(collision.GetComponent<MeshFilter>());
            }
        }

        private void MakeCorner(int angle, Vector3 cornerOffset)
        {
            var rotation = Quaternion.Euler(0, angle, 0);
            var corner = (GameObject) PrefabUtility.InstantiatePrefab(cornerPiece);
            corner.transform.position = rotation * cornerOffset;
            corner.transform.rotation = rotation;
            corner.transform.parent = surroundParentTransform;
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
            var newTile = (GameObject) UnityEditor.PrefabUtility.InstantiatePrefab(tile);
            newTile.transform.position = GetWorldPositionFromTileCoord(tileCoord);
            newTile.transform.rotation = new Quaternion
            {
                eulerAngles = new Vector3
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
            var ground = (GameObject) PrefabUtility.InstantiatePrefab(groundTile);
            ground.transform.position = new Vector3
            {
                x = groundX * unitsPerGroundLayer + unitsPerGroundLayer * .5f,
                z = groundZ * unitsPerGroundLayer + unitsPerGroundLayer * .5f
            };
            ground.transform.rotation = Quaternion.identity;
            ground.transform.parent = groundParentTransform.transform;
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
                    continue;
                }
            }

            while (childrenToDestroy.Count != 0)
            {
                DestroyImmediate(childrenToDestroy.Dequeue());
            }
        }
#endif
    }
}
