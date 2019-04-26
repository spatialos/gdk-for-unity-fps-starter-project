using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fps;
using Improbable.Gdk.Core;
using Improbable.Gdk.GameObjectCreation;
using Improbable.Gdk.PlayerLifecycle;
using Improbable.Worker.CInterop.Alpha;
using UnityEngine;

public class SimulatedPlayerWorkerConnector : DefaultWorkerConnector
{
    private const string AuthPlayer = "Prefabs/SimulatedPlayer/SimulatedPlayer";
    private const string NonAuthPlayer = "Prefabs/SimulatedPlayer/SimulatedPlayerProxy";

    private ILogDispatcher simulatedCoordinatorLogDispatcher;
    private bool connectToRemoteDeployment;

    private string simulatedPlayerDevAuthTokenId;
    private string simulatedPlayerTargetDeployment;

    public async Task ConnectSimulatedPlayer(ILogDispatcher logDispatcher, string simulatedPlayerDevAuthTokenId,
        string simulatedPlayerTargetDeployment)
    {
        simulatedCoordinatorLogDispatcher = logDispatcher;

        // If we're connecting simulated players to another deployment, we need to fetch locator tokens.
        if (!string.IsNullOrEmpty(simulatedPlayerTargetDeployment))
        {
            if (string.IsNullOrEmpty(simulatedPlayerDevAuthTokenId))
            {
                logDispatcher.HandleLog(LogType.Error,
                    new LogEvent("Failed to launch simulated player. No development authentication token specified."));
                return;
            }

            this.simulatedPlayerDevAuthTokenId = simulatedPlayerDevAuthTokenId;
            this.simulatedPlayerTargetDeployment = simulatedPlayerTargetDeployment;

            connectToRemoteDeployment = true;
        }

        Debug.Log(connectToRemoteDeployment);

        await Connect(WorkerUtils.UnityClient, new ForwardingDispatcher());
    }

    public void SpawnPlayer(int number)
    {
        var serializedArgs = Encoding.ASCII.GetBytes($"Simulated Player {number}");
        var sendSystem = Worker.World.GetExistingManager<SendCreatePlayerRequestSystem>();
        sendSystem.RequestPlayerCreation(serializedArgs);
    }

    protected override void HandleWorkerConnectionEstablished()
    {
        PlayerLifecycleHelper.AddClientSystems(Worker.World, false);

        var fallback = new GameObjectCreatorFromMetadata(Worker.WorkerType,
            Worker.Origin, Worker.LogDispatcher);

        GameObjectCreationHelper.EnableStandardGameObjectCreation(
            Worker.World,
            new AdvancedEntityPipeline(Worker, AuthPlayer, NonAuthPlayer, fallback));
    }

    protected override ConnectionService GetConnectionService()
    {
        return connectToRemoteDeployment ? ConnectionService.AlphaLocator : ConnectionService.Receptionist;
    }

    protected override ReceptionistConfig GetReceptionistConfig(string workerType)
    {
        var config = base.GetReceptionistConfig(workerType);

        // Force WorkerId to unique and not one from command line, since that will be the id of the coordinator.
        config.WorkerId = $"{workerType}-{Guid.NewGuid()}";

        return config;
    }

    protected override AlphaLocatorConfig GetAlphaLocatorConfig(string workerType)
    {
        var pit = GetDevelopmentPlayerIdentityToken(simulatedPlayerDevAuthTokenId, "SimulatedPlayer", GetDisplayName());
        var loginTokens = GetDevelopmentLoginTokens(workerType, pit);
        var loginToken = SelectLoginToken(loginTokens);

        return new AlphaLocatorConfig
        {
            LocatorHost = RuntimeConfigDefaults.LocatorHost,
            LocatorParameters = new LocatorParameters
            {
                PlayerIdentity = new PlayerIdentityCredentials
                {
                    LoginToken = loginToken,
                    PlayerIdentityToken = pit
                }
            }
        };
    }

    protected override string SelectLoginToken(List<LoginTokenDetails> loginTokens)
    {
        var selectedLoginToken = loginTokens.FirstOrDefault(token => token.DeploymentName == simulatedPlayerTargetDeployment)
            .LoginToken;

        if (selectedLoginToken == null)
        {
            simulatedCoordinatorLogDispatcher.HandleLog(LogType.Error,
                new LogEvent(
                    "Failed to launch simulated player. Login token for target deployment was not found in response. Does that deployment have the `dev_auth` tag?"));
        }

        return selectedLoginToken;
    }
}
