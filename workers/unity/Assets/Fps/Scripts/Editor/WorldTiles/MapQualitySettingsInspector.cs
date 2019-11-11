using System.Collections.Generic;
using Fps.WorldTiles;
using UnityEditor;
using UnityEngine;

namespace Fps.Editor
{
    [CustomEditor(typeof(MapQualitySettings))]
    public class MapQualitySettingsInspector : UnityEditor.Editor
    {
        private int activeQualityLevel;
        private MapQualitySettings settings;
        private int storedQualityLevel; // Used to restore project quality level when no longer previewing quality
        private readonly Color dimColor = new Color(.77f, .77f, .77f);

        private bool settingsAreCurrentlyInUse =>
            Application.isPlaying && MapQualitySettings.Instance == settings
            || MapQualitySettings.ShowPreview;

        private void OnEnable()
        {
            storedQualityLevel = QualitySettings.GetQualityLevel();
        }

        private void OnDisable()
        {
            if (!MapQualitySettings.ShowPreview)
            {
                return;
            }

            MapQualitySettings.ShowPreview = false;
            QualitySettings.SetQualityLevel(storedQualityLevel);
            settings.Apply();
        }

        public override void OnInspectorGUI()
        {
            settings = (MapQualitySettings) target;

            activeQualityLevel = QualitySettings.GetQualityLevel();

            DrawNotEditingActiveQualitySettingWarning();

            EditorGUI.BeginChangeCheck();

            RefreshQualityProperties();
            DrawPropertiesExcluding(serializedObject,
                serializedObject.FindProperty("Settings").name);

            GUILayout.Space(10);

            GUILayout.Label("Tile draw distances", EditorStyles.boldLabel);
            DrawTileQualityProperties();

            GUILayout.Space(10);

            GUILayout.Label("Resolution Scaling Fixed DPI Factor (Mobile / tablet devices)", EditorStyles.boldLabel);
            DrawDPIFactorProperties();

            GUILayout.Space(10);

            if ((!Application.isPlaying || MapQualitySettings.Instance == settings) && DrawPreviewToggle())
            {
                DrawQualityPreviewButtons();
            }

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
                settings.Apply();
            }
        }

        private void DrawNotEditingActiveQualitySettingWarning()
        {
            if (!Application.isPlaying || MapQualitySettings.Instance == settings)
            {
                return;
            }

            var assetLocation = AssetDatabase.GetAssetPath(MapQualitySettings.Instance.GetInstanceID());
            var locationString = string.IsNullOrEmpty(assetLocation)
                ? "a script-instantiated MapQualitySettings object"
                : $"the MapQualitySettings object at:-\n\t'{assetLocation}'";


            EditorGUILayout.HelpBox("Modifying these values will not affect the running game " +
                $"as it is using {locationString}", MessageType.Warning);
        }

        private void RefreshQualityProperties()
        {
            if (QualitiesPropertyMatchesProject())
            {
                return;
            }

            var newQualities = new List<MapQualityLevelData>();
            foreach (var qualityName in QualitySettings.names)
            {
                var newTileQualitySetting = new MapQualityLevelData();
                newTileQualitySetting.QualityName = qualityName;
                newTileQualitySetting.CheckoutDistance = MapQualitySettings.DefaultCheckoutDistance;
                foreach (var setting in settings.Settings)
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

            settings.Settings = newQualities;
        }

        private bool QualitiesPropertyMatchesProject()
        {
            if (settings.Settings.Count != QualitySettings.names.Length)
            {
                return false;
            }

            for (var i = 0; i < QualitySettings.names.Length; i++)
            {
                var name1 = settings.Settings[i].QualityName;
                var name2 = QualitySettings.names[i];
                if (name1 != name2)
                {
                    return false;
                }
            }

            return true;
        }

        private void DrawTileQualityProperties()
        {
            for (var i = 0; i < settings.Settings.Count; i++)
            {
                var qualityName = settings.Settings[i].QualityName;
                var checkoutDistance = settings.Settings[i].CheckoutDistance;

                GUI.color = settingsAreCurrentlyInUse && activeQualityLevel != i
                    ? dimColor
                    : Color.white;
                settings.Settings[i].CheckoutDistance = Mathf.Max(0,
                    EditorGUILayout.FloatField(qualityName, checkoutDistance));
                GUI.color = Color.white;
            }
        }

        private void DrawDPIFactorProperties()
        {
            GUI.color = Color.white;
            for (var i = 0; i < settings.Settings.Count; i++)
            {
                var qualityName = settings.Settings[i].QualityName;
                var dpiScalar = settings.Settings[i].DpiScalar;

                settings.Settings[i].DpiScalar = Mathf.Max(0,
                    EditorGUILayout.FloatField(qualityName, dpiScalar));
            }
        }

        private bool DrawPreviewToggle()
        {
            if (Application.isPlaying)
            {
                GUI.enabled = false;
                GUILayout.Toggle(true, "Show Preview");
                GUI.enabled = true;
                return true;
            }

            var togglePreview = GUILayout.Toggle(MapQualitySettings.ShowPreview, "Show Preview");

            if (togglePreview == MapQualitySettings.ShowPreview)
            {
                return togglePreview;
            }

            MapQualitySettings.ShowPreview = togglePreview;

            if (togglePreview)
            {
                storedQualityLevel = QualitySettings.GetQualityLevel();
            }
            else
            {
                QualitySettings.SetQualityLevel(storedQualityLevel);
            }

            return togglePreview;
        }

        private void DrawQualityPreviewButtons()
        {
            const int verticalLayoutThreshold = 5;

            GUILayout.Label("Current project quality level", EditorStyles.boldLabel);

            if (QualitySettings.names.Length < verticalLayoutThreshold)
            {
                EditorGUILayout.BeginHorizontal();
            }

            for (var i = 0; i < QualitySettings.names.Length; i++)
            {
                if (!GUILayout.Toggle(i == activeQualityLevel, QualitySettings.names[i], "toolbarButton"))
                {
                    continue;
                }

                activeQualityLevel = i;
                QualitySettings.SetQualityLevel(i);
            }

            if (QualitySettings.names.Length < verticalLayoutThreshold)
            {
                EditorGUILayout.EndHorizontal();
            }

            GUI.backgroundColor = Color.white;
        }
    }
}
