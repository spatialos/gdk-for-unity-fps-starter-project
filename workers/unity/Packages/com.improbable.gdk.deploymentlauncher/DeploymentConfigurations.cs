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
                SnapshotPath = string.Copy(SnapshotPath),
                LaunchJson = string.Copy(LaunchJson),
                Region = Region
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
