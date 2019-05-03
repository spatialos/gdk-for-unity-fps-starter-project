using System.Collections.Generic;
using Improbable.Gdk.Core;
using MeshUtilities;
using UnityEngine;

public static class TileCollapser
{
    private static readonly Dictionary<string, CombinedMeshAndMaterialsData> collapsedInstances =
        new Dictionary<string, CombinedMeshAndMaterialsData>();

    private static readonly List<MeshRenderer> renderComponentsCache = new List<MeshRenderer>();
    private static readonly List<MeshFilter> filterComponentsCache = new List<MeshFilter>();

    public static void CollapseMeshes(GameObject tile)
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

    public static void Clear()
    {
        collapsedInstances.Clear();
    }

    private static void DestroyMeshRenderers(GameObject tile)
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

    private static void ApplyCollapsed(GameObject tile)
    {
        var mf = tile.gameObject.AddComponent<MeshFilter>();
        var mr = tile.gameObject.AddComponent<MeshRenderer>();

        var cachedInstance = collapsedInstances[tile.name];

        mf.sharedMesh = cachedInstance.combinedMesh;
        mr.materials = cachedInstance.materialsOnMesh;
    }
}
