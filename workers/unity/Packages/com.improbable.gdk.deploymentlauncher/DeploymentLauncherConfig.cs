using System.Collections.Generic;
using Improbable.Gdk.Tools;
using UnityEngine;

namespace Improbable.Gdk.DeploymentManager
{
    [CreateAssetMenu(fileName = "SpatialOS Deployment Launcher Config", menuName = EditorConfig.ParentMenu + "/Deployment Launcher Config")]
    internal class DeploymentLauncherConfig : SingletonScriptableObject<DeploymentLauncherConfig>
    {
        [SerializeField] public AssemblyConfig AssemblyConfig = new AssemblyConfig();
        [SerializeField] public List<DeploymentConfig> DeploymentConfigs = new List<DeploymentConfig>();
    }
}
