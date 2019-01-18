using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Fps;
using Improbable.Gdk.Core;
using Improbable.Gdk.Tools;
using UnityEditor;
using UnityEngine;

namespace Improbable.Gdk.DeploymentManager
{
    internal class DeploymentManager
    {
        private const string DeploymentLauncherMenuItem = "SpatialOS/Deployment Launcher";
        private const int DeploymentLauncherPriority = 51;

        private static readonly string DotNetProjectPath =
            Path.GetFullPath(Path.Combine(Tools.Common.GetPackagePath("com.improbable.gdk.deploymentlauncher"),
                ".DeploymentLauncher/DeploymentLauncher.csproj"));

        private static readonly string DotNetWorkingDirectory =
            Path.GetFullPath(Path.Combine(Application.dataPath, ".."));

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
            private string uploadAssemblyName = "";
            private string assemblyName = "";
            private string deploymentName = "";
            private string snapshotPath = "snapshots/cloud.snapshot";
            private string mainLaunchJson = "cloud_launch_small.json";
            private string simPlayerLaunchJson = "cloud_launch_small_sim_players.json";
            private string simPlayerDeploymentName = "";
            private bool simPlayerDeploymentEnabled;
            private bool simPlayerCustomDeploymentNameEnabled;

            private List<DeploymentInfo> deploymentList;
            private int selectedDeployment;

            private Task<bool> runningUploadAssemblyTask;
            private Task<bool> runningLaunchTask;
            private Task<bool> runningStopTask;
            private Task<List<DeploymentInfo>> runningListTask;

            private void OnEnable()
            {
                deploymentList = new List<DeploymentInfo>();
            }

            private void OnGUI()
            {
                var newProjectName = EditorGUILayout.TextField("Project Name", projectName);
                if (!string.Equals(newProjectName, projectName))
                {
                    deploymentList.Clear();
                }

                projectName = newProjectName;
                var validProjectName = true;
                if (!ValidateProjectName(projectName))
                {
                    EditorGUILayout.HelpBox("Please specify a valid project name.", MessageType.Error);
                    validProjectName = false;
                }

                // Upload assembly.
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Assembly", EditorStyles.boldLabel);
                uploadAssemblyName = EditorGUILayout.TextField("Assembly Name", uploadAssemblyName).Trim();
                var validUploadAssembly = true;
                if (!ValidateAssemblyName(uploadAssemblyName))
                {
                    EditorGUILayout.HelpBox("Please specify a valid assembly name.", MessageType.Error);
                    validUploadAssembly = false;
                }

                using (new EditorGUI.DisabledGroupScope(runningUploadAssemblyTask != null || !validUploadAssembly ||
                    !validProjectName))
                {
                    if (GUILayout.Button("Upload Assembly"))
                    {
                        runningUploadAssemblyTask = TriggerUploadAssemblyAsync();
                    }
                }

                // Deployment launcher.
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Deployment Launcher", EditorStyles.boldLabel);
                assemblyName = EditorGUILayout.TextField("Assembly Name", assemblyName).Trim();
                deploymentName = EditorGUILayout.TextField("Deployment Name", deploymentName).Trim();
                snapshotPath = EditorGUILayout.TextField("Snapshot Path", snapshotPath);
                mainLaunchJson = EditorGUILayout.TextField("Config", mainLaunchJson);
                using (var simPlayerDeploymentScope =
                    new EditorGUILayout.ToggleGroupScope("Enable simulated players", simPlayerDeploymentEnabled))
                {
                    simPlayerDeploymentEnabled = simPlayerDeploymentScope.enabled;
                    using (var overrideNameScope =
                        new EditorGUILayout.ToggleGroupScope("Override name", simPlayerCustomDeploymentNameEnabled))
                    {
                        simPlayerCustomDeploymentNameEnabled = overrideNameScope.enabled;
                        simPlayerDeploymentName = EditorGUILayout.TextField("Deployment Name",
                            simPlayerCustomDeploymentNameEnabled
                                ? simPlayerDeploymentName
                                : deploymentName + "_sim_players");
                    }

                    simPlayerLaunchJson = EditorGUILayout.TextField("Config", simPlayerLaunchJson);
                }

                var validLaunchParameters = true;
                if (!ValidateAssemblyName(assemblyName))
                {
                    EditorGUILayout.HelpBox("Please specify a valid assembly name.", MessageType.Error);
                    validLaunchParameters = false;
                }
                else if (!ValidateDeploymentName(deploymentName))
                {
                    EditorGUILayout.HelpBox("Please specify a valid deployment name.", MessageType.Error);
                    validLaunchParameters = false;
                }
                else if (simPlayerDeploymentEnabled && !ValidateDeploymentName(simPlayerDeploymentName))
                {
                    EditorGUILayout.HelpBox("Please specify a valid simulated players deployment name.",
                        MessageType.Error);
                    validLaunchParameters = false;
                }

                using (new EditorGUI.DisabledGroupScope(runningLaunchTask != null || !validLaunchParameters ||
                    !validProjectName))
                {
                    if (GUILayout.Button(simPlayerDeploymentEnabled ? "Launch deployments" : "Launch deployment"))
                    {
                        runningLaunchTask = TriggerLaunchDeploymentAsync();
                    }
                }

                // Deployment manager.
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Deployment List", EditorStyles.boldLabel);
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.PrefixLabel("Deployments:");
                    if (runningStopTask != null)
                    {
                        EditorGUILayout.LabelField($"stopping '{deploymentList[selectedDeployment].Name}'");
                    }
                    else if (runningListTask != null)
                    {
                        EditorGUILayout.LabelField("refreshing");
                    }
                    else if (deploymentList != null && deploymentList.Count > 0)
                    {
                        var deploymentNames = new string[deploymentList.Count];
                        for (var i = 0; i < deploymentList.Count; ++i)
                        {
                            deploymentNames[i] = deploymentList[i].Name;
                        }

                        selectedDeployment = EditorGUILayout.Popup(selectedDeployment, deploymentNames);
                    }
                    else
                    {
                        EditorGUILayout.LabelField("no deployments");
                    }
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    using (new EditorGUI.DisabledGroupScope(runningListTask != null || runningStopTask != null ||
                        !validProjectName))
                    {
                        if (GUILayout.Button("Refresh"))
                        {
                            runningListTask = TriggerListDeploymentsAsync();
                        }
                    }

                    using (new EditorGUI.DisabledGroupScope(runningListTask != null || runningStopTask != null ||
                        deploymentList.Count == 0 || !validProjectName))
                    {
                        if (GUILayout.Button("Stop Deployment"))
                        {
                            runningStopTask = TriggerStopDeploymentAsync(deploymentList[selectedDeployment]);
                        }
                    }
                }
            }

