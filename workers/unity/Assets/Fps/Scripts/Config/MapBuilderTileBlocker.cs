using System.Collections;
using System.Collections.Generic;
using Fps;
using UnityEngine;

public class MapBuilderTileBlocker : MonoBehaviour
{
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, .3f, .3f, .2f);
        Gizmos.matrix = Matrix4x4.TRS(transform.position, Quaternion.identity, transform.localScale);
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        Gizmos.matrix = Matrix4x4.identity;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, .2f);
        Gizmos.matrix = Matrix4x4.TRS(transform.position, Quaternion.identity, transform.localScale);
        Gizmos.DrawCube(Vector3.zero, Vector3.one);
        Gizmos.matrix = Matrix4x4.identity;
    }
}
