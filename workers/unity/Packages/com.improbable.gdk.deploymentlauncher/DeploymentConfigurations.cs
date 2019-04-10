using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Improbable.Gdk.DeploymentManager
{
    [Serializable]
    public class DeploymentConfig
    {
        /// <summary>
        ///     The name of the SpatialOS project to launch in.
        /// </summary>
        public string ProjectName;

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
            ProjectName = "";
            AssemblyName = "";
            Deployment = new BaseDeploymentConfig();
            SimulatedPlayerDeploymentConfig = new List<SimulatedPlayerDeploymentConfig>();
        }

        public IEnumerable<string> GetErrors()
        {
            if (!ValidateAssembly())
            {
                yield return $"Assembly name {AssemblyName} invalid. Must follow the regex: ^[a-zA-Z0-9_.-]{{5,64}}";
            }

            if (!Deployment.ValidateName())
            {
                yield return $"Deployment name {Deployment.Name} invalid. Must follow the regex: ^[a-z0-9_]{{2,32}}$";
            }

            foreach (var simPlayerDepl in SimulatedPlayerDeploymentConfig)
            {
                if (!simPlayerDepl.ValidateName())
                {
                    yield return $"Deployment name {Deployment.Name} invalid. Must follow the regex: ^[a-z0-9_]{{2,32}}$";
                }
            }
        }

        /// <summary>
        ///     Deep copy this configuration object.
        /// </summary>
        /// <returns>A copy of this <see cref="DeploymentConfig" /> object.</returns>
        internal DeploymentConfig DeepCopy()
        {
            return new DeploymentConfig
            {
                ProjectName = ProjectName,
                AssemblyName = AssemblyName,
                Deployment = Deployment.DeepCopy(),
                SimulatedPlayerDeploymentConfig = SimulatedPlayerDeploymentConfig.Select(config => config.DeepCopy()).ToList(),
            };
        }

        private bool ValidateAssembly()
        {
            return !string.IsNullOrEmpty(AssemblyName) && Regex.Match(AssemblyName, "^[a-zA-Z0-9_.-]{5,64}$").Success;
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
            TargetDeploymentName = "";
            FlagPrefix = "";
            WorkerType = "";
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
            Name = "";
            SnapshotPath = "";
            LaunchJson = "";
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
                Tags = Tags.Select(string.Copy).ToList()
            };
        }

        internal bool ValidateName()
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
                ProjectName = ProjectName,
                AssemblyName = AssemblyName
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
