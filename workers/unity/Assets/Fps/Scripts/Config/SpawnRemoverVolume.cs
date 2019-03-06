using UnityEngine;

namespace Fps
{
    public class SpawnRemoverVolume : MonoBehaviour
    {
        private BoxCollider boxCollider;

        private void Awake()
        {
            boxCollider = GetComponent<BoxCollider>();
        }

        private void OnDrawGizmos()
        {
            if (boxCollider == null)
            {
                boxCollider = GetComponent<BoxCollider>();
            }

            Gizmos.color = new Color(1, 0, 0, .2f);
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            Gizmos.DrawCube(boxCollider.center, boxCollider.size);
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);
        }
    }
}
