using System;
using System.Collections.Generic;
using System.Linq;
using Improbable.Gdk.Tools.MiniJSON;

namespace Improbable.Gdk.DeploymentManager
{
    public static class Ipc
    {
        public enum ErrorCode : uint
        {
            FileNotFound = 1,
            Unauthenticated = 2,
            NotFound = 3,
            UnknownGrpcError = 4,
            Other = 5
        }

        public class Error
        {
            public uint Code;
            public string Message;

            public static Error FromStderr(List<string> stderr)
            {
                if (stderr.Count == 0)
                {
                    throw new ArgumentException("Cannot parse error from empty stderr.");
                }

                // We expect only the first line to be valid.
                var deserialized = Json.Deserialize(stderr[0]);

                return new Error
                {
                    Code = Convert.ToUInt32(deserialized["Code"]),
                    Message = (string) deserialized["Message"]
                };
            }
        }

        public static List<DeploymentInfo> GetDeploymentInfo(List<string> stdout, string projectName)
        {
            if (stdout.Count == 0)
            {
                throw new ArgumentException("Cannot parse deployment list from empty stdout.");
            }

            // We expect the first line to have all the JSON.
            var deserialized = Json.Deserialize(stdout[0]);
            var deployments = (List<object>) deserialized["Deployments"];

            return deployments.Select(depl =>
            {
                var json = (Dictionary<string, object>) depl;

                return new DeploymentInfo(projectName, (string) json["Name"], (string) json["Id"]);
            }).ToList();
        }
    }
}
