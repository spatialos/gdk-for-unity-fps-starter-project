using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[ExecuteInEditMode]
public class MapBuilder : MonoBehaviour
{
    public int layers = 3;
    public string seed = "SpatialOS GDK for Unity";
    public float emptyTileChance = 0.45f;
    public float cubeDensity = 0.15f;

    private int tileSeparation = 36;
    private float groundHeight = 0f;
    private float heightOffset = 0.5f;

    private int groundWidth;
    private int desertWidth;
    private int wallHeight = 4;
    private int cubeHeight = 4;

    private GameObject[] centreTiles;
    private GameObject[] levelTiles;
    private GameObject groundTile;
    private GameObject groundEdge;
    private GameObject desertPlane;
    private GameObject desertWall;
    private GameObject desertCube;

    private Transform tileParentTransform;
    private Transform groundParentTransform;
    private Transform desertParentTransform;
    private Transform cubeParentTransform;

    private const string TileParentName = "TileParent";
    private const string GroundParentName = "GroundParent";
    private const string DesertParentName = "DesertParent";
    private const string CubeParentName = "CubeParent";

    private const string LevelTilePath = "Prefabs/Level/Tiles";
    private const string GroundTilePath = "Prefabs/Level/Ground/Ground4x4";
    private const string GroundEdgePath = "Prefabs/Level/Ground/Edge";
    private const string DesertPlanePath = "Prefabs/Level/Desert/Plane";
    private const string DesertWallPath = "Prefabs/Level/Desert/Wall";
    private const string DesertCubePath = "Prefabs/Level/Desert/Cube";

    //central tiles hardcoded as per AAA's request
    private const string CentreTile0 = "Prefabs/Level/Tiles/Centre0";
    private const string CentreTile1 = "Prefabs/Level/Tiles/Centre1";
    private const string CentreTile2 = "Prefabs/Level/Tiles/Centre2";
    private const string CentreTile3 = "Prefabs/Level/Tiles/Centre3";

#if UNITY_EDITOR
    public void Build()
    {
        Random.InitState(seed.GetHashCode());
        PlaceTiles();
        PlaceGround();
        PlaceDesert();
        MakeLevelObjectStatic();
        Debug.Log($"Finished building world of size: {groundWidth} x {groundWidth}");
    }

