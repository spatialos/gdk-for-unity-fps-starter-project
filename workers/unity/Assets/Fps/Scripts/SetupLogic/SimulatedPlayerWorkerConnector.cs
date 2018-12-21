using System;
using System.Threading.Tasks;
using Fps;
using Improbable.Gdk.Core;
using Improbable.Gdk.GameObjectCreation;
using Improbable.Gdk.GameObjectRepresentation;
using Improbable.Gdk.PlayerLifecycle;
using Improbable.Worker.CInterop;
using Improbable.Worker.CInterop.Alpha;
using UnityEngine;

public class SimulatedPlayerWorkerConnector : DefaultWorkerConnector
{
    private const string AuthPlayer = "Prefabs/SimulatedPlayer/SimulatedPlayer";
    private const string NonAuthPlayer = "Prefabs/SimulatedPlayer/SimulatedPlayerProxy";

    private ILogDispatcher simulatedCoordinatorLogDispatcher;
    private bool connectToRemoteDeployment;
    private string loginToken;
    private string playerIdentityToken;

    public async Task ConnectSimulatedPlayer(ILogDispatcher logDispatcher, string SimulatedPlayerDevAuthTokenId, string SimulatedPlayerTargetDeployment)
    {
        simulatedCoordinatorLogDispatcher = logDispatcher;

        // If we're connecting simulated players to another deployment, we need to fetch locator tokens.
        if (!string.IsNullOrEmpty(SimulatedPlayerTargetDeployment))
        {
            if (string.IsNullOrEmpty(SimulatedPlayerDevAuthTokenId))
            {
                logDispatcher.HandleLog(LogType.Error, new LogEvent("Failed to launch simulated player. No development authentication token specified."));
                return;
            }

            // Obtain player identity token for simulated player.
            var playerIdentityTokenResponse = DevelopmentAuthentication.CreateDevelopmentPlayerIdentityTokenAsync("locator.improbable.io", 444,
                new PlayerIdentityTokenRequest
                {
                    DevelopmentAuthenticationTokenId = SimulatedPlayerDevAuthTokenId,
                    DisplayName = "",
                    PlayerId = "SimulatedPlayer"
                }).Get();
            if (playerIdentityTokenResponse.Value.Status != ConnectionStatusCode.Success)
            {
                logDispatcher.HandleLog(LogType.Error, new LogEvent("Failed to launch simulated player. Failed to obtain player identity token. Reason: " + playerIdentityTokenResponse?.Error));
                return;
            }
            playerIdentityToken = playerIdentityTokenResponse?.PlayerIdentityToken;

            // Obtain login token for simulated player.
            var loginTokenResponse = DevelopmentAuthentication.CreateDevelopmentLoginTokensAsync("locator.improbable.io", 444,
                new LoginTokensRequest
                {
                    PlayerIdentityToken = playerIdentityToken,
                    WorkerType = WorkerUtils.UnityClient,
                    DurationSeconds = 600
                }).Get();
            if (loginTokenResponse.Value.Status != ConnectionStatusCode.Success)
            {
                logDispatcher.HandleLog(LogType.Error, new LogEvent("Failed to launch simulated player. Failed to obtain login tokens. Reason: " + loginTokenResponse?.Error));
                return;
            }
            if (loginTokenResponse?.LoginTokens != null)
            {
                foreach (var loginTokenDetails in loginTokenResponse.Value.LoginTokens)
                {
                    if (loginTokenDetails.DeploymentName == SimulatedPlayerTargetDeployment)
                    {
                        loginToken = loginTokenDetails.LoginToken;
                    }
                }
            }

            if (loginToken == null)
            {
                logDispatcher.HandleLog(LogType.Error, new LogEvent("Failed to launch simulated player. Login token for target deployment was not found in response. Does that deployment have the `dev_auth` tag?"));
                return;
            }

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
            LocatorParameters = new Improbable.Worker.CInterop.Alpha.LocatorParameters
            {
                PlayerIdentity = new PlayerIdentityCredentials
                {
                    LoginToken = loginToken,
                    PlayerIdentityToken = playerIdentityToken,
                }
            }
        };
    }
}
