using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using Google.Protobuf.WellKnownTypes;
using Improbable.SpatialOS.Deployment.V1Alpha1;
using Improbable.SpatialOS.PlayerAuth.V2Alpha1;
using Improbable.SpatialOS.Snapshot.V1Alpha1;
using Newtonsoft.Json.Linq;

namespace Improbable
{
    internal class Program
    {
        private static string UploadSnapshot(SnapshotServiceClient client, string snapshotPath, string projectName,
            string deploymentName)
        {
            Console.WriteLine($"Uploading {snapshotPath} to project {projectName}");

            // Read snapshot.
            var bytes = File.ReadAllBytes(snapshotPath);

            if (bytes.Length == 0)
            {
                Console.Error.WriteLine($"Unable to load {snapshotPath}. Does the file exist?");
                return string.Empty;
            }

            // Create HTTP endpoint to upload to.
            var snapshotToUpload = new Snapshot
            {
                ProjectName = projectName,
                DeploymentName = deploymentName
            };

            using (var md5 = MD5.Create())
            {
                snapshotToUpload.Checksum = Convert.ToBase64String(md5.ComputeHash(bytes));
                snapshotToUpload.Size = bytes.Length;
            }

            var uploadSnapshotResponse =
                client.UploadSnapshot(new UploadSnapshotRequest { Snapshot = snapshotToUpload });
            snapshotToUpload = uploadSnapshotResponse.Snapshot;

            // Upload content.
            var httpRequest = WebRequest.Create(uploadSnapshotResponse.UploadUrl) as HttpWebRequest;
            httpRequest.Method = "PUT";
            httpRequest.ContentLength = snapshotToUpload.Size;
            httpRequest.Headers.Set("Content-MD5", snapshotToUpload.Checksum);

            using (var dataStream = httpRequest.GetRequestStream())
            {
                dataStream.Write(bytes, 0, bytes.Length);
            }

            // Block until we have a response.
            httpRequest.GetResponse();

            // Confirm that the snapshot was uploaded successfully.
            var confirmUploadResponse = client.ConfirmUpload(new ConfirmUploadRequest
            {
                DeploymentName = snapshotToUpload.DeploymentName,
                Id = snapshotToUpload.Id,
                ProjectName = snapshotToUpload.ProjectName
            });

            return confirmUploadResponse.Snapshot.Id;
        }

        private static int CreateDeployment(string[] args)
        {
            bool launchSimPlayerDeployment = args.Length == 9;

            var projectName = args[1];
            var assemblyName = args[2];
            var mainDeploymentName = args[3];
            var mainDeploymentJson = args[4];
            var mainDeploymentSnapshotFilePath = args[5];

            var simDeploymentName = string.Empty;
            var simDeploymentJson = string.Empty;

            if (launchSimPlayerDeployment)
            {
                simDeploymentName = args[6];
                simDeploymentJson = args[7];
            }

            // Create service clients.
            var playerAuthServiceClient = PlayerAuthServiceClient.Create();
            var snapshotServiceClient = SnapshotServiceClient.Create();
            var deploymentServiceClient = DeploymentServiceClient.Create();

            try
            {
                // Upload snapshots.
                var mainSnapshotId = UploadSnapshot(snapshotServiceClient, mainDeploymentSnapshotFilePath, projectName,
                    mainDeploymentName);

                if (mainSnapshotId.Length == 0)
                {
                    return 1;
                }

                // Create main deployment.
                var mainDeploymentConfig = new Deployment
                {
                    AssemblyId = assemblyName,
                    LaunchConfig = new LaunchConfig
                    {
                        ConfigJson = File.ReadAllText(mainDeploymentJson)
                    },
                    Name = mainDeploymentName,
                    ProjectName = projectName,
                    StartingSnapshotId = mainSnapshotId
                };

                if (launchSimPlayerDeployment)
                {
                    // This tag needs to be added to allow simulated clients to connect using login
                    // tokens generated with anonymous auth.
                    mainDeploymentConfig.Tag.Add("dev_login");
                }

                Console.WriteLine(
                    $"Creating the main deployment {mainDeploymentName} in project {projectName} with snapshot ID {mainSnapshotId}.");

                var mainDeploymentCreateOp = deploymentServiceClient.CreateDeployment(new CreateDeploymentRequest
                {
                    Deployment = mainDeploymentConfig
                }).PollUntilCompleted();

                Console.WriteLine("Successfully created the main deployment.");

                if (launchSimPlayerDeployment)
                {
                    // Create development authentication token used by the simulated players.
                    var dat = playerAuthServiceClient.CreateDevelopmentAuthenticationToken(
                        new CreateDevelopmentAuthenticationTokenRequest
                        {
                            Description = "DAT for sim worker deployment.",
                            Lifetime = Duration.FromTimeSpan(new TimeSpan(7, 0, 0, 0)),
                            ProjectName = projectName
                        });

                    // Add worker flags to sim deployment JSON.
                    var devAuthTokenIdFlag = new JObject();
                    devAuthTokenIdFlag.Add("name", "fps_simulated_players_dev_auth_token_id");
                    devAuthTokenIdFlag.Add("value", dat.DevelopmentAuthenticationToken.Id);

                    var targetDeploymentFlag = new JObject();
                    targetDeploymentFlag.Add("name", "fps_simulated_players_target_deployment");
                    targetDeploymentFlag.Add("value", mainDeploymentName);

                    var simWorkerConfigJson = File.ReadAllText(simDeploymentJson);
                    dynamic simWorkerConfig = JObject.Parse(simWorkerConfigJson);

                    for (var i = 0; i < simWorkerConfig.workers.Count; ++i)
                    {
                        if (simWorkerConfig.workers[i].worker_type == "SimulatedPlayerCoordinator")
                        {
                            simWorkerConfig.workers[i].flags.Add(devAuthTokenIdFlag);
                            simWorkerConfig.workers[i].flags.Add(targetDeploymentFlag);
                        }
                    }

                    simWorkerConfigJson = simWorkerConfig.ToString();

                    // Create simulated player deployment.
                    var simDeploymentConfig = new Deployment
                    {
                        AssemblyId = assemblyName,
                        LaunchConfig = new LaunchConfig
                        {
                            ConfigJson = simWorkerConfigJson
                        },
                        Name = simDeploymentName,
                        ProjectName = projectName
                    };

                    simDeploymentConfig.Tag.Add("simulated_clients");

                    Console.WriteLine(
                        $"Creating the simulated player deployment {simDeploymentName} in project {projectName} with snapshot ID {simSnapshotId}.");

                    var simDeploymentCreateOp = deploymentServiceClient.CreateDeployment(new CreateDeploymentRequest
                    {
                        Deployment = simDeploymentConfig
                    }).PollUntilCompleted();

                    Console.WriteLine("Successfully created the simulated player deployment.");
                }
            }
            catch (Grpc.Core.RpcException e)
            {
                if (e.Status.StatusCode == Grpc.Core.StatusCode.NotFound)
                {
                    Console.WriteLine(
                        $"Unable to launch the deployment(s). This is likely because the project '{projectName}' or assembly '{assemblyName}' doesn't exist.");
                }
                else
                {
                    throw;
                }
            }

            return 0;
        }

