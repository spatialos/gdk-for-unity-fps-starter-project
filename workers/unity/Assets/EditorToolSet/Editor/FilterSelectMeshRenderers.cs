using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class FilterSelectMeshRenderers : MonoBehaviour {

    
    [MenuItem("ImprobaTools/Select Mesh Renderers")]
    public static void SelectMeshRenderers()
    {
        var objs = new List<GameObject>(Selection.gameObjects);

        var selectObjs = new List<GameObject>();

        foreach (var go in objs)
        {
            var mr = go.GetComponent<MeshRenderer>();
            if (mr != null)
            {
                selectObjs.Add(go);
            }

            var mrs = go.GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < mrs.Length; i++)
            {
                selectObjs.Add(mrs[i].gameObject);
            }
        }

        var instanceIds = new List<int>();
        foreach (var selectObj in selectObjs)
        {
            instanceIds.Add(selectObj.GetInstanceID());
        }

        Selection.instanceIDs = instanceIds.ToArray();
    }
}
