using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapBuilder))]
public class MapBuilderInspector : Editor
{
    private const int WarnLayersThreshold = 25;

    public override void OnInspectorGUI()
    {
        var myTarget = (MapBuilder) target;

        myTarget.Layers = Mathf.Max(0,
            EditorGUILayout.IntField(new GUIContent(
                "Number of Tile Layers",
                "N layers corresponds to 4*(N^2) tiles."), myTarget.Layers));

        var numTiles = GetTotalTilesFromLayers(myTarget.Layers);

        GUI.color = myTarget.Layers < WarnLayersThreshold ? Color.white : Color.yellow;
        GUILayout.Label($"Number of tile to generate: {numTiles}");
        GUI.color = Color.white;

        myTarget.Seed = EditorGUILayout.TextField(new GUIContent(
                "Seed for Map Generator",
                "Different seeds produce different maps."),
            myTarget.Seed);

        myTarget.EmptyTileChance = EditorGUILayout.FloatField(new GUIContent(
                "Chance of Empty Tile",
                "The chance that a tile in one grid square of the world will be empty."),
            myTarget.EmptyTileChance);

        if (GUILayout.Button("Generate Map"))
        {
            if (myTarget.Layers < WarnLayersThreshold
                || EditorUtility.DisplayDialog("Generate Map Confirmation",
                    $"You are generating {numTiles} tiles. This can potentially take a VERY long time, " +
                    "and it is recommended that you save first!\n" +
                    "Do you wish to continue?",
                    "Continue",
                    "Cancel"))
            {
                myTarget.CleanAndBuild();
            }
        }

        if (GUILayout.Button("Clear Map"))
        {
            myTarget.Clean();
        }
    }

    private int GetTotalTilesFromLayers(int layers)
    {
        return Mathf.RoundToInt(Mathf.Pow(layers * 2, 2));
    }
}
