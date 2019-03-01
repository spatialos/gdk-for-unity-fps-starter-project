using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fps
{
    [CreateAssetMenu(fileName = "MapQualitySettings", menuName = "Improbable/Map Quality Settings")]
    public class MapQualitySettings : ScriptableObject
    {
        public const float DefaultCheckoutDistance = 300f;
        public const float DefaultDpiScalar = 1f;
        public static float DPIScalar => Instance.GetDPIScalar();


        public static bool ShowPreview;
        public List<MapQualityLevelData> Settings = new List<MapQualityLevelData>();

        private static MapQualitySettings instance;
        private float checkoutDistanceCache = -1;
        private float dpiScalarCache = -1;

        public static float CheckoutDistance => Instance.GetCheckoutDistance();

        public static MapQualitySettings Instance
        {
            get
            {
                if (instance == null)
                {
                    CreateDefaultInstance();
                }

                return instance;
            }

            set
            {
                if (value == null)
                {
                    CreateDefaultInstance();
                }
                else
                {
                    instance = value;
                    instance.Apply();
                }
            }
        }

        private static void CreateDefaultInstance()
        {
            Debug.LogWarning("MapQualitySettings: No settings specified. To fix this, use a " +
                "MapQualitySettingsPublisher and ensure it has a MapQualitySettings reference.\n");

            instance = CreateInstance<MapQualitySettings>();
            instance.Apply();
        }

        private float GetCheckoutDistance()
        {
            if (!Application.isPlaying && !ShowPreview)
            {
                return -1;
            }

            if (checkoutDistanceCache > 0)
            {
                return checkoutDistanceCache;
            }

            var activeQualityLevelName = QualitySettings.names[QualitySettings.GetQualityLevel()];
            foreach (var setting in Settings)
            {
                if (setting.QualityName != activeQualityLevelName)
                {
                    continue;
                }

                checkoutDistanceCache = setting.CheckoutDistance;
                return checkoutDistanceCache;
            }

            Debug.LogWarning($"Using default draw distance of {DefaultCheckoutDistance} for ALL quality levels.\n");
            checkoutDistanceCache = DefaultCheckoutDistance;
            return checkoutDistanceCache;
        }

        private float GetDPIScalar()
        {
            if (dpiScalarCache > 0)
            {
                return dpiScalarCache;
            }
            
            var activeQualityLevelName = QualitySettings.names[QualitySettings.GetQualityLevel()];

            foreach (var setting in Settings)
            {
                if (setting.QualityName != activeQualityLevelName)
                {
                    continue;
                }

                dpiScalarCache = setting.DpiScalar;
                return dpiScalarCache;
            }

            Debug.LogWarning("Quality setting not found; using default DPI scaling of " +
                $"{DefaultDpiScalar} for all quality levels");
            return DefaultDpiScalar;
        }

        public void Apply()
        {
            // Force a recache
            dpiScalarCache = -1;
            checkoutDistanceCache = -1;

            QualitySettings.resolutionScalingFixedDPIFactor = GetDPIScalar();
            var checkoutDistance = GetCheckoutDistance();
            Shader.SetGlobalFloat("_GlobalClipDistance", checkoutDistance);

            var tiles = FindObjectsOfType<TileEnabler>();
            foreach (var tile in tiles)
            {
                tile.CheckoutDistance = checkoutDistance;
            }
        }

#if UNITY_EDITOR
        // These functions ensure the shader clipping gets reset when the game returns to editor
        private void OnEnable()
        {
            UnityEditor.EditorApplication.playModeStateChanged += PlayModeStateChanged;
        }

        private void OnDisable()
        {
            UnityEditor.EditorApplication.playModeStateChanged -= PlayModeStateChanged;
            RestoreSettings();
        }

        private void PlayModeStateChanged(UnityEditor.PlayModeStateChange stateChange)
        {
            if (stateChange != UnityEditor.PlayModeStateChange.EnteredEditMode)
            {
                return;
            }

            RestoreSettings();
        }

        private void RestoreSettings()
        {
            QualitySettings.resolutionScalingFixedDPIFactor = 1;
            Shader.SetGlobalFloat("_GlobalClipDistance", -1);
            ShowPreview = false;
        }
#endif
    }

    [Serializable]
    public class MapQualityLevelData
    {
        public string QualityName;
        public float CheckoutDistance = MapQualitySettings.DefaultCheckoutDistance;
        public float DpiScalar = MapQualitySettings.DefaultDpiScalar;
    }
}
