using Fps.Guns;
using UnityEngine;

namespace Fps.Config
{
    public class GunDictionaryPublisher : MonoBehaviour, ISettingsPublisher
    {
        [SerializeField] private GunDictionary gunDictionary;
        [SerializeField] private int defaultGunIndex;

        public void Publish()
        {
            GunDictionary.Instance = gunDictionary;
            PlayerGunSettings.DefaultGunIndex = defaultGunIndex;
        }
    }
}
