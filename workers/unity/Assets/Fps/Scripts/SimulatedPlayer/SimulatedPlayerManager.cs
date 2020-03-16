using Fps.WorkerConnectors;
using Improbable.Gdk.Core;
using Improbable.Gdk.Subscriptions;
using UnityEngine;

namespace Fps.SimulatedPlayer
{
    public class SimulatedPlayerManager : MonoBehaviour
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
                coordinator.RegisterLocalSimulatedPlayer(entityId, gameObject);
            }
        }

        private void OnDestroy()
        {
            if (coordinator != null)
            {
                coordinator.UnregisterLocalSimulatedPlayer(entityId, gameObject);
            }
        }
    }
}
