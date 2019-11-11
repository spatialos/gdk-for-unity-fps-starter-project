using System.Collections.Generic;
using System.Linq;
using Fps.WorldTiles;
using UnityEngine;
using Random = System.Random;

namespace Fps.WorldTiles
{
    [CreateAssetMenu(fileName = "MapTemplate", menuName = "Improbable/Map Template")]
    public class MapTemplate : ScriptableObject
    {
        [SerializeField] internal Texture2D templateBitmap;
        [SerializeField] internal TileTypeCollection[] tileCollections;
        [SerializeField] internal TileTypeCollection defaultTileCollection;
        [SerializeField] internal float unitSize = 1;

        private readonly Dictionary<Color32, TileTypeCollection> tileLookup =
            new Dictionary<Color32, TileTypeCollection>();

        public MapTemplate Copy()
        {
            var clone = ScriptableObject.CreateInstance<MapTemplate>();
            clone.templateBitmap = templateBitmap;

            clone.tileCollections = new TileTypeCollection[tileCollections.Length];
            for (var i = 0; i < tileCollections.Length; i++)
            {
                clone.tileCollections[i] = tileCollections[i].Copy();
            }

            clone.defaultTileCollection = defaultTileCollection.Copy();

            clone.InitializeLookup();

            return clone;
        }

        public GameObject GetTileForLocation(Vector2 location, Random random)
        {
            var collection = defaultTileCollection;

            // Offset location so 0,0 is the middle of the bitmap
            location += new Vector2(templateBitmap.width / 2.0f - 1, templateBitmap.height / 2.0f - 1);
            location /= unitSize;
            var texelLocation = Vector2Int.FloorToInt(location);

            if (texelLocation.x >= 0 && texelLocation.x < templateBitmap.width &&
                texelLocation.y >= 0 && texelLocation.y < templateBitmap.height)
            {
                var index = ColorToLookup(templateBitmap.GetPixel(texelLocation.x, texelLocation.y));
                if (index.a == 0xff)
                {
                    if (!tileLookup.TryGetValue(index, out collection))
                    {
                        Debug.Log($"Unknown color: {index}\nThere are {tileLookup.Count} colors available.");
                        PrintColors();
                        collection = defaultTileCollection;
                    }
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
            var message = string.Join(" ", tileLookup.Select(element => $"[{element.Key}]"));
            Debug.Log(message);
        }

        private void OnValidate()
        {
            // Check for issues
            if (!templateBitmap.isReadable)
            {
                Debug.LogError($"Template bitmap {templateBitmap.name} must be Read/Write enabled.");
            }

            if (templateBitmap.format != TextureFormat.RGBA32)
            {
                Debug.LogError($"Template bitmap {templateBitmap.name} must be RGBA32.");
            }

            if (defaultTileCollection == null)
            {
                Debug.LogError($"Default Tile Collection must be set.");
            }

            if (unitSize <= 0)
            {
                unitSize = 1;
            }

            InitializeLookup();
        }

        private void InitializeLookup()
        {
            tileLookup.Clear();
            if (tileCollections == null || tileCollections.Length <= 0)
            {
                return;
            }

            // Fill lookup map
            foreach (var tileType in tileCollections)
            {
                if (tileType == null)
                {
                    continue;
                }

                var index = ColorToLookup(tileType.DisplayColor);
                if (tileLookup.ContainsKey(index))
                {
                    Debug.LogError(
                        $"Tile collection with color {ColorUtility.ToHtmlStringRGBA(tileType.DisplayColor)} already exists.");
                    continue;
                }

                tileLookup.Add(index, tileType);
            }
        }

        private Color32 ColorToLookup(Color color)
        {
            return new Color32(
                (byte) Mathf.Clamp(Mathf.RoundToInt(color.r * (float) byte.MaxValue), 0, (int) byte.MaxValue),
                (byte) Mathf.Clamp(Mathf.RoundToInt(color.g * (float) byte.MaxValue), 0, (int) byte.MaxValue),
                (byte) Mathf.Clamp(Mathf.RoundToInt(color.b * (float) byte.MaxValue), 0, (int) byte.MaxValue),
                (byte) Mathf.Clamp(Mathf.RoundToInt(color.a * (float) byte.MaxValue), 0, (int) byte.MaxValue));
        }
    }
}
