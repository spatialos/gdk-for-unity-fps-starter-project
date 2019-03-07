using UnityEngine;

namespace Fps
{
    [CreateAssetMenu(fileName = "New TileTypeCollection", menuName = "Improbable/Tile Type Collection")]
    public class TileTypeCollection : ScriptableObject
    {
        [SerializeField] private GameObject[] tiles;
        [SerializeField] [Range(0f, 1f)] private float chanceOfEmptyTile;
        [SerializeField] private Color displayColor = Color.blue;
        public Color DisplayColor => displayColor;

        public GameObject GetRandomTile()
        {
            var rand = Random.value;
            if (chanceOfEmptyTile == 0f || rand > chanceOfEmptyTile)
            {
                return tiles[Random.Range(0, tiles.Length)];
            }

            return null;
        }
    }
}
