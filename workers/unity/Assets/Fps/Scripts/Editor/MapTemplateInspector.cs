using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Fps.Editor
{
    [CustomEditor(typeof(MapTemplate))]
    public class MapTemplateInspector : UnityEditor.Editor
    {
        private MapTemplate template;

        public void OnEnable()
        {
            template = (MapTemplate) target;
        }

        public override void OnInspectorGUI()
        {
            int? elementToRemove = null;

            // Template bitmap asset
            template.templateBitmap = (Texture2D) EditorGUILayout.ObjectField(new GUIContent(
                    "Template bitmap",
                    "2D bitmap to indicate tile types."),
                template.templateBitmap,
                typeof(MapTemplate),
                false);

            // Units per tile
            template.unitSize = EditorGUILayout.FloatField("Tile size", template.unitSize);

            // Collection asset
            template.defaultTileCollection = (TileTypeCollection) EditorGUILayout.ObjectField(
                new GUIContent("Default Tile Collection"),
                template.defaultTileCollection,
                typeof(TileTypeCollection),
                false);

            EditorGUILayout.LabelField("Tile Collections");

            using (new EditorGUI.IndentLevelScope())
            {
                for (var i = 0; i < template.tileCollections.Length; i++)
                {
                    // Calculate positioning of the elements on this line
                    var collection = template.tileCollections[i];
                    var color = collection?.DisplayColor ?? Color.clear;

                    // Icons are 16 pixels in size. Align elements.
                    var rect = EditorGUILayout.GetControlRect(false, 16);
                    var colorRect = new Rect(rect.x + 16, rect.y, 16, rect.height);
                    var objectRect = new Rect(rect.x + 26, rect.y, rect.width - 58, rect.height);
                    var removeRect = new Rect(rect.width - 16, rect.y, 16, rect.height);

                    // Bitmap color
                    if (GUI.Button(colorRect, new GUIContent("", $"HTML RGB: {ColorUtility.ToHtmlStringRGBA(color)}"),
                        GUIStyle.none))
                    {
                        GUIUtility.systemCopyBuffer = ColorUtility.ToHtmlStringRGBA(color);
                    }

                    EditorGUI.DrawRect(colorRect, color);

                    // Collection asset
                    template.tileCollections[i] = (TileTypeCollection) EditorGUI.ObjectField(
                        objectRect,
                        template.tileCollections[i],
                        typeof(TileTypeCollection),
                        false);

                    // Remove entry
                    if (GUI.Button(removeRect, EditorGUIUtility.IconContent("TreeEditor.Trash"), GUIStyle.none))
                    {
                        // Save element index to remove
                        elementToRemove = i;
                    }
                }

                // Icon for new entry
                var newRect = EditorGUILayout.GetControlRect(false, 16);
                var boxRect = new Rect(newRect.x + 16, newRect.y, 16, newRect.height);
                GUI.Button(boxRect, EditorGUIUtility.IconContent("d_Toolbar Plus"), GUIStyle.none);

                // New entry selector
                var newCollection = (TileTypeCollection) EditorGUI.ObjectField(
                    new Rect(newRect.x + 26, newRect.y, newRect.width - 58, newRect.height),
                    null,
                    typeof(TileTypeCollection),
                    false);

                if (newCollection != null)
                {
                    Array.Resize(ref template.tileCollections, template.tileCollections.Length + 1);
                }

                // Remove requested element
                if (elementToRemove.HasValue)
                {
                    var temp = new List<TileTypeCollection>(template.tileCollections);
                    temp.RemoveAt(elementToRemove.Value);
                    template.tileCollections = temp.ToArray();
                }
            }
        }
    }
}
