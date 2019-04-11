using Fps;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraClipQuality : MonoBehaviour
{
    [SerializeField] private float checkoutBias = 10.0f;

    void Awake()
    {
        var camera = GetComponent<Camera>();
        camera.farClipPlane = MapQualitySettings.CheckoutDistance + checkoutBias;
    }
}
