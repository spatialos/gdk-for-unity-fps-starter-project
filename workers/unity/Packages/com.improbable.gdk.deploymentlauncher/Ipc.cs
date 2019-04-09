using System;
using System.Collections.Generic;
using System.Linq;
using Improbable.Gdk.Core.Collections;
using Improbable.Gdk.Tools.MiniJSON;
using UnityEngine;

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
            SnapshotUploadFailed = 5,
            OperationCancelled = 6,
            Unknown = 7,

            // Additional error code used by the parsing logic.
            CannotParseOutput = 8
        }

        public class Error
        {
            public ErrorCode Code;
            public string Message;

            public static Error FromStderr(IReadOnlyList<string> stderr)
            {
                if (stderr == null || stderr.Count == 0)
                {
                    throw new ArgumentException("Cannot parse error from empty stderr.");
                }

                try
                {
                    // We expect only the first line to be valid.
                    var deserialized = Json.Deserialize(stderr[0]);

                    return new Error
                    {
                        Code = (ErrorCode) Convert.ToUInt32(deserialized["Code"]),
                        Message = (string) deserialized["Message"]
                    };
                }
                catch (InvalidCastException e)
                {
                    return new Error
                    {
                        Code = ErrorCode.CannotParseOutput,
                        Message = $"Parse error: {e.Message}.\nRaw stderr: {string.Join("\n", stderr)}"
                    };
                }
                catch (KeyNotFoundException e)
                {
                    return new Error
                    {
                        Code = ErrorCode.CannotParseOutput,
                        Message = $"Parse error: {e.Message}.\nRaw stderr: {string.Join("\n", stderr)}"
                    };
                }
            }
        }

        public static Result<List<DeploymentInfo>, Error> GetDeploymentInfo(IReadOnlyList<string> stdout, string projectName)
        {
            if (stdout.Count == 0)
            {
                throw new ArgumentException("Cannot parse deployment list from empty stdout.");
            }

            try
            {
                // We expect the first line to have all the JSON.
                var deserialized = Json.Deserialize(stdout[0]);
                var deployments = (List<object>) deserialized["Deployments"];

                return Result<List<DeploymentInfo>, Error>.Ok(deployments.Select(depl =>
                {
                    var json = (Dictionary<string, object>) depl;

                    return new DeploymentInfo(projectName, (string) json["Name"], (string) json["Id"]);
                }).ToList());
            }
            catch (InvalidCastException e)
            {
                return Result<List<DeploymentInfo>, Error>.Error(new Error
                {
                    Code = ErrorCode.CannotParseOutput,
                    Message = $"Parse error: {e.Message}.\nRaw stderr: {string.Join("\n", stdout)}"
                });
            }
            catch (KeyNotFoundException e)
            {
                return Result<List<DeploymentInfo>, Error>.Error(new Error
                {
                    Code = ErrorCode.CannotParseOutput,
                    Message = $"Parse error: {e.Message}.\nRaw stderr: {string.Join("\n", stdout)}"
                });
            }
        }
    }
}
