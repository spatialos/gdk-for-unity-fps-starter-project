using UnityEngine;

namespace Fps.Respawning
{
    public class SpawnPointIndicator : MonoBehaviour
    {
        private static readonly Color gizmoColor = new Color(.1f, 1f, .1f, .8f); // Transparent green
        private static readonly Vector3 boxDimensions = new Vector3(.5f, 2f, .5f);

        private void OnDrawGizmos()
        {
            Gizmos.color = gizmoColor;
            var position = transform.position;
            var rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
            Gizmos.matrix = Matrix4x4.TRS(
                position,
                rotation,
                Vector3.one
            );

            Gizmos.DrawCube(Vector3.up * boxDimensions.y * .5f, boxDimensions);
            Gizmos.DrawLine(Vector3.forward * boxDimensions.z * .5f, Vector3.forward * boxDimensions.z);
        }
    }
}
