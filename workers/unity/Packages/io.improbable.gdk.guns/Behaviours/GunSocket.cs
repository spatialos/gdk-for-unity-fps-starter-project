using UnityEngine;
using UnityEngine.Rendering;

namespace Improbable.Gdk.Guns
{
    public class GunSocket : MonoBehaviour, IRequiresGun
    {
        [SerializeField] private UnityEngine.Transform gunSocket;
        [SerializeField] private bool gunCollidersEnabled;
        [SerializeField] private ShadowCastingMode shadowMode;

        public GunHandle Gun { get; private set; }

        private GameObject instantiatedGun;

        public void InformOfGun(GunSettings settings)
        {
            if (instantiatedGun != null)
            {
                Destroy(instantiatedGun);
            }

            // Instantiate at the socket, but keep its scale.
            instantiatedGun = Instantiate(settings.GunModel, gunSocket);
            instantiatedGun.transform.SetParent(gunSocket);
            instantiatedGun.transform.localPosition = Vector3.zero;
            instantiatedGun.transform.localEulerAngles = Vector3.zero;

            Gun = instantiatedGun.GetComponent<GunHandle>();

            SetCollision();
            SetShadowMode();
        }

        private void SetCollision()
        {
            var layer = gameObject.layer;
            var gunColliders = instantiatedGun.GetComponentsInChildren<Collider>();
            if (gunCollidersEnabled && gunColliders.Length == 0)
            {
                Debug.LogWarning("The Gun Manager has collision enabled, but the model has no colliders.");
            }

            foreach (var gunCollider in gunColliders)
            {
                gunCollider.enabled = gunCollidersEnabled;
                gunCollider.gameObject.layer = layer;
            }
        }

        private void SetShadowMode()
        {
            var gunRenderers = instantiatedGun.GetComponentsInChildren<Renderer>();
            foreach (var gunRenderer in gunRenderers)
            {
                gunRenderer.shadowCastingMode = shadowMode;
            }
        }
    }
}
