using UnityEngine;

public class RoomBounds : MonoBehaviour
{
    public Vector3 center;
    public Vector3 size;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position + center, size);
    }
}
