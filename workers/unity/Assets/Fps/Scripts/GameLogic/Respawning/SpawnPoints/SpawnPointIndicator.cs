using UnityEngine;

namespace Fps
{
    public class SpawnPointIndicator : MonoBehaviour
    {
        private static readonly Color transparentLightGreen = new Color(.1f, 1f, .1f, .8f);
        public static readonly Vector3 BoxDimensions = new Vector3(.5f, 2f, .5f);

        private void OnDrawGizmos()
        {
            Gizmos.color = transparentLightGreen;
            var position = transform.position;
            var rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
            Gizmos.matrix = Matrix4x4.TRS(
                position,
                rotation,
                Vector3.one
            );

            Gizmos.DrawCube(Vector3.up * BoxDimensions.y * .5f, BoxDimensions);
            Gizmos.DrawLine(Vector3.forward * BoxDimensions.z * .5f, Vector3.forward * BoxDimensions.z);
        }
    }
}
