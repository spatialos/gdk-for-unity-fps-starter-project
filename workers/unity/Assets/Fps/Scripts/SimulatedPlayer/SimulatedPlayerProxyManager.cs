using Improbable.Gdk.GameObjectRepresentation;
using Improbable.Worker;
using UnityEngine;

public class SimulatedPlayerProxyManager : MonoBehaviour
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
