using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace Fps
{
    [CreateAssetMenu(fileName = "MapTemplate", menuName = "Improbable/Map Template")]
    public class MapTemplate : ScriptableObject
    {
        [SerializeField] internal Texture2D templateBitmap;
        [SerializeField] internal TileTypeCollection[] tileCollections;
        [SerializeField] internal TileTypeCollection defaultTileCollection;
        [SerializeField] internal float unitSize = 1;

        private readonly Dictionary<Color32, TileTypeCollection> tileLookup = new Dictionary<Color32, TileTypeCollection>();

        public GameObject GetTileForLocation(Vector2 location, Random random)
        {
            TileTypeCollection collection = defaultTileCollection;

            // Offset location so 0,0 is the middle of the bitmap
            location += new Vector2(templateBitmap.width / 2.0f, templateBitmap.height / 2.0f);
            location /= unitSize;
            Vector2Int texelLocation = Vector2Int.FloorToInt(location);

            if (texelLocation.x >= 0 && texelLocation.x < templateBitmap.width &&
                texelLocation.y >= 0 && texelLocation.y < templateBitmap.height)
            {
                Color32 index = ColorToLookup(templateBitmap.GetPixel(texelLocation.x, texelLocation.y));
                if (!tileLookup.TryGetValue(index, out collection))
                {
                    Debug.Log($"Unknown color: {index}\nThere are {tileLookup.Count} colors available.");
                    PrintColors();
                    collection = defaultTileCollection;
                }
            }

            return collection.GetRandomTile(random);
        }

        private void OnEnable()
        {
            InitializeLookup();
        }

        private void PrintColors()
        {
            var message = tileLookup.Select(element => $"[{element.Key}]")
                .Aggregate("", (current, next) => $"{current} {next}");
            Debug.Log(message);
        }

        private void OnValidate()
        {
            // Check for issues
            if (!templateBitmap.isReadable)
            {
                Debug.LogError($"template bitmap {templateBitmap.name} must be Read/Write enabled.");
            }

            if (templateBitmap.format != TextureFormat.RGB24)
            {
                Debug.LogError($"template bitmap {templateBitmap.name} must be RGB24.");
            }

            if (defaultTileCollection == null)
            {
                Debug.LogError($"Default Tile Collection must be set.");
            }

            if (unitSize == 0)
            {
                unitSize = 1;
            }

            InitializeLookup();
        }

        private void InitializeLookup()
        {
            // Fill lookup map
            tileLookup.Clear();
            if (tileCollections != null && tileCollections.Length > 0)
            {
                foreach (var tileType in tileCollections)
                {
                    if (tileType == null)
                    {
                        continue;
                    }

                    var index = ColorToLookup(tileType.DisplayColor);
                    if (tileLookup.ContainsKey(index))
                    {
                        Debug.LogError($"Tile collection with color {ColorUtility.ToHtmlStringRGBA(tileType.DisplayColor)} already exists.");
                        continue;
                    }

                    tileLookup.Add(index, tileType);
                }
            }
        }

        private Color32 ColorToLookup(Color color)
        {
            return new Color32((byte) Mathf.Clamp(Mathf.RoundToInt(color.r * (float) byte.MaxValue), 0, (int) byte.MaxValue), (byte) Mathf.Clamp(Mathf.RoundToInt(color.g * (float) byte.MaxValue), 0, (int) byte.MaxValue), (byte) Mathf.Clamp(Mathf.RoundToInt(color.b * (float) byte.MaxValue), 0, (int) byte.MaxValue), (byte) Mathf.Clamp(Mathf.RoundToInt(color.a * (float) byte.MaxValue), 0, (int) byte.MaxValue));
        }
    }
}
