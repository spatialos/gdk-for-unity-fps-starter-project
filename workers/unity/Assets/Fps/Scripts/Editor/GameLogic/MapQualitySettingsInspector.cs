using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapQualitySettings))]
[CanEditMultipleObjects]
public class MapQualitySettingsInspector : Editor
{
    private GUIStyle activeQualityStyle;

    private int activeQualityLevel;

    public override void OnInspectorGUI()
    {
        activeQualityLevel = QualitySettings.GetQualityLevel();

        var previewProp = serializedObject.FindProperty("ShowPreview");

        RefreshQualityProperties();

        DrawPropertiesExcluding(serializedObject,
            serializedObject.FindProperty("Settings").name,
            previewProp.name);
        GUILayout.Space(10);

        GUILayout.Label("Tile draw distances", EditorStyles.boldLabel);
        DrawTileQualityProperties();

        GUILayout.Space(10);

        EditorGUILayout.PropertyField(previewProp);

        if (previewProp.boolValue)
        {
            DrawQualityPreviewButtons();
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void RefreshQualityProperties()
    {
        foreach (var t in targets)
        {
            var tileSettings = (MapQualitySettings) t;
            if (QualitiesPropertyMatchesProject(tileSettings))
            {
                continue;
            }

            var newQualities = new List<MapQualityLevelData>();
            foreach (var qualityName in QualitySettings.names)
            {
                var newTileQualitySetting = new MapQualityLevelData();
                newTileQualitySetting.QualityName = qualityName;
                newTileQualitySetting.CheckoutDistance = MapQualitySettings.DefaultCheckoutDistance;
                foreach (var setting in tileSettings.Settings)
                {
                    if (setting.QualityName != qualityName)
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
    }

    private void DrawTileQualityProperties()
    {
        var settings = (MapQualitySettings) target;
        var qualitySettingsList = serializedObject.FindProperty("Settings");
        for (var i = 0; i < qualitySettingsList.arraySize; i++)
        {
            var qualitySettings = qualitySettingsList.GetArrayElementAtIndex(i);
            var qualitySettingsName = qualitySettings.FindPropertyRelative("QualityName").stringValue;
            var qualitySettingsCheckoutDistance = qualitySettings.FindPropertyRelative("CheckoutDistance");

            var style = settings.ShouldApplyDrawDistance && i == QualitySettings.GetQualityLevel()
                ? activeQualityStyle
                : EditorStyles.numberField;

            GUI.backgroundColor = settings.ShouldApplyDrawDistance && i == activeQualityLevel
                ? new Color(0, 1f, 0)
                : Color.white;


            qualitySettingsCheckoutDistance.floatValue =
                EditorGUILayout.FloatField(qualitySettingsName, qualitySettingsCheckoutDistance.floatValue, style);
        }

        GUI.backgroundColor = Color.white;
    }

    private void DrawQualityPreviewButtons()
    {
        GUILayout.BeginVertical();

        var settings = (MapQualitySettings) target;

        for (var i = 0; i < QualitySettings.names.Length; i++)
        {
            GUI.backgroundColor = i == activeQualityLevel
                ? Color.green
                : Color.white;
            if (!GUILayout.Button(QualitySettings.names[i]))
            {
                continue;
            }

            activeQualityLevel = i;

            QualitySettings.SetQualityLevel(i);
            settings.OnValidate();
        }

        GUI.backgroundColor = Color.white;
        GUILayout.EndVertical();
    }

    private void OnEnable()
    {
        SetQualityStyle();
    }

    private void SetQualityStyle()
    {
        // If scripts are recompiled, Unity seems to have a delay before EditorStyles is safe to query
        // So, keep trying until it's ready.

        try
        {
            activeQualityStyle = new GUIStyle(EditorStyles.numberField) { fontStyle = FontStyle.Bold };
            Repaint();
        }
        catch (NullReferenceException)
        {
            activeQualityStyle = GUIStyle.none;
            EditorApplication.delayCall += SetQualityStyle;
        }
    }

    private bool QualitiesPropertyMatchesProject(MapQualitySettings mapQualitySettings)
    {
        if (mapQualitySettings.Settings.Count != QualitySettings.names.Length)
        {
            return false;
        }

        for (var i = 0; i < QualitySettings.names.Length; i++)
        {
            var name1 = mapQualitySettings.Settings[i].QualityName;
            var name2 = QualitySettings.names[i];
            if (name1 != name2)
            {
                return false;
            }
        }

        return true;
    }
}
