using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MeshUtilities
{
    /// <summary>
    ///     Container class for a combined mesh and an array of materials used in the mesh
    /// </summary>
    public class CombinedMeshAndMaterialsData
    {
        public Mesh combinedMesh;
        public Material[] materialsOnMesh;

        public CombinedMeshAndMaterialsData(Mesh combinedMesh, Material[] materialsOnMesh)
        {
            this.combinedMesh = combinedMesh;
            this.materialsOnMesh = materialsOnMesh;
        }
    }

    public class TileCombinedMeshProvider
    {
        private static int maxBakedScaleSize = 36;

        public static int MaxBakedScaleSize
        {
            get => maxBakedScaleSize;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("MaxBakedScaleSize must be greater than 0");
                }
                else
                {
                    maxBakedScaleSize = value;
                }
            }
        }

        public static CombinedMeshAndMaterialsData GetCombinedMeshes(Transform root)
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

            // Restore position
            root.transform.position = rootInitPos;
            root.transform.rotation = rootInitRot;

            return new CombinedMeshAndMaterialsData(allMaterialsCombined, orderedKeys);
        }

        private static Dictionary<Material, List<CombineInstance>> GatherMaterialsAndMeshes(Transform root)
        {
            var matsToMeshes = new Dictionary<Material, List<CombineInstance>>();

            var meshFilters = root.GetComponentsInChildren<MeshFilter>();
            for (var j = 0; j < meshFilters.Length; j++)
            {
                var meshFilter = meshFilters[j];
                var meshRenderer = meshFilter.GetComponent<MeshRenderer>();

                if (meshFilter.sharedMesh == null)
                {
                    Debug.LogWarning("Skipping missing mesh", meshFilter.gameObject);
                    continue;
                }

                var material = meshRenderer.sharedMaterial;
                var objMesh = Object.Instantiate(meshFilter.sharedMesh);

                ApplyScaleToVertexColours(ref objMesh, meshFilter.transform.localScale);

                var combineInstance = new CombineInstance();
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
            }

            return matsToMeshes;
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


        private static void ApplyScaleToVertexColours(ref Mesh mesh, Vector3 objLocalScale)
        {
            var colors = new Color[mesh.colors.Length];

            for (var i = 0; i < mesh.colors.Length; i++)
            {
                var c = mesh.colors[i];
                c.r = Mathf.Clamp(objLocalScale.x / MaxBakedScaleSize, 0f, 1f);
                c.g = Mathf.Clamp(objLocalScale.y / MaxBakedScaleSize, 0f, 1f);
                c.b = Mathf.Clamp(objLocalScale.z / MaxBakedScaleSize, 0f, 1f);
                colors[i] = c;
            }

            mesh.colors = colors;
        }
    }
}
