using Fps.WorkerConnectors;
using Improbable.Gdk.Core;
using Improbable.Gdk.Subscriptions;
using UnityEngine;

namespace Fps.SimulatedPlayer
{
    public class SimulatedPlayerProxyManager : MonoBehaviour
    {
        private EntityId entityId;
        private SimulatedPlayerCoordinatorWorkerConnector coordinator;

        private void Start()
        {
            coordinator = FindObjectOfType<SimulatedPlayerCoordinatorWorkerConnector>();
            if (coordinator != null)
            {
                var spatial = GetComponent<LinkedEntityComponent>();
                entityId = spatial.EntityId;
                coordinator.RegisterProxyPrefabForEntity(entityId, gameObject);
            }
        }

        private void OnDestroy()
        {
            if (coordinator != null)
            {
                coordinator.UnregisterProxyPrefabForEntity(entityId, gameObject);
            }
        }
    }
}
