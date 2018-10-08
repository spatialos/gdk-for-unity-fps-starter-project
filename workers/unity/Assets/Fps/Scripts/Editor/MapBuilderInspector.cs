using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapBuilder))]
public class MapBuilderInspector : Editor
{
    public override void OnInspectorGUI()
    {
        var myTarget = (MapBuilder) target;

        myTarget.layers = EditorGUILayout.IntField(new GUIContent(
                "Number of Tile Layers",
                "N layers corresponds to 4*(N^2) tiles."),
            myTarget.layers);

        myTarget.seed = EditorGUILayout.TextField(new GUIContent(
                "Seed for Map Generator",
                "Different seeds produce different maps."),
            myTarget.seed);

        myTarget.emptyTileChance = EditorGUILayout.FloatField(new GUIContent(
                "Chance of Empty Tile",
                "The chance that a tile in one grid square of the world will be empty."),
            myTarget.emptyTileChance);

        myTarget.cubeDensity = EditorGUILayout.FloatField(new GUIContent(
                "Desert Cube Density",
                "Cubes to place per 100x100m, averaged across the full desert area."),
            myTarget.cubeDensity);

        if (GUILayout.Button("Generate Map"))
        {
            myTarget.Clean();
            myTarget.Build();
        }

        if (GUILayout.Button("Clean Map"))
        {
            myTarget.Clean();
        }
    }
}
