using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using CommandLine;
using Google.Protobuf.WellKnownTypes;
using Improbable.SpatialOS.Deployment.V1Alpha1;
using Improbable.SpatialOS.PlayerAuth.V2Alpha1;
using Improbable.SpatialOS.Snapshot.V1Alpha1;
using Newtonsoft.Json.Linq;

namespace Improbable.Gdk.DeploymentLauncher
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            try
            {
                return Parser.Default.ParseArguments<Options.Create, Options.CreateSimulated, Options.Stop, Options.List>(args)
                    .MapResult(
                        (Options.Create createOptions) => Commands.Create.CreateDeployment(createOptions),
                        (Options.CreateSimulated createOptions) => Commands.Create.CreateSimulatedPlayerDeployment(createOptions),
                        (Options.Stop stopOptions) => Commands.Stop.StopDeployment(stopOptions),
                        (Options.List listOptions) => Commands.List.ListDeployments(listOptions),
                        errs => 1
                    );
            }
            catch (Grpc.Core.RpcException e)
            {
                // TODO: Write out in a nice way.
                if (e.Status.StatusCode == Grpc.Core.StatusCode.Unauthenticated)
                {
                    Console.WriteLine("<error:unauthenticated>");
                    return 1;
                }

                Console.Error.WriteLine($"Encountered an unknown gRPC error. Exception = {e.ToString()}");
                return 1;
            }
            catch (ArgumentNullException e)
            {
                // TODO: Look and see if this was fixed.
                // This is here to work around WF-464, present as of Platform SDK version 13.5.0.
                if (e.ParamName == "path")
                {
                    Console.WriteLine("<error:authentication>");
                    return 1;
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
