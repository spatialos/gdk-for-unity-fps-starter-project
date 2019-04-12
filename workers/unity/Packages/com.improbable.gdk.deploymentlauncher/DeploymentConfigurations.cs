using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Improbable.Gdk.Tools.MiniJSON;
using UnityEditor;
using UnityEngine;

namespace Improbable.Gdk.DeploymentManager
{
    [Serializable]
    public class DeploymentConfig
    {
        public class Errors
        {
            public readonly Dictionary<string, List<string>> DeplErrors = new Dictionary<string, List<string>>();

            public bool Any()
            {
                return DeplErrors.Any(pair => pair.Value.Count != 0);
            }

            public string FormatErrors()
            {
                var sb = new StringBuilder();

                foreach (var pair in DeplErrors)
                {
                    var deplName = pair.Key;

                    foreach (var error in pair.Value)
                    {
                        sb.AppendLine($" - {deplName}: {error}");
                    }
                }

                return sb.ToString();
            }
        }

        /// <summary>
        ///     The name of the assembly to use in the deployment.
        /// </summary>
        public string AssemblyName;

        /// <summary>
        ///     The main deployment configuration.
        /// </summary>
        public BaseDeploymentConfig Deployment;

        /// <summary>
        ///     List of simulated player deployments that will target this deployment.
        /// </summary>
        public List<SimulatedPlayerDeploymentConfig> SimulatedPlayerDeploymentConfig;

        public DeploymentConfig()
        {
            AssemblyName = string.Empty;
            Deployment = new BaseDeploymentConfig();
            SimulatedPlayerDeploymentConfig = new List<SimulatedPlayerDeploymentConfig>();
        }

        public Errors GetErrors()
        {
            var errors = new Errors();

            errors.DeplErrors.Add(Deployment.Name, Deployment.GetErrors().ToList());

            if (!ValidateAssembly(out var message))
            {
                errors.DeplErrors[Deployment.Name].Add(message);
            }


            foreach (var simPlayerDepl in SimulatedPlayerDeploymentConfig)
            {
                errors.DeplErrors.Add(simPlayerDepl.Name, simPlayerDepl.GetErrors().ToList());
            }

            return errors;
        }

        /// <summary>
        ///     Deep copy this configuration object.
        /// </summary>
        /// <returns>A copy of this <see cref="DeploymentConfig" /> object.</returns>
        internal DeploymentConfig DeepCopy()
        {
            return new DeploymentConfig
            {
                AssemblyName = AssemblyName,
                Deployment = Deployment.DeepCopy(),
                SimulatedPlayerDeploymentConfig = SimulatedPlayerDeploymentConfig.Select(config => config.DeepCopy()).ToList(),
            };
        }

        private bool ValidateAssembly(out string message)
        {
            if (string.IsNullOrEmpty(AssemblyName))
            {
                message = "Assembly Name cannot be empty.";
                return false;
            }


            if (!Regex.Match(AssemblyName, "^[a-zA-Z0-9_.-]{5,64}$").Success)
            {
                message = $"Assembly Name \"{AssemblyName}\" is invalid. Must conform to the regex: ^[a-zA-Z0-9_.-]{{5,64}}";
                return false;
            }

            message = null;
            return true;
        }
    }

    /// <summary>
    ///     Configuration that is specific to simulated player deployments.
    /// </summary>
    [Serializable]
    public class SimulatedPlayerDeploymentConfig : BaseDeploymentConfig
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

        public SimulatedPlayerDeploymentConfig()
        {
            TargetDeploymentName = string.Empty;
            FlagPrefix = string.Empty;
            WorkerType = string.Empty;
        }