    private void PlaceTiles()
    {
        centreTiles = new GameObject[]
        {
            Resources.Load<GameObject>(CentreTile0),
            Resources.Load<GameObject>(CentreTile1),
            Resources.Load<GameObject>(CentreTile2),
            Resources.Load<GameObject>(CentreTile3)
        };

        if (centreTiles.Any(t => t == null))
        {
            Debug.LogError("Central tile prefabs not loaded.");
            return;
        }

        levelTiles = Resources.LoadAll<GameObject>(LevelTilePath);

        if (levelTiles.Length <= 0)
        {
            Debug.LogError("Tile prefabs not loaded.");
            return;
        }

        tileParentTransform = new GameObject(TileParentName).transform;
        tileParentTransform.parent = gameObject.transform;

        var tileCoord = new Vector2Int();
        var diff = new Vector2Int(0, -1);

        var tileCount = Math.Pow(2 * layers, 2);

        for (var i = 0; i < tileCount; i++)
        {
            // -layers < x <= layers AND -layers < y <= layers
            if (-layers < tileCoord.x && tileCoord.x <= layers && -layers < tileCoord.y && tileCoord.y <= layers)
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

        tileParentTransform.position = new Vector3()
        {
            x = tileParentTransform.position.x,
            y = heightOffset,
            z = tileParentTransform.position.z
        };
    }

    private void PlaceTile(Vector2Int tileCoord)
    {
        if (Random.value < emptyTileChance)
        {
            return;
        }

        var tile = levelTiles[Random.Range(0, levelTiles.Length)];
        float rotation = 90 * Random.Range(0, 4);

        PlaceTile(tileCoord, tile, rotation);
    }

    private void PlaceTile(Vector2Int tileCoord, GameObject tile, float rotation)
    {
        var tileOffset = tileSeparation / 2;

        Instantiate(
            tile,
            new Vector3()
            {
                x = (tileCoord.x - 1) * tileSeparation + tileOffset,
                z = (tileCoord.y - 1) * tileSeparation + tileOffset
            },
            new Quaternion()
            {
                eulerAngles = new Vector3()
                {
                    y = rotation
                }
            },
            tileParentTransform.transform);
    }

    private void PlaceGround()
    {
        groundTile = Resources.Load<GameObject>(GroundTilePath);
        if (groundTile == null)
        {
            Debug.LogError("Ground prefab not loaded.");
            return;
        }

        groundEdge = Resources.Load<GameObject>(GroundEdgePath);
        if (groundEdge == null)
        {
            Debug.LogError("Ground edge prefab not loaded.");
            return;
        }

        groundParentTransform = new GameObject(GroundParentName).transform;
        groundParentTransform.parent = gameObject.transform;

        var groundLayers = (layers - 1) / 4 + 1;

        for (var x = -groundLayers; x < groundLayers; x++)
        {
            for (var z = -groundLayers; z < groundLayers; z++)
            {
                PlaceGroundTile(x, z);
            }
        }

        PlaceEdges(groundLayers);

        groundParentTransform.position = new Vector3()
        {
            x = groundParentTransform.position.x,
            y = heightOffset,
            z = groundParentTransform.position.z
        };
    }

    private void PlaceGroundTile(int groundX, int groundZ)
    {
        Instantiate(
            groundTile,
            new Vector3()
            {
                x = 144 * groundX + 74,
                y = groundHeight,
                z = 144 * groundZ + 70
            },
            Quaternion.identity,
            groundParentTransform.transform);
    }

    private void PlaceEdges(int groundLayers)
    {
        var edgeSize = 0.4f;
        groundWidth = groundLayers * 288 + 4;
        var edgeScale = new Vector3()
        {
            x = groundWidth / 10f,
            y = edgeSize,
            z = edgeSize
        };

        //top edge
        var topEdgeObject = Instantiate(
            groundEdge,
            new Vector3()
            {
                y = groundHeight,
                z = groundLayers * 144
            },
            Quaternion.identity,
            groundParentTransform.transform);
        topEdgeObject.transform.localScale = edgeScale;

        //left edge
        var leftEdgeObject = Instantiate(
            groundEdge,
            new Vector3()
            {
                x = -groundLayers * 144,
                y = groundHeight
            },
            new Quaternion()
            {
                eulerAngles = new Vector3()
                {
                    y = -90f
                }
            },
            groundParentTransform.transform);
        leftEdgeObject.transform.localScale = edgeScale;
    }

    private void PlaceDesert()
    {
        // desertPlane = Resources.Load<GameObject>(DesertPlanePath);
        // if (desertPlane == null)
        // {
        //     Debug.LogError("Desert plane prefab not loaded.");
        //     return;
        // }

        desertParentTransform = new GameObject(DesertParentName).transform;
        desertParentTransform.parent = gameObject.transform;

        // desertWidth = (groundWidth / 200 + 2) * 200;
        // desertWidth = (int) (4f * Mathf.Round(desertWidth / 4f));
        // desertWidth -= 4;
        //
        // var desertPlaneObject = Instantiate(
        //     desertPlane,
        //     new Vector3(),
        //     new Quaternion()
        //     {
        //         eulerAngles = new Vector3()
        //         {
        //             x = 90
        //         }
        //     },
        //     desertParentTransform.transform);
        // desertPlaneObject.transform.localScale = new Vector3()
        // {
        //     x = desertWidth,
        //     y = desertWidth,
        //     z = 1
        // };

        desertWall = Resources.Load<GameObject>(DesertWallPath);
        if (desertWall == null)
        {
            Debug.LogError("Desert wall prefab not loaded.");
            return;
        }

        desertWidth = groundWidth;
        var halfDesertWidth = desertWidth / 2;

        //top
        PlaceWall(0, halfDesertWidth, 180f);

        //bottom
        PlaceWall(0, -halfDesertWidth, 0f);

        //left
        PlaceWall(-halfDesertWidth, 0, 90f);

        //right
        PlaceWall(halfDesertWidth, 0, -90f);

        // desertCube = Resources.Load<GameObject>(DesertCubePath);
        // if (desertCube == null)
        // {
        //     Debug.LogError("Desert cube prefab not loaded.");
        //     return;
        // }
        //
        // cubeParentTransform = new GameObject(CubeParentName).transform;
        // cubeParentTransform.parent = gameObject.transform;
        //
        // var numCubes = (desertWidth * desertWidth - groundWidth * groundWidth) / 1000 * cubeDensity;
        // for (var i = 0; i < numCubes; i++)
        // {
        //     PlaceCube();
        // }
    }

    private void PlaceWall(int wallX, int wallZ, float rotation)
    {
        var wallScale = new Vector3()
        {
            x = wallHeight,
            y = desertWidth,
            z = 1
        };

        var desertWallObjectVisible = Instantiate(
            desertWall,
            new Vector3()
            {
                x = wallX,
                y = wallHeight / 2,
                z = wallZ
            },
            new Quaternion()
            {
                eulerAngles = new Vector3()
                {
                    x = 180,
                    y = rotation,
                    z = -90
                }
            },
            desertParentTransform.transform);
        desertWallObjectVisible.transform.localScale = wallScale;

        var desertWallObjectInvisible = Instantiate(
            desertWall,
            new Vector3()
            {
                x = wallX,
                y = wallHeight * 3 / 2,
                z = wallZ
            },
            new Quaternion()
            {
                eulerAngles = new Vector3()
                {
                    x = 180,
                    y = rotation,
                    z = -90
                }
            },
            desertParentTransform.transform);
        desertWallObjectInvisible.transform.localScale = wallScale;
        desertWallObjectInvisible.GetComponent<Renderer>().enabled = false;
    }

    private void PlaceCube()
    {
        var cubeX = Random.Range(-desertWidth / 2, desertWidth / 2);
        var cubeZ = Random.Range(-desertWidth / 2, desertWidth / 2);

        if (Math.Abs(cubeX) < groundWidth / 2 && Math.Abs(cubeZ) < groundWidth / 2)
        {
            return;
        }

        Instantiate(
            desertCube,
            new Vector3()
            {
                x = 4f * Mathf.Round(cubeX / 4f),
                y = cubeHeight,
                z = 4f * Mathf.Round(cubeZ / 4f)
            },
            Quaternion.identity,
            cubeParentTransform.transform);
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

            if (child.gameObject.name.Contains(DesertParentName))
            {
                childrenToDestroy.Enqueue(child.gameObject);
                continue;
            }

            if (child.gameObject.name.Contains(CubeParentName))
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
