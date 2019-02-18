using Improbable.SpatialOS.Deployment.V1Alpha1;
using LogLevel = Improbable.Worker.LogLevel;
using System.Collections.Generic;
using System.Linq;
using Improbable.SpatialOS.Platform.Common;
using Improbable.SpatialOS.Snapshot.V1Alpha1;
using System.Security.Cryptography;
using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;

namespace DeploymentManager
{
    public class DeploymentModifier
    {
        private static DeploymentServiceClient deploymentServiceClient;
        private static SnapshotServiceClient snapshotServiceClient; 

        public static void Init(string token)
        {
            var credentials = new PlatformRefreshTokenCredential(token);
            deploymentServiceClient = DeploymentServiceClient.Create(credentials: credentials);

            snapshotServiceClient = SnapshotServiceClient.Create(credentials: credentials);
        }

        public static string UploadSnapshot(string snapshotPath, string projectName, string deploymentName)
        {
            // Read snapshot.
            var result = string.Empty;
            var assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream($"DeploymentManager.default.snapshot"))
            using (StreamReader reader = new StreamReader(stream))
            {
                result = reader.ReadToEnd();
            }

            var bytes = Encoding.ASCII.GetBytes(result);

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
                snapshotServiceClient.UploadSnapshot(new UploadSnapshotRequest { Snapshot = snapshotToUpload });
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
            var confirmUploadResponse = snapshotServiceClient.ConfirmUpload(new ConfirmUploadRequest
            {
                DeploymentName = snapshotToUpload.DeploymentName,
                Id = snapshotToUpload.Id,
                ProjectName = snapshotToUpload.ProjectName
            });

            return confirmUploadResponse.Snapshot.Id;
        }

        public static Deployment GetDeployment(string deploymentId, string projectName)
        {
            var request = new GetDeploymentRequest
            {
                Id = deploymentId,
                ProjectName = projectName,
            };

            var response = deploymentServiceClient.GetDeployment(request);
            return response.Deployment;
        }

        public static void AddDeploymentTag(string deploymentId, string projectName, string tag)
        {
            var deployment = GetDeployment(deploymentId, projectName);

            if (!deployment.Tag.Contains(tag))
            {
                deployment.Tag.Add(tag);

                var request = new UpdateDeploymentRequest
                {
                    Deployment = deployment
                };

                deploymentServiceClient.UpdateDeployment(request);
            }
        }

        public static void RemoveDeploymentTag(string deploymentId, string projectName, string tag)
        {
            var deployment = GetDeployment(deploymentId, projectName);

            if (deployment.Tag.Contains(tag))
            {
                deployment.Tag.Remove(tag);

                var request = new UpdateDeploymentRequest
                {
                    Deployment = deployment
                };

                deploymentServiceClient.UpdateDeployment(request);
            }
        }

        public static void UpdateDeploymentTag(string deploymentId, string projectName, string tagPrefix, string value)
        {
            var deployment = GetDeployment(deploymentId, projectName);
            var tags = deployment.Tag.ToList();
            foreach (var tag in tags)
            {
                if (tag.Contains(tagPrefix))
                {
                    deployment.Tag.Remove(tag);
                }
            }
            deployment.Tag.Add($"{tagPrefix}_{value}");

            var request = new UpdateDeploymentRequest
            {
                Deployment = deployment
            };

            deploymentServiceClient.UpdateDeployment(request);
        }

        public static Google.LongRunning.Operation<Deployment, CreateDeploymentMetadata> CreateDeployment(DeploymentTemplate template)
        {
                var request = new CreateDeploymentRequest
                {
                    Deployment = new Deployment
                    {
                        AssemblyId = template.AssemblyId,
                        LaunchConfig = template.LaunchConfig,
                        Name = template.DeploymentName,
                        ProjectName = template.ProjectName,
                        StartingSnapshotId = template.SnapshotId,
                    }
                };

                return deploymentServiceClient.CreateDeployment(request);
        }

        public static void StopDeployment(Deployment deployment)
        {
                var request = new StopDeploymentRequest
                {
                    Id = deployment.Id,
                    ProjectName = deployment.ProjectName,
                };

                deploymentServiceClient.StopDeployment(request);
        }

        public static List<Deployment> ListDeployments(string projectName)
        {
            var request = new ListDeploymentsRequest
            {
                ProjectName = projectName,
            };

            var response = deploymentServiceClient.ListDeployments(request);
            return response.ToList();
        }

        public static int GetMaxWorkerCapacity(string deploymentId, string projectName, string workerType)
        {
            var deployment = GetDeployment(deploymentId, projectName);
            foreach (var workerCapacity in deployment.WorkerConnectionCapacities)
            {
                if (workerCapacity.WorkerType == workerType)
                {
                    return workerCapacity.MaxCapacity;
                }
            }

            Log.Print(LogLevel.Error, $"Couldn't find worker type {workerType} in deployment {deployment.Name}");
            return -1;
        }

        public static int GetRemainingWorkerCapacity(string deploymentId, string projectName, string workerType)
        {
            var deployment = GetDeployment(deploymentId, projectName);
            foreach (var workerCapacity in deployment.WorkerConnectionCapacities)
            {
                if (workerCapacity.WorkerType == workerType)
                {
                    return workerCapacity.RemainingCapacity;
                }
            }

            Log.Print(LogLevel.Error, $"Couldn't find worker type {workerType} in deployment {deployment.Name}");
            return -1;
        }
    }
}
