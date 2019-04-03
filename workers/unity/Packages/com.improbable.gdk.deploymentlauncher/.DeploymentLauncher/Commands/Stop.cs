using System;
using Improbable.SpatialOS.Deployment.V1Alpha1;

namespace Improbable.Gdk.DeploymentLauncher.Commands
{
    public static class Stop
    {
        public static int StopDeployment(Options.Stop options)
        {
            var deploymentServiceClient = DeploymentServiceClient.Create();

            try
            {
                deploymentServiceClient.StopDeployment(new StopDeploymentRequest
                {
                    Id = options.DeploymentId,
                    ProjectName = options.ProjectName
                });
            }
            catch (Grpc.Core.RpcException e)
            {
                if (e.Status.StatusCode == Grpc.Core.StatusCode.NotFound)
                {
                    // TODO: Write out error properly.
                    Console.WriteLine("<error:unknown-deployment>");
                }
                else
                {
                    throw;
                }
            }

            return 0;
        }
    }
}