            private void Update()
            {
                if (runningUploadAssemblyTask != null && runningUploadAssemblyTask.IsCompleted)
                {
                    runningUploadAssemblyTask = null;
                    Repaint();
                }

                if (runningLaunchTask != null && runningLaunchTask.IsCompleted)
                {
                    runningLaunchTask = null;
                    Repaint();
                }

                if (runningStopTask != null && runningStopTask.IsCompleted)
                {
                    if (runningStopTask.Result)
                    {
                        deploymentList.RemoveAt(selectedDeployment);
                        selectedDeployment = 0;
                    }
                    else
                    {
                        Debug.LogError(
                            $"Failed to stop deployment {deploymentList[selectedDeployment].Name} with ID {deploymentList[selectedDeployment].Id}.");
                    }

                    runningStopTask = null;
                    Repaint();
                }

                if (runningListTask != null && runningListTask.IsCompleted)
                {
                    deploymentList.Clear();
                    if (runningListTask.Result != null)
                    {
                        deploymentList = runningListTask.Result;
                    }
                    else
                    {
                        Debug.LogError("Failed to refresh deployments list.");
                    }

                    runningListTask = null;
                    Repaint();
                }
            }

            private async Task<bool> TriggerUploadAssemblyAsync()
            {
                if (!Tools.Common.CheckDependencies())
                {
                    return false;
                }

                var arguments = new List<string>
                {
                    "cloud",
                    "upload",
                    uploadAssemblyName,
                    "--project_name",
                    projectName
                };
                var processResult = await RedirectedProcess.RunInAsync(ProjectRootPath, Tools.Common.SpatialBinary,
                    arguments.ToArray(), true, true);
                if (processResult.ExitCode == 0)
                {
                    Debug.Log($"Uploaded assembly {uploadAssemblyName} to project {projectName} successfully.");
                    return true;
                }
                else
                {
                    Debug.LogError($"Failed to upload assembly {uploadAssemblyName} to project {projectName}.");
                    return false;
                }
            }

            private async Task<bool> TriggerLaunchDeploymentAsync()
            {
                if (!Tools.Common.CheckDependencies())
                {
                    return false;
                }

                var simSnapshotPath = GenerateTempSnapshot();
                var arguments = new List<string>
                {
                    "create",
                    projectName,
                    assemblyName,
                    deploymentName,
                    Path.Combine(ProjectRootPath, mainLaunchJson),
                    Path.Combine(ProjectRootPath, snapshotPath)
                };
                if (simPlayerDeploymentEnabled)
                {
                    arguments.AddRange(new List<string>
                    {
                        deploymentName + "_sim_workers",
                        Path.Combine(ProjectRootPath, simPlayerLaunchJson),
                        simSnapshotPath
                    });
                }

                var processResult = await RedirectedProcess.RunInAsync(DotNetWorkingDirectory,
                    Tools.Common.DotNetBinary, ConstructArguments(arguments), true, true);
                return processResult.ExitCode != 0;
            }

