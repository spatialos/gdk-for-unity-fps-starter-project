using Improbable.Worker;
using Improbable.Worker.Alpha;
using System.Collections.Generic;

namespace DeploymentManager
{
    public class Connector
    {
        public static string LocatorHost = "locator.improbable.io";
        public static ushort LocatorPort = 444;

        public Connection Connection { get; private set; }

        private readonly string workerType;
        private readonly Connection metaConnection;
        private readonly string deploymentName;

        public Connector(string workerType)
        {
            this.workerType = workerType;
        }

        public Connector(string workerType, string deploymentName, Connection metaConnection = null)
        {
            this.metaConnection = metaConnection;
            this.workerType = workerType;
            this.deploymentName = deploymentName;
        }

        public string ReceivePlayerIdentityToken(string developmentAuthToken)
        {
            var playerIdentityTokenResponse = DevelopmentAuthentication.CreateDevelopmentPlayerIdentityTokenAsync(
            LocatorHost,
            LocatorPort,
            new PlayerIdentityTokenRequest
            {
                DevelopmentAuthenticationTokenId = developmentAuthToken,
                DisplayName = workerType,
                PlayerId = workerType,
                UseInsecureConnection = false,
            }).Get();


            if (playerIdentityTokenResponse.Status != ConnectionStatusCode.Success)
            {
                Log.Print(LogLevel.Fatal, playerIdentityTokenResponse.Error, metaConnection);
            }

            return playerIdentityTokenResponse.PlayerIdentityToken;
        }

        public List<LoginTokenDetails> ReceiveLoginTokenDetails(string playerIdentityToken)
        {
            var loginTokenDetailsResponse = DevelopmentAuthentication.CreateDevelopmentLoginTokensAsync(
                LocatorHost,
                LocatorPort,
                new LoginTokensRequest
                {
                    PlayerIdentityToken = playerIdentityToken,
                    UseInsecureConnection = false,
                    WorkerType = workerType,
                }).Get();

            if (loginTokenDetailsResponse.Status != ConnectionStatusCode.Success)
            {
                Log.Print(LogLevel.Fatal, loginTokenDetailsResponse.Error, metaConnection);
            }

            return loginTokenDetailsResponse.LoginTokens;
        }

        public string ReceiveLoginToken(List<LoginTokenDetails> loginTokenDetails)
        {
            var loginToken = string.Empty;
            foreach (var detail in loginTokenDetails)
            {
                if (detail.DeploymentName == deploymentName)
                {
                    loginToken = detail.LoginToken;
                }
            }

            if (string.IsNullOrEmpty(loginToken))
            {
                Log.Print(LogLevel.Fatal, $"Failed to find deployment {deploymentName}. Was it tagged with 'dev_login'?", metaConnection);
            }

            return loginToken;
        }

        public Connection TryToConnect(ConnectionParameters connectionParameters, Improbable.Worker.Alpha.LocatorParameters locatorParameters)
        {
            using (var locator = new Improbable.Worker.Alpha.Locator(LocatorHost, LocatorPort, locatorParameters))
            {
                using (var connectionFuture = locator.ConnectAsync(connectionParameters))
                {
                    Connection = connectionFuture.Get();

                    if (Connection.GetConnectionStatusCode() != ConnectionStatusCode.Success)
                    {
                        Log.Print(LogLevel.Info, $"Failed to connect to deployment {deploymentName}. Reason: {Connection.GetConnectionStatusDetailString()}", metaConnection);
                    }

                    return Connection;
                }
            }
        }

        public Connection TryToConnect(ConnectionParameters connectionParameters, string host, ushort port, string workerName)
        {
            using (var connectionFuture = Connection.ConnectAsync(host, port, workerName, connectionParameters))
            {
                Connection = connectionFuture.Get();

                if (Connection.GetConnectionStatusCode() != ConnectionStatusCode.Success)
                {
                    Log.Print(LogLevel.Fatal, $"Failed to connect to deployment {deploymentName}. Reason: {Connection.GetConnectionStatusDetailString()}", metaConnection);
                }

                return Connection;
            }
        }
    }
}
