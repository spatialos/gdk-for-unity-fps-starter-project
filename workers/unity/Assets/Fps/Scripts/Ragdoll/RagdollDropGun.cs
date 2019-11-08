using System;
using System.Threading.Tasks;
using Fps.Guns;
using UnityEngine;

namespace Fps.Ragdoll
{
    [RequireComponent(typeof(PoolableRagdoll), typeof(GunSocket))]
    public class RagdollDropGun : MonoBehaviour
    {
        [Tooltip(
            "How long the ragdoll holds on to the gun before dropping it.\n(If immediate, the gun won't carry any momentum.)")]
        [SerializeField]
        private float holdOnToGunTime = 0f;

        private PoolableRagdoll ragdoll;
        private GunSocket gunSocket;

        private void Awake()
        {
            ragdoll = GetComponent<PoolableRagdoll>();
            gunSocket = GetComponent<GunSocket>();
        }

        private void OnEnable()
        {
            // Clean up the gun when the ragdoll is cleaned up.
            ragdoll.OnCleanup += Cleanup;

            // Drop the gun, after a delay (less than the cleanup time)
            if (holdOnToGunTime <= 0)
            {
                DropGun();
            }
            else if (holdOnToGunTime < ragdoll.TimeUntilCleanup)
            {
                DelayedDrop(holdOnToGunTime);
            }
        }

        private void OnDisable()
        {
            if (ragdoll.OnCleanup != null)
            {
                ragdoll.OnCleanup -= Cleanup;
            }
        }

        private async void DelayedDrop(float delay)
        {
            await Task.Delay(TimeSpan.FromSeconds(delay));
            DropGun();
        }

        private void DropGun()
        {
            if (gunSocket == null || gunSocket.Gun == null)
            {
                return;
            }

            var gun = gunSocket.Gun.gameObject;
            gun.transform.SetParent(null);
            // Add the launch force to the gun that is added to the ragdoll
            var gunRigidbody = gun.AddComponent<Rigidbody>();
            var ragdollRigidbody = ragdoll.CentreBone;
            if (ragdollRigidbody != null)
            {
                gunRigidbody.AddForce(ragdollRigidbody.velocity, ForceMode.VelocityChange);
            }
        }

        private void Cleanup()
        {
            Destroy(gunSocket.Gun.gameObject);
        }
    }
}
