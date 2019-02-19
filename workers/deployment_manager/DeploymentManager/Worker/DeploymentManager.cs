using System;
using System.IO;
using System.Reflection;
using System.Threading;
using Improbable.SpatialOS.Deployment.V1Alpha1;
using Improbable.Worker;
using Improbable.Worker.Alpha;
using Deployment = Improbable.SpatialOS.Deployment.V1Alpha1.Deployment;

namespace DeploymentManager
{
    internal class DeploymentManager
    {
        public static string WorkerType = "DeploymentManager";

        private readonly DeploymentManagerOptions options;
        private readonly string devAuthToken;

        private Connection metaConnection;

        public DeploymentManager(DeploymentManagerOptions options)
        {
            this.options = options;

            var serviceAccountToken = GetFileContent("ServiceAccountToken.txt");
            DeploymentModifier.Init(serviceAccountToken);
            WorkerAuthenticator.Init(serviceAccountToken);

            // Connect to meta deployment
            var connectionParameters = new ConnectionParameters
            {
                WorkerType = WorkerType,
            };

            var connector = new Connector(WorkerType);
            metaConnection = connector.TryToConnect(connectionParameters, options.ReceptionistHost, options.ReceptionistPort, options.WorkerId);

            devAuthToken = WorkerAuthenticator.RetrieveDevelopmentAuthenticationToken(options.ProjectName);
        }

        private string GetFileContent(string fileName)
        {
            var result = string.Empty;
            var assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream($"DeploymentManager.{fileName}"))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public void ObserveDeployments()
        {
            // spin up the other deployments
            for (var i = 0; i < options.NumberOfDeployments; i++)
            {
                // TODO check if deployment is already running
                var deploymentName = $"{options.DeploymentPrefix}_{i}";
                new Thread(new ParameterizedThreadStart(StartDeployment)).Start(deploymentName);
            }

            while (metaConnection.GetConnectionStatusCode() == ConnectionStatusCode.Success)
            {
                using (var dispatcher = new Dispatcher())
                using (var opList = metaConnection.GetOpList(0))
                {
                    dispatcher.Process(opList);
                }
            }

            Log.Print(LogLevel.Info, "Disconnected from SpatialOS");
        }

        private void StartDeployment(Object obj)
        {
            try
            {
                string deploymentName = (string)obj;

                var snapshotPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "default.snapshot");
                var launchConfig = new LaunchConfig
                {
                    ConfigJson = GetFileContent("launch_config.json"),
                };

                Log.Print(LogLevel.Info, $"Uploading {snapshotPath} to project {options.ProjectName}", metaConnection);
                var snapshotId = DeploymentModifier.UploadSnapshot(snapshotPath, options.ProjectName, deploymentName);

                if (snapshotId.Length == 0)
                {
                    Log.Print(LogLevel.Error, "Something went wrong during snapshot upload.", metaConnection);
                    return;
                }

                var template = new DeploymentTemplate
                {
                    AssemblyId = options.AssemblyName,
                    DeploymentName = deploymentName,
                    LaunchConfig = launchConfig,
                    ProjectName = options.ProjectName,
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
                        DeploymentModifier.AddDeploymentTag(deploymentId, options.ProjectName, "dev_login");
                        DeploymentModifier.AddDeploymentTag(deploymentId, options.ProjectName, "ttl_1_hour");

                        var connection = ConnectToDeployment(metaConnection, deploymentOperation.Result);
                        var handler = new SpatialOSReceiveHandler(connection, deploymentId, options.ProjectName);
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
            var playerIdentityToken = connector.GetPlayerIdentityToken(devAuthToken);
            var loginTokenDetails = connector.GetLoginTokenDetails(playerIdentityToken);
            var loginToken = connector.GetLoginToken(loginTokenDetails);

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
            var maxPlayerCount = DeploymentModifier.GetMaxWorkerCapacity(deploymentId, options.ProjectName, options.PlayerType);
            var remainingPlayerCount = DeploymentModifier.GetRemainingWorkerCapacity(deploymentId, options.ProjectName, options.PlayerType);
            var newPlayerCount = maxPlayerCount - remainingPlayerCount;
            DeploymentModifier.UpdateDeploymentTag(deploymentId, options.ProjectName, "players", newPlayerCount.ToString());
        }
    }
}
