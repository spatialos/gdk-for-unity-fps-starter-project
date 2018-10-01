using Improbable.Gdk.GameObjectRepresentation;
using Improbable.Worker;
using UnityEngine;

public class FakePlayerProxyManager : MonoBehaviour
{
    private EntityId entityId;
    private FakeClientCoordinatorWorkerConnector coordinator;

    private void Start()
    {
        coordinator = FindObjectOfType<FakeClientCoordinatorWorkerConnector>();
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
