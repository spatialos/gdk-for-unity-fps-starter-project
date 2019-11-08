using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Fps.WorldTiles
{
    public class TileCollapser
    {
        private readonly Dictionary<string, CombinedMeshAndMaterialsData> collapsedInstances =
            new Dictionary<string, CombinedMeshAndMaterialsData>();

        private readonly List<MeshRenderer> renderComponentsCache = new List<MeshRenderer>();
        private readonly List<MeshFilter> filterComponentsCache = new List<MeshFilter>();

        public void CollapseMeshes(GameObject tile)
        {
            if (!collapsedInstances.ContainsKey(tile.name))
            {
                var combined = TileCombinedMeshProvider.GetCombinedMeshes(tile.transform);
                combined.combinedMesh.name = $"{tile.name}_Mesh";
                collapsedInstances.Add(tile.name, combined);
            }

            DestroyMeshRenderers(tile);
            ApplyCollapsed(tile);

            renderComponentsCache.Clear();
            filterComponentsCache.Clear();
        }

        private void DestroyMeshRenderers(GameObject tile)
        {
            tile.GetComponentsInChildren<MeshRenderer>(renderComponentsCache);
            foreach (var meshRenderer in renderComponentsCache)
            {
                Object.DestroyImmediate(meshRenderer);
            }

            tile.GetComponentsInChildren<MeshFilter>(filterComponentsCache);
            foreach (var meshFilter in filterComponentsCache)
            {
                Object.DestroyImmediate(meshFilter);
            }
        }

        private void ApplyCollapsed(GameObject tile)
        {
            var mf = tile.gameObject.AddComponent<MeshFilter>();
            var mr = tile.gameObject.AddComponent<MeshRenderer>();

            var cachedInstance = collapsedInstances[tile.name];

            mf.sharedMesh = cachedInstance.combinedMesh;
            mr.materials = cachedInstance.materialsOnMesh;
        }
    }
}
