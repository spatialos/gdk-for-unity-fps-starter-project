using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Improbable.Gdk.Core.Collections;
using Improbable.Gdk.Core.Editor;
using Improbable.Gdk.DeploymentManager.Commands;
using Improbable.Gdk.Tools;
using Improbable.Gdk.Tools.MiniJSON;
using UnityEditor;
using UnityEngine;

namespace Improbable.Gdk.DeploymentManager
{
    internal class DeploymentLauncherWindow : EditorWindow
    {
        internal const string BuiltInErrorIcon = "console.erroricon.sml";
        internal const string BuiltInRefreshIcon = "Refresh";

        private DeploymentLauncherConfig launcherConfig;

        private static readonly Vector2 SmallIconSize = new Vector2(12, 12);
        private Material spinnerMaterial;

        private Vector2 scrollPos;
        private string projectName;
        private TaskManager manager = new TaskManager();
        private UiStateManager stateManager = new UiStateManager();

        private int selectedDeploymentIndex;

        [MenuItem("SpatialOS/Deployment Launcher", false, 51)]
        private static void LaunchDeploymentMenu()
        {
            var inspectorWindowType = typeof(EditorWindow).Assembly.GetType("UnityEditor.InspectorWindow");
            var deploymentWindow = GetWindow<DeploymentLauncherWindow>(inspectorWindowType);
            deploymentWindow.titleContent.text = "Deployment Launcher";
            deploymentWindow.titleContent.tooltip = "A tab for managing your SpatialOS deployments.";
            deploymentWindow.Show();
        }

        private void OnEnable()
        {
            launcherConfig = DeploymentLauncherConfig.GetInstance();
            projectName = GetProjectName();
            spinnerMaterial = new Material(Shader.Find("UI/Default"));
        }

        private void Update()
        {
            manager.Update();

            foreach (var wrappedTask in manager.CompletedUploadTasks)
            {
                if (wrappedTask.Task.Result.ExitCode != 0)
                {
                    Debug.LogError($"Upload of {wrappedTask.Context.AssemblyName} failed.");
                }
                else
                {
                    Debug.Log($"Upload of {wrappedTask.Context.AssemblyName} succeeded.");
                }
            }

            manager.CompletedUploadTasks.Clear();

            foreach (var wrappedTask in manager.CompletedLaunchTasks)
            {
                var result = wrappedTask.Task.Result;
                if (result.IsOkay)
                {
                    Application.OpenURL($"https://console.improbable.io/projects/{projectName}/deployments/{wrappedTask.Context.Name}/overview");
                }
                else
                {
                    var error = result.UnwrapError();
                    Debug.LogError($"Launch of {wrappedTask.Context.Name} failed. Code: {error.Code} Message:{error.Message}");
                }
            }

            manager.CompletedLaunchTasks.Clear();
        }

        private void OnGUI()
        {
            if (launcherConfig == null)
            {
                EditorGUILayout.HelpBox($"Could not find a {nameof(DeploymentLauncherConfig)} instance.\nPlease create one via the Assets > Create > SpatialOS menu.", MessageType.Info);
                return;
            }

            if (projectName == null)
            {
                EditorGUILayout.HelpBox("Could not parse your SpatialOS project name. See the Console for more details", MessageType.Error);
                return;
            }

            using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollPos))
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("Project Name", projectName);

                    var buttonIcon = new GUIContent(EditorGUIUtility.IconContent(BuiltInRefreshIcon))
                    {
                        tooltip = "Refresh your project name."
                    };

                    GUILayout.Space(EditorGUIUtility.currentViewWidth * 0.6f);

