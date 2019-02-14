using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Improbable.SpatialOS.Deployment.V1Alpha1;
using Improbable.Worker;
using Improbable.Worker.Alpha;
using Deployment = Improbable.SpatialOS.Deployment.V1Alpha1.Deployment;

namespace DeploymentManager
{
    internal class DeploymentManager
    {
        public static string WorkerType = "DeploymentManager";

        private string projectName;
        private string assemblyName;
        private string deploymentNamePrefix;
        private string devAuthToken;
        private int numberOfDeployments;
        private string clientType;
        private int maxPlayerCount;
        private int playerCount;

        private Connection metaConnection;

        public DeploymentManager(string receptionistHost, ushort receptionistPort, string workerId, string projectName, string clientType,
            string assemblyName, string deploymentNamePrefix, int numberOfDeployments, int maxPlayerCount)
        {
            this.projectName = projectName;
            this.assemblyName = assemblyName;
            this.deploymentNamePrefix = deploymentNamePrefix;
            this.numberOfDeployments = numberOfDeployments;
            this.clientType = clientType;
            this.maxPlayerCount = maxPlayerCount;

            var serviceAccountToken = GetFileContent("ServiceAccountToken.txt");
            DeploymentModifier.Init(serviceAccountToken);
            WorkerAuthenticator.Init(serviceAccountToken);

            // Connect to meta deployment
            var connectionParameters = new ConnectionParameters
            {
                WorkerType = WorkerType,
            };

            var connector = new Connector(WorkerType, string.Empty);
            metaConnection = connector.TryToConnect(connectionParameters, receptionistHost, receptionistPort, workerId);

            devAuthToken = WorkerAuthenticator.RetrieveDevelopmentAuthenticationToken(projectName);
        }

        private string GetFileContent(string fileName)
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName)))
            {
                Log.Print(LogLevel.Warn, $"Was not able to find file in path {filePath}");
            }

            return File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName));
        }

        public void ObserveDeployments()
        {
            // spin up the other deployments
            var snapshotPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "default.snapshot");
            var launchConfig = new LaunchConfig
            {
                ConfigJson = GetFileContent("launch_config.json"),
            };

            Log.Print(LogLevel.Info, $"Number of deployments {numberOfDeployments}");
            for (var i = 0; i < numberOfDeployments; i++)
            {
                var deploymentName = $"{deploymentNamePrefix}_{i}";
                Task.Run(() => StartDeployment(deploymentName, snapshotPath, launchConfig));
            }

            while (metaConnection.GetConnectionStatusCode() == ConnectionStatusCode.Success)
            {
                // to make sure the application stays alive
            }

            Log.Print(LogLevel.Info, "Disconnected from SpatialOS");
        }

        private void StartDeployment(string deploymentName, string snapshotPath, LaunchConfig launchConfig)
        {
            try
            {
                Log.Print(LogLevel.Info, $"Uploading {snapshotPath} to project {projectName}", metaConnection);
                var snapshotId = DeploymentModifier.UploadSnapshot(snapshotPath, projectName, deploymentName);

                if (snapshotId.Length == 0)
                {
                    Log.Print(LogLevel.Error, "Something went wrong during snapshot upload.", metaConnection);
                    return;
                }

                var template = new DeploymentTemplate
                {
                    AssemblyId = assemblyName,
                    DeploymentName = deploymentName,
                    LaunchConfig = launchConfig,
                    ProjectName = projectName,
                    SnapshotId = snapshotId,
                };

                while (metaConnection.GetConnectionStatusCode() == ConnectionStatusCode.Success)
                {
                    Log.Print(LogLevel.Info, $"Creating deployment {deploymentName}.", metaConnection);
                    var deploymentOperation = DeploymentModifier.CreateDeployment(template).PollUntilCompleted();

                    if (deploymentOperation.IsCompleted)
                    {
                        Log.Print(LogLevel.Info, $"Successfully created deployment {deploymentName}.", metaConnection);

                        var deploymentId = deploymentOperation.Result.Id;
                        DeploymentModifier.AddDeploymentTag(deploymentId, projectName, "dev_login");
                        DeploymentModifier.AddDeploymentTag(deploymentId, projectName, "ttl_1_hour");

                        var connection = ConnectToDeployment(metaConnection, deploymentOperation.Result);
                        var handler = new SpatialOSReceiveHandler(connection, deploymentId, projectName);
                        var time = DateTime.Now;

                        while (connection.GetConnectionStatusCode() == ConnectionStatusCode.Success)
                        {
                            if (time < DateTime.Now)
                            {
                                UpdatePlayerCount(deploymentId, connection);
                                time = DateTime.Now.AddSeconds(30);
                            }

                            handler.ProcessOps();
                        }

                        Log.Print(LogLevel.Info, $"Lost connection to deployment {deploymentName}. Status: {connection.GetConnectionStatusCode()}. " +
                            "Restarting it.", metaConnection);
                    }
                    else if (deploymentOperation.IsFaulted)
                    {
                        Log.Print(LogLevel.Error, $"Failed to create deployment {deploymentName} with exception {deploymentOperation.Exception.Message}.",
                            metaConnection);
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                Log.PrintException(e, metaConnection);
            }
        }

        private Connection ConnectToDeployment(Connection connection, Deployment deployment)
        {
            var connector = new Connector(WorkerType, deployment.Name, connection);
            var playerIdentityToken = connector.ReceivePlayerIdentityToken(devAuthToken);
            var loginTokenDetails = connector.ReceiveLoginTokenDetails(playerIdentityToken);
            var loginToken = connector.ReceiveLoginToken(loginTokenDetails);

            var connectionParameters = new ConnectionParameters
            {
                WorkerType = WorkerType,
                Network =
                    {
                        UseExternalIp = true,
                        ConnectionType = NetworkConnectionType.Tcp,
                        ConnectionTimeoutMillis = 10000,
                    },
            };

            var locatorParameters = new Improbable.Worker.Alpha.LocatorParameters
            {
                PlayerIdentity = new PlayerIdentityCredentials
                {
                    LoginToken = loginToken,
                    PlayerIdentityToken = playerIdentityToken,
                }
            };

            return connector.TryToConnect(connectionParameters, locatorParameters);
        }

        private void UpdatePlayerCount(string deploymentId, Connection connection)
        {
            var maxPlayerCount = DeploymentModifier.GetMaxWorkerCapacity(deploymentId, projectName, clientType);
            var remainingPlayerCount = DeploymentModifier.GetRemainingWorkerCapacity(deploymentId, projectName, clientType);
            var newPlayerCount = maxPlayerCount - remainingPlayerCount;
            DeploymentModifier.UpdateDeploymentTag(deploymentId, projectName, "players", newPlayerCount.ToString());
        }
    }
}
