using Fps;
using Improbable.Gdk.Core;
using UnityEngine;

public class FakeClientCoordinatorWorkerConnector : WorkerConnectorBase
{
    public GameObject FakeClientWorkerConnector;

    protected override async void Start()
    {
        Application.targetFrameRate = 60;
        await AttemptConnect();
    }

    protected override string GetWorkerType()
    {
        return WorkerUtils.FakeClientCoorindator;
    }

    protected override void HandleWorkerConnectionEstablished()
    {
        base.HandleWorkerConnectionEstablished();

        if (FakeClientWorkerConnector != null)
        {
            Instantiate(FakeClientWorkerConnector, transform.position, transform.rotation);
        }
    }
}
