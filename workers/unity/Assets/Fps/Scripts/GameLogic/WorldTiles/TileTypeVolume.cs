using Fps;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class TileTypeVolume : MonoBehaviour
{
    public TileTypeCollection TypeCollection;
    private BoxCollider boxCollider;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    private void OnDrawGizmos()
    {
        if (TypeCollection == null)
        {
            return;
        }

        if (boxCollider == null)
        {
            boxCollider = GetComponent<BoxCollider>();
        }

        Gizmos.color = TypeCollection.DisplayColor;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        Gizmos.DrawCube(boxCollider.center, boxCollider.size);
    }

    public GameObject GetRandomTile()
    {
        if (TypeCollection == null)
        {
            return null;
        }

        return TypeCollection.GetRandomTile();
    }
}
