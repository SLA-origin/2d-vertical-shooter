using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameObject[] enemies;
    public Transform[] spawnPoints; // 위에서 아래로 내려오는 위치 
    public EnemySpawner[] spawners; // 사이드 위치 
    [SerializeField] private float respawnDelay = 1f;

    // --- 자동 스폰을 위한 변수 추가 ---
    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 2.0f; // 적 생성 간격 (초)
    private float spawnTimer = 0f;                   // 타이머 계산용
    // ------------------------------
    
    private int score = 0;
    private int lives = 3;
    private bool isGameOver = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (UIManager.instance != null)
            UIManager.instance.InitializeUI(score, lives);
    }
    
    void Update()
    {
        if (isGameOver)
            return;

        // [수정] 마우스 클릭 로직을 삭제하고 자동 타이머 로직으로 교체
        spawnTimer += Time.deltaTime; // 프레임마다 시간을 더함

        if (spawnTimer >= spawnInterval)
        {
            CreateEnemy();    // 적 생성
            spawnTimer = 0f;  // 타이머 초기화
        }
    }

    private void CreateEnemy()
    {
        if (ObjectManager.Instance == null) return;

        // 스폰 방식 결정: 0 = 위에서 하강, 1 = 사이드 진입
        int dice = Random.Range(0, 2);

        Vector3 spawnPos;
        Vector2 moveDir;

        if (dice == 0)
        {
            // 위쪽 스폰 포인트 중 랜덤 선택
            if (spawnPoints == null || spawnPoints.Length == 0) return;
            spawnPos = spawnPoints[Random.Range(0, spawnPoints.Length)].position;
            moveDir  = Vector2.down; // 아래로 내려옵니다
        }
        else
        {
            // 사이드 스포너 중 랜덤 선택
            if (spawners == null || spawners.Length == 0) return;
            EnemySpawner spawner = spawners[Random.Range(0, spawners.Length)];
            spawnPos = spawner.startPoint.position;
            moveDir  = spawner.GetDir().normalized;
        }

        // 적 종류 랜덤 선택: 0=대형(L), 1=중형(M), 2=소형(S)
        // Instantiate 대신 ObjectManager 풀에서 꺼냅니다
        int enemyType = Random.Range(0, 3);
        GameObject enemyGo = enemyType == 0 ? ObjectManager.Instance.GetEnemyL(spawnPos) :
                             enemyType == 1 ? ObjectManager.Instance.GetEnemyM(spawnPos) :
                                              ObjectManager.Instance.GetEnemyS(spawnPos);

        Enemy enemy = enemyGo.GetComponent<Enemy>();
        if (enemy != null)
            enemy.StartMove(moveDir); // 이동 방향 전달
    }
    
    public void AddScore(int amount)
    {
        score += amount;
        if (UIManager.instance != null)
            UIManager.instance.UpdateScore(score);
    }

    public void DecreaseLife()
    {
        if (lives <= 0) return;
        lives--;
        if (UIManager.instance != null)
            UIManager.instance.UpdateLife(lives);
    }

    public void HandlePlayerDeath(Player deadPlayer)
    {
        if (isGameOver) return;
        DecreaseLife();

        if (lives <= 0)
        {
            isGameOver = true;
            if (UIManager.instance != null)
                UIManager.instance.ShowGameOver();

            if (deadPlayer != null)
                Destroy(deadPlayer.gameObject);
            return;
        }

        if (deadPlayer != null)
            deadPlayer.Respawn(respawnDelay);
    }
}