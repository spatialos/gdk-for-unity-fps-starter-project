using Improbable.Gdk.Core;
using Improbable.Worker.CInterop;
using Improbable.Worker.CInterop.Alpha;

namespace Fps
{
    public class GetDeploymentsState : SessionState
    {
        private readonly State nextState;
        private Future<LoginTokensResponse?> loginTokensResponse;

        public GetDeploymentsState(State nextState, UIManager manager, ConnectionStateMachine owner) : base(manager, owner)
        {
            this.nextState = nextState;
        }

        public override void StartState()
        {
            loginTokensResponse = DevelopmentAuthentication.CreateDevelopmentLoginTokensAsync(
                RuntimeConfigDefaults.LocatorHost,
                RuntimeConfigDefaults.LocatorPort,
                new LoginTokensRequest
                {
                    WorkerType = WorkerUtils.UnityClient,
                    PlayerIdentityToken = Blackboard.PlayerIdentityToken,
                    UseInsecureConnection = false,
                    DurationSeconds = 120,
                }
            );
        }

        public override void ExitState()
        {
            loginTokensResponse.Dispose();
        }

        public override void Tick()
        {
            if (!loginTokensResponse.TryGet(out var result))
            {
                return;
            }

            if (!result.HasValue)
            {
                ScreenManager.StartStatus.ShowFailedToGetDeploymentsText(
                    $"Failed to retrieve any login tokens.\nUnknown error - Please report this to us!");
                Owner.SetState(Owner.StartState, 2f);
                return;
            }

            if (result.Value.Status.Code != ConnectionStatusCode.Success)
            {
                ScreenManager.StartStatus.ShowFailedToGetDeploymentsText(
                    $"Failed to retrieve any login tokens.\n Error code: {result.Value.Status.Code}");
                Owner.SetState(Owner.StartState, 2f);
                return;
            }

            Blackboard.LoginTokens = result.Value.LoginTokens;

            if (Blackboard.LoginTokens.Count == 0)
            {
                ScreenManager.StartStatus.ShowFailedToGetDeploymentsText("No deployments are available.");
                Owner.SetState(Owner.StartState, 2f);
                return;
            }

            Owner.SetState(nextState);
        }
    }
}
