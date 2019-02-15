using System.Collections.Generic;
using Improbable.Gdk.Core;
using MeshUtilities;
using UnityEngine;

public class TileCollapser : MonoBehaviour
{
    private readonly Dictionary<string, CombinedMeshAndMaterialsData> collapsedInstances =
        new Dictionary<string, CombinedMeshAndMaterialsData>();

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
    }

    private void DestroyMeshRenderers(Transform obj)
    {
        foreach (var meshRenderer in obj.GetComponentsInChildren<MeshRenderer>())
        {
            UnityObjectDestroyer.Destroy(meshRenderer);
        }

        foreach (var meshFilter in obj.GetComponentsInChildren<MeshFilter>())
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
