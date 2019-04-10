using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Improbable.Gdk.DeploymentManager
{
    internal class DeploymentLauncherUi : EditorWindow
    {
        internal const string BuiltInErrorIcon = "console.erroricon.sml";
        internal const string BuiltInWarningIcon = "console.warnicon.sml";
        internal const string BuiltInTrashIcon = "TreeEditor.Trash";

        [MenuItem("SpatialOS/Deployment Launcher", false, 51)]
        private static void LaunchDeploymentMenu()
        {
            var inspectorWindowType = typeof(EditorWindow).Assembly.GetType("UnityEditor.InspectorWindow");
            var deploymentWindow = GetWindow<DeploymentLauncherUi>(inspectorWindowType);
            deploymentWindow.titleContent.text = "Deployment Launcher";
            deploymentWindow.titleContent.tooltip = "A tab for managing your SpatialOS deployments.";
            deploymentWindow.Show();
        }

        private DeploymentLauncherConfig launcherConfig;

        private readonly Dictionary<int, object> localState = new Dictionary<int, object>();

        private void OnEnable()
        {
            launcherConfig = DeploymentLauncherConfig.GetInstance();
        }

        private void OnGUI()
        {
            if (launcherConfig == null)
            {
                GUILayout.Label($"Could not find a {nameof(DeploymentLauncherConfig)} instance.\nPlease create one via the Assets > Create > SpatialOS menu.");
            }

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
                    launcherConfig.DeploymentConfigs.Add(new DeploymentConfig());
                }
            }

            AssetDatabase.SaveAssets();
        }

        private AssemblyConfig DrawAssemblyConfig(AssemblyConfig config)
        {
            GUILayout.Label("Assembly Upload", EditorStyles.boldLabel);
            DrawHorizontalLine(5);

            var copy = config.DeepCopy();

            using (new EditorGUILayout.VerticalScope())
            {
                copy.AssemblyName = EditorGUILayout.TextField("Assembly Name", config.AssemblyName);

                GUILayout.Space(EditorGUIUtility.standardVerticalSpacing);

                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("Upload Assembly"))
                    {
                    }
                }
            }

            DrawHorizontalLine(5);

            return copy;
        }

        private (bool, DeploymentConfig) DrawDeploymentConfig(DeploymentConfig config)
        {
            var foldoutState = GetStateObject<bool>(config.Deployment.Name.GetHashCode());
            var copy = config.DeepCopy();

            // TODO: Render errors.
            var errors = copy.GetErrors().ToList();

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    foldoutState = EditorGUILayout.Foldout(foldoutState, new GUIContent(config.Deployment.Name), true);

                    GUILayout.FlexibleSpace();

                    if (errors.Count != 0)
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

                        for (int i = 0; i < copy.SimulatedPlayerDeploymentConfig.Count; i++)
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

                GUILayout.Space(15);

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

                if (check.changed)
                {
                    SetStateObject(copy.Deployment.Name.GetHashCode(), foldoutState);
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
            var foldoutState = GetStateObject<bool>(config.Name.GetHashCode());

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    foldoutState = EditorGUILayout.Foldout(foldoutState, new GUIContent($"Simulated Player Deployment {index + 1}"), true);

                    GUILayout.FlexibleSpace();

                    var buttonContent = new GUIContent(string.Empty, "Remove deployment configuration");
                    buttonContent.image = EditorGUIUtility.IconContent(BuiltInTrashIcon).image;

                    if (GUILayout.Button(buttonContent, EditorStyles.miniButton))
                    {
                        return (true, null);
                    }
                }

                using (new EditorGUI.IndentLevelScope())
                using (new EditorGUILayout.VerticalScope())
                {
                    if (foldoutState)
                    {
                        RenderBaseDeploymentConfig(config, copy);
                    }
                }

                if (check.changed)
                {
                    SetStateObject(copy.Name.GetHashCode(), foldoutState);
                }
            }

            return (false, copy);
        }

        private void UpdateSimulatedDeploymentNames(DeploymentConfig config)
        {
            for (var i = 0; i < config.SimulatedPlayerDeploymentConfig.Count; i++)
            {
                var previousFoldoutState =
                    GetStateObject<bool>(config.SimulatedPlayerDeploymentConfig[i].Name.GetHashCode());

                config.SimulatedPlayerDeploymentConfig[i].Name = $"{config.Deployment.Name}_sim{i + 1}";
                config.SimulatedPlayerDeploymentConfig[i].TargetDeploymentName = config.Deployment.Name;

                SetStateObject(config.SimulatedPlayerDeploymentConfig[i].Name.GetHashCode(), previousFoldoutState);
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

        private T GetStateObject<T>(int hash) where T : new()
        {
            if (!localState.TryGetValue(hash, out var value))
            {
                value = new T();
                localState.Add(hash, value);
            }

            return (T) value;
        }

        private void SetStateObject<T>(int hash, T obj)
        {
            localState[hash] = (object) obj;
        }
    }
}
