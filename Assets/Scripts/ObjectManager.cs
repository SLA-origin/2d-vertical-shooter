using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 오브젝트 풀링(Object Pooling)을 전담하는 싱글톤 매니저입니다.
///
/// [오브젝트 풀링이란?]
/// 오브젝트를 게임 중에 매번 생성(Instantiate)하고 삭제(Destroy)하면
/// 메모리 할당/해제가 반복되어 게임이 버벅입니다(GC Spike).
/// 풀링은 오브젝트를 미리 만들어두고, 필요할 때 꺼내 쓰고(SetActive true),
/// 쓰고 나면 삭제하지 않고 되돌려 놓는(SetActive false) 방식입니다.
///
/// [사용 방법]
///   꺼내기 : ObjectManager.Instance.GetEnemyL(위치);
///   돌려주기: ObjectManager.Instance.Release(gameObject);
/// </summary>
public class ObjectManager : MonoBehaviour
{
    // 싱글톤 — 게임 어디서든 ObjectManager.Instance로 접근합니다
    public static ObjectManager Instance { get; private set; }

    // ── Inspector 연결 필드 ──────────────────────────────────────
    [Header("Enemy Prefabs (적 프리팹)")]
    public GameObject enemyLPrefab;     // 대형 적 (EnemyL)
    public GameObject enemyMPrefab;     // 중형 적 (EnemyM)
    public GameObject enemySPrefab;     // 소형 적 (EnemyS)

    [Header("Item Prefabs (아이템 프리팹)")]
    public GameObject itemCoinPrefab;   // 코인 아이템
    public GameObject itemPowerPrefab;  // 파워 아이템
    public GameObject itemBoomPrefab;   // 봄 아이템

    [Header("Player Bullet Prefabs (플레이어 총알 프리팹)")]
    public GameObject playerBulletAPrefab;  // 기본 총알 (SideBullet)
    public GameObject playerBulletBPrefab;  // 강화 총알 (CenterBullet)

    [Header("Enemy Bullet Prefabs (적 총알 프리팹)")]
    public GameObject bulletEnemyAPrefab;   // 적 기본 총알
    public GameObject bulletEnemyBPrefab;   // 적 강화 총알

    // ── 각 풀의 오브젝트 목록 (비활성 대기 상태로 보관) ──────────
    private List<GameObject> enemyLPool       = new List<GameObject>();
    private List<GameObject> enemyMPool       = new List<GameObject>();
    private List<GameObject> enemySPool       = new List<GameObject>();
    private List<GameObject> itemCoinPool     = new List<GameObject>();
    private List<GameObject> itemPowerPool    = new List<GameObject>();
    private List<GameObject> itemBoomPool     = new List<GameObject>();
    private List<GameObject> playerBulletAPool = new List<GameObject>();
    private List<GameObject> playerBulletBPool = new List<GameObject>();
    private List<GameObject> bulletEnemyAPool  = new List<GameObject>();
    private List<GameObject> bulletEnemyBPool  = new List<GameObject>();

    private void Awake()
    {
        // 싱글톤 패턴: 이미 인스턴스가 있으면 중복 오브젝트를 제거합니다
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // ── 풀 초기화 (요구사항 수량대로 미리 생성) ──────────────
        InitPool(enemyLPool,        enemyLPrefab,        10);   // EnemyL  10개
        InitPool(enemyMPool,        enemyMPrefab,        10);   // EnemyM  10개
        InitPool(enemySPool,        enemySPrefab,        20);   // EnemyS  20개
        InitPool(itemCoinPool,      itemCoinPrefab,      20);   // ItemCoin  20개
        InitPool(itemPowerPool,     itemPowerPrefab,     10);   // ItemPower 10개
        InitPool(itemBoomPool,      itemBoomPrefab,      10);   // ItemBoom  10개
        InitPool(playerBulletAPool, playerBulletAPrefab, 100);  // PlayerBulletA 100개
        InitPool(playerBulletBPool, playerBulletBPrefab, 100);  // PlayerBulletB 100개
        InitPool(bulletEnemyAPool,  bulletEnemyAPrefab,  100);  // BulletEnemyA  100개
        InitPool(bulletEnemyBPool,  bulletEnemyBPrefab,  100);  // BulletEnemyB  100개
    }

    // ────────────────────────────────────────────────────────────
    // 내부 헬퍼 메서드
    // ────────────────────────────────────────────────────────────

    /// <summary>
    /// 풀 초기화: count개만큼 프리팹을 미리 생성하고 비활성화 상태로 보관합니다.
    /// </summary>
    private void InitPool(List<GameObject> pool, GameObject prefab, int count)
    {
        if (prefab == null)
        {
            // Inspector에서 프리팹을 연결하지 않으면 이 경고가 뜹니다
            Debug.LogWarning($"[ObjectManager] 프리팹이 Inspector에 연결되지 않았습니다.");
            return;
        }

        for (int i = 0; i < count; i++)
        {
            GameObject go = Instantiate(prefab); // 생성
            go.SetActive(false);                  // 비활성화(대기 상태)
            pool.Add(go);                         // 목록에 추가
        }
    }

