using Improbable.SpatialOS.Deployment.V1Alpha1;

namespace DeploymentManager
{
    public struct DeploymentTemplate
    {
        public string AssemblyId;
        public string DeploymentName;
        public string ProjectName;
        public string SnapshotId;
        public LaunchConfig LaunchConfig;
    }
}
