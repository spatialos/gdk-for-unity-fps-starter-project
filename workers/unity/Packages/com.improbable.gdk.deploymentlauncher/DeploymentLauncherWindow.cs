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
        private const string BuiltInErrorIcon = "console.erroricon.sml";
        private const string BuiltInRefreshIcon = "Refresh";
        private const string BuiltInWebIcon = "BuildSettings.Web.Small";

        private static readonly Vector2 SmallIconSize = new Vector2(12, 12);
        private readonly Color horizontalLineColor = new Color(0.3f, 0.3f, 0.3f, 1);
        private Material spinnerMaterial;

        private readonly TaskManager manager = new TaskManager();
        private readonly UIStateManager stateManager = new UIStateManager();

        private DeploymentLauncherConfig launcherConfig;
        private int selectedDeploymentIndex;
        private Vector2 scrollPos;
        private string projectName;

        private List<DeploymentInfo> listedDeployments = new List<DeploymentInfo>();
        private int selectedListedDeploymentIndex;

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

            if (launcherConfig != null && projectName != null)
            {
                launcherConfig.SetProjectName(projectName);
                EditorUtility.SetDirty(launcherConfig);
                AssetDatabase.SaveAssets();
            }

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

                    Debug.LogError($"Launch of {wrappedTask.Context.Name} failed. Code: {error.Code} Message: {error.Message}");
                    HandleErrorGeneric(error);
                }
            }

            manager.CompletedLaunchTasks.Clear();

            foreach (var wrappedTask in manager.CompletedListTasks)
            {
                var result = wrappedTask.Task.Result;
                if (result.IsOkay)
                {
                    listedDeployments = result.Unwrap();
                    listedDeployments.Sort((first, second) => string.Compare(first.Name, second.Name, StringComparison.Ordinal));
                    selectedDeploymentIndex = -1;
                }
                else
                {
                    var error = result.UnwrapError();

                    Debug.LogError($"Failed to list deployments in project {wrappedTask.Context}. Code: {error.Code} Message: {error.Message}");
                    HandleErrorGeneric(error);
                }
            }

            manager.CompletedListTasks.Clear();

            foreach (var wrappedTask in manager.CompletedStopTasks)
            {
                var result = wrappedTask.Task.Result;
                var info = wrappedTask.Context;
                if (result.IsOkay)
                {
                    Debug.Log($"Stopped deployment: \"{info.Name}\" successfully.");
                }
                else
                {
                    var error = result.UnwrapError();

                    Debug.LogError($"Failed to stop deployment: \"{info.Name}\". Code: {error.Code} Message: {error.Message}.");
                    HandleErrorGeneric(error);
                }
            }

            manager.CompletedStopTasks.Clear();
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
                    using (new EditorGUI.DisabledScope(manager.IsActive))
                    {
                        var buttonIcon = new GUIContent(EditorGUIUtility.IconContent(BuiltInRefreshIcon))
                        {
                            tooltip = "Refresh your project name."
                        };

                        if (GUILayout.Button(buttonIcon, EditorStyles.miniButton, GUILayout.ExpandWidth(false)))
                        {
                            projectName = GetProjectName();
                            launcherConfig.SetProjectName(projectName);
                            EditorUtility.SetDirty(launcherConfig);
                            AssetDatabase.SaveAssets();
                        }
                    }

                    EditorGUILayout.LabelField("Project Name", projectName);
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

                if (launcherConfig.DeploymentConfigs.Count > 0)
                {
                    using (new GUILayout.HorizontalScope())
                    {
                        selectedDeploymentIndex = EditorGUILayout.Popup("Deployment", selectedDeploymentIndex,
                            launcherConfig.DeploymentConfigs.Select(config => config.Deployment.Name).ToArray());

                        var isValid = IsSelectedValid(launcherConfig.DeploymentConfigs, selectedDeploymentIndex);

                        var hasErrors = isValid && launcherConfig.DeploymentConfigs[selectedDeploymentIndex].GetErrors().Any();

                        using (new EditorGUI.DisabledScope(!isValid || hasErrors || manager.IsActive))
                        {
                            if (GUILayout.Button("Launch deployment"))
                            {
                                var deplConfig = launcherConfig.DeploymentConfigs[selectedDeploymentIndex];

                                manager.Launch(deplConfig.ProjectName, deplConfig.AssemblyName, deplConfig.Deployment);

                                foreach (var simPlayerDepl in deplConfig.SimulatedPlayerDeploymentConfigs)
                                {
                                    manager.Launch(deplConfig.ProjectName, deplConfig.AssemblyName, simPlayerDepl);
                                }
                            }
                        }
                    }
                }

                DrawHorizontalLine(5);
                GUILayout.Label("Live Deployments", EditorStyles.boldLabel);
                DrawDeploymentList();

                scrollPos = scrollView.scrollPosition;

                if (check.changed)
                {
                    EditorUtility.SetDirty(launcherConfig);
                    AssetDatabase.SaveAssets();
                }

                if (manager.IsActive)
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
                using (new EditorGUILayout.HorizontalScope())
                {
                    copy.AssemblyName = EditorGUILayout.TextField("Assembly Name", config.AssemblyName);

                    if (GUILayout.Button("Generate", GUILayout.ExpandWidth(false)))
                    {
                        copy.AssemblyName = $"{projectName}_{DateTime.Now.ToString("MMdd_hhmm")}";
                    }
                }

                copy.ShouldForceUpload = EditorGUILayout.Toggle("Force Upload", config.ShouldForceUpload);

                GUILayout.Space(EditorGUIUtility.standardVerticalSpacing);

                using (new EditorGUILayout.HorizontalScope())
                {
                    var shouldBeVertical = EditorGUIUtility.currentViewWidth < 550;
                    /* Response Layout, Intuitive API! */
                    if (shouldBeVertical)
                    {
                        EditorGUILayout.BeginVertical();
                    }
                    else
                    {
                        GUILayout.FlexibleSpace();
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

                        using (new EditorGUI.DisabledScope(manager.IsActive))
                        {
                            if (GUILayout.Button("Upload assembly"))
                            {
                                manager.Upload(config);
                            }
                        }
                    }

                    if (shouldBeVertical)
                    {
                        EditorGUILayout.EndVertical();
                    }
                }

                if (error != null)
                {
                    EditorGUILayout.HelpBox(error, MessageType.Error);
                }
            }

            DrawHorizontalLine(3);

            return copy;
        }

        private (bool, DeploymentConfig) DrawDeploymentConfig(DeploymentConfig config)
        {
            var foldoutState = stateManager.GetStateObjectOrDefault<bool>(config.Deployment.Name.GetHashCode());
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

                        for (var i = 0; i < copy.SimulatedPlayerDeploymentConfigs.Count; i++)
                        {
                            var simConfig = copy.SimulatedPlayerDeploymentConfigs[i];
                            var (shouldRemove, updated) = DrawSimulatedConfig(i, simConfig);

                            GUILayout.Space(5);

                            if (shouldRemove)
                            {
                                copy.SimulatedPlayerDeploymentConfigs.RemoveAt(i);
                                i--;
                                UpdateSimulatedDeploymentNames(copy);
                            }
                            else
                            {
                                copy.SimulatedPlayerDeploymentConfigs[i] = updated;
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
                            newSimPlayerDepl.Name = $"{config.Deployment.Name}_sim{config.SimulatedPlayerDeploymentConfigs.Count + 1}";

                            copy.SimulatedPlayerDeploymentConfigs.Add(newSimPlayerDepl);
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
                for (var i = 0; i < dest.Tags.Count; i++)
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
            var foldoutState = stateManager.GetStateObjectOrDefault<bool>(config.Name.GetHashCode());

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

        private void DrawDeploymentList()
        {
            if (listedDeployments.Count == 0)
            {
                using (new EditorGUILayout.VerticalScope())
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        GUILayout.FlexibleSpace();
                        GUILayout.Label("Could not find any live deployments.");
                        GUILayout.FlexibleSpace();
                    }

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        GUILayout.FlexibleSpace();
                        GUILayout.Label("Press the \"Refresh\" button to search again.");
                        GUILayout.FlexibleSpace();
                    }
                }
            }
            else
            {
                // Temporarily change the label width field to allow better spacing in the deployment list screen.
                var previousWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 250f;

                for (var index = 0; index < listedDeployments.Count; index++)
                {
                    var deplInfo = listedDeployments[index];

                    var foldoutState = stateManager.GetStateObjectOrDefault<bool>(deplInfo.Id.GetHashCode());
                    using (var check = new EditorGUI.ChangeCheckScope())
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        foldoutState = EditorGUILayout.Foldout(foldoutState, new GUIContent(deplInfo.Name), true);

                        var buttonIcon = new GUIContent(EditorGUIUtility.IconContent(BuiltInWebIcon))
                        {
                            tooltip = "Open this deployment in your browser."
                        };

                        if (GUILayout.Button(buttonIcon, EditorStyles.miniButton, GUILayout.ExpandWidth(false)))
                        {
                            Application.OpenURL($"https://console.improbable.io/projects/{projectName}/deployments/{deplInfo.Name}/overview/{deplInfo.Id}");
                        }

                        if (check.changed)
                        {
                            stateManager.SetStateObject(deplInfo.Id.GetHashCode(), foldoutState);
                        }
                    }

                    using (new EditorGUI.IndentLevelScope())
                    using (new EditorGUILayout.VerticalScope())
                    {
                        if (foldoutState)
                        {
                            EditorGUILayout.LabelField("Start Time", deplInfo.StartTime);
                            EditorGUILayout.LabelField("Region", deplInfo.Region);

                            if (deplInfo.Workers.Count > 0)
                            {
                                EditorGUILayout.LabelField("Connected Workers");
                                using (new EditorGUI.IndentLevelScope())
                                {
                                    foreach (var workerPair in deplInfo.Workers)
                                    {
                                        EditorGUILayout.LabelField(workerPair.Key, $"{workerPair.Value}");
                                    }
                                }
                            }

                            if (deplInfo.Tags.Count > 0)
                            {
                                EditorGUILayout.LabelField("Tags");
                                using (new EditorGUI.IndentLevelScope())
                                {
                                    foreach (var tag in deplInfo.Tags)
                                    {
                                        EditorGUILayout.LabelField(tag);
                                    }
                                }
                            }
                        }

                        DrawHorizontalLine(3);
                    }
                }

                EditorGUIUtility.labelWidth = previousWidth;
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();

                using (new EditorGUI.DisabledScope(manager.IsActive))
                {
                    if (GUILayout.Button("Refresh"))
                    {
                        manager.List(projectName);
                    }
                }
            }

            if (listedDeployments.Count > 0)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    selectedListedDeploymentIndex = EditorGUILayout.Popup("Deployment", selectedListedDeploymentIndex,
                        listedDeployments.Select(config => config.Name).ToArray());

                    using (new EditorGUI.DisabledScope(!IsSelectedValid(listedDeployments, selectedListedDeploymentIndex) || manager.IsActive))
                    {
                        if (GUILayout.Button("Stop deployment"))
                        {
                            manager.Stop(listedDeployments[selectedListedDeploymentIndex]);
                        }
                    }
                }
            }
        }

        private void UpdateSimulatedDeploymentNames(DeploymentConfig config)
        {
            for (var i = 0; i < config.SimulatedPlayerDeploymentConfigs.Count; i++)
            {
                var previousFoldoutState =
                    stateManager.GetStateObjectOrDefault<bool>(config.SimulatedPlayerDeploymentConfigs[i].Name.GetHashCode());

                config.SimulatedPlayerDeploymentConfigs[i].Name = $"{config.Deployment.Name}_sim{i + 1}";
                config.SimulatedPlayerDeploymentConfigs[i].TargetDeploymentName = config.Deployment.Name;

                stateManager.SetStateObject(config.SimulatedPlayerDeploymentConfigs[i].Name.GetHashCode(), previousFoldoutState);
            }
        }

        private void DrawHorizontalLine(int height)
        {
            var rect = EditorGUILayout.GetControlRect(false, height, EditorStyles.foldout);
            using (new Handles.DrawingScope(horizontalLineColor))
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
                return null;
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

        private void HandleErrorGeneric(Ipc.Error error)
        {
            if (error.Code == Ipc.ErrorCode.Unauthenticated)
            {
                Debug.LogError("Please login with \"spatial auth login\" and try again.");
            }
        }

        private bool IsSelectedValid<T>(List<T> list, int index)
        {
            return index >= 0 && index < list.Count;
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
            public bool IsActive => isUploading || isLaunching || isListing || isStopping;

            public readonly List<WrappedTask<RedirectedProcessResult, AssemblyConfig>> CompletedUploadTasks = new List<WrappedTask<RedirectedProcessResult, AssemblyConfig>>();
            public readonly List<WrappedTask<Result<RedirectedProcessResult, Ipc.Error>, BaseDeploymentConfig>> CompletedLaunchTasks = new List<WrappedTask<Result<RedirectedProcessResult, Ipc.Error>, BaseDeploymentConfig>>();
            public readonly List<WrappedTask<Result<List<DeploymentInfo>, Ipc.Error>, string>> CompletedListTasks = new List<WrappedTask<Result<List<DeploymentInfo>, Ipc.Error>, string>>();
            public readonly List<WrappedTask<Result<RedirectedProcessResult, Ipc.Error>, DeploymentInfo>> CompletedStopTasks = new List<WrappedTask<Result<RedirectedProcessResult, Ipc.Error>, DeploymentInfo>>();

            private WrappedTask<RedirectedProcessResult, AssemblyConfig> uploadTask;
            private WrappedTask<Result<RedirectedProcessResult, Ipc.Error>, BaseDeploymentConfig> launchTask;
            private WrappedTask<Result<RedirectedProcessResult, Ipc.Error>, DeploymentInfo> stopTask;
            private WrappedTask<Result<List<DeploymentInfo>, Ipc.Error>, string> listTask;

            private bool isUploading;
            private bool isLaunching;
            private bool isListing;
            private bool isStopping;

            private Queue<(string, string, BaseDeploymentConfig)> queuedLaunches = new Queue<(string, string, BaseDeploymentConfig)>();

            public void Upload(AssemblyConfig config)
            {
                EditorApplication.LockReloadAssemblies();
                isUploading = true;
                uploadTask = Assembly.UploadAsync(config);
            }

            public void Launch(string projectName, string assemblyName, BaseDeploymentConfig config)
            {
                if (isLaunching)
                {
                    queuedLaunches.Enqueue((projectName, assemblyName, config));
                    return;
                }

                EditorApplication.LockReloadAssemblies();
                isLaunching = true;
                launchTask = Deployment.LaunchAsync(projectName, assemblyName, config);
            }

            public void List(string projectName)
            {
                EditorApplication.LockReloadAssemblies();
                isListing = true;
                listTask = Deployment.ListAsync(projectName);
            }

            public void Stop(DeploymentInfo info)
            {
                EditorApplication.LockReloadAssemblies();
                isStopping = true;
                stopTask = Deployment.StopAsync(info);
            }

            public void Update()
            {
                if (uploadTask?.Task.IsCompleted == true)
                {
                    isUploading = false;
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
                        isLaunching = false;
                    }
                }

                if (listTask?.Task.IsCompleted == true)
                {
                    isListing = false;
                    CompletedListTasks.Add(listTask);
                    listTask = null;
                }

                if (stopTask?.Task.IsCompleted == true)
                {
                    isStopping = false;
                    CompletedStopTasks.Add(stopTask);
                    stopTask = null;
                }

                if (!IsActive)
                {
                    EditorApplication.UnlockReloadAssemblies();
                }
            }

            public string GetStatusMessage()
            {
                var sb = new StringBuilder();

                if (isUploading)
                {
                    sb.AppendLine($"Uploading assembly \"{uploadTask.Context.AssemblyName}\".");
                }

                if (isLaunching)
                {
                    sb.AppendLine($"Launching deployment \"{launchTask.Context.Name}\".");
                }

                if (isListing)
                {
                    sb.AppendLine($"Listing deployments in project \"{listTask.Context}\"");
                }

                if (isStopping)
                {
                    sb.AppendLine($"Stopping deployment \"{stopTask.Context.Name}\"");
                }

                if (IsActive)
                {
                    sb.Append("Assembly reloading locked.");
                }

                return sb.ToString();
            }
        }
    }
}
