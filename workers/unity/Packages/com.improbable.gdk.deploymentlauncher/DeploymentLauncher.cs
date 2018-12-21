using Fps;
using Improbable.Gdk.Core;
using Improbable.Gdk.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Improbable.Gdk.DeploymentManager
{
    internal class DeploymentManager
    {
        private const string DeploymentLauncherMenuItem = "SpatialOS/Deployment Launcher";
        private const int DeploymentLauncherPriority = 51;

        private static readonly string DotNetProjectPath =
            Path.GetFullPath(Path.Combine(Tools.Common.GetPackagePath("com.improbable.gdk.deploymentlauncher"), ".DeploymentLauncher/DeploymentLauncher.csproj"));
        private static readonly string DotNetWorkingDirectory = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
        private static readonly string ProjectRootPath =
            Path.Combine(Application.dataPath, "../../../");

        [MenuItem(DeploymentLauncherMenuItem, false, DeploymentLauncherPriority)]
        private static void LaunchDeploymentMenu()
        {
            // Show existing window instance. If one doesn't exist, make one.
            var inspectorWindowType = typeof(EditorWindow).Assembly.GetType("UnityEditor.InspectorWindow");
            var deploymentWindow = EditorWindow.GetWindow<DeploymentEditorWindow>(new Type[] { inspectorWindowType });
            deploymentWindow.titleContent.text = "Deployments";
            deploymentWindow.titleContent.tooltip = "A tab for managing your SpatialOS deployments.";
            deploymentWindow.Show();
        }

        internal class DeploymentInfo
        {
            public string Name;
            public string Id;
        }

        internal class DeploymentEditorWindow : EditorWindow
        {
            private string projectName = "unity_gdk";
            private string assemblyName;
            private string deploymentName;
            private string snapshotPath = "snapshots/cloud.snapshot";
            private string mainLaunchJson = "cloud_launch_small.json";
            private string simWorkerLaunchJson = "cloud_launch_small_sim_workers.json";
            private string simWorkerDeploymentName;
            private bool simWorkerDeploymentEnabled;
            private bool simWorkerCustomDeploymentNameEnabled;

            List<DeploymentInfo> deploymentList;
            int selectedDeployment;

            private Task<bool> runningLaunchTask;
            private Task<bool> runningStopTask;
            private Task<List<DeploymentInfo>> runningListTask;

            void OnEnable()
            {
                deploymentList = new List<DeploymentInfo>();
            }

            void OnGUI()
            {
                projectName = EditorGUILayout.TextField("Project Name", projectName);

                // Deployment launcher.
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Deployment Launcher", EditorStyles.boldLabel);
                assemblyName = EditorGUILayout.TextField("Assembly Name", assemblyName);
                deploymentName = EditorGUILayout.TextField("Deployment Name", deploymentName);
                snapshotPath = EditorGUILayout.TextField("Snapshot Path", snapshotPath);
                mainLaunchJson = EditorGUILayout.TextField("Config", mainLaunchJson);
                simWorkerDeploymentEnabled = EditorGUILayout.BeginToggleGroup("Enable simulated worker deployment", simWorkerDeploymentEnabled);
                simWorkerCustomDeploymentNameEnabled = EditorGUILayout.BeginToggleGroup("Override name", simWorkerCustomDeploymentNameEnabled);
                simWorkerDeploymentName = EditorGUILayout.TextField("Deployment Name", simWorkerCustomDeploymentNameEnabled ? simWorkerDeploymentName : deploymentName + "_sim_worker");
                EditorGUILayout.EndToggleGroup();
                simWorkerLaunchJson = EditorGUILayout.TextField("Config", simWorkerLaunchJson);
                EditorGUILayout.EndToggleGroup();

                EditorGUI.BeginDisabledGroup(runningLaunchTask != null);
                if (GUILayout.Button("Launch deployment"))
                {
                    runningLaunchTask = TriggerLaunchDeploymentAsync();
                }
                EditorGUI.EndDisabledGroup();

                // Deployment manager.
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Deployment List", EditorStyles.boldLabel);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Deployments:");
                if (runningStopTask != null)
                {
                    EditorGUILayout.LabelField($"stopping '{deploymentList[selectedDeployment].Name}'");
                }
                else if (runningListTask != null)
                {
                    EditorGUILayout.LabelField("refreshing");
                }
                else if (deploymentList != null)
                {
                    string[] deploymentNames = new string[deploymentList.Count];
                    for (var i = 0; i < deploymentList.Count; ++i)
                    {
                        deploymentNames[i] = deploymentList[i].Name;
                    }
                    selectedDeployment = EditorGUILayout.Popup(selectedDeployment, deploymentNames);
                }
                EditorGUILayout.EndHorizontal();
                EditorGUI.BeginDisabledGroup(runningListTask != null || runningStopTask != null || deploymentList.Count == 0);
                if (GUILayout.Button("Stop Deployment"))
                {
                    runningStopTask = TriggerStopDeploymentAsync(deploymentList[selectedDeployment].Id);
                }
                EditorGUI.EndDisabledGroup();
                EditorGUI.BeginDisabledGroup(runningListTask != null || runningStopTask != null);
                if (GUILayout.Button("Refresh Deployments"))
                {
                    runningListTask = TriggerListDeploymentsAsync();
                }
                EditorGUI.EndDisabledGroup();
            }

            void Update()
            {
                if (runningStopTask != null && runningStopTask.IsCompleted)
                {
                    try
                    {
                        if (runningStopTask.Result)
                        {
                            deploymentList.RemoveAt(selectedDeployment);
                            selectedDeployment = 0;
                        }
                        else
                        {
                            throw new Exception($"Failed to stop deployment {deploymentList[selectedDeployment].Name} with ID {deploymentList[selectedDeployment].Id}.");
                        }
                    }
                    finally
                    {
                        runningStopTask = null;
                        Repaint();
                    }
                }

                if (runningListTask != null && runningListTask.IsCompleted)
                {
                    try
                    {
                        deploymentList.Clear();
                        if (runningListTask.Result != null)
                        {
                            deploymentList = runningListTask.Result;
                        }
                        else
                        {
                            throw new Exception("Failed to refresh deployments list.");
                        }
                    }
                    finally
                    {
                        runningListTask = null;
                        Repaint();
                    }
                }
            }

            private async Task<bool> TriggerLaunchDeploymentAsync()
            {
                if (!CheckDependencies())
                {
                    return false;
                }

                assemblyName = assemblyName.Trim();
                deploymentName = deploymentName.Trim();

                if (string.IsNullOrEmpty(assemblyName))
                {
                    EditorUtility.DisplayDialog("Unable to start a deployment", "Please specify a valid assembly name.",
                        "Close");
                    return false;
                }

                if (string.IsNullOrEmpty(deploymentName))
                {
                    EditorUtility.DisplayDialog("Unable to start a deployment",
                        "Please specify a valid deployment name.", "Close");
                    return false;
                }

                var simSnapshotPath = GenerateTempSnapshot();

                var arguments = new List<string>{
                    "create",
                    projectName,
                    assemblyName,
                    deploymentName,
                    Path.Combine(ProjectRootPath, mainLaunchJson),
                    Path.Combine(ProjectRootPath, snapshotPath)
                };
                if (simWorkerDeploymentEnabled)
                {
                    arguments.AddRange(new List<string>
                    {
                        deploymentName + "_sim_workers",
                        simSnapshotPath,
                        Path.Combine(ProjectRootPath, simWorkerLaunchJson)
                    });
                }

                var processResult = await RedirectedProcess.RunInAsync(DotNetWorkingDirectory, Tools.Common.DotNetBinary, ConstructArguments(arguments), true, true);
                return processResult.ExitCode != 0;
            }

            private async Task<bool> TriggerStopDeploymentAsync(string deploymentId)
            {
                if (!CheckDependencies())
                {
                    return false;
                }

                var arguments = new List<string>{
                    "stop",
                    projectName,
                    deploymentId
                };
                var processResult = await RedirectedProcess.RunInAsync(DotNetWorkingDirectory, Tools.Common.DotNetBinary, ConstructArguments(arguments), false, true);
                return processResult.ExitCode != 0;
            }

            private async Task<List<DeploymentInfo>> TriggerListDeploymentsAsync()
            {
                if (!CheckDependencies())
                {
                    return null;
                }

                var arguments = new List<string> {
                    "list",
                    projectName
                };
                var processResult = await RedirectedProcess.RunInAsync(DotNetWorkingDirectory, Tools.Common.DotNetBinary, ConstructArguments(arguments), false, true);

                if (processResult.ExitCode != 0)
                {
                    return null;
                }

                // Get deployments from output.
                var deploymentList = new List<DeploymentInfo>();
                foreach (var line in processResult.Stdout)
                {
                    Debug.Log("Line: " + line);
                    var tokens = line.Split(' ');
                    if (tokens.Length != 2)
                    {
                        continue;
                    }
                    deploymentList.Add(new DeploymentInfo
                    {
                        Id = tokens[0],
                        Name = tokens[1]
                    });
                }
                return deploymentList;
            }

            private static string GenerateTempSnapshot()
            {
                string snapshotPath = FileUtil.GetUniqueTempPathInProject();

                Snapshot snapshot = new Snapshot();
                var simulatedPlayerCoordinatorTrigger = FpsEntityTemplates.SimulatedPlayerCoordinatorTrigger();
                snapshot.AddEntity(simulatedPlayerCoordinatorTrigger);
                snapshot.WriteToFile(snapshotPath);

                return snapshotPath;
            }

            private static bool CheckDependencies()
            {
                var hasDotnet = !string.IsNullOrEmpty(Tools.Common.DotNetBinary);

                if (hasDotnet)
                {
                    return true;
                }

                var builder = new StringBuilder();

                builder.AppendLine(
                    "The SpatialOS GDK for Unity requires 'dotnet' on your PATH to run its tooling.");
                builder.AppendLine();

                builder.AppendLine("Could not find 'dotnet' on your PATH.");

                builder.AppendLine();
                builder.AppendLine("If these exist on your PATH, restart Unity and Unity Hub.");
                builder.AppendLine();
                builder.AppendLine("Otherwise, install them by following our setup guide:");
                builder.AppendLine("https://docs.improbable.io/unity/alpha/content/get-started/set-up");

                EditorApplication.delayCall += () =>
                {
                    EditorUtility.DisplayDialog("GDK dependencies check failed", builder.ToString(), "OK");
                };

                return false;
            }

            private static string[] ConstructArguments(List<string> args)
            {
                var baseArgs = new List<string>
                    {
                        "run",
                        "-p",
                        $"\"{DotNetProjectPath}\"",
                        "--",
                    };
                foreach (var arg in args)
                {
                    baseArgs.Add($"\"{arg}\"");
                }
                return baseArgs.ToArray();
            }
        }
    }
}
