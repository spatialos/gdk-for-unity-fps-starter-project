using UnityEngine;

namespace Fps
{
    [CreateAssetMenu(fileName = "New Prefab Collection", menuName = "Improbable/Create Prefab Collection")]
    public class TileTypeCollection : ScriptableObject
    {
        [SerializeField] private GameObject[] Tiles;
        [Range(0f, 1f)] public float ChanceOfEmptyTile;
        public Color DisplayColor = Color.blue;

        public GameObject GetRandomTile()
        {
            var rand = Random.value;
            if (ChanceOfEmptyTile == 0f || rand > ChanceOfEmptyTile)
            {
                return Tiles[Random.Range(0, Tiles.Length)];
            }

            return null;
        }
    }
}
