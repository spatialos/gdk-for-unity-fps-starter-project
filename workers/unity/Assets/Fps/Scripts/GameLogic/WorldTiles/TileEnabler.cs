using UnityEngine;

namespace Fps
{
    public class TileEnabler : MonoBehaviour
    {
        public Transform PlayerTransform;

        private bool renderersEnabled = true;

        public float CheckoutDistance
        {
            set => CheckoutDistanceSquared = Mathf.Pow(value, 2);
        }

        private float CheckoutDistanceSquared;

        private MeshRenderer meshRenderer;

        public void Initialize(bool isClient)
        {
            if (!isClient)
            {
                Destroy(this);
                return;
            }

            if (CheckoutDistanceSquared == 0)
            {
                CheckoutDistance = MapQualitySettings.CheckoutDistance;
            }

            meshRenderer = GetComponent<MeshRenderer>();
        }

        private void Update()
        {
            if (PlayerTransform == null)
            {
                return;
            }

            var distanceSquared = Vector3.SqrMagnitude(PlayerTransform.position - transform.position);

            if (renderersEnabled && distanceSquared > CheckoutDistanceSquared)
            {
                ToggleRenderers();
            }
            else if (!renderersEnabled && distanceSquared <= CheckoutDistanceSquared)
            {
                ToggleRenderers();
            }
        }

        private void ToggleRenderers()
        {
            if (meshRenderer == null)
            {
                return;
            }

            renderersEnabled = !renderersEnabled;
            meshRenderer.enabled = renderersEnabled;
        }
    }
}
