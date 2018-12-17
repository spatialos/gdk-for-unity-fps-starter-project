using System;
using Fps;
using Improbable.Gdk.Core;
using Improbable.Gdk.GameObjectCreation;
using Improbable.Gdk.GameObjectRepresentation;
using Improbable.Gdk.PlayerLifecycle;

public class SimulatedPlayerWorkerConnector : DefaultWorkerConnector
{
    private const string AuthPlayer = "Prefabs/SimulatedPlayer/SimulatedPlayer";
    private const string NonAuthPlayer = "Prefabs/SimulatedPlayer/SimulatedPlayerProxy";

    private async void Start()
    {
        await Connect(WorkerUtils.SimulatedPlayer, new ForwardingDispatcher());
    }

    protected override void HandleWorkerConnectionEstablished()
    {
        PlayerLifecycleHelper.AddClientSystems(Worker.World);

        GameObjectRepresentationHelper.AddSystems(Worker.World);

        var fallback = new GameObjectCreatorFromMetadata(Worker.WorkerType,
            Worker.Origin, Worker.LogDispatcher);

        GameObjectCreationHelper.EnableStandardGameObjectCreation(
            Worker.World,
            new AdvancedEntityPipeline(Worker, AuthPlayer, NonAuthPlayer, fallback));

        Worker.World.GetOrCreateManager<CommandFrameSystem>();
    }

    protected override ReceptionistConfig GetReceptionistConfig(string workerType)
    {
        var config = base.GetReceptionistConfig(workerType);

        // Force WorkerId to unique and not one from command line, since that will be the id of the coordinator.
        config.WorkerId = $"{workerType}-{Guid.NewGuid()}";

        return config;
    }
}
