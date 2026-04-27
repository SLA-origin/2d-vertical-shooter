using UnityEngine;

public class AreaDrawer : MonoBehaviour
{
    public static AreaDrawer Instance { get; private set; }

    public Transform topLeft;
    public Transform topRight;
    public Transform bottomLeft;
    public Transform bottomRight;

    private void Awake()
    {
        Instance = this;
    }

    public bool IsOutOfBounds(Vector3 position)
    {
        if (topLeft == null || topRight == null || bottomLeft == null || bottomRight == null)
            return false;

        float minX = Mathf.Min(topLeft.position.x, bottomLeft.position.x);
        float maxX = Mathf.Max(topRight.position.x, bottomRight.position.x);
        float minY = Mathf.Min(bottomLeft.position.y, bottomRight.position.y);
        float maxY = Mathf.Max(topLeft.position.y, topRight.position.y);

        return position.x < minX || position.x > maxX ||
               position.y < minY || position.y > maxY;
    }

    private void OnDrawGizmos()
    {
        if (topLeft == null || topRight == null || bottomLeft == null || bottomRight == null)
            return;

        Gizmos.color = Color.green;

        Gizmos.DrawLine(topLeft.position, topRight.position);
        Gizmos.DrawLine(topRight.position, bottomRight.position);
        Gizmos.DrawLine(bottomRight.position, bottomLeft.position);
        Gizmos.DrawLine(bottomLeft.position, topLeft.position);
    }
}