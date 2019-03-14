using UnityEngine;
using Random = System.Random;

namespace Fps
{
    [CreateAssetMenu(fileName = "New TileTypeCollection", menuName = "Improbable/Tile Type Collection")]
    public class TileTypeCollection : ScriptableObject
    {
        [SerializeField] private GameObject[] tiles;
        [SerializeField] [Range(0f, 1f)] private float chanceOfEmptyTile;
        [SerializeField] private Color displayColor = Color.blue;
        public Color DisplayColor => displayColor;

        public GameObject GetRandomTile(Random random)
        {
            var rand = random.NextDouble();
            if (chanceOfEmptyTile == 0f || rand > chanceOfEmptyTile)
            {
                return tiles[random.Next(0, tiles.Length)];
            }

            return null;
        }
    }
}
