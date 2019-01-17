using System.Reflection;
using UnityEditor;
using UnityEngine;

public class SnapTool : EditorWindow
{

    private static bool initialized;

    [MenuItem("ImprobaTools/Double Grid _]")]
    public static void DoubleGrid()
    {
        if (SnapsAreEqual())
        {
            snapValue = EditorPrefs.GetFloat("MoveSnapX") * 2f;
        }
        else
        {
            snapValue = 1f;
        }

        if (snapValue <= 1f / 16f)
        {
            snapValue = 1f / 16f;
        }

        ApplySnap(Vector3.one * snapValue);
    }

    [MenuItem("ImprobaTools/Half Grid _[")]
    public static void HalfGrid()
    {
        if (SnapsAreEqual())
        {
            snapValue = EditorPrefs.GetFloat("MoveSnapX") / 2f;
        }
        else
        {
            snapValue = 1f;
        }

        if (snapValue <= 1f / 16f)
        {
            snapValue = 1f / 16f;
        }

        ApplySnap(Vector3.one * snapValue);
    }


    private static bool SnapsAreEqual()
    {
        var snapX = EditorPrefs.GetFloat("MoveSnapX");
        var snapY = EditorPrefs.GetFloat("MoveSnapY");
        var snapZ = EditorPrefs.GetFloat("MoveSnapZ");

        return snapX == snapY && snapY == snapZ;
    }


    private static void ApplySnap(Vector3 snap)
    {
        var assembly = Assembly.Load("UnityEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
        var t = assembly.GetType("UnityEditor.SnapSettings");
        var p = t.GetProperty("move");
        p.SetValue("move", snap, null);
    }

    private static double displaySnapTime;
    private static float snapValue;

    private static void OnSceneGUI(SceneView sceneView)
    {
        Debug.Log("OnSceneGUI");
        if (displaySnapTime <= 0)
        {
            return;
        }

        GUI.Label(new Rect(0, 0, 300, 300), snapValue.ToString());
        if (EditorApplication.timeSinceStartup - displaySnapTime > 2000)
        {
            displaySnapTime = 0;
        }
    }
}
