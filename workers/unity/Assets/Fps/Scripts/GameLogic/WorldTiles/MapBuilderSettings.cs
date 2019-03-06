using UnityEngine;

namespace Fps
{
    [CreateAssetMenu(fileName = "MapBuilderSettings", menuName = "Improbable/Map Builder Settings")]
    public class MapBuilderSettings : ScriptableObject
    {
        public int SmallWorldLayerCount = 4;
        public int LargeWorldLayerCount = 24;

        public GameObject WorldTileVolumes;

        // Measurements.
        // All sizes are 1:1 ratio in X/Z, so we just define one value to represent both axis.
        public int UnitsPerBlock = 4; // One textured square on the ground is a 'block'.
        public int UnitsPerTile => 9 * UnitsPerBlock;

        public int TilesPerGroundLayer = 4; // Ground layers are large quads that encompass 4x4 tiles.
        public int BoundaryCollisionHeight = 16;

        public int unitsPerGroundLayer => TilesPerGroundLayer * UnitsPerTile;
        public TileTypeCollection DefaultTileType;
    }
}
