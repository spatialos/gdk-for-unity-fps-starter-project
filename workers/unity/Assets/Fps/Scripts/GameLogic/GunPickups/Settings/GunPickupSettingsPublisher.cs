using UnityEngine;

namespace Fps
{
    public class GunPickupSettingsPublisher : MonoBehaviour, ISettingsPublisher
    {
        [SerializeField] private GunPickupSettings pickupSettings;

        public void Publish()
        {
            GunPickupSettings.Instance = pickupSettings;
        }
    }
}
