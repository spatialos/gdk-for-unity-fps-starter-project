using UnityEngine;

namespace Fps
{
    public class TileEnabler : MonoBehaviour
    {
        public Transform PlayerTransform;
        public bool IsClient;

        private bool renderersEnabled = true;

        private const int LevelCheckoutDistance = 300;
        private const int LevelCheckoutDistanceSquared = LevelCheckoutDistance * LevelCheckoutDistance;

        private Renderer[] childRenderers;

        private void Awake()
        {
            childRenderers = GetComponentsInChildren<Renderer>();
        }

        private void Start()
        {
            if (!IsClient)
            {
                Destroy(this);
            }
        }

        private void Update()
        {
            if (PlayerTransform != null)
            {
                var distanceSquared = Vector3.SqrMagnitude(PlayerTransform.position - transform.position);

                if (renderersEnabled && distanceSquared > LevelCheckoutDistanceSquared)
                {
                    ToggleRenderers();
                }
                else if (!renderersEnabled && distanceSquared <= LevelCheckoutDistanceSquared)
                {
                    ToggleRenderers();
                }
            }
        }

        private void ToggleRenderers()
        {
            renderersEnabled = !renderersEnabled;
            foreach (var childRenderer in childRenderers)
            {
                childRenderer.enabled = renderersEnabled;
            }
        }
    }
}
