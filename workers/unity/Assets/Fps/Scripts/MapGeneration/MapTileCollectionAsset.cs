using UnityEngine;

namespace Fps
{
    [CreateAssetMenu(fileName = "MapTileCollection", menuName = "New Map Tile Collection")]
    public class MapTileCollectionAsset : ScriptableObject
    {
        [SerializeField] private GameObject[] TilePrefabs;
        public int Count => TilePrefabs.Length;
        public Bounds Bounds { get; private set; }
        public Color PixelRepresentationColor = Color.white;


        public GameObject GetRandomTile()
        {
            return Count == 0 ? null : GetPrefab(Random.Range(0, Count));
        }

        public GameObject GetPrefab(int index)
        {
            if (TilePrefabs.Length != 0)
            {
                return TilePrefabs[index % TilePrefabs.Length];
            }

            Debug.LogWarning($"No tiles found in tile collection {name}");
            return null;
        }
    }
}
