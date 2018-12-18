using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Fps;
using Improbable.Gdk.Core;
using Improbable.Gdk.Tools;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Improbable.Gdk.SimLauncher
{
    public class DeploymentLauncher
    {
        private static readonly string DotNetProjectPath =
            Path.GetFullPath(Path.Combine(Tools.Common.GetPackagePath("com.improbable.gdk.simlauncher"), ".DeploymentLauncher/DeploymentLauncher.csproj"));

        private static readonly string ProjectRootPath =
            Path.Combine(Application.dataPath, "../../../");

        private const string SimLauncherMenuItem = "SpatialOS/Launch Deployment with simulated players";

        private const int SimLauncherPriority = 51;

        private const string ProjectName = "unity_gdk";

        private class LaunchDeploymentEditorWindow : EditorWindow
        {
            private string assemblyName;
            private string deploymentName;
            private string snapshotPath = "snapshots/cloud.snapshot";
            private string mainLaunchJson = "cloud_launch_small.json";
            private string simWorkerLaunchJson = "cloud_launch_small_sim_workers.json";

            private bool deploymentRunning;

            private Task runningLaunchTask;
            private bool simWorkerDeploymentEnabled;

            void OnGUI()
            {
                assemblyName = EditorGUILayout.TextField("Assembly Name", assemblyName);
                deploymentName = EditorGUILayout.TextField("Deployment Name", deploymentName);
                snapshotPath = EditorGUILayout.TextField("Snapshot Path", snapshotPath);
                if (runningLaunchTask != null)
                {
                    if (GUILayout.Button("Cancel Launch"))
                    {
                        OnClickShutdownDeployment();
                    }

                    if (runningLaunchTask.Wait(0))
                    {
                        deploymentRunning = true;
                    }
                }
                else if (deploymentRunning)
                {
                    GUILayout.TextArea("Launching deployment...");
                    if (GUILayout.Button("Shutdown deployment" + (simWorkerDeploymentEnabled ? "s" : "")))
                    {
                        OnClickShutdownDeployment();
                    }
                }
                else
                {
                    simWorkerDeploymentEnabled = EditorGUILayout.Toggle("Sim Worker Deployment Enabled", simWorkerDeploymentEnabled);
                    mainLaunchJson = EditorGUILayout.TextField("Launch config (main)", mainLaunchJson);
                    if (simWorkerDeploymentEnabled)
                    {
                        simWorkerLaunchJson =
                            EditorGUILayout.TextField("Launch config (sim workers)", simWorkerLaunchJson);
                    }

                    if (GUILayout.Button("Launch deployment"))
                    {
                        OnClickLaunchDeployment();
                    }
                }
            }

            void OnClickLaunchDeployment()
            {
                assemblyName = assemblyName.Trim();
                deploymentName = deploymentName.Trim();

                if (String.IsNullOrEmpty(assemblyName))
                {
                    EditorUtility.DisplayDialog("Unable to start a deployment", "Please specify a valid assembly name.",
                        "Close");
                    return;
                }

                if (String.IsNullOrEmpty(deploymentName))
                {
                    EditorUtility.DisplayDialog("Unable to start a deployment",
                        "Please specify a valid deployment name.", "Close");
                    return;
                }

                var simSnapshotPath = GenerateTempSnapshot();
                Debug.Log($"Created a snapshot for the sim deployment at {simSnapshotPath}.");

                var workingDirectory = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
                var command = Tools.Common.DotNetBinary;
                var arguments = ConstructArguments(assemblyName, deploymentName,
                    Path.Combine(ProjectRootPath, snapshotPath),
                    Path.Combine(ProjectRootPath, mainLaunchJson),
                    simSnapshotPath,
                    Path.Combine(ProjectRootPath, simWorkerLaunchJson));

                Debug.Log($"Launching {command} with arguments {String.Join(" ", arguments)}");

                runningLaunchTask = Task.Run(() =>
                {
                    var info = new ProcessStartInfo(command, String.Join(" ", arguments))
                    {
                        CreateNoWindow = true,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        WorkingDirectory = workingDirectory
                    };

                    using (var process = Process.Start(info))
                    {
                        if (process == null)
                        {
                            throw new Exception(
                                $"Failed to run {info.FileName} {info.Arguments}\nIs the .NET Core SDK installed?");
                        }

                        process.EnableRaisingEvents = true;

                        void OnReceived(object sender, DataReceivedEventArgs args)
                        {
                            if (String.IsNullOrEmpty(args.Data))
                            {
                                return;
                            }

                            Debug.Log(args.Data);
                        }

                        process.OutputDataReceived += OnReceived;
                        process.ErrorDataReceived += OnReceived;

                        process.BeginOutputReadLine();
                        process.BeginErrorReadLine();

                        process.WaitForExit();

                        if (process.ExitCode != 0)
                        {
                            Debug.LogError("Failed to launch deployment.");
                        }
                    }
                });
            }

            private void OnClickCancelLaunchDeployment()
            {
                deploymentRunning = false;
                runningLaunchTask = null;
            }

            private void OnClickShutdownDeployment()
            {
                deploymentRunning = false;
                runningLaunchTask = null;
            }
        }

        [MenuItem(SimLauncherMenuItem, false, SimLauncherPriority)]
        private static void LaunchDeploymentMenu()
        {
            if (!CheckDependencies())
            {
                return;
            }

            // Show existing window instance. If one doesn't exist, make one.
            var launchDeploymentWindow = ScriptableObject.CreateInstance<LaunchDeploymentEditorWindow>();
            launchDeploymentWindow.position = EditorWindow.GetWindow<EditorWindow>("Inspector").position;
            launchDeploymentWindow.Show();
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
            var hasDotnet = !String.IsNullOrEmpty(Tools.Common.DotNetBinary);

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

        private static string[] ConstructArguments(string assemblyName, string deploymentName, string mainSnapshotPath, string mainLaunchJson, string simSnapshotPath, string simLaunchJson)
        {
            var toolsConfig = GdkToolsConfiguration.GetOrCreateInstance();

            var baseArgs = new List<string>
            {
                "run",
                "-p",
                $"\"{DotNetProjectPath}\"",
                "--",
                "create",
                $"\"{ProjectName}\"",
                $"\"{assemblyName}\"",
                $"\"{deploymentName}\"",
                $"\"{mainLaunchJson}\"",
                $"\"{mainSnapshotPath}\"",
                $"\"{deploymentName}_sim_workers\"",
                $"\"{simLaunchJson}\"",
                $"\"{simSnapshotPath}\""
            };

            return baseArgs.ToArray();
        }
    }
}
