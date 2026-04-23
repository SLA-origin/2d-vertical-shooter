using UnityEngine;

/// <summary>
/// 플레이어 총알의 이동 및 수명을 관리합니다.
/// 매 프레임 위쪽으로 이동하며, 화면을 벗어나면 씬에서 제거됩니다.
/// </summary>
public class PlayerBullet : MonoBehaviour
{
    [SerializeField] private float speed = 10f; // 총알 이동 속도

    public int damage = 10;
    
    void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);

        if (AreaDrawer.Instance != null && AreaDrawer.Instance.IsOutOfBounds(transform.position))
            Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 적 총알과의 물리 충돌 무시 (플레이어 총알이 적 총알을 밀어내지 않도록)
        if (collision.gameObject.CompareTag("EnemyBullet"))
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collision.collider);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 적 총알과 겹쳐도 아무 처리 하지 않음
        if (other.CompareTag("EnemyBullet"))
            return;
    }
}