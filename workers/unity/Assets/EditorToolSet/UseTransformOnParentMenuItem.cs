using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class UseTransformOnParentMenuItem : MonoBehaviour
{


    private const string UseTransformOnParentMenuName = "ImprobaTools/Set Parent Transform to...";


    [MenuItem(UseTransformOnParentMenuName + "/Child position")]
    public static void ChildPosition_Apply()
    {
        var selected = Selection.activeTransform;
        Transform parent = selected.parent;
        RecordObjects(parent, "Set Parent Position To Child");
        EditorTools.UseTransformOnParent(parent, selected.position, parent.rotation, parent.localScale);
    }
    [MenuItem(UseTransformOnParentMenuName + "/Child rotation")]
    public static void ChildRotation_Apply()
    {
        var selected = Selection.activeTransform;
        Transform parent = selected.parent;
        RecordObjects(parent, "Set Parent Rotation To Child");
        EditorTools.UseTransformOnParent(parent, parent.position, selected.rotation, parent.localScale);
    }
    [MenuItem(UseTransformOnParentMenuName + "/Child scale")]
    public static void ChildScale_Apply()
    {
        var selected = Selection.activeTransform;
        Transform parent = selected.parent;
        RecordObjects(parent, "Set Parent Scale To Child");

        EditorTools.UseTransformOnParent(parent, parent.position, parent.rotation, Vector3.Scale(parent.localScale, selected.localScale));
    }

    private static void RecordObjects(Transform parent, string undoName)
    {
        var undoObjs = new Object[parent.childCount + 1];
        undoObjs[0] = parent;
        for (int i = 1; i <= parent.childCount; i++)
        {
            undoObjs[i] = parent.GetChild(i - 1);
        }
        Undo.RecordObjects(undoObjs, undoName);
    }

    [MenuItem(UseTransformOnParentMenuName + "/Child position", true)]
    [MenuItem(UseTransformOnParentMenuName + "/Child scale", true)]
    [MenuItem(UseTransformOnParentMenuName + "/Child rotation", true)]
    public static bool ValidatePosition()
    {
        return Validate();
    }


    public static bool Validate()
    {
        var selected = Selection.activeTransform;

        if (selected == null)
        {
            return false;
        }

        if (Selection.objects.Length > 1)
        {
            return false;
        }

        if (selected.parent == null)
        {
            return false;
        }

        return true;
    }

   


}