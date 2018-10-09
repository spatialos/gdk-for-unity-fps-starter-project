using Improbable.Gdk.GameObjectRepresentation;
using Improbable.Worker;
using UnityEngine;

public class SimulatedPlayerManager : MonoBehaviour
{
    private EntityId entityId;
    private SimulatedPlayerCoordinatorWorkerConnector coordinator;

    private void Start()
    {
        coordinator = FindObjectOfType<SimulatedPlayerCoordinatorWorkerConnector>();
        if (coordinator != null)
        {
            var spatial = GetComponent<SpatialOSComponent>();
            entityId = spatial.SpatialEntityId;
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
