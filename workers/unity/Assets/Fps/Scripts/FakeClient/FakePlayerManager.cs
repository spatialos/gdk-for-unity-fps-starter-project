using Improbable.Gdk.GameObjectRepresentation;
using Improbable.Worker;
using UnityEngine;

public class FakePlayerManager : MonoBehaviour
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
            coordinator.RegisterLocalFakePlayer(entityId, gameObject);
        }
    }

    private void OnDestroy()
    {
        if (coordinator != null)
        {
            coordinator.UnregisterLocalFakePlayer(entityId, gameObject);
        }
    }
}
