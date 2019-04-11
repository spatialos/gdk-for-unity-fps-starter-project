using UnityEngine;

namespace Fps
{
    public class TileEnabler : MonoBehaviour
    {
        public Transform PlayerTransform;

        public float CheckoutDistance
        {
            set => CheckoutDistanceSquared = Mathf.Pow(value, 2);
        }

        private float CheckoutDistanceSquared;

        public void Initialize(bool isClient)
        {
            if (!isClient)
            {
                Destroy(this);
            }
        }
    }
}
