using System;
using System.Collections.Generic;
using System.Linq;
using Fps;
using UnityEngine;
using Random = UnityEngine.Random;

[ExecuteInEditMode]
public class MapBuilder : MonoBehaviour
{
    public int Layers = 3;
    public string Seed = "SpatialOS GDK for Unity";
    public float EmptyTileChance = 0.2f;

    private const int TileSeparation = 36;

    private int groundWidth;
    private const int BlockSize = 4;

    private GameObject[] centreTiles;
    private GameObject[] levelTiles;
    private GameObject groundTile;
    private GameObject groundEdge;
    private GameObject surroundWall;
    private GameObject cornerPiece;

    private Transform tileParentTransform;
    private Transform groundParentTransform;
    private Transform surroundParentTransform;
    private Transform cubeParentTransform;
    private Transform spawnPointSystemTransform;

    private const string TileParentName = "TileParent";
    private const string GroundParentName = "GroundParent";
    private const string SurroundParentName = "SurroundParent";
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

    private int groundLayers => (Layers - 1) / 4 + 1;

#if UNITY_EDITOR
    public void CleanAndBuild()
    {
        if (TryLoadResources() == false)
        {
            Debug.LogError("Generation aborted due to error");
            return;
        }

        Clean();

        InitializeGroupsAndComponents();
        Random.InitState(Seed.GetHashCode());

        var originalPosition = transform.position;
        var originalRotation = transform.rotation;
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        PlaceTiles();
        PlaceGround();
        FillSurround();
        MakeLevelObjectStatic();

        transform.position = originalPosition;
        transform.rotation = originalRotation;

        var worldSize = groundLayers * TileSeparation * 4 * 2 + 4;
        Debug.Log($"Finished building world of size: {worldSize} x {worldSize}");
    }

    private void InitializeGroupsAndComponents()
    {
        if (!gameObject.GetComponent<TileSettings>())
        {
            gameObject.AddComponent<TileSettings>();
        }

        if (!GetComponentInChildren<SpawnPoints>())
        {
            spawnPointSystemTransform = MakeGroup(SpawnPointSystemName);
            spawnPointSystemTransform.gameObject.AddComponent<SpawnPoints>();
        }

        groundParentTransform = MakeGroup(GroundParentName);

        surroundParentTransform = MakeGroup(SurroundParentName);

        tileParentTransform = MakeGroup(TileParentName);
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

        centreTiles = new[]
        {
            Resources.Load<GameObject>(CentreTile0),
            Resources.Load<GameObject>(CentreTile1),
            Resources.Load<GameObject>(CentreTile2),
            Resources.Load<GameObject>(CentreTile3)
        };

        if (centreTiles.Any(t => t == null))
        {
            Debug.LogError("Failed to load CentreTile resource (Expecting all the following paths to exist: " +
                $"\t{CentreTile0}" +
                $"\t{CentreTile1}" +
                $"\t{CentreTile2}" +
                $"\t{CentreTile3}");
            return false;
        }

        levelTiles = Resources.LoadAll<GameObject>(LevelTilePath);

        if (levelTiles.Length <= 0)
        {
            Debug.LogError($"Failed to load resource at {LevelTilePath}");
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

        Debug.LogError($"Failed to load resource at {resourcePath}");
        return false;
    }

    private Transform MakeGroup(string groupName)
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
        for (var i = -groundLayers; i < groundLayers; i++)
        {
            var offset = TileSeparation * 4 * i + TileSeparation * 2;
            MakeEdge(offset, 0);
            MakeEdge(offset, 90);
            MakeEdge(offset, 180);
            MakeEdge(offset, 270);
        }

        for (var i = 0; i < 360; i += 90)
        {
            MakeCorner(i);
        }
    }

    private void MakeEdge(float offset, int angle)
    {
        var rotation = Quaternion.Euler(0, angle, 0);

        var floor = Instantiate(groundEdge,
            rotation * new Vector3(offset, 0, groundLayers * TileSeparation * 4 + BlockSize * 0.25f),
            rotation * Quaternion.Euler(90, 0, 0),
            groundParentTransform);
        floor.transform.localScale = new Vector3(144, 2, 1);

        var wall = Instantiate(surroundWall,
            rotation * new Vector3(offset, BlockSize * .5f, groundLayers * TileSeparation * 4 + BlockSize * .5f),
            rotation,
            surroundParentTransform);
        wall.transform.localScale = new Vector3(144, 4, 1);

        var wallFloor = Instantiate(groundEdge,
            rotation * new Vector3(offset, BlockSize, groundLayers * TileSeparation * 4 + BlockSize),
            rotation * Quaternion.Euler(90, 0, 0),
            surroundParentTransform);
        wallFloor.transform.localScale = new Vector3(144, 4, 1);

        // Collision
        var collisionHeight = 16;
        var collision = Instantiate(surroundWall,
            rotation * new Vector3(offset, BlockSize + collisionHeight * .5f,
                groundLayers * TileSeparation * 4 + BlockSize * .5f),
            rotation,
            surroundParentTransform);
        collision.transform.localScale = new Vector3(144 + 4, collisionHeight, 1); // Collisions overlap to fill corners
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

    private void MakeCorner(int angle)
    {
        var rotation = Quaternion.Euler(0, angle, 0);
        Instantiate(cornerPiece,
            rotation * new Vector3(-groundLayers * TileSeparation * 4, 0, groundLayers * TileSeparation * 4),
            rotation,
            surroundParentTransform);
    }

    private void PlaceTiles()
    {
        var tileCoord = new Vector2Int();
        var diff = new Vector2Int(0, -1);

        var tileCount = Math.Pow(2 * Layers, 2);

        for (var i = 0; i < tileCount; i++)
        {
            // -layers < x <= layers AND -layers < y <= layers
            if (-Layers < tileCoord.x && tileCoord.x <= Layers
                && -Layers < tileCoord.y && tileCoord.y <= Layers)
            {
                if (i < 4)
                {
                    PlaceTile(tileCoord, centreTiles[i], 0);
                }
                else
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

    private void PlaceTile(Vector2Int tileCoord)
    {
        if (Random.value < EmptyTileChance)
        {
            return;
        }

        var tile = levelTiles[Random.Range(0, levelTiles.Length)];
        float rotation = 90 * Random.Range(0, 4);

        PlaceTile(tileCoord, tile, rotation);
    }

    private void PlaceTile(Vector2Int tileCoord, GameObject tile, float rotation)
    {
        var tileOffset = TileSeparation / 2;

        Instantiate(
            tile,
            new Vector3
            {
                x = (tileCoord.x - 1) * TileSeparation + tileOffset,
                z = (tileCoord.y - 1) * TileSeparation + tileOffset
            },
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
        for (var x = -groundLayers; x < groundLayers; x++)
        {
            for (var z = -groundLayers; z < groundLayers; z++)
            {
                PlaceGroundTile(x, z);
            }
        }
    }

    private void PlaceGroundTile(int groundX, int groundZ)
    {
        Instantiate(
            groundTile,
            new Vector3
            {
                x = groundX * 144 + 72,
                z = groundZ * 144 + 72
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
