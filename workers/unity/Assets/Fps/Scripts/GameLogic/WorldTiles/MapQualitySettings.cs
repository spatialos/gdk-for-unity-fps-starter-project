using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fps
{
    [CreateAssetMenu(fileName = "MapQualitySettings", menuName = "Improbable/Map Quality Settings")]
    public class MapQualitySettings : ScriptableObject
    {
        public const float DefaultCheckoutDistance = 60f;

        public static bool ShowPreview;
        public List<MapQualityLevelData> Settings = new List<MapQualityLevelData>();

        private static MapQualitySettings instance;
        private float checkoutDistanceCache = -1;

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


        public void Apply()
        {
            checkoutDistanceCache = -1; // Force a checkout distance calculation based on project Quality level
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
            RestoreClipDistance();
        }

        private void PlayModeStateChanged(UnityEditor.PlayModeStateChange stateChange)
        {
            if (stateChange != UnityEditor.PlayModeStateChange.EnteredEditMode)
            {
                return;
            }

            RestoreClipDistance();
        }

        private void RestoreClipDistance()
        {
            Shader.SetGlobalFloat("_GlobalClipDistance", -1);
            ShowPreview = false;
        }
#endif
    }

    [Serializable]
    public class MapQualityLevelData
    {
        public string QualityName;
        public float CheckoutDistance;
    }
}
