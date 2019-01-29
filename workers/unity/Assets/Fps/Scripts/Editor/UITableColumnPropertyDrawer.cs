using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(UITable.ColumnList))]
public class UITableColumnPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var ls = EditorGUIUtility.standardVerticalSpacing;
        var lh = EditorGUIUtility.singleLineHeight;

        var columns = property.FindPropertyRelative("Columns");
        var numelements = columns.arraySize;

        EditorGUI.BeginProperty(position, label, property);

        var halfSeparator = 2;

        var c1Width = Mathf.RoundToInt(position.width * .33f) - halfSeparator;
        var c2Width = Mathf.RoundToInt(position.width - c1Width) - halfSeparator;

        var c1Rect = new Rect(position.x, position.y, c1Width, lh);
        var c2Rect = new Rect(position.x + c1Width + halfSeparator * 2, position.y, c2Width, lh);
        GUI.Label(c1Rect, "Title");
        GUI.Label(c2Rect, "Percentage");

        c1Rect.y += lh + ls;
        c2Rect.y = c1Rect.y;

        if (numelements > 0)
        {
            for (var i = 0; i < numelements; i++)
            {
                var column = columns.GetArrayElementAtIndex(i);
                var titleProp = column.FindPropertyRelative("Title");
                var percProp = column.FindPropertyRelative("Percentage");
                EditorGUI.PropertyField(c1Rect, titleProp, GUIContent.none);

                if (i == numelements - 1)
                {
                    GUI.enabled = false;
                    percProp.floatValue = 1f;
                }

                //EditorGUI.Slider(c2Rect, percProp, 0f, 1f, GUIContent.none);
                var buffer = .025f;
                var min = i == 0
                    ? 0
                    : columns.GetArrayElementAtIndex(i - 1).FindPropertyRelative("Percentage").floatValue;
                var max = i < numelements - 1
                    ? columns.GetArrayElementAtIndex(i + 1).FindPropertyRelative("Percentage").floatValue
                    : 1f;

                if (i < numelements - 1)
                {
                    percProp.floatValue =
                        Mathf.Clamp(
                            GUI.HorizontalSlider(c2Rect, percProp.floatValue, 0, 1f, "ControlLabel",
                                "TextFieldDropDownText"), min + buffer, max - buffer);
                }


                var numFieldWidth = 10; // distance from right of inspector to end of slider

                var affectStart = (c2Rect.width - numFieldWidth) * min;
                var affectEnd = (c2Rect.width - numFieldWidth) * percProp.floatValue;
                DrawRect(new Rect(
                        c2Rect.x + affectStart,
                        c2Rect.y,
                        affectEnd - affectStart,
                        6),
                    Color.gray, 1f);

                c1Rect.y += lh + ls;
                c2Rect.y = c1Rect.y;
            }
        }

        GUI.enabled = true;

        if (GUI.Button(c1Rect, "Add"))
        {
            var squeezeColumnsBy = 1 / numelements + 1;
            for (int i = 0; i < numelements; i++)
            {
                columns.GetArrayElementAtIndex(i).FindPropertyRelative("Percentage").floatValue *= squeezeColumnsBy;
            }

            columns.arraySize++;
            columns.GetArrayElementAtIndex(numelements).FindPropertyRelative("Percentage").floatValue = 1;
        }

        EditorGUI.EndProperty();
    }

    public override bool CanCacheInspectorGUI(SerializedProperty property)
    {
        return false;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var size = property.FindPropertyRelative("Columns").arraySize + 2;
        return size * (EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight);
    }

    private void DrawRect(Rect rect, Color col, float alpha = .05f)
    {
        col.a = alpha;
        GUI.DrawTexture(rect, EditorGUIUtility.whiteTexture, ScaleMode.StretchToFill, true, 1, col, 0, 0);
    }
}
