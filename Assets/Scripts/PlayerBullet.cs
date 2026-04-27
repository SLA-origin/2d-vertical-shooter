using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    public int damage = 10;

    // 화면 경계값 (상수 처리하거나 외부에서 설정 가능)
    private const float Y_LIMIT = 5.5f;

    void Update()
    {
        // 위로 이동
        transform.Translate(Vector3.up * speed * Time.deltaTime);

        // 화면 위를 벗어나면 Destroy 대신 풀로 반환
        if (transform.position.y > Y_LIMIT)
        {
            ObjectManager.Instance.Release(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 적과 충돌했을 때 Destroy 대신 풀로 반환
        // (Enemy.OnTriggerEnter2D에서 이미 처리하지만 총알 쪽에서도 방어적으로 처리)
        if (other.CompareTag("Enemy"))
        {
            ObjectManager.Instance.Release(this.gameObject);
        }

        // 적 총알과는 물리적 충돌을 무시합니다
        if (other.CompareTag("EnemyBullet")) return;
    }
}