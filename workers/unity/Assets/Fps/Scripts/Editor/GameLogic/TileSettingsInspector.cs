using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TileSettings))]
[CanEditMultipleObjects]
public class TileQualitySettingsInspector : Editor
{
    private SerializedProperty tileQualitySettings;
    private GUIStyle activeQualityStyle;

    private int activeQualityLevel;

    public override void OnInspectorGUI()
    {
        activeQualityLevel = QualitySettings.GetQualityLevel();
        DrawPropertiesExcluding(serializedObject, serializedObject.FindProperty("Settings").name);
        DrawTileQualityProperties();

        DrawQualityPreviewButtons();

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawQualityPreviewButtons()
    {
        GUILayout.Label("Preview quality setting", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        for (var i = 0; i < QualitySettings.names.Length; i++)
        {
            GUI.backgroundColor = i == activeQualityLevel ? Color.green : Color.white;
            if (GUILayout.Button(QualitySettings.names[i]))
            {
                QualitySettings.SetQualityLevel(i);
                activeQualityLevel = i;
                (target as TileSettings).OnValidate();
            }
        }

        GUI.backgroundColor = Color.white;
        GUILayout.EndHorizontal();
    }

    private void DrawTileQualityProperties()
    {
        GUILayout.Space(10);
        EditorGUI.BeginChangeCheck();
        GUILayout.Label("Quality properties", EditorStyles.boldLabel);
        for (var i = 0; i < targets.Length; i++)
        {
            if (!QualitiesPropertyMatchesProject(targets[i] as TileSettings))
            {
                FixQualitiesProperty(targets[i] as TileSettings);
            }
        }

        var tileQualitySettings = (target as TileSettings).Settings;
        for (var i = 0; i < tileQualitySettings.Count; i++)
        {
            var tileQualitySetting = tileQualitySettings[i];
            var style = i == QualitySettings.GetQualityLevel() ? activeQualityStyle : EditorStyles.numberField;

            GUI.backgroundColor = i == activeQualityLevel ? new Color(0, 1f, 0) : Color.white;

            tileQualitySetting.CheckoutDistance = EditorGUILayout.FloatField(tileQualitySetting.QualityName,
                tileQualitySetting.CheckoutDistance, style);
        }

        GUI.backgroundColor = Color.white;

        if (EditorGUI.EndChangeCheck())
        {
            (serializedObject.targetObject as MonoBehaviour).SendMessage("OnValidate", null,
                SendMessageOptions.DontRequireReceiver);
        }
    }

    private void FixQualitiesProperty(TileSettings tileSettings)
    {
        var newQualities = new List<TileQualityData>();
        for (var i = 0; i < QualitySettings.names.Length; i++)
        {
            var newTileQualitySetting = new TileQualityData();
            newTileQualitySetting.QualityName = QualitySettings.names[i];
            newTileQualitySetting.CheckoutDistance = TileSettings.DefaultCheckoutDistance;
            foreach (var setting in tileSettings.Settings)
            {
                if (setting.QualityName != QualitySettings.names[i])
                {
                    continue;
                }

                newTileQualitySetting.CheckoutDistance = setting.CheckoutDistance;
                break;
            }

            newQualities.Add(newTileQualitySetting);
        }

        tileSettings.Settings = newQualities;
    }

    private void OnEnable()
    {
        activeQualityStyle = new GUIStyle(EditorStyles.numberField) { fontStyle = FontStyle.Bold };
        tileQualitySettings = serializedObject.FindProperty("TileQualitySettings");
    }


    private bool QualitiesPropertyMatchesProject(TileSettings tileSettings)
    {
        if (tileSettings.Settings.Count != QualitySettings.names.Length)
        {
            Debug.LogWarning(
                $"Arrays are of different lengths! {tileSettings.Settings.Count} and {QualitySettings.names.Length}");
            return false;
        }

        for (var i = 0; i < QualitySettings.names.Length; i++)
        {
            var name1 = tileSettings.Settings[i].QualityName;
            var name2 = QualitySettings.names[i];
            if (name1 == name2)
            {
                continue;
            }

            Debug.LogWarning($"Names did not match! {name1} and {name2}");
            return false;
        }

        return true;
    }
}
