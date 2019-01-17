using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class HierarchyToolsWindow : EditorWindow
{
    private List<Transform> storedTransformSelection = new List<Transform>();
    private static HierarchyToolsWindow window;
    private string _textFieldGroup = "Group";
    private GUIStyle labelStyle;
    private bool childOriginMode_Position = true;
    private bool childOriginMode_Scale = true;
    private bool childOriginMode_Rotation = true;


    [MenuItem("ImprobaTools/HierarchyTools...")]
    public static void OpenHierarchyToolsWindow()
    {
        window = CreateInstance<HierarchyToolsWindow>();
        window.Show(true);
    }


    private void OnEnable()
    {
        labelStyle = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).GetStyle("label");
    }

    private void OnDestroy()
    {
        Debug.Log("Destroyed HierarchyWindow");
        window = null;
    }

    private void OnSelectionChange()
    {
        Repaint();
    }

    private void OnGUI()
    {
        using (new GUILayout.HorizontalScope())
        {
            using (new GUILayout.VerticalScope())
            {
                DrawGeneralActions();

                DrawStoredActions();
            }

            GUILayout.FlexibleSpace();
        }
    }

    private void DrawStoredActions()
    {
        GUILayout.Label("Selection storing", EditorStyles.boldLabel);
        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.Label(
                storedTransformSelection.Count > 0
                    ? string.Format("{0} stored objects", storedTransformSelection.Count)
                    : "No stored objects", EditorStyles.helpBox);
            Button_SelectStored();
        }

        using (new GUILayout.HorizontalScope())
        {
            Button_AddStored();
            Button_ClearStored();
        }

        using (new GUILayout.HorizontalScope())
        {
            GUILayout.Label("Actions");
        }

        using (var scope = new GUILayout.HorizontalScope())
        {
            Button_ParentStored();
            Button_CopyPrefabToSelected();
        }
    }

    private void DrawGeneralActions()
    {
        GUILayout.Label("General actions", EditorStyles.boldLabel);
        using (new EditorGUILayout.HorizontalScope())
        {
            Button_Group();
            _textFieldGroup = GUILayout.TextField(_textFieldGroup);
        }

        using (new EditorGUILayout.HorizontalScope())
        {
            Button_SetParentOrigin();
            labelStyle.fontStyle = childOriginMode_Position ? FontStyle.Bold : FontStyle.Normal;
            labelStyle.normal.textColor = childOriginMode_Position ? Color.green : Color.red * 0.75f;
            if (GUILayout.Button("P", labelStyle))
            {
                childOriginMode_Position = !childOriginMode_Position;
            }

            labelStyle.normal.textColor = childOriginMode_Scale ? Color.green : Color.red * 0.75f;
            labelStyle.fontStyle = childOriginMode_Scale ? FontStyle.Bold : FontStyle.Normal;
            if (GUILayout.Button("S", labelStyle))
            {
                childOriginMode_Scale = !childOriginMode_Scale;
            }

            labelStyle.normal.textColor = childOriginMode_Rotation ? Color.green : Color.red * 0.75f;
            labelStyle.fontStyle = childOriginMode_Rotation ? FontStyle.Bold : FontStyle.Normal;
            if (GUILayout.Button("R", labelStyle))
            {
                childOriginMode_Rotation = !childOriginMode_Rotation;
            }

            labelStyle.fontStyle = FontStyle.Normal;
            GUILayout.FlexibleSpace();
        }

        Button_SelectParent();
        Button_SelectChildren();
    }

    private void Button_SetParentOrigin()
    {
        if (GUILayout.Button("Set Parent Origin"))
        {
            var selected = Selection.activeTransform;
            var parent = selected.parent;

            EditorTools.UseTransformOnParent(parent,
                childOriginMode_Position ? selected.position : parent.position,
                childOriginMode_Rotation ? selected.rotation : parent.rotation,
                childOriginMode_Scale ? Vector3.Scale(parent.localScale, selected.localScale) : parent.localScale);
        }
    }

    private void Button_CopyPrefabToSelected()
    {
        GUI.enabled = Selection.objects.Length == 1 && PrefabUtility.GetPrefabObject(Selection.activeObject) != null &&
            storedTransformSelection.Count > 0;
        if (GUILayout.Button("Copy Prefab To Stored"))
        {
            var objs = new List<GameObject>();
            foreach (var transform in storedTransformSelection)
            {
                var obj = (GameObject) PrefabUtility.InstantiatePrefab(Selection.activeObject);

                /*obj.transform.localPosition = Vector3.zero;
                obj.transform.localRotation = Quaternion.identity;
                obj.transform.localScale = Vector3.one;*/
                obj.transform.position = transform.position;
                obj.transform.localScale = transform.localScale;
                obj.transform.rotation = transform.rotation;
                objs.Add(obj);
            }

            EditorTools.SelectList(objs);
            foreach (var gameObject in objs)
            {
                EditorGUIUtility.PingObject(gameObject);
            }

            objs.Clear();
        }

        GUI.enabled = true;
    }

    private void Button_Group()
    {
        GUI.enabled = Selection.transforms.Length > 0;
        var pressed = GUILayout.Button("Group Selected");
        GUI.enabled = true;

        if (!pressed)
        {
            return;
        }

        var transforms = Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.Editable);
        if (transforms.Length == 0)
        {
            return;
        }

        _textFieldGroup = _textFieldGroup.Trim();
        if (_textFieldGroup == "")
        {
            _textFieldGroup = "Group";
        }

        var newGroup = new GameObject(_textFieldGroup);
        newGroup.transform.parent = Selection.activeTransform.parent;
        newGroup.transform.localPosition = Vector3.zero;
        newGroup.transform.localRotation = Quaternion.identity;

        foreach (var transform in transforms)
        {
            Debug.Log($"Parenting {transform.gameObject.name} to {newGroup.name}");
            Undo.SetTransformParent(transform, newGroup.transform, "Group");
        }

        //EditorGUIUtility.PingObject(newGroup);
        Selection.activeGameObject = newGroup;

        GUI.enabled = true;
    }

    private void Button_AddStored()
    {
        GUI.enabled = Selection.transforms.Length > 0;
        if (GUILayout.Button("Add"))
        {
            foreach (var selected in Selection.GetTransforms(SelectionMode.Unfiltered))
            {
                if (storedTransformSelection.Count == 0 || storedTransformSelection.Any(stored => stored != selected))
                {
                    storedTransformSelection.Add(selected);
                }
            }
        }

        GUI.enabled = true;
    }

    private void Button_SelectStored()
    {
        GUI.enabled = storedTransformSelection.Count > 0;
        if (GUILayout.Button("Select"))
        {
            SelectStored();
        }

        GUI.enabled = true;
    }

    private void Button_ClearStored()
    {
        GUI.enabled = storedTransformSelection.Count > 0;
        if (GUILayout.Button("Clear"))
        {
            storedTransformSelection.Clear();
        }

        GUI.enabled = true;
    }

    private void Button_ParentStored()
    {
        GUI.enabled = Selection.transforms.Length == 1 && storedTransformSelection.Count > 0 &&
            !storedTransformSelection.Contains(Selection.activeTransform);
        if (GUILayout.Button("Parent to selected"))
        {
            var parent = Selection.activeTransform;
            SelectStored();
            foreach (var obj in Selection.GetTransforms(SelectionMode.TopLevel & SelectionMode.Editable))
            {
                Undo.SetTransformParent(obj, parent, "Parent");
            }

            Selection.GetTransforms(SelectionMode.Unfiltered);
            EditorGUIUtility.PingObject(parent.GetChild(0));
        }

        if (GUILayout.Button("Parent to root"))
        {
            SelectStored();
            foreach (var obj in Selection.GetTransforms(SelectionMode.TopLevel & SelectionMode.Editable))
            {
                Undo.SetTransformParent(obj, null, "Parent to root");
            }

            Selection.GetTransforms(SelectionMode.Unfiltered);
            EditorGUIUtility.PingObject(Selection.transforms[0]);
        }

        GUI.enabled = true;
    }

    private void Button_SelectParent()
    {
        if (!GUILayout.Button("Select Parent"))
        {
            return;
        }

        if (Selection.activeTransform == null || Selection.activeTransform.parent == null)
        {
            return;
        }

        Selection.instanceIDs = new[]
        {
            Selection.activeTransform.parent.gameObject.GetInstanceID()
        };
    }

    private void Button_SelectChildren()
    {
        if (!GUILayout.Button("Select children"))
        {
            return;
        }

        var currentSelection = Selection.transforms;
        var newSelection = new List<int>();

        foreach (var t in currentSelection)
        {
            var children = t.GetComponentsInChildren<Transform>();

            if (children.Length == 1)
            {
                newSelection.Add(t.gameObject.GetInstanceID());
                continue;
            }

            foreach (var child in children)
            {
                if (child == t)
                {
                    continue;
                }

                newSelection.Add(child.gameObject.GetInstanceID());
            }
        }

        Selection.instanceIDs = newSelection.ToArray();
    }

    private void SelectStored()
    {
        EditorTools.SelectList(storedTransformSelection);
    }
}