                    using (new EditorGUI.DisabledScope(manager.IsLaunching || manager.IsUploading))
                    {
                        if (GUILayout.Button(buttonIcon, EditorStyles.miniButton))
                        {
                            projectName = GetProjectName();
                        }
                    }
                }

                DrawHorizontalLine(5);

                launcherConfig.AssemblyConfig = DrawAssemblyConfig(launcherConfig.AssemblyConfig);

                GUILayout.Label("Deployment Configurations", EditorStyles.boldLabel);

                for (var index = 0; index < launcherConfig.DeploymentConfigs.Count; index++)
                {
                    var deplConfig = launcherConfig.DeploymentConfigs[index];
                    var (shouldRemove, updated) = DrawDeploymentConfig(deplConfig);
                    if (shouldRemove)
                    {
                        launcherConfig.DeploymentConfigs.RemoveAt(index);
                        index--;
                    }
                    else
                    {
                        launcherConfig.DeploymentConfigs[index] = updated;
                    }
                }

                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("Add new deployment configuration"))
                    {
                        var deploymentConfig = new DeploymentConfig
                        {
                            AssemblyName = launcherConfig.AssemblyConfig.AssemblyName,
                            Deployment = new BaseDeploymentConfig
                            {
                                Name = $"deployment_{launcherConfig.DeploymentConfigs.Count}"
                            }
                        };

                        launcherConfig.DeploymentConfigs.Add(deploymentConfig);
                    }
                }

                using (new GUILayout.HorizontalScope())
                {
                    selectedDeploymentIndex = EditorGUILayout.Popup("Deployment", selectedDeploymentIndex,
                        launcherConfig.DeploymentConfigs.Select(config => config.Deployment.Name).ToArray());

                    var isValid = selectedDeploymentIndex >= 0 &&
                        selectedDeploymentIndex < launcherConfig.DeploymentConfigs.Count;

                    var hasErrors = isValid && launcherConfig.DeploymentConfigs[selectedDeploymentIndex].GetErrors().Any();

                    using (new EditorGUI.DisabledScope(!isValid || hasErrors || manager.IsLaunching))
                    {
                        if (GUILayout.Button("Launch deployment"))
                        {
                            var deplConfig = launcherConfig.DeploymentConfigs[selectedDeploymentIndex];

                            manager.Launch(projectName, deplConfig.AssemblyName, deplConfig.Deployment);

                            foreach (var simPlayerDepl in deplConfig.SimulatedPlayerDeploymentConfig)
                            {
                                manager.Launch(projectName, deplConfig.AssemblyName, simPlayerDepl);
                            }
                        }
                    }
                }

                scrollPos = scrollView.scrollPosition;

                if (check.changed)
                {
                    EditorUtility.SetDirty(launcherConfig);
                    AssetDatabase.SaveAssets();
                }

                if (manager.IsUploading || manager.IsLaunching)
                {
                    EditorGUILayout.HelpBox(manager.GetStatusMessage(), MessageType.Info);
                    var rect = EditorGUILayout.GetControlRect(false, 20);
                    DrawSpinner(Time.realtimeSinceStartup * 10, rect);
                    Repaint();
                }
            }
        }

        private AssemblyConfig DrawAssemblyConfig(AssemblyConfig config)
        {
            GUILayout.Label("Assembly Upload", EditorStyles.boldLabel);

            var copy = config.DeepCopy();
            var error = config.GetError();

            using (new EditorGUILayout.VerticalScope())
            {
                copy.AssemblyName = EditorGUILayout.TextField("Assembly Name", config.AssemblyName);
                copy.ShouldForceUpload = EditorGUILayout.Toggle("Force Upload", config.ShouldForceUpload);

                GUILayout.Space(EditorGUIUtility.standardVerticalSpacing);


                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("Generate assembly name"))
                    {
                        copy.AssemblyName = $"{projectName}_{DateTime.Now.ToString("MMdd_hhmm")}";
                    }

                    using (new EditorGUI.DisabledScope(error != null))
                    {
                        if (GUILayout.Button("Assign assembly name to deployments"))
                        {
                            foreach (var deplConfig in launcherConfig.DeploymentConfigs)
                            {
                                deplConfig.AssemblyName = launcherConfig.AssemblyConfig.AssemblyName;
                            }
                        }

                        using (new EditorGUI.DisabledScope(manager.IsUploading))
                        {
                            if (GUILayout.Button("Upload Assembly"))
                            {
                                manager.Upload(projectName, config);
                            }
                        }
                    }
                }

                if (error != null)
                {
                    EditorGUILayout.HelpBox(error, MessageType.Error);
                }
            }

            DrawHorizontalLine(5);

            return copy;
        }

        private (bool, DeploymentConfig) DrawDeploymentConfig(DeploymentConfig config)
        {
            var foldoutState = stateManager.GetStateObject<bool>(config.Deployment.Name.GetHashCode());
            var copy = config.DeepCopy();

            var errors = copy.GetErrors();

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    foldoutState = EditorGUILayout.Foldout(foldoutState, new GUIContent(config.Deployment.Name), true);

                    GUILayout.FlexibleSpace();

                    using (new EditorGUIUtility.IconSizeScope(SmallIconSize))
                    {
                        if (errors.Any())
                        {
                            GUILayout.Label(new GUIContent(EditorGUIUtility.IconContent(BuiltInErrorIcon))
                            {
                                tooltip = "One or more errors in deployment configuration."
                            });
                        }

                        var buttonContent = new GUIContent(string.Empty, "Remove deployment configuration");
                        buttonContent.image = EditorGUIUtility.IconContent("Toolbar Minus").image;

                        if (GUILayout.Button(buttonContent, EditorStyles.miniButton))
                        {
                            return (true, null);
                        }
                    }
                }

                using (new EditorGUI.IndentLevelScope())
                using (new EditorGUILayout.VerticalScope())
                {
                    if (foldoutState)
                    {
                        copy.AssemblyName = EditorGUILayout.TextField("Assembly Name", config.AssemblyName);
                        RenderBaseDeploymentConfig(config.Deployment, copy.Deployment);

                        if (copy.Deployment.Name != config.Deployment.Name)
                        {
                            UpdateSimulatedDeploymentNames(copy);
                        }

                        GUILayout.Space(10);

                        EditorGUILayout.LabelField("Simulated Player Deployments");

                        for (var i = 0; i < copy.SimulatedPlayerDeploymentConfig.Count; i++)
                        {
                            var simConfig = copy.SimulatedPlayerDeploymentConfig[i];
                            var (shouldRemove, updated) = DrawSimulatedConfig(i, simConfig);

                            GUILayout.Space(5);

                            if (shouldRemove)
                            {
                                copy.SimulatedPlayerDeploymentConfig.RemoveAt(i);
                                i--;
                                UpdateSimulatedDeploymentNames(copy);
                            }
                            else
                            {
                                copy.SimulatedPlayerDeploymentConfig[i] = updated;
                            }
                        }
                    }
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (foldoutState)
                    {
                        GUILayout.FlexibleSpace();

                        if (GUILayout.Button("Add simulated player deployment"))
                        {
                            var newSimPlayerDepl = new SimulatedPlayerDeploymentConfig();
                            newSimPlayerDepl.TargetDeploymentName = config.Deployment.Name;
                            newSimPlayerDepl.Name = $"{config.Deployment.Name}_sim{config.SimulatedPlayerDeploymentConfig.Count + 1}";

                            copy.SimulatedPlayerDeploymentConfig.Add(newSimPlayerDepl);
                        }
                    }
                }

                if (errors.Any())
                {
                    EditorGUILayout.HelpBox($"This deployment configuration has the following errors:\n\n{errors.FormatErrors()}", MessageType.Error);
                }

                if (check.changed)
                {
                    stateManager.SetStateObject(copy.Deployment.Name.GetHashCode(), foldoutState);
                }
            }

            DrawHorizontalLine(5);

            return (false, copy);
        }

        private void RenderBaseDeploymentConfig(BaseDeploymentConfig source, BaseDeploymentConfig dest)
        {
            using (new EditorGUI.DisabledScope(source is SimulatedPlayerDeploymentConfig))
            {
                dest.Name = EditorGUILayout.TextField("Deployment Name", source.Name);
            }

            dest.SnapshotPath = EditorGUILayout.TextField("Snapshot Path", source.SnapshotPath);
            dest.LaunchJson = EditorGUILayout.TextField("Launch Config", source.LaunchJson);
            dest.Region = (DeploymentRegionCode) EditorGUILayout.EnumPopup("Region", source.Region);

            EditorGUILayout.LabelField("Tags");

            using (new EditorGUI.IndentLevelScope())
            {
                for (int i = 0; i < dest.Tags.Count; i++)
                {
                    dest.Tags[i] = EditorGUILayout.TextField($"Tag #{i + 1}", dest.Tags[i]);
                }

                dest.Tags.Add(EditorGUILayout.TextField($"Tag #{dest.Tags.Count + 1}", ""));

                dest.Tags = dest.Tags.Where(tag => !string.IsNullOrEmpty(tag)).ToList();
            }
        }

        private (bool, SimulatedPlayerDeploymentConfig) DrawSimulatedConfig(int index, SimulatedPlayerDeploymentConfig config)
        {
            var copy = config.DeepCopy();
            var foldoutState = stateManager.GetStateObject<bool>(config.Name.GetHashCode());

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    foldoutState = EditorGUILayout.Foldout(foldoutState, new GUIContent($"Simulated Player Deployment {index + 1}"), true);

                    GUILayout.FlexibleSpace();

                    using (new EditorGUIUtility.IconSizeScope(SmallIconSize))
                    {
                        var buttonContent = new GUIContent(string.Empty, "Remove simulated player deployment");
                        buttonContent.image = EditorGUIUtility.IconContent("Toolbar Minus").image;

                        if (GUILayout.Button(buttonContent, EditorStyles.miniButton))
                        {
                            return (true, null);
                        }
                    }
                }

                using (new EditorGUI.IndentLevelScope())
                using (new EditorGUILayout.VerticalScope())
                {
                    if (foldoutState)
                    {
                        RenderBaseDeploymentConfig(config, copy);
                        copy.FlagPrefix = EditorGUILayout.TextField("Flag Prefix", config.FlagPrefix);
                        copy.WorkerType = EditorGUILayout.TextField("Worker Type", config.WorkerType);
                    }
                }

                if (check.changed)
                {
                    stateManager.SetStateObject(copy.Name.GetHashCode(), foldoutState);
                }
            }

            return (false, copy);
        }

        private void UpdateSimulatedDeploymentNames(DeploymentConfig config)
        {
            for (var i = 0; i < config.SimulatedPlayerDeploymentConfig.Count; i++)
            {
                var previousFoldoutState =
                    stateManager.GetStateObject<bool>(config.SimulatedPlayerDeploymentConfig[i].Name.GetHashCode());

                config.SimulatedPlayerDeploymentConfig[i].Name = $"{config.Deployment.Name}_sim{i + 1}";
                config.SimulatedPlayerDeploymentConfig[i].TargetDeploymentName = config.Deployment.Name;

                stateManager.SetStateObject(config.SimulatedPlayerDeploymentConfig[i].Name.GetHashCode(), previousFoldoutState);
            }
        }

        private void DrawHorizontalLine(int height)
        {
            var rect = EditorGUILayout.GetControlRect(false, height, EditorStyles.foldout);
            using (new Handles.DrawingScope(new Color(0.3f, 0.3f, 0.3f, 1)))
            {
                Handles.DrawLine(new Vector2(rect.x, rect.yMax), new Vector2(rect.xMax, rect.yMax));
            }

            GUILayout.Space(rect.height);
        }

        private string GetProjectName()
        {
            var spatialJsonFile = Path.Combine(Tools.Common.SpatialProjectRootDir, "spatialos.json");

            if (!File.Exists(spatialJsonFile))
            {
                Debug.LogError($"Could not find a spatialos.json file located at: {spatialJsonFile}");
                return null;
            }

            var data = Json.Deserialize(File.ReadAllText(spatialJsonFile));

            if (data == null)
            {
                Debug.LogError($"Could not parse spatialos.json file located at: {spatialJsonFile}");
            }

            try
            {
                return (string) data["name"];
            }
            catch (KeyNotFoundException e)
            {
                Debug.LogError($"Could not find a \"name\" field in {spatialJsonFile}.\n Raw exception: {e.Message}");
                return null;
            }
        }

        private void DrawSpinner(float value, Rect rect)
        {
            // There are 11 frames in the spinner animation, 0 till 11.
            var imageId = Mathf.RoundToInt(value) % 12;
            var icon = EditorGUIUtility.IconContent($"d_WaitSpin{imageId:D2}");
            EditorGUI.DrawPreviewTexture(rect, icon.image, spinnerMaterial, ScaleMode.ScaleToFit, 1);
        }

        private class TaskManager
        {
            public bool IsUploading { get; private set; }
            public bool IsLaunching { get; private set; }

            public readonly List<WrappedTask<RedirectedProcessResult, AssemblyConfig>> CompletedUploadTasks = new List<WrappedTask<RedirectedProcessResult, AssemblyConfig>>();
            public readonly List<WrappedTask<Result<RedirectedProcessResult, Ipc.Error>, BaseDeploymentConfig>> CompletedLaunchTasks = new List<WrappedTask<Result<RedirectedProcessResult, Ipc.Error>, BaseDeploymentConfig>>();

            private WrappedTask<RedirectedProcessResult, AssemblyConfig> uploadTask;
            private WrappedTask<Result<RedirectedProcessResult, Ipc.Error>, BaseDeploymentConfig> launchTask;
            private WrappedTask<Result<RedirectedProcessResult, Ipc.Error>, DeploymentInfo> stopTask;
            private WrappedTask<Result<List<DeploymentInfo>, Ipc.Error>, string> listTask;

            private Queue<(string, string, BaseDeploymentConfig)> queuedLaunches = new Queue<(string, string, BaseDeploymentConfig)>();

            public void Upload(string projectName, AssemblyConfig config)
            {
                EditorApplication.LockReloadAssemblies();
                IsUploading = true;
                uploadTask = Assembly.UploadAsync(projectName, config);
            }

            public void Launch(string projectName, string assemblyName, BaseDeploymentConfig config)
            {
                if (IsLaunching)
                {
                    queuedLaunches.Enqueue((projectName, assemblyName, config));
                    return;
                }

                EditorApplication.LockReloadAssemblies();
                IsLaunching = true;
                launchTask = Deployment.LaunchAsync(projectName, assemblyName, config);
            }

            public void Update()
            {
                if (uploadTask?.Task.IsCompleted == true)
                {
                    IsUploading = false;
                    EditorApplication.UnlockReloadAssemblies();
                    CompletedUploadTasks.Add(uploadTask);
                    uploadTask = null;
                }

                if (launchTask?.Task.IsCompleted == true)
                {
                    CompletedLaunchTasks.Add(launchTask);

                    if (queuedLaunches.Count > 0)
                    {
                        var (projectName, assemblyName, config) = queuedLaunches.Dequeue();
                        launchTask = Deployment.LaunchAsync(projectName, assemblyName, config);
                    }
                    else
                    {
                        launchTask = null;
                        IsLaunching = false;
                        EditorApplication.UnlockReloadAssemblies();
                    }
                }
            }

            public string GetStatusMessage()
            {
                var sb = new StringBuilder();

                if (IsUploading)
                {
                    sb.AppendLine($"Uploading assembly \"{uploadTask.Context.AssemblyName}\".");
                }

                if (IsLaunching)
                {
                    sb.AppendLine($"Launching deployment \"{launchTask.Context.Name}\".");
                }

                if (IsLaunching || IsUploading)
                {
                    sb.Append("Assembly reloading locked.");
                }

                return sb.ToString();
            }
        }
    }
}
