using System.Collections.Generic;
using Improbable.Gdk.Core;
using MeshUtilities;
using UnityEngine;

public class TileCollapser : MonoBehaviour
{
    private readonly Dictionary<string, CombinedMeshAndMaterialsData> collapsedInstances =
        new Dictionary<string, CombinedMeshAndMaterialsData>();

    private static readonly List<MeshRenderer> renderComponentsCache = new List<MeshRenderer>();
    private static readonly List<MeshFilter> filterComponentsCache = new List<MeshFilter>();

    public void CollapseMeshes()
    {
        for (var i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);

            if (!collapsedInstances.ContainsKey(child.name))
            {
                var combined = TileCombinedMeshProvider.GetCombinedMeshes(child);
                combined.combinedMesh.name = $"{child.name}_Mesh";
                collapsedInstances.Add(child.name, combined);
            }

            DestroyMeshRenderers(child);
            ApplyCollapsed(child);
        }

        renderComponentsCache.Clear();
        filterComponentsCache.Clear();
    }

    private void DestroyMeshRenderers(Transform obj)
    {
        obj.GetComponentsInChildren<MeshRenderer>(renderComponentsCache);
        foreach (var meshRenderer in renderComponentsCache)
        {
            UnityObjectDestroyer.Destroy(meshRenderer);
        }

        obj.GetComponentsInChildren<MeshFilter>(filterComponentsCache);
        foreach (var meshFilter in filterComponentsCache)
        {
            UnityObjectDestroyer.Destroy(meshFilter);
        }
    }

    private void ApplyCollapsed(Transform obj)
    {
        var mf = obj.gameObject.AddComponent<MeshFilter>();
        var mr = obj.gameObject.AddComponent<MeshRenderer>();

        mf.sharedMesh = collapsedInstances[obj.name].combinedMesh;
        mr.materials = collapsedInstances[obj.name].materialsOnMesh;
    }
}
