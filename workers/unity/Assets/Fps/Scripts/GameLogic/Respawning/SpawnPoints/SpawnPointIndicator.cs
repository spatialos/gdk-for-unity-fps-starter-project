using UnityEngine;

namespace Fps
{
    public class SpawnPointIndicator : MonoBehaviour
    {
        private readonly Vector3 boxDimensions = new Vector3(1, .2f, 1);

        private void OnDrawGizmos()
        {
            var position = transform.position;
            var rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
            Gizmos.matrix = Matrix4x4.TRS(
                position,
                rotation,
                Vector3.one
            );

            Gizmos.color = new Color(0,0,1,.2f);
            Gizmos.DrawCube(Vector3.zero, boxDimensions);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(Vector3.zero, boxDimensions);
            Gizmos.DrawLine(Vector3.zero, Vector3.forward * boxDimensions.z / 2);
        }
    }
}
