using System;
using System.Threading.Tasks;
using Fps;
using Improbable.Gdk.Core;
using Improbable.Gdk.GameObjectCreation;
using Improbable.Gdk.GameObjectRepresentation;
using Improbable.Gdk.PlayerLifecycle;
using Improbable.Worker.CInterop.Alpha;
using UnityEngine;

public class SimulatedPlayerWorkerConnector : DefaultWorkerConnector
{
    private const string AuthPlayer = "Prefabs/SimulatedPlayer/SimulatedPlayer";
    private const string NonAuthPlayer = "Prefabs/SimulatedPlayer/SimulatedPlayerProxy";

    public string LoginToken;
    public string PlayerIdentityToken;

    private bool connectToRemoteDeployment;

    public async Task ConnectSimulatedPlayer(ILogDispatcher logDispatcher, string SimulatedPlayerDevAuthTokenId, string SimulatedPlayerTargetDeployment)
    {
        Debug.Log(SimulatedPlayerTargetDeployment);
        if (!string.IsNullOrEmpty(SimulatedPlayerTargetDeployment))
        {
            var playerIdentityTokenResponse = DevelopmentAuthentication.CreateDevelopmentPlayerIdentityTokenAsync("locator.improbable.io", 444,
                new PlayerIdentityTokenRequest
                {
                    DevelopmentAuthenticationTokenId = SimulatedPlayerDevAuthTokenId,
                    DisplayName = "",
                    PlayerId = ""
                }).Get();
            PlayerIdentityToken = playerIdentityTokenResponse?.PlayerIdentityToken;

            logDispatcher.HandleLog(LogType.Warning, new LogEvent("PIT:" + PlayerIdentityToken));

            var loginTokenResponse = DevelopmentAuthentication.CreateDevelopmentLoginTokensAsync("locator.improbable.io", 444,
                new LoginTokensRequest
                {
                    PlayerIdentityToken = PlayerIdentityToken,
                    WorkerType = "SimulatedPlayer"
                }).Get();

            if (loginTokenResponse?.LoginTokens != null)
            {
                foreach (var loginTokenDetails in loginTokenResponse.Value.LoginTokens)
                {
                    if (loginTokenDetails.DeploymentName == SimulatedPlayerTargetDeployment)
                    {
                        LoginToken = loginTokenDetails.LoginToken;
                    }
                }
            }

            logDispatcher.HandleLog(LogType.Warning, new LogEvent("LT: " + LoginToken));
            connectToRemoteDeployment = true;
        }

        await Connect(WorkerUtils.UnityClient, new ForwardingDispatcher());
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

    protected override AlphaLocatorConfig GetAlphaLocatorConfig()
    {
        return new AlphaLocatorConfig
        {
            LocatorHost = "locator.improbable.io",
            LocatorParameters = new LocatorParameters
            {
                PlayerIdentity = new PlayerIdentityCredentials
                {
                    LoginToken = LoginToken,
                    PlayerIdentityToken = PlayerIdentityToken,
                }
            }
        };
    }
}
