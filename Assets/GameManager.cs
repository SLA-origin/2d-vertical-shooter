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
        if (enemies == null || enemies.Length == 0)
            return;

        GameObject prefab = enemies[Random.Range(0, enemies.Length)];
        var dice = Random.Range(0, 2); 

        GameObject enemyGo = Instantiate(prefab);
        var enemy = enemyGo.GetComponent<Enemy>();
        
        if (dice == 0)
        {
            if (spawnPoints == null || spawnPoints.Length == 0) return;

            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            enemyGo.transform.position = spawnPoint.position;
            enemy.StartMove(Vector2.down);
        }
        else
        {
            if (spawners == null || spawners.Length == 0) return;

            EnemySpawner spawner = spawners[Random.Range(0, spawners.Length)];
            enemyGo.transform.position = spawner.startPoint.position;
            enemy.StartMove(spawner.GetDir().normalized);
        }
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