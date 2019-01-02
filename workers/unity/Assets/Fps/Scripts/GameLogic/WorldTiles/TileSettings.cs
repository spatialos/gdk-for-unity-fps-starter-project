using System;
using System.Collections.Generic;
using Fps;
using UnityEngine;

public class TileSettings : MonoBehaviour
{
    [Tooltip("ForceIsClient only functions in editor")]
    public bool ForceIsClient;

    public const float DefaultCheckoutDistance = 300f;
    public const float DefaultCheckoutDistanceSquared = 300f * 300f;
    public static float CheckoutDistance => Instance.CheckoutDistanceInternal;
    public List<TileQualityData> Settings = new List<TileQualityData>();

    private static TileSettings Instance;
    private float checkoutDistanceCache = -1;

    public void OnValidate()
    {
        ApplyCheckoutDistance();
    }

    private void Awake()
    {
        Instance = this;
        ApplyCheckoutDistance();

        Editor_ApplyForceIsClient();
    }

    private void ApplyCheckoutDistance()
    {
        checkoutDistanceCache = -1;

        Shader.SetGlobalFloat("_GlobalClipDistance", CheckoutDistanceInternal);

        Editor_ApplyCheckoutDistanceToTiles();
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
            for (var i = 0; i < Settings.Count; i++)
            {
                if (Settings[i].QualityName != activeQualityLevelName)
                {
                    continue;
                }

                checkoutDistanceCache = Settings[i].CheckoutDistance;
                return checkoutDistanceCache;
            }

            Debug.LogWarning(
                $"No checkout distance found in settings! Using default value of {DefaultCheckoutDistance}");
            checkoutDistanceCache = DefaultCheckoutDistance;
            return DefaultCheckoutDistance;
        }
    }

    #region Editor functions

    private void Editor_ApplyForceIsClient()
    {
        if (!ForceIsClient)
        {
            return;
        }

#if UNITY_EDITOR
        foreach (var tile in FindObjectsOfType<TileEnabler>())
        {
            tile.IsClient = true;
        }
#endif
    }

    private void Editor_ApplyCheckoutDistanceToTiles()
    {
#if UNITY_EDITOR
        foreach (var tile in FindObjectsOfType<TileEnabler>())
        {
            tile.CheckoutDistance = CheckoutDistanceInternal;
        }
#endif
    }

    #endregion

}

[Serializable]
public class TileQualityData
{
    public string QualityName;
    public float CheckoutDistance;
}
