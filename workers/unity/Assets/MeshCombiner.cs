using System.Collections.Generic;
using Fps;
using UnityEngine;

public class MeshCombiner : MonoBehaviour
{
    private void Start()
    {
        var mapBuilder = FindObjectOfType<MapBuilder>();

        var tileEnablers = mapBuilder.GetComponentsInChildren<TileEnabler>();

        var tileMeshMap = new Dictionary<TileEnabler, CombineInstance[]>();

        for (var i = 0; i < tileEnablers.Length; i++)
        {
            var meshFilters = tileEnablers[i].GetComponentsInChildren<MeshFilter>();

            var meshes = new Mesh[meshFilters.Length];

            var meshCombineInstances = new CombineInstance[meshFilters.Length];

            for (var j = 0; j < meshFilters.Length; j++)
            {
                var cm = new CombineInstance();
                cm.mesh = meshFilters[j].sharedMesh;
                cm.subMeshIndex = 0;
                meshCombineInstances[j] = cm;
            }
        }
    }
}
