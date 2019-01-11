using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

/// <summary>
/// Searches Assets/ for models and reports if an instance of each type of model exists in the active scene or not.
/// </summary>
public class MuseumAssetReporter : MonoBehaviour
{
    // TODO
    // Finish the class! 
    [MenuItem("Assets/Validate Museum")]
    public static void ValidateMuseum()
    {
        var scene = SceneManager.GetActiveScene();
        var isMuseum = IsMuseum(scene);
        if (!isMuseum)
        {
            Debug.LogWarning($"Scene {scene.name} is not a museum (must contain 'Museum' in the name)");
            return;
        }

        //var rootObjects = scene.GetRootGameObjects();
    }

    public static bool IsMuseum(Scene scene)
    {
        return scene.name.Contains("Museum");
    }
}
