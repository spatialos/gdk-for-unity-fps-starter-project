using UnityEngine;
using UnityEngine.Profiling;
using Random = System.Random;

namespace Fps.WorldTiles
{
    [CreateAssetMenu(fileName = "New TileTypeCollection", menuName = "Improbable/Tile Type Collection")]
    public class TileTypeCollection : ScriptableObject
    {
        [SerializeField] private Color displayColor = Color.blue;
        [SerializeField] private GameObject[] tiles;
        [SerializeField] [Range(0f, 1f)] private float chanceOfEmptyTile;
        public Color DisplayColor => displayColor;

        private GameObject[] optimizedTiles;

        public TileTypeCollection Copy()
        {
            return Object.Instantiate(this);
        }

        public GameObject GetRandomTile(Random random)
        {
            var rand = random.NextDouble();
            if (chanceOfEmptyTile == 0f || rand > chanceOfEmptyTile)
            {
                return optimizedTiles[random.Next(0, optimizedTiles.Length)];
            }

            return null;
        }

        public void LoadAndOptimizeTiles()
        {
            var collapser = new TileCollapser();
            optimizedTiles = new GameObject[tiles.Length];

            for (var i = 0; i < tiles.Length; i++)
            {
                Profiler.BeginSample("OptimizeTile");
                var tile = Instantiate(tiles[i]);
                tile.name = tiles[i].name;
                collapser.CollapseMeshes(tile);
                optimizedTiles[i] = tile;
                Profiler.EndSample();
            }
        }

        public void Clear()
        {
            for (var i = 0; i < optimizedTiles.Length; i++)
            {
                DestroyImmediate(optimizedTiles[i]);
            }

            optimizedTiles = null;
        }

        private void OnValidate()
        {
            displayColor.a = 1;
        }
    }
}
