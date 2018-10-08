using Improbable.Gdk.Guns;
using Improbable.Gdk.Ragdoll;
using UnityEngine;

namespace Fps
{
    [RequireComponent(typeof(RagdollSpawner), typeof(GunManager))]
    public class InformRagdollOfGun : MonoBehaviour
    {
        private RagdollSpawner ragdollSpawner;
        private GunManager gunListener;

        private void Awake()
        {
            ragdollSpawner = GetComponent<RagdollSpawner>();
            gunListener = GetComponent<GunManager>();
        }

        private void OnEnable()
        {
            ragdollSpawner.OnRagdollSpawned += InformOfGun;
        }

        private void OnDisable()
        {
            ragdollSpawner.OnRagdollSpawned -= InformOfGun;
        }

        private void InformOfGun(GameObject ragdoll)
        {
            var gunSocket = ragdoll.GetComponent<GunSocket>();
            if (gunSocket != null)
            {
                var gunSettings = gunListener.CurrentGunSettings;
                foreach (var gunUser in gunSocket.GetComponents<IRequiresGun>())
                {
                    gunUser.InformOfGun(gunSettings);
                }
            }
        }
    }
}
