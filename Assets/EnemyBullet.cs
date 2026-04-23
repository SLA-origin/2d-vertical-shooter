using System;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    private bool isMove = false;
    private Vector3 dir;
    public float speed = 1f;
    public int damage = 25;

    private void Update()
    {
        
        if (!isMove)
            return;

        transform.Translate(dir * speed * Time.deltaTime, Space.World);

        if (AreaDrawer.Instance != null && AreaDrawer.Instance.IsOutOfBounds(transform.position))
            Destroy(gameObject);
    }

    public void StartMove(Vector3 dir)
    {
        isMove = true;
        this.dir = dir.normalized;
    }
}