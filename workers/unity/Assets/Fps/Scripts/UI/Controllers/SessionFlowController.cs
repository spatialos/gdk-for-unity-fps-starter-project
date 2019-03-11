using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Threading;
using Improbable.Gdk.Core;
using Improbable.Gdk.PlayerLifecycle;
using Improbable.Worker.CInterop;
using Improbable.Worker.CInterop.Alpha;
using UnityEngine;

namespace Fps
{
    public class SessionFlowController : MonoBehaviour
    {
        public DeploymentListScreenController DeploymentListController;
        
        private ClientWorkerHandler clientWorkerHandler;
        private string authToken;
        private string deployment;

        public void OnEnable()
        {
            var textAsset = Resources.Load<TextAsset>("DevAuthToken");
            if (textAsset != null)
            {
                authToken = textAsset.text.Trim();
            }
            else
            {
                throw new MissingManifestResourceException("Unable to find DevAuthToken.txt inside the Resources folder. Ensure to generate one.");
            }

            ConnectionStateReporter.OnConnectionStateChange += OnConnectionStateChange;
            clientWorkerHandler = GetComponent<ClientWorkerHandler>();
            StartSessionFlow();
        }

        public void StartSessionFlow()
        {
            ConnectionStateReporter.SetState(ConnectionStateReporter.State.GettingDeploymentList);
            StartCoroutine(LookForDeployments());
        }

        private void OnConnectionStateChange(ConnectionStateReporter.State state, string information)
        {
            Debug.Log(state);
            switch (state)
            {
                case ConnectionStateReporter.State.None:
                case ConnectionStateReporter.State.WorkerDisconnected:
                case ConnectionStateReporter.State.SpawningFailed:
                    StartSessionFlow();
                    break;
                case ConnectionStateReporter.State.QuickJoin:
                    StartCoroutine(SelectDeployment());
                    break;
                case ConnectionStateReporter.State.Connecting:
                    deployment = information;
                    break;
                case ConnectionStateReporter.State.Connected:
                    ConnectionStateReporter.SetState(ConnectionStateReporter.State.WaitingForGameStart);
                    StartCoroutine(PollDeployment());
                    break;
            }
        }

        private IEnumerator SelectDeployment()
        {
            while (true)
            {
                var pit = GetDevelopmentPlayerIdentityToken(authToken, "unknown player", string.Empty);
                var loginTokens = GetDevelopmentLoginTokens(WorkerUtils.UnityClient, pit);
                foreach (var loginToken in loginTokens)
                {
                    if (loginToken.Tags.Contains("status_lobby"))
                    {
                        ConnectionStateReporter.SetState(ConnectionStateReporter.State.Connecting, loginToken.DeploymentName);
                        yield break;
                    }
                }
                
                yield return new WaitForSeconds(5);
            }
        }

        private IEnumerator PollDeployment()
        {
            while (true)
            {
                var foundDeployment = false;
                var pit = GetDevelopmentPlayerIdentityToken(authToken, "unknown player", string.Empty);
                var loginTokens = GetDevelopmentLoginTokens(WorkerUtils.UnityClient, pit);
                foreach (var loginToken in loginTokens)
                {
                    Debug.Log($"cur depl {loginToken.DeploymentName}");

                    if (loginToken.DeploymentName == deployment)
                    {
                        foundDeployment = true;
                        foreach (var tag in loginToken.Tags)
                        {
                            if (tag == "status_running")
                            {
                                ConnectionStateReporter.SetState(ConnectionStateReporter.State.GameReady);
                                yield break;
                            }
                            
                            if (tag == "status_stopping" || tag == "status_stopped")
                            {
                                ConnectionStateReporter.SetState(ConnectionStateReporter.State.None);
                                yield break;
                            }
                        }
                    }
                }

                Debug.Log($"deployment name {deployment}");

                Debug.Log($"Found deployment {foundDeployment}");
                
                if (!foundDeployment)
                {
                    ConnectionStateReporter.SetState(ConnectionStateReporter.State.None);
                    yield break;
                }
                
                yield return new WaitForSeconds(5);
            }
        }

