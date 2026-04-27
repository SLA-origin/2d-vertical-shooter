using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum EnemyType { A, B, C }

    // 이 적이 발사할 총알 종류 (Inspector에서 A 또는 B로 선택)
    public enum BulletType { A, B }

    private SpriteRenderer sr;

    public int speed = 1;
    public int health;           // Inspector에서 설정한 최대 체력
    public Sprite[] sprites;
    public EnemyType enemyType;
    public BulletType bulletType = BulletType.A; // 발사할 총알 풀 선택

    // [수정] 단일 Transform에서 배열(Array)로 변경! 이제 여러 개의 총구를 담을 수 있습니다.
    public Transform[] firePoints; 

    private float delta = 0;
    public float gap = 0.1f;
    private Vector3 dir;
    private bool isMove = false;

    // 풀링용: 최초 Inspector에서 설정한 체력을 기억해 두었다가 재활용 시 복원
    private int _initialHealth;

    void Start()
    {
        // Awake에서 sr을 이미 얻으므로, 여기서는 firePoints 자동 탐색만 합니다
        // (Start는 첫 번째 활성화 시 한 번만 실행됩니다)

        // Inspector에 firePoints가 비어 있으면 자식에서 FirePoint_L / FirePoint_R을 자동으로 찾습니다
        if (firePoints == null || firePoints.Length == 0)
        {
            var list = new System.Collections.Generic.List<Transform>();
            Transform fpL = transform.Find("FirePoint_L");
            Transform fpR = transform.Find("FirePoint_R");
            if (fpL != null) list.Add(fpL);
            if (fpR != null) list.Add(fpR);
            if (list.Count > 0) firePoints = list.ToArray();
        }
    }

    // Awake는 오브젝트가 처음 생성될 때 딱 한 번 실행됩니다
    // 컴포넌트 참조 취득과 초기값 저장을 여기서 합니다
    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        _initialHealth = health; // Inspector 값을 기억해 둡니다
    }

    // OnEnable: 풀에서 꺼낼 때(SetActive true)마다 호출됩니다
    // 이전 전투에서 남은 상태(체력 소진, 이동 중 등)를 초기화합니다
    private void OnEnable()
    {
        health = _initialHealth;          // 체력 복원
        isMove = false;                   // 이동 정지
        delta  = 0;                       // 발사 타이머 초기화
        if (sr != null && sprites.Length > 0)
            sr.sprite = sprites[0];       // 스프라이트 원래대로
    }

    // OnDisable: 풀로 반환될 때(SetActive false)마다 호출됩니다
    // 대기 중인 Invoke(예약 함수)를 모두 취소합니다
    private void OnDisable()
    {
        CancelInvoke(); // 피격 시 0.1초 후 스프라이트 복원 예약이 남아있을 수 있어 취소
    }

    void Update()
    {
        if (isMove)
        {
            transform.Translate(this.dir * speed * Time.deltaTime, Space.World);

            if (enemyType == EnemyType.C)
            {
                delta += Time.deltaTime;
                if (delta > 1f)
                {
                    Fire();
                    delta = 0;
                }
            }
        }

        if (AreaDrawer.Instance != null && AreaDrawer.Instance.IsOutOfBounds(transform.position))
            ObjectManager.Instance.Release(gameObject); // Destroy 대신 풀로 반환
    }

    private void Fire()
    {
        // 플레이어 찾기 (태그 우선순위 최적화)
        var playerGo = GameObject.FindWithTag("Player");
        if (playerGo == null) playerGo = GameObject.Find("player");
        if (playerGo == null) playerGo = GameObject.Find("Player");

        if (playerGo != null && firePoints != null)
        {
            // 모든 총구(firePoints)를 순회하며 총알을 발사합니다
            foreach (Transform fp in firePoints)
            {
                if (fp == null) continue;

                // Instantiate 대신 ObjectManager 풀에서 총알을 꺼냅니다
                GameObject bulletGo = bulletType == BulletType.A
                    ? ObjectManager.Instance.GetBulletEnemyA(fp.position)
                    : ObjectManager.Instance.GetBulletEnemyB(fp.position);

                var bullet = bulletGo.GetComponent<EnemyBullet>();
                if (bullet != null)
                {
                    // 각 총구 위치에서 플레이어를 향한 방향 계산
                    var shootDir = (playerGo.transform.position - fp.position).normalized;
                    bullet.StartMove(shootDir);
                }
            }
        }
    }

    private void Hit(int damage)
    {
        health -= damage;
        if (sprites.Length > 1) sr.sprite = sprites[1];
        Invoke("ReturnDefaultSprite", 0.1f);

        if (health <= 0)
        {
            // 타입별 점수 지급: A=100, B=200, C=300
            if (GameManager.Instance != null)
            {
                int scoreValue = enemyType == EnemyType.A ? 100 :
                                 enemyType == EnemyType.B ? 200 : 300;
                GameManager.Instance.AddScore(scoreValue);
            }
            ObjectManager.Instance.Release(gameObject); // Destroy 대신 풀로 반환
        }
    }

    public void StartMove(Vector3 dir)
    {
        this.dir = dir.normalized;
        DrawArrow.ForDebug2D(this.transform.position, dir, 10f, Color.red);
        isMove = true;
    }

    private void ReturnDefaultSprite()
    {
        if (sprites.Length > 0) sr.sprite = sprites[0];
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("PlayerBullet"))
        {
            PlayerBullet playerBullet = other.gameObject.GetComponent<PlayerBullet>();
            if (playerBullet != null)
            {
                Hit(playerBullet.damage);
            }
            // 총알도 Destroy 대신 풀로 반환
            ObjectManager.Instance.Release(other.gameObject);
        }
    }
}