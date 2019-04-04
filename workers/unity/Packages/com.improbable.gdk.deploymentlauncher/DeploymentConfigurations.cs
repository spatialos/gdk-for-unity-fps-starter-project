using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Improbable.Gdk.DeploymentManager
{
    public class DeploymentConfig
    {
        /// <summary>
        ///     The name of the deployment to launch.
        /// </summary>
        public string Name;

        /// <summary>
        ///     The name of the SpatialOS project to launch in.
        /// </summary>
        public string ProjectName;

        /// <summary>
        ///     The name of the assembly to use in the deployment.
        /// </summary>
        public string AssemblyName;

        /// <summary>
        ///     The relative path from the root of the SpatialOS project to the snapshot.
        /// </summary>
        public string SnapshotPath;

        /// <summary>
        ///     The relative path from the root of the SpatialOS project to the launch json.
        /// </summary>
        public string LaunchJson;

        /// <summary>
        ///     The region to launch the deployment in.
        /// </summary>
        public DeploymentRegionCode Region;

        /// <summary>
        ///     Configuration that is specific to simulated player deployments. This will be non-null if this
        ///     deployment is a simulated player deployment.
        /// </summary>
        public SimulatedPlayerDeploymentConfig SimulatedPlayerDeploymentConfig;

        /// <summary>
        ///     Tags to add to the deployment.
        /// </summary>
        public List<string> Tags = new List<string>();

        /// <summary>
        ///     Deep copy this configuration object.
        /// </summary>
        /// <returns>A copy of this <see cref="DeploymentConfig"/> object.</returns>
        internal DeploymentConfig DeepCopy()
        {
            return new DeploymentConfig
            {
                Name = string.Copy(Name),
                ProjectName = string.Copy(ProjectName),
                AssemblyName = string.Copy(AssemblyName),
                SnapshotPath = SnapshotPath == null ? null : string.Copy(SnapshotPath),
                LaunchJson = string.Copy(LaunchJson),
                Region = Region,
                SimulatedPlayerDeploymentConfig = SimulatedPlayerDeploymentConfig?.DeepCopy(),
                Tags = Tags.Select(string.Copy).ToList()
            };
        }

        private bool ValidateAssembly()
        {
            return !string.IsNullOrEmpty(AssemblyName) && Regex.Match(AssemblyName, "^[a-zA-Z0-9_.-]{5,64}$").Success;
        }

        private bool ValidateNameName()
        {
            return !string.IsNullOrEmpty(Name) && Regex.Match(Name, "^[a-z0-9_]{2,32}$").Success;
        }
    }

    /// <summary>
    ///     Configuration that is specific to simulated player deployments.
    /// </summary>
    public class SimulatedPlayerDeploymentConfig
    {
        /// <summary>
        ///     The name of the deployment that the simulated players should connect into.
        /// </summary>
        public string TargetDeploymentName;

        /// <summary>
        ///     The flag prefix for the simulated player coordinator worker flags.
        /// </summary>
        public string FlagPrefix;

        /// <summary>
        ///     The simulated player coordinator worker type.
        /// </summary>
        public string WorkerType;

        internal SimulatedPlayerDeploymentConfig DeepCopy()
        {
            return new SimulatedPlayerDeploymentConfig
            {
                TargetDeploymentName = string.Copy(TargetDeploymentName),
                FlagPrefix = string.Copy(FlagPrefix),
                WorkerType = string.Copy(WorkerType)
            };
        }
    }

    public enum DeploymentRegionCode
    {
        US,
        EU
    }

    public class DeploymentInfo
    {
        /// <summary>
        ///     The SpatialOS project that the deployment is running in.
        /// </summary>
        public string ProjectName { get; }

        /// <summary>
        ///     The name of the deployment.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     The id of the deployment.
        /// </summary>
        public string Id { get; }

        public DeploymentInfo(string projectName, string name, string id)
        {
            ProjectName = projectName;
            Name = name;
            Id = id;
        }
    }

    public class AssemblyConfig
    {
        /// <summary>
        ///     The project to upload this assembly to.
        /// </summary>
        public string ProjectName;

        /// <summary>
        ///     The name of this assembly.
        /// </summary>
        public string AssemblyName;

        internal AssemblyConfig DeepCopy()
        {
            return new AssemblyConfig
            {
                ProjectName = string.Copy(ProjectName),
                AssemblyName = string.Copy(AssemblyName)
            };
        }
    }
}
