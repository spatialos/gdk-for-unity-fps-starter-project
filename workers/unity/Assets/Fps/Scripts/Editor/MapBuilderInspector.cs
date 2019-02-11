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
            var myTarget = (MapBuilder) target;

            myTarget.Layers = Mathf.Max(0,
                EditorGUILayout.IntField(new GUIContent(
                    "Number of Tile Layers",
                    "N layers corresponds to 4*(N^2) tiles."), myTarget.Layers));

            myTarget.EmptyTileChance =
                Mathf.Clamp(
                    EditorGUILayout.FloatField(
                        new GUIContent("Chance of Empty Tile",
                            "The chance that a tile in one grid square of the world will be empty."),
                        myTarget.EmptyTileChance), 0f, 1f);

            var numTiles = Mathf.RoundToInt(GetTotalTilesFromLayers(myTarget.Layers) * (1f - myTarget.EmptyTileChance));

            GUI.color = numTiles < WarnTilesThreshold ? Color.white : Color.yellow;
            GUILayout.Label($"Number of tiles to generate: ~{numTiles}");
            GUI.color = Color.white;

            myTarget.Seed = EditorGUILayout.TextField(new GUIContent(
                    "Seed for Map Generator",
                    "Different seeds produce different maps."),
                myTarget.Seed);


            if (GUILayout.Button("Generate Map"))
            {
                if (numTiles < WarnTilesThreshold
                    || GetGenerationUserConfirmation(numTiles))
                {
                    myTarget.CleanAndBuild(myTarget.Layers, myTarget.Seed, myTarget.EmptyTileChance);
                }
            }

            if (GUILayout.Button("Clear Map"))
            {
                myTarget.Clean();
            }
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
