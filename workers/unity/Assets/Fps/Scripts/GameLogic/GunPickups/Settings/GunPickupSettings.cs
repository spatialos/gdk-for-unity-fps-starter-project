using UnityEngine;

namespace Fps
{
    public class GunPickupSettings : ScriptableObject
    {
        public static GunPickupSettings Instance { private get; set; }
        [SerializeField] private GameObject gunPickupPrefab;

        [Tooltip("The time until the pickup reappears after the gun has been picked up.")] [SerializeField]
        private float pickupRespawnTime;

        [Tooltip("The distance from which the gun can be picked up.")] [SerializeField]
        private float pickupRadius;

        public static GameObject GunPickupPrefab => Instance.gunPickupPrefab;

        public static float PickupRespawnTime => Instance.pickupRespawnTime;

        public static float PickupRadius => Instance.pickupRadius;
    }
}