            private async Task<bool> TriggerStopDeploymentAsync(DeploymentInfo deployment)
            {
                if (!Tools.Common.CheckDependencies())
                {
                    return false;
                }

                var arguments = new List<string>
                {
                    "stop",
                    projectName,
                    deployment.Id
                };
                var processResult = await RunDeploymentLauncherHelperAsync(arguments, true);
                if (processResult.ExitCode != 0)
                {
                    if (processResult.Stdout.Count > 0 && processResult.Stdout[0] == "<error:unknown-deployment>")
                    {
                        Debug.LogError(
                            $"Unable to stop deployment {deployment.Name}. Has the deployment been stopped already?");
                    }

                    return false;
                }
                else
                {
                    Debug.Log($"Deployment {deployment.Name} stopped.");
                    return true;
                }
            }

            private async Task<List<DeploymentInfo>> TriggerListDeploymentsAsync()
            {
                if (!Tools.Common.CheckDependencies())
                {
                    return null;
                }

                var arguments = new List<string>
                {
                    "list",
                    projectName
                };
                var processResult = await RunDeploymentLauncherHelperAsync(arguments);
                if (processResult.ExitCode != 0)
                {
                    return null;
                }

                // Get deployments from output.
                var deploymentList = new List<DeploymentInfo>();
                foreach (var line in processResult.Stdout)
                {
                    var tokens = line.Split(' ');
                    if (tokens.Length != 3)
                    {
                        continue;
                    }

                    if (tokens[0] != "<deployment>")
                    {
                        continue;
                    }

                    deploymentList.Add(new DeploymentInfo
                    {
                        Id = tokens[1],
                        Name = tokens[2]
                    });
                }

                return deploymentList.OrderBy(o => o.Name).ToList();
            }

            private async Task<RedirectedProcessResult> RunDeploymentLauncherHelperAsync(List<string> args,
                bool redirectStdout = false)
            {
                var processResult = await RedirectedProcess.RunInAsync(DotNetWorkingDirectory,
                    Tools.Common.DotNetBinary, ConstructArguments(args), redirectStdout, true);
                if (processResult.ExitCode != 0)
                {
                    // Examine the failure reason.
                    var failureReason = processResult.Stdout.Count > 0 ? processResult.Stdout[0] : "";
                    if (failureReason == "<error:unauthenticated>")
                    {
                        // The reason this task failed is because we are authenticated. Try authenticating.
                        Debug.Log(
                            "Failed to connect to the SpatialOS platform due to being unauthenticated. Running `spatial auth login` then retrying the last operation...");
                        var spatialAuthLoginResult = await RedirectedProcess.RunInAsync(DotNetWorkingDirectory,
                            Tools.Common.SpatialBinary, new string[] { "auth", "login" }, false, true);
                        if (spatialAuthLoginResult.ExitCode == 0)
                        {
                            // Re-run the task.
                            processResult = await RedirectedProcess.RunInAsync(DotNetWorkingDirectory,
                                Tools.Common.DotNetBinary, ConstructArguments(args), false, true);
                        }
                        else
                        {
                            Debug.Log("Failed to run `spatial auth login`.");
                        }
                    }
                }

                return processResult;
            }

            private static bool ValidateProjectName(string projectName)
            {
                if (string.IsNullOrEmpty(projectName))
                {
                    return false;
                }

                return Regex.Match(projectName, "^[a-z0-9_]{3,32}$").Success;
            }

            private static bool ValidateAssemblyName(string assemblyName)
            {
                if (string.IsNullOrEmpty(assemblyName))
                {
                    return false;
                }

                return Regex.Match(assemblyName, "^[a-zA-Z0-9_.-]{5,64}$").Success;
            }

            private static bool ValidateDeploymentName(string deploymentName)
            {
                if (string.IsNullOrEmpty(deploymentName))
                {
                    return false;
                }

                return Regex.Match(deploymentName, "^[a-z0-9_]{2,32}$").Success;
            }

            private static string GenerateTempSnapshot()
            {
                var snapshotPath = FileUtil.GetUniqueTempPathInProject();

                var snapshot = new Snapshot();
                var simulatedPlayerCoordinatorTrigger = FpsEntityTemplates.SimulatedPlayerCoordinatorTrigger();
                snapshot.AddEntity(simulatedPlayerCoordinatorTrigger);
                snapshot.WriteToFile(snapshotPath);

                return snapshotPath;
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
                baseArgs.AddRange(args.Select(arg => $"\"{arg}\""));
                return baseArgs.ToArray();
            }
        }
    }
}
