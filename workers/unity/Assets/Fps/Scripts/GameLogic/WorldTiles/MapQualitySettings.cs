using System;
using System.Collections.Generic;
using Fps;
using UnityEngine;

public class MapQualitySettings : MonoBehaviour
{
    public bool Preview;

    public const float DefaultCheckoutDistance = 300f;
    public const float DefaultCheckoutDistanceSquared = 300f * 300f;
    public static float CheckoutDistance => Instance.CheckoutDistanceInternal;
    public List<MapQualityLevelData> Settings = new List<MapQualityLevelData>();

    private static MapQualitySettings Instance;
    private float checkoutDistanceCache = -1;
    public bool ShouldApplyDrawDistance => Preview || Application.isPlaying;

    public void OnValidate()
    {
        ApplyCheckoutDistance();
    }

    private void Awake()
    {
        Instance = this;
        ApplyCheckoutDistance();
    }

    public void ApplyCheckoutDistance()
    {
        checkoutDistanceCache = -1;

        var value = ShouldApplyDrawDistance
            ? CheckoutDistanceInternal
            : -1;

        Shader.SetGlobalFloat("_GlobalClipDistance", value);

        if (Application.isEditor)
        {
            ApplyCheckoutDistanceToTiles();
        }
    }

    private float CheckoutDistanceInternal
    {
        get
        {
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

            Debug.LogWarning(
                $"No checkout distance found in settings! Using default value of {DefaultCheckoutDistance}");
            checkoutDistanceCache = DefaultCheckoutDistance;
            return DefaultCheckoutDistance;
        }
    }

    #region Editor functions

    private void ApplyCheckoutDistanceToTiles()
    {
        var value = ShouldApplyDrawDistance
            ? CheckoutDistanceInternal
            : -1;

        foreach (var tile in FindObjectsOfType<TileEnabler>())
        {
            tile.CheckoutDistance = value;
        }
    }

    #endregion
}

[Serializable]
public class MapQualityLevelData
{
    public string QualityName;
    public float CheckoutDistance;
}
