using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CollisionAssetPostProcessor : AssetPostprocessor
{
    public const string BOX_COLLISION_PREFIX = "UBX_"; // Unreal compatible
    public const string CAPSULE_COLLISION_PREFIX = "UCP_"; // Unreal compatible
    public const string SPHERE_COLLISION_PREFIX = "USP_"; // Unreal compatible
    public const string CONVEX_COLLISION_PREFIX = "UCX_"; // Unreal compatible
    public const string MESH_COLLISION_PREFIX = "MSH_"; // UNITY-ONLY, creates mesh collision on object and removes renderer
    public void OnPostprocessModel(GameObject go)
    {
        _currentProcessingModel = go;
        ProcessChildren(go.transform);
    }

    private static GameObject _currentProcessingModel;
    void ProcessChildren(Transform parent, int depth = 0)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            var child = parent.GetChild(i);

            if (child.childCount > 0)
            {
                ProcessChildren(child, depth + 1);
            }

            if (child.name.Length <= 4)
            {
                continue;
            }

            var prefix = child.name.Substring(0, 4);
            switch (prefix)
            {
                case BOX_COLLISION_PREFIX:
                    if (!NameIsValid(child.name))
                    {
                        break;
                    }
                    child.gameObject.AddComponent<BoxCollider>();
                    DestroyMeshComponents(child);
                    break;
                case CAPSULE_COLLISION_PREFIX:
                    if (!NameIsValid(child.name))
                    {
                        break;
                    }
                    DestroyMeshComponents(child);
                    child.gameObject.AddComponent<CapsuleCollider>();
                    break;
                case SPHERE_COLLISION_PREFIX:
                    if (!NameIsValid(child.name))
                    {
                        break;
                    }
                    DestroyMeshComponents(child);
                    child.gameObject.AddComponent<SphereCollider>();
                    break;
                case CONVEX_COLLISION_PREFIX:
                    if (!NameIsValid(child.name))
                    {
                        break;
                    }

                    var meshcolliderConvex = child.gameObject.AddComponent<MeshCollider>();
                    meshcolliderConvex.sharedMesh = child.GetComponent<MeshFilter>().sharedMesh;
                    meshcolliderConvex.convex = true;
                    DestroyMeshComponents(child);
                    break;
                case MESH_COLLISION_PREFIX:
                    var meshcollider = child.gameObject.AddComponent<MeshCollider>();
                    meshcollider.sharedMesh = child.GetComponent<MeshFilter>().sharedMesh;
                    DestroyMeshComponents(child);
                    break;
            }
        }
    }

    void DestroyMeshComponents(Transform t)
    {
        var meshCollider = t.gameObject.GetComponent<MeshCollider>();
        if (meshCollider != null)
        {
            Object.DestroyImmediate(meshCollider);
        }
        var meshRenderer = t.gameObject.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            Object.DestroyImmediate(meshRenderer);
        }
        var meshFilter = t.gameObject.GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            Object.DestroyImmediate(meshFilter);
        }
    }

    bool NameIsValid(string name)
    {
        var valid = name.Length >= 4 + _currentProcessingModel.name.Length
                    && name.Substring(4, _currentProcessingModel.name.Length) == _currentProcessingModel.name;

        if (valid) return true;
        Debug.LogWarning("Skipping possible collider " + name + " as name failed validation test (PREFIX_MeshName...)");
        return false;
    }

}
