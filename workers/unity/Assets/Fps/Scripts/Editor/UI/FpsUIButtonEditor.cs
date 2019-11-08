using Fps.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Fps.Editor
{
    [CustomEditor(typeof(FpsUIButton))]
    public class FpsUIButtonEditor : UnityEditor.UI.ButtonEditor
    {
        public override void OnInspectorGUI()
        {
            var button = (FpsUIButton) target;

            button.IsDarkThemeEnabled = EditorGUILayout.Toggle("Dark Style", button.IsDarkThemeEnabled);
            button.TargetFill =
                (Image) EditorGUILayout.ObjectField("Target Fill", button.TargetFill, typeof(Image), true);
            button.TargetFrame =
                (Image) EditorGUILayout.ObjectField("Target Frame", button.TargetFrame, typeof(Image), true);

            serializedObject.Update();
            var textOptions = serializedObject.FindProperty(nameof(FpsUIButton.TextOptions));

            EditorGUILayout.PropertyField(textOptions, new GUIContent("Text Options"), true);
            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();
        }
    }
}
