using Fps;
using Improbable.Gdk.Core;

public class FakeClientWorkerConnector : WorkerConnector
{
    private async void Start()
    {
        await Connect(WorkerUtils.FakeClient, new ForwardingDispatcher()).ConfigureAwait(false);
    }
}
