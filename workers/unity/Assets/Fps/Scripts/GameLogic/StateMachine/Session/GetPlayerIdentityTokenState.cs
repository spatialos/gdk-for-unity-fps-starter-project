using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Improbable.Gdk.Core;
using Improbable.Worker.CInterop;
using Improbable.Worker.CInterop.Alpha;

namespace Fps
{
    public class GetPlayerIdentityTokenState : SessionState
    {
        private Future<PlayerIdentityTokenResponse?> pitResponse;
        private State nextState;

        public GetPlayerIdentityTokenState(State nextState, ScreenUIController controller, ConnectionStateMachine owner) : base(controller, owner)
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
                    DevelopmentAuthenticationTokenId = Owner.DevAuthToken,
                    PlayerId = Owner.PlayerName,
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

            if (!result.HasValue || result.Value.Status.Code != ConnectionStatusCode.Success)
            {
                ShowErrorMessage($"Failed to retrieve player identity token.\n Error code: {result.Value.Status}");
                Owner.SetState(Owner.StartState);
            }

            Owner.PlayerIdentityToken = result.Value.PlayerIdentityToken;
            Owner.SetState(new GetDeploymentsState(nextState, Controller, Owner));
        }
    }
}
