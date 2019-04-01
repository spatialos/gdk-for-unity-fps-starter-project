using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
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

        private static readonly string ProjectRootPath =
            Path.Combine(Application.dataPath, "../../../");

        private static readonly string ConsoleURLFormat =
            "https://console.improbable.io/projects/{0}/deployments/{1}/overview";

        private static readonly Material spinnerMaterial = new Material(Shader.Find("UI/Default"));

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

        internal enum DeploymentRegionCode
        {
            US,
            EU
        }

        internal class DeploymentEditorWindow : EditorWindow
        {
            private string projectName = "unity_gdk";
            private string uploadAssemblyName = "";
            private string assemblyName = "";
            private string deploymentName = "";
            private string snapshotPath = "snapshots/cloud.snapshot";
            private string mainLaunchJson = "cloud_launch_large.json";
            private string simPlayerLaunchJson = "cloud_launch_large_sim_players.json";
            private string simPlayerDeploymentName = "";

            private bool forceUploadAssembly = false;
            private bool simPlayerDeploymentEnabled;
            private bool simPlayerCustomDeploymentNameEnabled;

            private DeploymentRegionCode deploymentRegionCode = DeploymentRegionCode.US;

            private List<DeploymentInfo> deploymentList;
            private int selectedDeployment;

            private DeploymentTasks.WrappedTask<bool> runningUploadAssemblyTask;
            private DeploymentTasks.WrappedTask<bool> runningLaunchTask;
            private DeploymentTasks.WrappedTask<bool> runningStopTask;
            private DeploymentTasks.WrappedTask<List<DeploymentInfo>> runningListTask;

            private void OnEnable()
            {
                deploymentList = new List<DeploymentInfo>();

                AppDomain.CurrentDomain.ProcessExit += CleanUp;
            }

            private void CleanUp(object sender, EventArgs args)
            {
                CleanupTask(runningUploadAssemblyTask);
                CleanupTask(runningLaunchTask);
                CleanupTask(runningStopTask);
                CleanupTask(runningListTask);
            }

            private void CleanupTask<T>(DeploymentTasks.WrappedTask<T> wrappedTask)
            {
                if (wrappedTask != null)
                {
                    wrappedTask.CancelSource.Cancel();
                    wrappedTask.Task.Wait();
                    wrappedTask.Dispose();
                }
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
                forceUploadAssembly = EditorGUILayout.Toggle("Force Upload", forceUploadAssembly);
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
                snapshotPath = EditorGUILayout.TextField("Snapshot File", snapshotPath);
                mainLaunchJson = EditorGUILayout.TextField("Launch Config File", mainLaunchJson);
                deploymentRegionCode = (DeploymentRegionCode) EditorGUILayout.EnumPopup(
                    "Deployment Region", deploymentRegionCode);
                using (var simPlayerDeploymentScope =
                    new EditorGUILayout.ToggleGroupScope("Enable Simulated Players", simPlayerDeploymentEnabled))
                {
                    simPlayerDeploymentEnabled = simPlayerDeploymentScope.enabled;
                    using (var overrideNameScope =
                        new EditorGUILayout.ToggleGroupScope("Override Name", simPlayerCustomDeploymentNameEnabled))
                    {
                        simPlayerCustomDeploymentNameEnabled = overrideNameScope.enabled;
                        simPlayerDeploymentName = EditorGUILayout.TextField("Deployment Name",
                            simPlayerCustomDeploymentNameEnabled
                                ? simPlayerDeploymentName
                                : deploymentName + "_sim_players");
                    }

                    simPlayerLaunchJson = EditorGUILayout.TextField("Launch Config File", simPlayerLaunchJson);
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
                    if (GUILayout.Button(simPlayerDeploymentEnabled ? "Launch Deployments" : "Launch Deployment"))
                    {
                        runningLaunchTask = TriggerLaunchDeploymentAsync();
                    }
                }

                if (runningLaunchTask != null)
                {
                    EditorGUILayout.HelpBox("Launching deployment(s). Assembly reloading locked.", MessageType.Info);
                    var rect = EditorGUILayout.GetControlRect(false, 20);
                    DrawSpinner(Time.realtimeSinceStartup * 10, rect);
                    Repaint();
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
                if (runningUploadAssemblyTask != null && runningUploadAssemblyTask.Task.IsCompleted)
                {
                    runningUploadAssemblyTask.Dispose();
                    runningUploadAssemblyTask = null;
                    Repaint();
                }

                if (runningLaunchTask != null && runningLaunchTask.Task.IsCompleted)
                {
                    runningLaunchTask.Dispose();
                    runningLaunchTask = null;
                    Repaint();

                    EditorApplication.UnlockReloadAssemblies();
                }

                if (runningStopTask != null && runningStopTask.Task.IsCompleted)
                {
                    if (runningStopTask.Task.Result)
                    {
                        deploymentList.RemoveAt(selectedDeployment);
                        selectedDeployment = 0;
                    }
                    else
                    {
                        Debug.LogError(
                            $"Failed to stop deployment {deploymentList[selectedDeployment].Name} with ID {deploymentList[selectedDeployment].Id}.");
                    }

                    runningStopTask.Dispose();
                    runningStopTask = null;
                    Repaint();
                }

                if (runningListTask != null && runningListTask.Task.IsCompleted)
                {
                    deploymentList.Clear();
                    if (runningListTask.Task.Result != null)
                    {
                        deploymentList = runningListTask.Task.Result;
                    }
                    else
                    {
                        Debug.LogError("Failed to refresh deployments list.");
                    }

                    runningListTask.Dispose();
                    runningListTask = null;
                    Repaint();
                }
            }

            private DeploymentTasks.WrappedTask<bool> TriggerUploadAssemblyAsync()
            {
                var arguments = new string[]
                {
                    "cloud",
                    "upload",
                    uploadAssemblyName,
                    "--project_name",
                    projectName,
                    forceUploadAssembly ? "--json_output --force" : "--json_output"
                };

                return DeploymentTasks.TriggerUploadAssemblyAsync(arguments,
                    () => Debug.Log($"Uploaded assembly {uploadAssemblyName} to project {projectName} successfully."),
                    () => Debug.LogError($"Failed to upload assembly {uploadAssemblyName} to project {projectName}.")
                );
            }

            private DeploymentTasks.WrappedTask<bool> TriggerLaunchDeploymentAsync()
            {
                var arguments = new List<string>
                {
                    "create",
                    projectName,
                    assemblyName,
                    deploymentName,
                    Path.Combine(ProjectRootPath, mainLaunchJson),
                    Path.Combine(ProjectRootPath, snapshotPath),
                    deploymentRegionCode.ToString()
                };

                if (simPlayerDeploymentEnabled)
                {
                    arguments.AddRange(new List<string>
                    {
                        simPlayerDeploymentName,
                        Path.Combine(ProjectRootPath, simPlayerLaunchJson)
                    });
                }

                return DeploymentTasks.TriggerLaunchDeploymentAsync(arguments, () =>
                {
                    Application.OpenURL(string.Format(ConsoleURLFormat, projectName, deploymentName));
                    if (simPlayerDeploymentEnabled)
                    {
                        Application.OpenURL(string.Format(ConsoleURLFormat, projectName, simPlayerDeploymentName));
                    }
                });
            }

            private DeploymentTasks.WrappedTask<bool> TriggerStopDeploymentAsync(DeploymentInfo deployment)
            {
                var arguments = new List<string>
                {
                    "stop",
                    projectName,
                    deployment.Id
                };

                return DeploymentTasks.TriggerStopDeploymentAsync(arguments,
                    () => { Debug.Log($"Deployment {deployment.Name} stopped."); },
                    processResult =>
                    {
                        if (processResult.Stdout.Count > 0 && processResult.Stdout[0] == "<error:unknown-deployment>")
                        {
                            Debug.LogError(
                                $"Unable to stop deployment {deployment.Name}. Has the deployment been stopped already?");
                        }
                    });
            }

            private DeploymentTasks.WrappedTask<List<DeploymentInfo>> TriggerListDeploymentsAsync()
            {
                var arguments = new List<string>
                {
                    "list",
                    projectName
                };

                return DeploymentTasks.TriggerListDeploymentsAsync(arguments);
            }

            private static bool ValidateProjectName(string projectName)
            {
                return !string.IsNullOrEmpty(projectName) && Regex.Match(projectName, "^[a-z0-9_]{3,32}$").Success;
            }

            private static bool ValidateAssemblyName(string assemblyName)
            {
                return !string.IsNullOrEmpty(assemblyName) && Regex.Match(assemblyName, "^[a-zA-Z0-9_.-]{5,64}$").Success;
            }

            private static bool ValidateDeploymentName(string deploymentName)
            {
                return !string.IsNullOrEmpty(deploymentName) && Regex.Match(deploymentName, "^[a-z0-9_]{2,32}$").Success;
            }

            private void DrawSpinner(float value, Rect rect)
            {
                // There are 11 frames in the spinner animation, 0 till 11.
                var imageId = Mathf.RoundToInt(value) % 12;
                var icon = EditorGUIUtility.IconContent($"d_WaitSpin{imageId:D2}");
                EditorGUI.DrawPreviewTexture(rect, icon.image, spinnerMaterial, ScaleMode.ScaleToFit, 1);
            }
        }
    }
}
