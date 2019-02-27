using UnityEngine;

namespace Fps
{
    [CreateAssetMenu(fileName = "New Prefab Collection", menuName = "Improbable/Create Prefab Collection")]
    public class TileCollection : ScriptableObject
    {
        public GameObject[] Tiles;
        [Range(0f, 1f)] public float ChanceOfEmptyTile;
    }
}
