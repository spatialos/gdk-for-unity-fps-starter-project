using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class ObjectPlacer : EditorWindow
{
    private int[] _storedSelection;
    private readonly List<PaletteItem> _palette = new List<PaletteItem>();
    private PlacerObjectSetInstance _objectSet;
    private bool _paused;

    private float _paintDelay = 0.1f;
    private float _timeSinceLastPaint = -1f;
    private bool _painting;


    [MenuItem("ImprobaTools/ObjectPlacer")]
    public static void CollisionSpinklerMenu()
    {
        var w = GetWindow<ObjectPlacer>(true);
        var r = w.position;
        r.x = -111111; // Hide the window off-screen
        r.y = -111111;
        w.position = r;
    }

    void Awake()
    {
        StoreSelection();
        Selection.activeObject = null;
    }

    void OnDestroy()
    {
        RetrieveSelection();
    }

    void OnEnable()
    {
        SceneView.onSceneGUIDelegate += OnSceneGUI;
    }

    void OnDisable()
    {
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
    }
    void StoreSelection()
    {
        var selected = Selection.gameObjects;
        _storedSelection = new int[selected.Length];
        _palette.Clear();
        for (var i = 0; i < selected.Length; i++)
        {
            _palette.Add(new PaletteItem(selected[i]));
            _storedSelection[i] = selected[i].GetInstanceID();
        }
    }

    void RetrieveSelection()
    {
        Selection.instanceIDs = _storedSelection;
    }


    void OnSceneGUI(SceneView view)
    {
        DoSceneGUIInterface();


        if (_paused)
        {
            return;
        }

        // Lock normal mouse tools
        if (Event.current.type == EventType.Layout)
        {
            HandleUtility.AddDefaultControl(0);
        }

        if (!_painting && Event.current.type == EventType.MouseDown && Event.current.button == 0 && !Event.current.alt)
        {
            StartPainting();
        }

        if (_painting && (Event.current.type == EventType.MouseUp || Event.current.type == EventType.MouseLeaveWindow || Event.current.alt))
        {
            StopPainting();
        }

        if (_painting)
        {
            _timeSinceLastPaint += 1f / 60f;

            if (_timeSinceLastPaint >= _paintDelay)
            {
                _timeSinceLastPaint = 0;
                TryPaintObject();
            }
        }
    }

    private void DoSceneGUIInterface()
    {
        Handles.BeginGUI();
        GUILayout.BeginArea(new Rect(20, 20, 300, 120));
        var rect = EditorGUILayout.BeginVertical();

        var defaultBgColor = GUI.backgroundColor;
        GUI.backgroundColor = _objectSet == null ? Color.red : Color.green;
        GUI.Box(rect, GUIContent.none);
        GUI.Box(rect, GUIContent.none);
        GUI.Box(rect, GUIContent.none);
        GUI.backgroundColor = defaultBgColor;
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Object Placement Mode");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        // Let selection of Object Placer Sets in the project view change the current set
        var activeObj = EditorUtility.InstanceIDToObject(Selection.activeInstanceID);
        if (!_paused && activeObj && activeObj.GetType() == typeof(PlacerObjectSetInstance))
        {
            _objectSet = activeObj as PlacerObjectSetInstance;
        }

        _objectSet = (PlacerObjectSetInstance)EditorGUILayout.ObjectField(_objectSet,
            typeof(PlacerObjectSetInstance),
            true);

        if (!_paused && _objectSet != null)
        {
            Selection.activeInstanceID = _objectSet.GetInstanceID();
        }

        _paintDelay = EditorGUILayout.Slider("Paint delay", _paintDelay, 0.1f, 1f);


        GUI.backgroundColor = _paused ? Color.grey : defaultBgColor;
        if (GUILayout.Button(_paused ? "Unpause" : "Pause"))
        {
            _paused = !_paused;
        }
        GUI.backgroundColor = defaultBgColor;

        if (GUILayout.Button("Close"))
        {
            Close();
        }

        EditorGUILayout.EndVertical();
        GUILayout.EndArea();
        Handles.EndGUI();
    }

    void StartPainting()
    {
        _painting = true;
        TryPaintObject();
        _timeSinceLastPaint = 0;
    }

    void StopPainting()
    {
        _painting = false;
    }

    void TryPaintObject()
    {
        var screenPos = new Vector2
        {
            x = Event.current.mousePosition.x,
            y = Camera.current.pixelHeight - Event.current.mousePosition.y
        };

        var worldRay = SceneView.currentDrawingSceneView.camera.ScreenPointToRay(screenPos);
        RaycastHit hit;
        if (Physics.Raycast(worldRay, out hit, SceneView.currentDrawingSceneView.camera.farClipPlane))
        {
            SpawnObject(hit);
        }
    }

    // Candidate for toolkit static method
    private static UnityEngine.Object GetPrefabObject(GameObject go)
    {
        var prefabAsset = PrefabUtility.GetCorrespondingObjectFromSource(go) ??
                          PrefabUtility.FindPrefabRoot(go);
        if (string.IsNullOrEmpty(AssetDatabase.GetAssetPath(prefabAsset)))
        {
            prefabAsset = null;
        }

        return prefabAsset;
    }

    float GetSlope(Vector3 normal)
    {
        return Vector3.Angle(Vector3.up, normal);
    }

    private GameObject SpawnObject(RaycastHit hitData)
    {
        if (_objectSet != null && _objectSet.Content.Objects.Count > 0)
        {
            var slope = GetSlope(hitData.normal);
            if (slope != Mathf.Clamp(slope, _objectSet.Content.ValidSlopeMin,
                    _objectSet.Content.ValidSlopeMax))
            {
                return null;
            }
            GameObject newObj;
            // Get random item form object set
            var picked = new PaletteItem(
                _objectSet.Content.Objects[Random.Range(0, _objectSet.Content.Objects.Count)]);
            var groupName = _objectSet.name;
            // Create prefab or non-prefab object
            if (picked.HasPrefab)
            {
                newObj = PrefabUtility.InstantiatePrefab(picked.PrefabAsset) as GameObject;
                if (newObj == null)
                {
                    Debug.LogError("Failed to create object from " + picked.PrefabAsset + " with path " + AssetDatabase.GetAssetPath(picked.PrefabAsset));
                    return null;
                }
                newObj.transform.position = hitData.point;
                newObj.transform.parent = GameObject.Find(groupName)?.transform ?? new GameObject(groupName).transform;
            }
            else
            {
                newObj = Instantiate(picked.GameObject, hitData.point, Quaternion.identity, GameObject.Find(groupName)?.transform ?? new GameObject(groupName).transform);
            }
            UnityEditor.Undo.RegisterCreatedObjectUndo(newObj, "Undo Object Placement");

            // Apply translate/rotate settings

            newObj.transform.Translate(hitData.normal * Random.Range(_objectSet.Content.MinZOffset, _objectSet.Content.MaxZOffset));

            Quaternion rotation = Quaternion.identity;

            if (_objectSet.Content.AlignToNormal)
            {
                var normalAlignRotation = Quaternion.FromToRotation(Vector3.up, hitData.normal);
                var angle = Quaternion.Angle(Quaternion.identity, normalAlignRotation);
                if (angle > _objectSet.Content.AlignMaxAngle)
                {
                    var t = _objectSet.Content.AlignMaxAngle / angle;
                    normalAlignRotation = Quaternion.Slerp(Quaternion.identity, normalAlignRotation, t);
                }

                rotation *= normalAlignRotation;
            }

            if (_objectSet.Content.RandomPitch)
            {
                var rand = Random.Range(0, _objectSet.Content.RandomPitchValue);
                rand = _objectSet.Content.RandomPitchValue;
                var randomPitchRotation = Quaternion.AngleAxis(rand, new Vector3(Random.value - .5f, 0, Random.value - .5f));
                rotation *= randomPitchRotation;
            }

            if (_objectSet.Content.RandomYaw)
            {
                var randomYawRotation =
                    Quaternion.AngleAxis(Random.Range(_objectSet.Content.YawMin, _objectSet.Content.YawMax),
                        Vector3.up);
                rotation *= randomYawRotation;
            }

            newObj.transform.rotation = rotation;

            if (_objectSet.Content.ApplyScale)
            {
                newObj.transform.localScale =
                    Vector3.one * Random.Range(_objectSet.Content.ScaleMin, _objectSet.Content.ScaleMax);
            }

            return newObj;
        }
        else
        {
            Debug.LogWarning("No object set selected! Here's a cube instead.");
            EditorApplication.ExecuteMenuItem("GameObject/3D Object/Cube");
            var newObj = Selection.activeGameObject;
            newObj.transform.position = hitData.point;
            newObj.transform.parent = GameObject.Find("PaintedCubes")?.transform ?? new GameObject("PaintedCubes").transform;
            return newObj;
        }
    }

    private struct PaletteItem
    {
        public readonly Object PrefabAsset;
        public readonly GameObject GameObject;
        public readonly bool HasPrefab;

        public PaletteItem(GameObject gameObject)
        {
            PrefabAsset = GetPrefabObject(gameObject);
            this.GameObject = gameObject;
            HasPrefab = PrefabAsset != null;
        }
    }
}
