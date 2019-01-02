using System;
using System.Collections.Generic;
using Fps;
using UnityEngine;

public class TileSettings : MonoBehaviour
{
    [Tooltip("Only functional in editor")] public bool ForceIsClient;

    private static TileSettings Instance;
    public const float DefaultCheckoutDistance = 300f;
    public const float DefaultCheckoutDistanceSquared = 300f * 300f;
    public static float CheckoutDistance => Instance.CheckoutDistanceInternal;

    public List<TileQualityData> Settings = new List<TileQualityData>();


#if UNITY_EDITOR
    public void OnValidate()
    {
        ReapplySettings();
    }

    private void ReapplySettings()
    {
        checkoutDistanceCache = -1;

        Shader.SetGlobalFloat("_GlobalClipDistance", CheckoutDistanceInternal);
        foreach (var tile in FindObjectsOfType<TileEnabler>())
        {
            tile.CheckoutDistance = CheckoutDistanceInternal;
        }
    }
#endif

    private float CheckoutDistanceInternal
    {
        get
        {
            if (checkoutDistanceCache > 0)
            {
                return checkoutDistanceCache;
            }

            var activeQualityLevelName = QualitySettings.names[QualitySettings.GetQualityLevel()];
            for (var i = 0; i < Settings.Count; i++)
            {
                if (Settings[i].QualityName != activeQualityLevelName)
                {
                    continue;
                }

                checkoutDistanceCache = Settings[i].CheckoutDistance;
                Shader.SetGlobalFloat("_GlobalClipDistance", checkoutDistanceCache);
                return checkoutDistanceCache;
            }

            Debug.LogWarning(
                $"No checkout distance found in settings! Using default value of {DefaultCheckoutDistance}");
            checkoutDistanceCache = DefaultCheckoutDistance;
            return DefaultCheckoutDistance;
        }
    }

    private float checkoutDistanceCache = -1;

    private void Awake()
    {
#if UNITY_EDITOR
        if (ForceIsClient)
        {
            foreach (var tile in FindObjectsOfType<TileEnabler>())
            {
                tile.IsClient = true;
            }
        }
#endif

        Instance = this;
    }
}

[Serializable]
public class TileQualityData
{
    public string QualityName;
    public float CheckoutDistance;
}
