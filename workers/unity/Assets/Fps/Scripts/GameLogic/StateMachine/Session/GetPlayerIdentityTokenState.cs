using Improbable.Gdk.Core;
using Improbable.Worker.CInterop;
using Improbable.Worker.CInterop.Alpha;

namespace Fps
{
    public class GetPlayerIdentityTokenState : SessionState
    {
        private readonly State nextState;
        private Future<PlayerIdentityTokenResponse?> pitResponse;

        public GetPlayerIdentityTokenState(State nextState, UIManager manager, ConnectionStateMachine owner) : base(manager, owner)
        {
            this.nextState = nextState;
        }

        public override void StartState()
        {
            pitResponse = DevelopmentAuthentication.CreateDevelopmentPlayerIdentityTokenAsync(
                RuntimeConfigDefaults.LocatorHost,
                RuntimeConfigDefaults.AnonymousAuthenticationPort,
                new PlayerIdentityTokenRequest
                {
                    DevelopmentAuthenticationToken = Blackboard.DevAuthToken,
                    PlayerId = Blackboard.PlayerName,
                    DisplayName = string.Empty,
                }
            );
        }

        public override void ExitState()
        {
            pitResponse.Dispose();
        }

        public override void Tick()
        {
            if (!pitResponse.TryGet(out var result))
            {
                return;
            }

            if (!result.HasValue)
            {
                ScreenManager.StartStatus.ShowFailedToGetDeploymentsText(
                    $"Failed to retrieve player identity token.\nUnknown error - Please report this to us!");
                Owner.SetState(Owner.StartState, 2f);
            }

            if (result.Value.Status.Code != ConnectionStatusCode.Success)
            {
                ScreenManager.StartStatus.ShowFailedToGetDeploymentsText(
                    $"Failed to retrieve player identity token.\n Error code: {result.Value.Status.Code}");
                Owner.SetState(Owner.StartState, 2f);
            }

            Blackboard.PlayerIdentityToken = result.Value.PlayerIdentityToken;
            Owner.SetState(new GetDeploymentsState(nextState, Manager, Owner));
        }
    }
}
