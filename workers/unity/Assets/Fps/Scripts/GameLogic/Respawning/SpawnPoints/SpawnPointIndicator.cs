using UnityEngine;

namespace Fps
{
    public class SpawnPointIndicator : MonoBehaviour
    {
        private static readonly Color col = new Color(.3f, 1f, .3f, .3f);
        public static readonly Vector3 BoxDimensions = new Vector3(.5f, 2f, .5f);

        private void OnDrawGizmos()
        {
            Gizmos.color = col;
            var position = transform.position;
            var rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
            Gizmos.matrix = Matrix4x4.TRS(
                position,
                rotation,
                Vector3.one
            );

            Gizmos.DrawCube(Vector3.zero, BoxDimensions);
            Gizmos.DrawLine(Vector3.down * BoxDimensions.y * .5f,
                Vector3.down * BoxDimensions.y * .5f + Vector3.forward * BoxDimensions.z);
        }
    }
}
