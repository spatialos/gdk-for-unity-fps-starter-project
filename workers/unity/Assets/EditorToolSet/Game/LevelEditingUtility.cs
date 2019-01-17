using System;
using System.Collections;
using System.Collections.Generic;
using Fps;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[ExecuteInEditMode]
public class LevelEditingUtility : MonoBehaviour, IScenePostProcess
{
    public GameObject SaveLevelPrefabOnPlay;
    public bool MoveSpawnToSceneCamera;

    private void OnValidate()
    {
        if (SaveLevelPrefabOnPlay == null)
        {
            return;
        }

        var prefabParent = PrefabUtility.GetCorrespondingObjectFromSource(SaveLevelPrefabOnPlay);
        if (prefabParent == null)
        {
            Debug.LogWarning(SaveLevelPrefabOnPlay.name + " is not a prefab");
            SaveLevelPrefabOnPlay = null;
        }
    }

    public void OnEnable()
    {
        EditorApplication.playModeStateChanged += PlayModeStateChanged;
    }

    public void OnDisable()
    {
        EditorApplication.playModeStateChanged -= PlayModeStateChanged;
    }

    private void PlayModeStateChanged(PlayModeStateChange stateChange)
    {
        switch (stateChange)
        {
            case PlayModeStateChange.EnteredEditMode:
                sceneCameraAvailable = false;
                break;
            case PlayModeStateChange.ExitingEditMode:
                var sceneCamera = SceneView.lastActiveSceneView.camera;
                if (sceneCamera == null)
                {
                    sceneCameraAvailable = false;
                    return;
                }

                sceneCameraAvailable = true;
                sceneCameraPosition = sceneCamera.transform.position;
                sceneCameraRotation = sceneCamera.transform.rotation;
                break;
        }
    }

    public void RunPostProcess()
    {
        if (MoveSpawnToSceneCamera && sceneCameraAvailable)
        {
            PrepSpawns();
            Debug.LogWarningFormat(
                "Launched with spawn position set to scene camera. <b>This will be saved into the level prefab!</b> Your scene prefab instance will be unaffected.");
        }

        var prefabParent = PrefabUtility.GetCorrespondingObjectFromSource(SaveLevelPrefabOnPlay);
        var gameObject = PrefabUtility.FindValidUploadPrefabInstanceRoot(SaveLevelPrefabOnPlay);
        PrefabUtility.ReplacePrefab(gameObject, prefabParent, ReplacePrefabOptions.ConnectToPrefab);

        if (MoveSpawnToSceneCamera && sceneCameraAvailable)
        {
            RestoreSpawns();
        }
    }

    private SpawnPointIndicator[] spawns;
    private Vector3 spawnRestorePosition;
    private Quaternion spawnRestoreRotation;

    [SerializeField][HideInInspector] private Vector3 sceneCameraPosition;
    [SerializeField][HideInInspector] private Quaternion sceneCameraRotation;
    [SerializeField][HideInInspector] private bool sceneCameraAvailable;

    private void PrepSpawns()
    {
        spawns = SaveLevelPrefabOnPlay.GetComponentsInChildren<SpawnPointIndicator>();
        if (spawns.Length == 0)
        {
            return;
        }

        for (var i = 0; i < spawns.Length - 1; i++)
        {
            spawns[i].gameObject.SetActive(false);
        }

        var spawn = spawns[spawns.Length - 1];

        spawnRestorePosition = spawn.transform.position;
        spawnRestoreRotation = spawn.transform.rotation;

        spawn.transform.position = sceneCameraPosition;
        spawn.transform.rotation = Quaternion.Euler(0, sceneCameraRotation.eulerAngles.y, 0);
    }

    private void RestoreSpawns()
    {
        if (spawns == null || spawns.Length == 0)
        {
            return;
        }

        var spawn = spawns[spawns.Length - 1];
        spawn.transform.position = spawnRestorePosition;
        spawn.transform.rotation = spawnRestoreRotation;
        for (var i = 0; i < spawns.Length - 1; i++)
        {
            spawns[i].gameObject.SetActive(true);
        }

        spawns = null;
    }
}

public interface IScenePostProcess
{
    void RunPostProcess();
}
#endif
