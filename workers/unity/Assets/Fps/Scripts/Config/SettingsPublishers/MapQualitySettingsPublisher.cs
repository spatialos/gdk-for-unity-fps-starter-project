using Fps.WorldTiles;
using UnityEngine;

namespace Fps.Config
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
