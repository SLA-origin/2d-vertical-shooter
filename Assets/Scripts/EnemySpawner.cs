using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public enum Direction
    {
        LEFT,
        RIGHT
    }
    public Transform startPoint;
    public Transform endPoint;
    public Direction direction;

    public bool normalized;

    public Vector3 GetDir()
    {
        if (normalized)
            return (endPoint.position - startPoint.position).normalized;
        else
            return endPoint.position - startPoint.position;
    }
}