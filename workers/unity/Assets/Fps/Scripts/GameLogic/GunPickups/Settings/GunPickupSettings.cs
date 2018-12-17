using UnityEngine;

namespace Fps.GunPickups
{
    public class GunPickupSettings : ScriptableObject
    {
        public static GunPickupSettings Instance { private get; set; }
        [SerializeField] private GameObject gunPickupPrefab;

        [Tooltip("The time until the pickup reappears after the gun has been picked up.")]
        [SerializeField] private float pickupRespawnTime;

        [Tooltip("The distance from which the gun can be picked up.")]
        [SerializeField] private float pickupRadius;

        public static GameObject GunPickupPrefab
        {
            get { return Instance.gunPickupPrefab; }
        }
        public static float PickupRespawnTime
        {
            get { return Instance.pickupRespawnTime; }
        }
        public static float PickupRadius
        {
            get { return Instance.pickupRadius; }
        }
    }
}