        internal new SimulatedPlayerDeploymentConfig DeepCopy()
        {
            return new SimulatedPlayerDeploymentConfig
            {
                Name = Name,
                SnapshotPath = SnapshotPath,
                LaunchJson = LaunchJson,
                Region = Region,
                Tags = Tags.Select(string.Copy).ToList(),

                TargetDeploymentName = TargetDeploymentName,
                FlagPrefix = FlagPrefix,
                WorkerType = WorkerType
            };
        }
    }

    [Serializable]
    public class BaseDeploymentConfig
    {
        /// <summary>
        ///     The name of the deployment to launch.
        /// </summary>
        public string Name;

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
        ///     Tags to add to the deployment.
        /// </summary>
        public List<string> Tags;

        public BaseDeploymentConfig()
        {
            Name = string.Empty;
            SnapshotPath = string.Empty;
            LaunchJson = string.Empty;
            Region = DeploymentRegionCode.EU;
            Tags = new List<string>();
        }

        internal BaseDeploymentConfig DeepCopy()
        {
            return new BaseDeploymentConfig
            {
                Name = Name,
                SnapshotPath = SnapshotPath,
                LaunchJson = LaunchJson,
                Region = Region,
                Tags = Tags.ToList()
            };
        }

        internal IEnumerable<string> GetErrors()
        {
            {
                if (!ValidateName(out var message))
                {
                    yield return message;
                }
            }

            {
                if (!ValidateLaunchJson(out var message))
                {
                    yield return message;
                }
            }

            {
                if (!ValidateSnapshotPath(out var message))
                {
                    yield return message;
                }
            }

            {
                foreach (var tag in Tags)
                {
                    if (!ValidateTag(tag, out var message))
                    {
                        yield return message;
                    }
                }
            }
        }

        private bool ValidateName(out string message)
        {
            if (string.IsNullOrEmpty(Name))
            {
                message = "Deployment Name cannot be empty.";
                return false;
            }

            if (!Regex.Match(Name, "^[a-z0-9_]{2,32}$").Success)
            {
                message = $"Deployment Name \"{Name}\" invalid. Must conform to the regex: ^[a-z0-9_]{{2,32}}$";
                return false;
            }

            message = null;
            return true;
        }

        private bool ValidateLaunchJson(out string message)
        {
            if (string.IsNullOrEmpty(LaunchJson))
            {
                message = "Launch Config cannot be empty";
                return false;
            }

            var filePath = Path.Combine(Tools.Common.SpatialProjectRootDir, LaunchJson);

            if (!File.Exists(filePath))
            {
                message = $"Launch Config file at {filePath} cannot be found.";
                return false;
            }

            message = null;
            return true;
        }

        private bool ValidateSnapshotPath(out string message)
        {
            if (string.IsNullOrEmpty(SnapshotPath))
            {
                message = null;
                return true;
            }

            var filePath = Path.Combine(Tools.Common.SpatialProjectRootDir, SnapshotPath);

            if (!File.Exists(filePath))
            {
                message = $"Snapshot file at {filePath} cannot be found.";
                return false;
            }

            message = null;
            return true;
        }

        private bool ValidateTag(string tag, out string message)
        {
            if (!Regex.IsMatch(tag, "^[A-Za-z0-9][A-Za-z0-9_]{2,32}$"))
            {
                message = $"Tag \"{tag}\" invalid. Must conform to the regex: ^[A-Za-z0-9][A-Za-z0-9_]{{2,32}}$";
                return false;
            }

            message = null;
            return true;
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

    [Serializable]
    public class AssemblyConfig
    {
        /// <summary>
        ///     The name of this assembly.
        /// </summary>
        public string AssemblyName;

        public bool ShouldForceUpload;

        internal AssemblyConfig DeepCopy()
        {
            return new AssemblyConfig
            {
                AssemblyName = AssemblyName,
                ShouldForceUpload = ShouldForceUpload
            };
        }
    }

    // TODO: Remove after moved to Tools

    public abstract class SingletonScriptableObject<TSelf> : ScriptableObject
        where TSelf : SingletonScriptableObject<TSelf>
    {
        private static readonly List<TSelf> Instances =
            new List<TSelf>();

        public virtual void OnEnable()
        {
            if (!IsAnAsset())
            {
                // This is not an asset, so don't register it as an instance.
                return;
            }

            var self = (TSelf) this;

            if (Instances.Find(instance => instance != self))
            {
                Debug.LogError(
                    $"There are multiple copies of {SelfType} present. Please pick one and delete the other.");
            }

            if (!Instances.Contains(self))
            {
                Instances.Add(self);
            }
        }

        protected bool IsAnAsset()
        {
            var assetPath = AssetDatabase.GetAssetPath(this);

            // If there is an asset path, it is in assets.
            return !string.IsNullOrEmpty(assetPath);
        }

        public void OnDisable()
        {
            if (!IsAnAsset())
            {
                return;
            }

            var self = (TSelf) this;

            if (Instances.Contains(self))
            {
                Instances.Remove(self);
            }
        }

        private static readonly Type SelfType = typeof(TSelf);

        public static TSelf GetInstance()
        {
            // Clean up dead ones.
            Instances.RemoveAll(item => item == null);

            if (Instances.Count > 0)
            {
                return Instances[0];
            }

            if (SingletonScriptableObjectLoader.LoadingInstances.Contains(SelfType))
            {
                return null;
            }

            SingletonScriptableObjectLoader.LoadingInstances.Add(SelfType);

            try
            {
                var allInstanceGuidsInAssetDatabase =
                    AssetDatabase.FindAssets("t:" + SelfType.Name);

                foreach (var instanceGUID in allInstanceGuidsInAssetDatabase)
                {
                    var instancePath = AssetDatabase.GUIDToAssetPath(instanceGUID);

                    var loadedInstance = AssetDatabase.LoadAssetAtPath<TSelf>(instancePath);

                    // onload should have been called here, but if not, ensure it's in the list.
                    if (loadedInstance == null)
                    {
                        continue;
                    }

                    if (Instances.Find(instance => instance != loadedInstance))
                    {
                        Debug.LogError(
                            $"There are multiple copies of {SelfType} present. Please pick one and delete the other.");
                    }

                    if (!Instances.Contains(loadedInstance))
                    {
                        Instances.Add(loadedInstance);
                    }
                }
            }
            finally
            {
                SingletonScriptableObjectLoader.LoadingInstances.Remove(SelfType);
            }

            return Instances.FirstOrDefault();
        }
    }

    internal static class SingletonScriptableObjectLoader
    {
        internal static readonly HashSet<Type> LoadingInstances = new HashSet<Type>();
    }
}
