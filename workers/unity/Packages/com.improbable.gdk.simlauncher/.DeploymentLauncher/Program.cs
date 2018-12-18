using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using Google.Protobuf.WellKnownTypes;
using Improbable.SpatialOS.Deployment.V1Alpha1;
using Improbable.SpatialOS.PlayerAuth.V2Alpha1;
using Improbable.SpatialOS.Snapshot.V1Alpha1;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Improbable
{
    internal class Program
    {
        private static string UploadSnapshot(SnapshotServiceClient client, string snapshotPath, string projectName, string deploymentName)
        {
            Console.WriteLine($"Uploading {snapshotPath} to project {projectName}");

            // Read snapshot.
            var bytes = File.ReadAllBytes(snapshotPath);
            if (bytes.Length == 0)
            {
                Console.Error.WriteLine($"Unable to load {snapshotPath}. Does the file exist?");
                return "";
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
            var uploadSnapshotResponse = client.UploadSnapshot(new UploadSnapshotRequest {Snapshot = snapshotToUpload});
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
            var projectName = args[1];
            var assemblyName = args[2];
            var mainDeploymentName = args[3];
            var mainDeploymentJson = args[4];
            var mainDeploymentSnapshotFilePath = args[5];
            var simDeploymentName = args[6];
            var simDeploymentJson = args[7];
            var simDeploymentSnapshotFilePath = args[8];

            // Create service clients.
            var playerAuthServiceClient = PlayerAuthServiceClient.Create();
            var snapshotServiceClient = SnapshotServiceClient.Create();
            var deploymentServiceClient = DeploymentServiceClient.Create();

            // Create development authentication token used by the fake players.
            var dat = playerAuthServiceClient.CreateDevelopmentAuthenticationToken(
                new CreateDevelopmentAuthenticationTokenRequest
                {
                    Description = "DAT for sim worker deployment.",
                    Lifetime = Duration.FromTimeSpan(new TimeSpan(7, 0, 0, 0)),
                    ProjectName = projectName
                });
            Console.WriteLine(dat.DevelopmentAuthenticationToken.Id);

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

            // Upload snapshots.
            var mainSnapshotId = UploadSnapshot(snapshotServiceClient, mainDeploymentSnapshotFilePath, projectName,
                mainDeploymentName);
            var simSnapshotId = UploadSnapshot(snapshotServiceClient, simDeploymentSnapshotFilePath, projectName,
                simDeploymentName);
            if (mainSnapshotId.Length == 0 || simSnapshotId.Length == 0)
            {
                return 1;
            }

            // Create both deployments.
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
            mainDeploymentConfig.Tag.Add("dev_login");
            var simDeploymentConfig = new Deployment
            {
                AssemblyId = assemblyName,
                LaunchConfig = new LaunchConfig
                {
                    ConfigJson = simWorkerConfigJson
                },
                Name = simDeploymentName,
                ProjectName = projectName,
                StartingSnapshotId = simSnapshotId
            };
            simDeploymentConfig.Tag.Add("simulated_clients");

            Console.WriteLine(
                $"Creating the main deployment {mainDeploymentName} in project {projectName} with snapshot ID {mainSnapshotId}.");
            var mainDeploymentCreateOp = deploymentServiceClient.CreateDeployment(new CreateDeploymentRequest
            {
                Deployment = mainDeploymentConfig
            }).PollUntilCompleted();
            Console.WriteLine("Successfully created the main deployment.");
            Console.WriteLine(
                $"Creating the sim worker deployment {simDeploymentName} in project {projectName} with snapshot ID {simSnapshotId}.");
            var simDeploymentCreateOp = deploymentServiceClient.CreateDeployment(new CreateDeploymentRequest
            {
                Deployment = simDeploymentConfig
            }).PollUntilCompleted();
            Console.WriteLine("Successfully created the sim worker deployment.");

            return 0;
        }

        private static int DestroyDeployment(string[] args)
        {
            var projectName = args[1];
            var mainDeploymentName = args[2];
            var simDeploymentName = args[3];

            var deploymentServiceClient = DeploymentServiceClient.Create();
            Console.WriteLine($"Stopping the main deployment {mainDeploymentName} in project {projectName}.");
            deploymentServiceClient.StopDeployment(new StopDeploymentRequest
            {
                Id = mainDeploymentName,
                ProjectName = projectName
            });
            Console.WriteLine($"Stopping the main deployment {simDeploymentName} in project {projectName}.");
            deploymentServiceClient.StopDeployment(new StopDeploymentRequest
            {
                Id = simDeploymentName,
                ProjectName = projectName
            });

            return 0;
        }

        private static void ShowUsage()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("DeploymentLauncher.exe create <project-name> <assembly-name> <main-deployment-name> <main-deployment-json> <main-deployment-snapshot> <sim-deployment-name> <sim-deployment-json> <sim-deployment-snapshot>");
            Console.WriteLine("DeploymentLauncher.exe destroy <project-name> <main-deployment-name> <sim-deployment-name>");
        }

        private static int Main(string[] args)
        {
            if (args.Length == 0 ||
                args[0] == "create" && args.Length != 9 ||
                args[0] == "destroy" && args.Length != 4)
            {
                ShowUsage();
                return 1;
            }

            if (args[0] == "create")
            {
                return CreateDeployment(args);
            }

            if (args[0] == "destroy")
            {
                return DestroyDeployment(args);
            }

            ShowUsage();
            return 1;
        }
    }
}
