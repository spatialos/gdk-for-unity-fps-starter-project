using System.Collections.Generic;
using MeshUtilities;
using UnityEngine;

public class TileCollapser : MonoBehaviour
{
    private readonly Dictionary<string, CombinedMeshAndMaterialsData> collapsedInstances =
        new Dictionary<string, CombinedMeshAndMaterialsData>();

    private void Awake()
    {
        for (var i = 0; i < transform.childCount; i++)
        {
            Debug.Log(transform.childCount);
            var child = transform.GetChild(i);

            if (!collapsedInstances.ContainsKey(child.name))
            {
                var combined = TileCombinedMeshProvider.GetCombinedMeshes(child);
                combined.combinedMesh.name = child.name + "_Mesh";
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
            Destroy(meshRenderer);
        }

        foreach (var meshFilter in obj.GetComponentsInChildren<MeshFilter>())
        {
            Destroy(meshFilter);
        }
    }

    private void ApplyCollapsed(Transform obj)
    {
        Debug.Log("Collapsing tiles on obj " + obj.gameObject);
        var mf = obj.gameObject.AddComponent<MeshFilter>();
        var mr = obj.gameObject.AddComponent<MeshRenderer>();

        mf.sharedMesh = collapsedInstances[obj.name].combinedMesh;
        mr.materials = collapsedInstances[obj.name].materialsOnMesh;
    }
}