        private IEnumerator LookForDeployments()
        {  
            var deploymentDatas = new List<DeploymentData>();
            while (deploymentDatas.Count == 0)
            {
                var pit = GetDevelopmentPlayerIdentityToken(authToken, "unknown player", string.Empty);
                var loginTokens = GetDevelopmentLoginTokens(WorkerUtils.UnityClient, pit);
                foreach (var loginToken in loginTokens)
                {
                    var playerCount = 0;
                    var maxPlayerCount = 0;
                    var isAvailable = false;
                    foreach (var tag in loginToken.Tags)
                    {
                        if (tag.StartsWith("players"))
                        {
                            playerCount = int.Parse(tag.Split('_').Last());
                        }
                        else if (tag.StartsWith("max_players"))
                        {
                            maxPlayerCount = int.Parse(tag.Split('_').Last());
                        }
                        else if (tag.StartsWith("status"))
                        {
                            var state = tag.Split('_').Last();
                            isAvailable = state == "lobby" || state == "running";
                        }
                    }

                    var deploymentData = new DeploymentData(loginToken.DeploymentName, playerCount, maxPlayerCount,
                        isAvailable);
                    deploymentDatas.Add(deploymentData);
                }

                if (deploymentDatas.Count == 0)
                {
                    yield return new WaitForSeconds(5);
                }
            }

            clientWorkerHandler.CreateClientWorker();
            DeploymentListController.SetDeployments(deploymentDatas.ToArray());
            ConnectionStateReporter.SetState(ConnectionStateReporter.State.DeploymentListAvailable);
        }

        private string GetDevelopmentPlayerIdentityToken(string authToken, string playerId, string displayName)
        {
            var result = DevelopmentAuthentication.CreateDevelopmentPlayerIdentityTokenAsync(
                RuntimeConfigDefaults.LocatorHost,
                RuntimeConfigDefaults.AnonymousAuthenticationPort,
                new PlayerIdentityTokenRequest
                {
                    DevelopmentAuthenticationTokenId = authToken,
                    PlayerId = playerId,
                    DisplayName = displayName,
                }
            ).Get();

            if (!result.HasValue)
            {
                ConnectionStateReporter.SetState(ConnectionStateReporter.State.FailedToGetDeploymentList);
                throw new AuthenticationFailedException("Did not receive a player identity token.");
            }

            if (result.Value.Status.Code != ConnectionStatusCode.Success)
            {
                ConnectionStateReporter.SetState(ConnectionStateReporter.State.FailedToGetDeploymentList);
                throw new AuthenticationFailedException("Failed to retrieve a player identity token.\n" +
                    $"error code: {result.Value.Status}\nDetails: {result.Value.Status.Detail}");
            }

            return result.Value.PlayerIdentityToken;
        }

        /// <summary>
        ///     Retrieves the login tokens for all active deployments that the player
        ///     can connect to via the anonymous authentication flow.
        /// </summary>
        /// <param name="workerType">The type of the worker that wants to connect.</param>
        /// <param name="playerIdentityToken">The player identity token of the player that wants to connect.</param>
        /// <returns>A list of all available login tokens and their deployments.</returns>
        private List<LoginTokenDetails> GetDevelopmentLoginTokens(string workerType, string playerIdentityToken)
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
                ConnectionStateReporter.SetState(ConnectionStateReporter.State.FailedToGetDeploymentList);
                throw new AuthenticationFailedException("Did not receive any login tokens back.");
            }

            if (result.Value.Status.Code != ConnectionStatusCode.Success)
            {
                ConnectionStateReporter.SetState(ConnectionStateReporter.State.FailedToGetDeploymentList);
                throw new AuthenticationFailedException("Failed to retrieve any login tokens.\n" +
                    $"error code: {result.Value.Status}\nDetails: {result.Value.Status.Detail}");
            }

            return result.Value.LoginTokens;
        }
    }
}