        private static int StopDeployment(string[] args)
        {
            var projectName = args[1];
            var deploymentId = args[2];

            var deploymentServiceClient = DeploymentServiceClient.Create();

            try
            {
                deploymentServiceClient.StopDeployment(new StopDeploymentRequest
                {
                    Id = deploymentId,
                    ProjectName = projectName
                });
            }
            catch (Grpc.Core.RpcException e)
            {
                if (e.Status.StatusCode == Grpc.Core.StatusCode.NotFound)
                {
                    Console.WriteLine("<error:unknown-deployment>");
                }
                else
                {
                    throw;
                }
            }

            return 0;
        }

        private static int ListDeployments(string[] args)
        {
            var projectName = args[1];

            var deploymentServiceClient = DeploymentServiceClient.Create();
            var listDeploymentsResult = deploymentServiceClient.ListDeployments(new ListDeploymentsRequest
            {
                ProjectName = projectName
            });

            foreach (var deployment in listDeploymentsResult)
            {
                if (deployment.Status == Deployment.Types.Status.Running)
                {
                    Console.WriteLine($"<deployment> {deployment.Id} {deployment.Name}");
                }
            }

            return 0;
        }

        private static void ShowUsage()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine(
                "DeploymentManager create <project-name> <assembly-name> <main-deployment-name> <main-deployment-json> <main-deployment-snapshot> [<sim-deployment-name> <sim-deployment-json> <sim-deployment-snapshot>]");
            Console.WriteLine("DeploymentManager stop <project-name> <deployment-id>");
            Console.WriteLine("DeploymentManager list <project-name>");
        }

        private static int Main(string[] args)
        {
            if (args.Length == 0 ||
                args[0] == "create" && (args.Length != 9 && args.Length != 6) ||
                args[0] == "stop" && args.Length != 3 ||
                args[0] == "list" && args.Length != 2)
            {
                ShowUsage();
                return 1;
            }

            try
            {
                if (args[0] == "create")
                {
                    return CreateDeployment(args);
                }

                if (args[0] == "stop")
                {
                    return StopDeployment(args);
                }

                if (args[0] == "list")
                {
                    return ListDeployments(args);
                }
            }
            catch (Grpc.Core.RpcException e)
            {
                if (e.Status.StatusCode == Grpc.Core.StatusCode.Unauthenticated)
                {
                    Console.WriteLine("<error:unauthenticated>");
                }
                else
                {
                    Console.Error.WriteLine($"Encountered an unknown gRPC error. Exception = {e.ToString()}");
                }
            }
            catch (ArgumentNullException e)
            {
                // This is here to work around WF-464, present as of Platform SDK version 13.5.0.
                if (e.ParamName == "path")
                {
                    Console.WriteLine("<error:authentication>");
                }
                else
                {
                    throw;
                }
            }

            return 1;
        }
    }
}
