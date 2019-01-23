// C# example:
// Automatically creates a game object with a primitive mesh renderer and appropriate collider.

using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class ScenePostProcessor : Editor
{
    [PostProcessScene(2)]
    public static void OnPostprocessScene()
    {
        Debug.Log("PostProcessor called");
        var scenePostProcesses = FindObjectsOfType<MonoBehaviour>().OfType<IScenePostProcess>();
        foreach (var scenePostProcess in scenePostProcesses)
        {
            Debug.Log("Processing " + scenePostProcess);
            scenePostProcess.RunPostProcess();
        }
    }
}
