using UnityEngine;

namespace Fps
{
    public class TileEnabler : MonoBehaviour
    {
        public Transform PlayerTransform;
        public bool IsClient;

        private bool renderersEnabled = true;

        public float CheckoutDistance
        {
            set => CheckoutDistanceSquared = Mathf.Pow(value, 2);
        }

        private float CheckoutDistanceSquared = TileSettings.DefaultCheckoutDistanceSquared;

        private MeshRenderer meshRenderer;

        private void Start()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            if (!IsClient)
            {
                Destroy(this);
                return;
            }

            CheckoutDistanceSquared = Mathf.Pow(TileSettings.CheckoutDistance, 2);
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
