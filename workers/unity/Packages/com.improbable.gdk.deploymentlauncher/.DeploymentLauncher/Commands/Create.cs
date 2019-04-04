using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using Google.Protobuf.WellKnownTypes;
using Improbable.SpatialOS.Deployment.V1Alpha1;
using Improbable.SpatialOS.PlayerAuth.V2Alpha1;
using Improbable.SpatialOS.Snapshot.V1Alpha1;
using Newtonsoft.Json.Linq;

namespace Improbable.Gdk.DeploymentLauncher.Commands
{
    public static class Create
    {
        public static int CreateDeployment(Options.Create options)
        {
            return CreateDeploymentInternal(options, opts => File.ReadAllText(opts.LaunchJsonPath));
        }

        public static int CreateSimulatedPlayerDeployment(Options.CreateSimulated options)
        {
            return CreateDeploymentInternal(options, opts => ModifySimulatedPlayerLaunchJson(options));
        }

        private static int CreateDeploymentInternal<TOptions>(TOptions options, Func<TOptions, string> getLaunchConfigJson)
            where TOptions : Options.Create
        {
            var snapshotServiceClient = SnapshotServiceClient.Create();
            var deploymentServiceClient = DeploymentServiceClient.Create();

            try
            {
                var deployment = new Deployment
                {
                    AssemblyId = options.AssemblyName,
                    LaunchConfig = new LaunchConfig
                    {
                        ConfigJson = getLaunchConfigJson(options)
                    },
                    Name = options.DeploymentName,
                    ProjectName = options.ProjectName,
                    RegionCode = options.Region
                };

                if (options.SnapshotPath != null)
                {
                    var snapshotId = UploadSnapshot(snapshotServiceClient, options.SnapshotPath, options.ProjectName,
                        options.DeploymentName);

                    if (string.IsNullOrEmpty(snapshotId))
                    {
                        return 1;
                    }

                    deployment.StartingSnapshotId = snapshotId;
                }

                if (options.Tags != null)
                {
                    foreach (var tag in options.Tags)
                    {
                        deployment.Tag.Add(tag);
                    }
                }

                var deploymentOp = deploymentServiceClient.CreateDeployment(new CreateDeploymentRequest
                {
                    Deployment = deployment
                }).PollUntilCompleted();

                if (deploymentOp.Result.Status != Deployment.Types.Status.Running)
                {
                    Ipc.WriteError(Ipc.ErrorCode.Other, "Deployment failed to start for an unknown reason.");
                    return 1;
                }
            }
            catch (Grpc.Core.RpcException e)
            {
                if (e.Status.StatusCode == Grpc.Core.StatusCode.NotFound)
                {
                    Ipc.WriteError(Ipc.ErrorCode.NotFound, e.Status.Detail);
                    return 1;
                }

                throw;
            }

            return 0;
        }

        private static string ModifySimulatedPlayerLaunchJson(Options.CreateSimulated options)
        {
            var playerAuthServiceClient = PlayerAuthServiceClient.Create();

            // Create development authentication token used by the simulated players.
            var dat = playerAuthServiceClient.CreateDevelopmentAuthenticationToken(
                new CreateDevelopmentAuthenticationTokenRequest
                {
                    Description = "DAT for sim worker deployment.",
                    Lifetime = Duration.FromTimeSpan(new TimeSpan(7, 0, 0, 0)),
                    ProjectName = options.ProjectName
                });

            // Add worker flags to sim deployment JSON.
            var devAuthTokenIdFlag = new JObject
            {
                { "name", $"{options.FlagPrefix}_simulated_players_dev_auth_token_id" },
                { "value", dat.DevelopmentAuthenticationToken.Id }
            };

            var targetDeploymentFlag = new JObject
            {
                { "name", $"{options.FlagPrefix}_simulated_players_target_deployment" },
                { "value", options.TargetDeployment }
            };

            var launchConfigRaw = File.ReadAllText(options.LaunchJsonPath);
            dynamic launchConfig = JObject.Parse(launchConfigRaw);

            for (var i = 0; i < launchConfig.workers.Count; ++i)
            {
                if (launchConfig.workers[i].worker_type == options.SimulatedCoordinatorWorkerType)
                {
                    launchConfig.workers[i].flags.Add(devAuthTokenIdFlag);
                    launchConfig.workers[i].flags.Add(targetDeploymentFlag);
                }
            }

            return launchConfig.ToString();
        }

        private static string UploadSnapshot(SnapshotServiceClient client, string snapshotPath, string projectName,
            string deploymentName)
        {
            if (!File.Exists(snapshotPath))
            {
                Ipc.WriteError(Ipc.ErrorCode.FileNotFound, $"Could not find snapshot file at: {snapshotPath}");
                return null;
            }
            // Read snapshot.
            var bytes = File.ReadAllBytes(snapshotPath);

            if (bytes.Length == 0)
            {
                Ipc.WriteError(Ipc.ErrorCode.Other, $"Snapshot file at {snapshotPath} has zero bytes.");
                return null;
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
    }
}