    /// <summary>
    /// 풀에서 비활성 오브젝트를 꺼내 지정 위치/회전으로 활성화합니다.
    /// 모든 오브젝트가 사용 중이면 새로 생성해서 풀에 추가합니다(동적 확장).
    /// </summary>
    private GameObject GetFromPool(List<GameObject> pool, GameObject prefab, Vector3 position, Quaternion rotation)
    {
        // 풀에서 비활성화된 오브젝트를 찾습니다
        for (int i = pool.Count - 1; i >= 0; i--)
        {
            // Destroy()로 삭제된 오브젝트가 남아있을 경우 풀에서 제거
            if (pool[i] == null)
            {
                pool.RemoveAt(i);
                continue;
            }

            if (!pool[i].activeInHierarchy)
            {
                pool[i].transform.position = position;
                pool[i].transform.rotation = rotation;
                pool[i].SetActive(true); // ← 이 시점에 OnEnable()이 호출됩니다
                return pool[i];
            }
        }

        // 풀이 꽉 찼을 경우 — 새로 생성하고 풀 목록에 추가
        Debug.LogWarning($"[ObjectManager] 풀 용량 초과! 오브젝트를 동적 생성합니다.");
        GameObject newGo = Instantiate(prefab, position, rotation);
        pool.Add(newGo);
        return newGo;
    }

    // ────────────────────────────────────────────────────────────
    // 공개 메서드 — 외부에서 이 메서드들을 호출합니다
    // ────────────────────────────────────────────────────────────

    /// <summary>
    /// 사용이 끝난 오브젝트를 풀로 반환합니다.
    /// Destroy() 대신 이 메서드를 호출하세요.
    /// </summary>
    public void Release(GameObject go)
    {
        go.SetActive(false); // 비활성화 = 풀 반환. OnDisable()이 호출됩니다
    }

    // ── 적 꺼내기 ──────────────────────────────────────────────
    /// <summary>pos 위치에 대형 적을 풀에서 꺼냅니다.</summary>
    public GameObject GetEnemyL(Vector3 pos)
        => GetFromPool(enemyLPool, enemyLPrefab, pos, Quaternion.identity);

    /// <summary>pos 위치에 중형 적을 풀에서 꺼냅니다.</summary>
    public GameObject GetEnemyM(Vector3 pos)
        => GetFromPool(enemyMPool, enemyMPrefab, pos, Quaternion.identity);

    /// <summary>pos 위치에 소형 적을 풀에서 꺼냅니다.</summary>
    public GameObject GetEnemyS(Vector3 pos)
        => GetFromPool(enemySPool, enemySPrefab, pos, Quaternion.identity);

    // ── 아이템 꺼내기 ──────────────────────────────────────────
    /// <summary>pos 위치에 코인 아이템을 풀에서 꺼냅니다.</summary>
    public GameObject GetItemCoin(Vector3 pos)
        => GetFromPool(itemCoinPool, itemCoinPrefab, pos, Quaternion.identity);

    /// <summary>pos 위치에 파워 아이템을 풀에서 꺼냅니다.</summary>
    public GameObject GetItemPower(Vector3 pos)
        => GetFromPool(itemPowerPool, itemPowerPrefab, pos, Quaternion.identity);

    /// <summary>pos 위치에 봄 아이템을 풀에서 꺼냅니다.</summary>
    public GameObject GetItemBoom(Vector3 pos)
        => GetFromPool(itemBoomPool, itemBoomPrefab, pos, Quaternion.identity);

    // ── 플레이어 총알 꺼내기 ───────────────────────────────────
    /// <summary>pos 위치에 플레이어 기본 총알(A)을 풀에서 꺼냅니다.</summary>
    public GameObject GetPlayerBulletA(Vector3 pos, Quaternion rot)
        => GetFromPool(playerBulletAPool, playerBulletAPrefab, pos, rot);

    /// <summary>pos 위치에 플레이어 강화 총알(B)을 풀에서 꺼냅니다.</summary>
    public GameObject GetPlayerBulletB(Vector3 pos, Quaternion rot)
        => GetFromPool(playerBulletBPool, playerBulletBPrefab, pos, rot);

    // ── 적 총알 꺼내기 ─────────────────────────────────────────
    /// <summary>pos 위치에 적 기본 총알(A)을 풀에서 꺼냅니다.</summary>
    public GameObject GetBulletEnemyA(Vector3 pos)
        => GetFromPool(bulletEnemyAPool, bulletEnemyAPrefab, pos, Quaternion.identity);

    /// <summary>pos 위치에 적 강화 총알(B)을 풀에서 꺼냅니다.</summary>
    public GameObject GetBulletEnemyB(Vector3 pos)
        => GetFromPool(bulletEnemyBPool, bulletEnemyBPrefab, pos, Quaternion.identity);
}
