using System;
using Improbable.SpatialOS.Deployment.V1Alpha1;

namespace Improbable.Gdk.DeploymentLauncher.Commands
{
    public static class List
    {
        public static int ListDeployments(Options.List options)
        {
            var deploymentServiceClient = DeploymentServiceClient.Create();
            var listDeploymentsResult = deploymentServiceClient.ListDeployments(new ListDeploymentsRequest
            {
                ProjectName = options.ProjectName
            });

            foreach (var deployment in listDeploymentsResult)
            {
                if (deployment.Status == Deployment.Types.Status.Running)
                {
                    // TODO: Write out results properly.
                    Console.WriteLine($"<deployment> {deployment.Id} {deployment.Name}");
                }
            }

            return 0;
        }
    }
}
