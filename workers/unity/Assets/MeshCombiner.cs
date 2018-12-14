using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     Gathers all MeshFilters in object and its children, and combines them all into 1 mesh
///     with sub-meshes and materials.
///     Additionally, stores original object scaling to vertex colours.
///     Currently does NOT support source meshes with more than 1 material on them.
/// </summary>
public class MeshCombiner : MonoBehaviour
{
    // TODO remove LOD behaviour

    public bool ignoreLods;

    public int MaxBakedScaleSize = 36;

    private void Awake()
    {
        CombineMeshes(transform);
    }

    private void CombineMeshes(Transform root)
    {
        // Store position
        var rootInitPos = root.transform.position;
        var rootInitRot = root.transform.rotation;

        // Reset to identity
        root.transform.position = Vector3.zero;
        root.transform.rotation = Quaternion.identity;

        // Gather + combine
        var materialsAndMeshes = GatherMaterialsAndMeshes(root);
        var orderedKeys = GetOrderedKeys(materialsAndMeshes);
        var allMaterialsCombined = CombineMaterialsAndMeshes(root, materialsAndMeshes, orderedKeys);

        // Apply to components
        var meshFilter = root.GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            meshFilter = root.gameObject.AddComponent<MeshFilter>();
        }

        meshFilter.sharedMesh = allMaterialsCombined;

        var meshRenderer = root.GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            meshRenderer = root.gameObject.AddComponent<MeshRenderer>();
        }

        meshRenderer.sharedMaterials = orderedKeys;

        // Restore position
        root.transform.position = rootInitPos;
        root.transform.rotation = rootInitRot;
    }

    private static Material[] GetOrderedKeys(Dictionary<Material, List<CombineInstance>> materialsAndMeshes)
    {
        var orderedKeys = new Material[materialsAndMeshes.Keys.Count];
        var k = 0;
        foreach (var key in materialsAndMeshes.Keys)
        {
            orderedKeys[k++] = key;
        }

        return orderedKeys;
    }

    private static Mesh CombineMaterialsAndMeshes(Transform root,
        IReadOnlyDictionary<Material, List<CombineInstance>> materialsAndMeshes,
        IEnumerable<Material> orderedMaterials)
    {
        var singleMaterialsCombinedInstances = new List<CombineInstance>();

        foreach (var material in orderedMaterials)
        {
            var sourceMeshes = materialsAndMeshes[material];
            var singleMaterialCombined = new Mesh();
            singleMaterialCombined.CombineMeshes(sourceMeshes.ToArray());

            var singleMaterialCombinedInstance = new CombineInstance();
            singleMaterialCombinedInstance.mesh = singleMaterialCombined;
            singleMaterialCombinedInstance.transform = root.localToWorldMatrix;
            singleMaterialsCombinedInstances.Add(singleMaterialCombinedInstance);
        }

        var allMaterialsCombined = new Mesh();
        allMaterialsCombined.CombineMeshes(singleMaterialsCombinedInstances.ToArray(), false);
        return allMaterialsCombined;
    }

    private Dictionary<Material, List<CombineInstance>> GatherMaterialsAndMeshes(Transform root)
    {
        var meshFilters = root.GetComponentsInChildren<MeshFilter>();
        var matsToMeshes = new Dictionary<Material, List<CombineInstance>>();
        for (var j = 0; j < meshFilters.Length; j++)
        {
            var meshFilter = meshFilters[j];
            if (meshFilter.gameObject.name.Contains("LOD"))
            {
                if (ignoreLods)
                {
                    continue;
                }

                var highestLOD = GetHighestLOD(meshFilter.transform.parent);
                var lowestLOD = GetLowestLOD(meshFilter.transform.parent);
                var thisLOD = GetLODFromName(meshFilter.gameObject.name);

                if (thisLOD != lowestLOD)
                {
                    var collider = meshFilter.GetComponent<Collider>();
                    if (collider)
                    {
                        collider.enabled = false;
                    }
                }

                if (thisLOD != highestLOD)
                {
                    meshFilter.GetComponent<MeshRenderer>().enabled = false;
                    continue;
                }
            }


            var meshRenderer = meshFilter.GetComponent<MeshRenderer>();

            var material = meshRenderer.sharedMaterial;
            var combineInstance = new CombineInstance();


            var objMesh = meshFilter.mesh;
            ApplyScaleToVertexColours(meshFilter.transform.localScale, ref objMesh);

            combineInstance.mesh = objMesh;
            combineInstance.transform = meshFilter.transform.localToWorldMatrix;

            if (matsToMeshes.ContainsKey(material))
            {
                matsToMeshes[material].Add(combineInstance);
            }
            else
            {
                matsToMeshes.Add(material, new List<CombineInstance>());
                matsToMeshes[material].Add(combineInstance);
            }


            meshFilter.GetComponent<MeshRenderer>().enabled = false;
        }

        return matsToMeshes;
    }

    private int GetHighestLOD(Transform lodParent)
    {
        var highest = 0;
        for (var i = 0; i < lodParent.childCount; i++)
        {
            var lodNum = GetLODFromName(lodParent.GetChild(i).name);
            if (lodNum > highest)
            {
                highest = lodNum;
            }
        }

        return highest;
    }

    private int GetLowestLOD(Transform lodParent)
    {
        var lowest = int.MaxValue;
        for (var i = 0; i < lodParent.childCount; i++)
        {
            var lodNum = GetLODFromName(lodParent.GetChild(i).name);
            if (lodNum < lowest)
            {
                lowest = lodNum;
            }
        }

        return lowest;
    }

    private void ApplyScaleToVertexColours(Vector3 objLocalScale, ref Mesh mesh)
    {
        var colors = new Color[mesh.colors.Length];

		for (var i =0; i < mesh.colors.Length; i++) {
			var c = mesh.colors[i];
			c.r = Mathf.Clamp(objLocalScale.x / MaxBakedScaleSize, 0f, 1f);
            c.g = Mathf.Clamp(objLocalScale.y / MaxBakedScaleSize, 0f, 1f);
            c.b = Mathf.Clamp(objLocalScale.z / MaxBakedScaleSize, 0f, 1f);
			colors[i] = c;
		}
		/*
        var objCol = new Color
        {
            r = Mathf.Clamp(objLocalScale.x / MaxBakedScaleSize, 0f, 1f),
            g = Mathf.Clamp(objLocalScale.y / MaxBakedScaleSize, 0f, 1f),
            b = Mathf.Clamp(objLocalScale.z / MaxBakedScaleSize, 0f, 1f)
        };

        for (var i = 0; i < colors.Length; i++)
        {
            colors[i] = objCol;
        }*/

        mesh.colors = colors;
    }

    private int GetLODFromName(string name)
    {
        Debug.Assert(name.Contains("LOD"));
        var pieces = name.Split(new string[] { "LOD" }, StringSplitOptions.RemoveEmptyEntries);
        if (pieces.Length <= 1)
        {
            throw new ArgumentException($"Expected more than one part to LOD obj name {name} once split ");
        }

        return int.Parse(pieces[pieces.Length - 1]);
    }
}
