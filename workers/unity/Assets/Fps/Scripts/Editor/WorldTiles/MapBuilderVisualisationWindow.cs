using System.Collections;
using Fps.WorldTiles;
using UnityEditor;
using UnityEngine;

namespace Fps.Editor
{
    public class MapBuilderVisualisationWindow : EditorWindow
    {
        private const int WarnTilesThreshold = 2500;

        private int layerCount = 4;
        private string seed = "SpatialOS GDK for Unity";

        private MapBuilder mapBuilder;
        private MapTemplate mapTemplate;

        private const string MapBuilderMenuItem = "SpatialOS/Map Builder";
        private const int MapBuilderMenuPriority = 52;

        [MenuItem(MapBuilderMenuItem, false, MapBuilderMenuPriority)]
        private static void LaunchMapBuilderMenu()
        {
            // Show existing window instance. If one doesn't exist, make one.
            var inspectorWindowType = typeof(EditorWindow).Assembly.GetType("UnityEditor.InspectorWindow");
            var deploymentWindow = GetWindow<MapBuilderVisualisationWindow>(inspectorWindowType);
            deploymentWindow.titleContent.text = "Map Builder Visualisation";
            deploymentWindow.titleContent.tooltip = "A tool for visualising the level to be generated at runtime.";
            deploymentWindow.Show();
        }

        private void SetupMapBuilder()
        {
            mapBuilder = new MapBuilder(mapTemplate, new GameObject("FPS-Level_Visualisation"));
        }

        public void OnGUI()
        {
            layerCount = Mathf.Max(0,
                EditorGUILayout.IntField(new GUIContent(
                    "Number of Tile Layers",
                    "N layers corresponds to 4*(N^2) tiles."), layerCount));

            var numTiles = Mathf.RoundToInt(GetTotalTilesFromLayers(layerCount));

            GUI.color = numTiles < WarnTilesThreshold ? Color.white : Color.yellow;
            GUILayout.Label($"Number of tiles to generate: ~{numTiles}");
            GUI.color = Color.white;

            seed = EditorGUILayout.TextField(new GUIContent(
                    "Seed for Map Generator",
                    "Different seeds produce different maps."),
                seed);

            mapTemplate = (MapTemplate) EditorGUILayout.ObjectField(new GUIContent(
                    "Map Template",
                    "Different seeds produce different maps."),
                mapTemplate,
                typeof(MapTemplate),
                false);

            EditorGUI.BeginDisabledGroup(mapTemplate == null);
            if (GUILayout.Button("Generate Map"))
            {
                if (numTiles < WarnTilesThreshold
                    || GetGenerationUserConfirmation(numTiles))
                {
                    if (mapBuilder == null || mapBuilder.InvalidMapBuilder)
                    {
                        SetupMapBuilder();
                    }

                    UnwindCoroutine(mapBuilder.CleanAndBuild(layerCount, seed));
                }
            }

            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(mapBuilder == null);
            if (GUILayout.Button("Clear Map"))
            {
                mapBuilder.Clean();
            }

            EditorGUI.EndDisabledGroup();
        }

        private bool GetGenerationUserConfirmation(int numTiles)
        {
            return EditorUtility.DisplayDialog("Generate Map Confirmation",
                $"You are generating {numTiles} tiles. This can potentially take a VERY long time, " +
                "and it is recommended that you save first!\n" +
                "Do you wish to continue?",
                "Continue",
                "Cancel");
        }

        private int GetTotalTilesFromLayers(int layers)
        {
            return Mathf.RoundToInt(Mathf.Pow(layers * 2, 2));
        }

        private void UnwindCoroutine(IEnumerator enumerator)
        {
            while (enumerator.MoveNext())
            {
                if (enumerator.Current is IEnumerator nestedEnumerator)
                {
                    UnwindCoroutine(nestedEnumerator);
                }
            }
        }
    }
}
