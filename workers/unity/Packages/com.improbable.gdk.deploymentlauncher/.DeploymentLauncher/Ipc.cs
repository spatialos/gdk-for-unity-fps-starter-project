using System;
using System.Collections.Generic;
using System.Linq;
using Improbable.SpatialOS.Deployment.V1Alpha1;
using Newtonsoft.Json;

namespace Improbable.Gdk.DeploymentLauncher
{
    /// <summary>
    ///     Methods for inter-process communication with the parent process (Unity). All via JSON.
    /// </summary>
    public static class Ipc
    {
        public enum ErrorCode : uint
        {
            FileNotFound = 1,
            Unauthenticated = 2,
            NotFound = 3,
            UnknownGrpcError = 4,
            SnapshotUploadFailed = 5,
            OperationCancelled = 6,
            Unknown = 7
        }

        public static void WriteError(ErrorCode code, string message)
        {
            var json = JsonConvert.SerializeObject(new Error(code, message));
            Console.Error.WriteLine(json);
        }

        public static void WriteDeploymentInfo(IEnumerable<Deployment> deployments)
        {
            var wrapper = new DeploymentListWrapper
            {
                Deployments = deployments.Select(depl => new InternalDeployment(depl)).ToList()
            };
            var json = JsonConvert.SerializeObject(wrapper);
            Console.WriteLine(json);
        }

        private struct Error
        {
            public uint Code;
            public string Message;

            public Error(ErrorCode code, string message)
            {
                Code = (uint) code;
                Message = message;
            }
        }

        private struct InternalDeployment
        {
            public string Id;
            public string Name;

            public InternalDeployment(Deployment deployment)
            {
                Id = deployment.Id;
                Name = deployment.Name;
            }
        }

        // Force Newtonsoft to _not_ serialize the list as the root object.
        private struct DeploymentListWrapper
        {
            public List<InternalDeployment> Deployments;
        }
    }
}
