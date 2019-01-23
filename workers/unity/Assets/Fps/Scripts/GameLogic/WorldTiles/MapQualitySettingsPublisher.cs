using UnityEngine;

namespace Fps
{
    public class MapQualitySettingsPublisher : MonoBehaviour, ISettingsPublisher
    {
        [SerializeField] private MapQualitySettings mapQualitySettings;

        public void Publish()
        {
            MapQualitySettings.Instance = mapQualitySettings;
        }
    }
}
