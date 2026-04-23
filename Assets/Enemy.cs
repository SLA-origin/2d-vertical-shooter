using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum EnemyType { A, B, C }
    private SpriteRenderer sr;

    public int speed = 1;
    public int health;
    public Sprite[] sprites;
    public EnemyType enemyType;
    public GameObject bulletPrefab;

    // [수정] 단일 Transform에서 배열(Array)로 변경! 이제 여러 개의 총구를 담을 수 있습니다.
    public Transform[] firePoints; 

    private float delta = 0;
    public float gap = 0.1f;
    private Vector3 dir;
    private bool isMove = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

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
            Destroy(gameObject);
    }

    private void Fire()
    {
        // 플레이어 찾기 (태그 우선순위 최적화)
        var playerGo = GameObject.FindWithTag("Player");
        if (playerGo == null) playerGo = GameObject.Find("player");
        if (playerGo == null) playerGo = GameObject.Find("Player");

        if (playerGo != null && bulletPrefab != null && firePoints != null)
        {
            // [수정] 모든 총구(firePoints)를 순회하며 총알을 발사합니다.
            foreach (Transform fp in firePoints)
            {
                if (fp == null) continue; // 총구가 비어있으면 건너뜁니다.

                GameObject bulletGo = Instantiate(bulletPrefab, fp.position, Quaternion.identity);
                
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
            Destroy(gameObject);
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
            Destroy(other.gameObject);
        }
    }
}