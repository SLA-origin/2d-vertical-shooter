using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 플레이어의 입력 처리, 이동 및 총알 발사를 담당합니다.
/// power 값(1~3)에 따라 발사 패턴이 달라집니다.
/// </summary>
public class Player : MonoBehaviour
{
    [SerializeField] private Transform firePoint;             // 총알이 생성될 기준 위치
    [SerializeField] private GameObject sideBulletPrefab;    // 좌우 총알 프리팹
    [SerializeField] private GameObject centerBulletPrefab;  // 중앙 강화 총알 프리팹 (power 3 전용)
    [SerializeField] private float moveSpeed = 5f;           // 이동 속도
    [SerializeField] private float fireRate = 0.1f;          // 발사 간격 (초) — 값이 작을수록 빠르게 발사
    [SerializeField] private float sideOffset = 0.25f;       // 좌우 총알의 중앙으로부터 떨어진 거리
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int enemyCollisionDamage = 999;
    [SerializeField] private float respawnInvincibleDuration = 1.2f;

    public int power = 1; // 현재 파워 레벨 (1: 단발, 2: 양옆 2발, 3: 중앙+양옆 3발)

    private float _fireTimer;       // 마지막 발사 이후 경과 시간
    private Vector2 _spriteExtents; // 스프라이트 절반 크기 (월드 단위)
    private Animator _animator;     // 애니메이션 제어
    private Collider2D _collider;
    private int _currentHealth;
    private bool _isDead;
    private bool _isInvincible;
    private Vector3 _spawnPosition;

    // Animator State 파라미터 값 상수
    private const int StateIdle  = 0;
    private const int StateLeft  = 1;
    private const int StateRight = 2;

    void Start()
    {
        // 스프라이트의 절반 크기를 미리 계산해 경계 계산에 활용
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            _spriteExtents = sr.bounds.extents;

        _animator = GetComponent<Animator>();
        _collider = GetComponent<Collider2D>();
        _spawnPosition = transform.position;
        _currentHealth = maxHealth;
    }

    void Update()
    {
        if (_isDead)
            return;

        Move();

        if (Input.GetMouseButton(0))
        {
            _fireTimer += Time.deltaTime;

            // fireRate 간격마다 한 번씩 발사
            if (_fireTimer >= fireRate)
            {
                Fire();
                _fireTimer = 0f;
            }
        }
    }

    /// <summary>
    /// GetAxisRaw 입력을 받아 플레이어를 이동시키고 애니메이션 상태를 전환합니다.
    /// 대각선 이동 시 속도가 빨라지지 않도록 방향 벡터를 정규화합니다.
    /// 이동 후 화면 경계를 벗어나지 않도록 위치를 Clamp합니다.
    /// </summary>
    private void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // 대각선 이동 시 단위벡터로 정규화하여 속도를 일정하게 유지
        Vector3 direction = new Vector3(h, v, 0f).normalized;
        transform.Translate(direction * moveSpeed * Time.deltaTime);

        // 화면 경계를 월드 좌표로 변환 (뷰포트 0,0 ~ 1,1)
        Vector3 minBounds = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f, 0f));
        Vector3 maxBounds = Camera.main.ViewportToWorldPoint(new Vector3(1f, 1f, 0f));

        // 스프라이트 절반 크기만큼 경계를 안쪽으로 좁혀 이미지가 잘리지 않도록 함
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minBounds.x + _spriteExtents.x, maxBounds.x - _spriteExtents.x);
        pos.y = Mathf.Clamp(pos.y, minBounds.y + _spriteExtents.y, maxBounds.y - _spriteExtents.y);
        transform.position = pos;

        // 수평 입력에 따라 애니메이션 State 전환 (좌: 1, 우: 2, 정지: 0)
        if (h < 0f)
            _animator?.SetInteger("State", StateLeft);
        else if (h > 0f)
            _animator?.SetInteger("State", StateRight);
        else
            _animator?.SetInteger("State", StateIdle);
    }

    /// <summary>
    /// 현재 power 레벨에 맞는 발사 패턴을 실행합니다.
    /// </summary>
    private void Fire()
    {
        switch (power)
        {
            case 1: // 중앙 단발
                SpawnBullet(sideBulletPrefab, Vector3.zero);
                break;

            case 2: // 좌우 2발
                sideOffset = 0.1f;
                SpawnBullet(sideBulletPrefab, Vector3.left  * sideOffset);
                SpawnBullet(sideBulletPrefab, Vector3.right * sideOffset);
                break;

            case 3: // 중앙 강화탄 + 좌우 2발
                sideOffset = 0.25f;
                SpawnBullet(centerBulletPrefab, Vector3.zero);
                SpawnBullet(sideBulletPrefab,   Vector3.left  * sideOffset);
                SpawnBullet(sideBulletPrefab,   Vector3.right * sideOffset);
                break;
        }
    }

    /// <summary>
    /// 지정한 프리팹을 firePoint 기준 offset 위치에 생성합니다.
    /// </summary>
    /// <param name="prefab">생성할 총알 프리팹</param>
    /// <param name="offset">firePoint로부터의 로컬 오프셋</param>
    private void SpawnBullet(GameObject prefab, Vector3 offset)
    {
        Instantiate(prefab, firePoint.position + offset, firePoint.rotation);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isDead)
            return;

        if (other.CompareTag("EnemyBullet"))
        {
            
            EnemyBullet bullet = other.GetComponent<EnemyBullet>();

            TakeDamage(bullet.damage);
            
            Destroy(other.gameObject);
            
            return;
        }

        if (other.CompareTag("Enemy"))
        {
            TakeDamage(enemyCollisionDamage);
        }
    }

    public void TakeDamage(int damage)
    {
        if (_isDead || _isInvincible)
            return;

        _currentHealth -= Mathf.Max(0, damage);
        
        GameManager.Instance.DecreaseLife();    // 생명을 감소 
        
        if (_currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        if (_isDead)
            return;

        _isDead = true;

        if (GameManager.Instance != null)
            GameManager.Instance.HandlePlayerDeath(this);
    }

    public void Respawn(float delay)
    {
        StartCoroutine(RespawnRoutine(delay));
    }

    private IEnumerator RespawnRoutine(float delay)
    {
        _isDead = true;

        if (_collider != null)
            _collider.enabled = false;

        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        transform.position = _spawnPosition;
        _currentHealth = maxHealth;

        if (_collider != null)
            _collider.enabled = true;

        _isDead = false;
        StartCoroutine(InvincibleRoutine(respawnInvincibleDuration));
    }

    private IEnumerator InvincibleRoutine(float duration)
    {
        _isInvincible = true;
        yield return new WaitForSeconds(duration);
        _isInvincible = false;
    }
}