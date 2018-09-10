using Improbable.Gdk.Guns;
using UnityEngine;

namespace Fps
{
    public class GunDictionaryPublisher : MonoBehaviour, ISettingsPublisher
    {
        [SerializeField] private GunDictionary gunDictionary;
        [SerializeField] private int[] defaultGunLoadout;

        public void Publish()
        {
            GunDictionary.Instance = gunDictionary;
            PlayerGunSettings.DefaultGunLoadout = defaultGunLoadout;
        }
    }
}
