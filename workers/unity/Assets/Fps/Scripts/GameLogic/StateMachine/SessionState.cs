using System.Collections.Generic;
using System.Resources;
using Improbable.Gdk.Core;
using Improbable.Worker.CInterop;
using Improbable.Worker.CInterop.Alpha;
using UnityEngine;

namespace Fps
{
    public abstract class SessionState : State
    {
        private const string PlayerTag = "player";

        private readonly string authToken;

        protected readonly ScreenUIController Controller;
        protected readonly ConnectionStateMachine Owner;

        protected SessionState(ScreenUIController controller, ConnectionStateMachine owner)
        {
            Controller = controller;
            Owner = owner;

            var textAsset = Resources.Load<TextAsset>("DevAuthToken");
            if (textAsset != null)
            {
                authToken = textAsset.text.Trim();
            }
            else
            {
                throw new MissingManifestResourceException("Unable to find DevAuthToken.txt inside the Resources folder. Ensure to generate one.");
            }
        }

        protected string GetDevelopmentPlayerIdentityToken()
        {
            var result = DevelopmentAuthentication.CreateDevelopmentPlayerIdentityTokenAsync(
                RuntimeConfigDefaults.LocatorHost,
                RuntimeConfigDefaults.AnonymousAuthenticationPort,
                new PlayerIdentityTokenRequest
                {
                    DevelopmentAuthenticationTokenId = authToken,
                    PlayerId = PlayerTag,
                    DisplayName = string.Empty,
                }
            ).Get();

            if (!result.HasValue)
            {
                throw new AuthenticationFailedException("Did not receive a player identity token.");
            }

            if (result.Value.Status.Code != ConnectionStatusCode.Success)
            {
                throw new AuthenticationFailedException("Failed to retrieve a player identity token.\n" +
                    $"error code: {result.Value.Status}\nDetails: {result.Value.Status.Detail}");
            }

            return result.Value.PlayerIdentityToken;
        }

        protected List<LoginTokenDetails> GetDevelopmentLoginTokens(string workerType, string playerIdentityToken)
        {
            var result = DevelopmentAuthentication.CreateDevelopmentLoginTokensAsync(
                RuntimeConfigDefaults.LocatorHost,
                RuntimeConfigDefaults.AnonymousAuthenticationPort,
                new LoginTokensRequest
                {
                    WorkerType = workerType,
                    PlayerIdentityToken = playerIdentityToken,
                    UseInsecureConnection = false,
                    DurationSeconds = 120,
                }
            ).Get();

            if (!result.HasValue)
            {
                throw new AuthenticationFailedException("Did not receive any login tokens back.");
            }

            if (result.Value.Status.Code != ConnectionStatusCode.Success)
            {
                throw new AuthenticationFailedException("Failed to retrieve any login tokens.\n" +
                    $"error code: {result.Value.Status}\nDetails: {result.Value.Status.Detail}");
            }

            return result.Value.LoginTokens;
        }
    }
}
