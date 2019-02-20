using UnityEditor;
using UnityEngine;

namespace Fps
{
    [CustomEditor(typeof(MapBuilder))]
    public class MapBuilderInspector : Editor
    {
        private const int WarnTilesThreshold = 2500;

        public override void OnInspectorGUI()
        {
            var mapBuilder = (MapBuilder) target;
            var defaultTileCollectionProp = serializedObject.FindProperty(nameof(mapBuilder.DefaultTileCollection));


            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(MapBuilder.MapTileLookupTexture)));

            EditorGUILayout.PropertyField(defaultTileCollectionProp);

            if (defaultTileCollectionProp.objectReferenceValue != null)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(MapBuilder.ExtraTileCollections)), true);
            }

            mapBuilder.Layers = Mathf.Max(0,
                EditorGUILayout.IntField(new GUIContent(
                    "Number of Tile Layers",
                    "N layers corresponds to 4*(N^2) tiles."), mapBuilder.Layers));

            mapBuilder.EmptyTileChance =
                Mathf.Clamp(
                    EditorGUILayout.FloatField(
                        new GUIContent("Chance of Empty Tile",
                            "The chance that a tile in one grid square of the world will be empty."),
                        mapBuilder.EmptyTileChance), 0f, 1f);

            var numTiles =
                Mathf.RoundToInt(GetTotalTilesFromLayers(mapBuilder.Layers) * (1f - mapBuilder.EmptyTileChance));

            GUI.color = numTiles < WarnTilesThreshold ? Color.white : Color.yellow;
            GUILayout.Label($"Number of tiles to generate: ~{numTiles}");
            GUI.color = Color.white;

            mapBuilder.Seed = EditorGUILayout.TextField(new GUIContent(
                    "Seed for Map Generator",
                    "Different seeds produce different maps."),
                mapBuilder.Seed);


            GUI.enabled = defaultTileCollectionProp.objectReferenceValue != null;
            if (GUILayout.Button(new GUIContent("Generate Map", "Default Tile Collection must have a reference")))
            {
                if (numTiles < WarnTilesThreshold
                    || GetGenerationUserConfirmation(numTiles))
                {
                    mapBuilder.CleanAndBuild();
                }
            }

            GUI.enabled = true;

            if (GUILayout.Button("Clear Map"))
            {
                mapBuilder.Clean();
            }

            serializedObject.ApplyModifiedProperties();
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
    }
}
