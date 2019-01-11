using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

public static class EditorTools
{

    public static void SelectList(List<Transform> transforms)
    {
        Debug.Log(transforms);
        var ids = new int[transforms.Count];
        for (var i = 0; i < transforms.Count; i++)
        {
            ids[i] = transforms[i].gameObject.GetInstanceID();
        }
        Selection.instanceIDs = ids;
    }

    public static void SelectList(List<GameObject> gameObjects)
    {
        var ids = new int[gameObjects.Count];
        for (var i = 0; i < gameObjects.Count; i++)
        {
            ids[i] = gameObjects[i].GetInstanceID();
        }
        Selection.instanceIDs = ids;
    }

    public static void UseTransformOnParent(Transform parent, Vector3 position, Quaternion rotation, Vector3 scale)
    {
        // Temp object to move to (easy way to zero out requested transforms)
        Transform tempObject = new GameObject().transform;

        tempObject.position = position;
        tempObject.rotation = rotation;
        tempObject.localScale = scale;

        // Move to temp object
        while (parent.childCount > 0)
        {
            parent.GetChild(0).parent = tempObject;
        }

        parent.position = position;
        parent.rotation = rotation;
        parent.localScale = scale;

        // Move back to original parent
        while (tempObject.childCount > 0)
        {
            tempObject.GetChild(0).parent = parent;
        }

        // Destroy the temp

        Object.DestroyImmediate(tempObject.gameObject);
    }
}
