using System;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    private bool isMove = false;
    private Vector3 dir;
    public float speed = 1f;
    public int damage = 25;

    // OnEnable: 풀에서 꺼낼 때(SetActive true)마다 호출 — 이동 상태를 초기화
    private void OnEnable()
    {
        isMove = false; // 이동 중지 상태로 초기화 (StartMove가 호출되어야 이동을 시작합니다)
    }

    private void Update()
    {
        
        if (!isMove)
            return;

        transform.Translate(dir * speed * Time.deltaTime, Space.World);

        if (AreaDrawer.Instance != null && AreaDrawer.Instance.IsOutOfBounds(transform.position))
            ObjectManager.Instance.Release(gameObject); // Destroy 대신 풀로 반환
    }

    public void StartMove(Vector3 dir)
    {
        isMove = true;
        this.dir = dir.normalized;
    }
}