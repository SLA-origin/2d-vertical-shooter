using System.Collections.Generic;
using UnityEngine;

// 선생님 코드에서 호출하는 ObjectPoolManager입니다.
// 적기 3종(A/B/C)을 미리 만들어두고, 필요할 때 꺼내서 재사용합니다.
public class ObjectPoolManager : MonoBehaviour
{
    // 어디서든 ObjectPoolManager.instance 로 접근 가능 (소문자 instance)
    public static ObjectPoolManager instance;

    [Header("적 프리팹 연결 (Inspector에서 드래그)")]
    public GameObject enemyAPrefab; // Enemy A 프리팹
    public GameObject enemyBPrefab; // Enemy B 프리팹
    public GameObject enemyCPrefab; // Enemy C 프리팹

    [Header("각 풀의 초기 생성 수량")]
    public int poolSize = 5; // 처음에 각 종류별로 몇 개씩 만들어둘지

    // 실제 오브젝트들을 보관하는 리스트 (비활성 상태로 대기)
    private List<GameObject> poolA = new List<GameObject>();
    private List<GameObject> poolB = new List<GameObject>();
    private List<GameObject> poolC = new List<GameObject>();

    private void Awake()
    {
        // 싱글톤 설정: 씬에 하나만 존재하도록 보장
        instance = this;

        // 게임 시작 전에 미리 오브젝트들을 생성해서 풀에 보관
        FillPool(poolA, enemyAPrefab, poolSize);
        FillPool(poolB, enemyBPrefab, poolSize);
        FillPool(poolC, enemyCPrefab, poolSize);
    }

    // 풀 초기화: 프리팹을 count개 만들어서 비활성화 상태로 리스트에 추가
    private void FillPool(List<GameObject> pool, GameObject prefab, int count)
    {
        if (prefab == null)
        {
            Debug.LogError("[ObjectPoolManager] 프리팹이 Inspector에 연결되지 않았습니다!");
            return;
        }

        for (int i = 0; i < count; i++)
        {
            GameObject go = Instantiate(prefab);
            go.SetActive(false); // 비활성화 상태로 대기
            pool.Add(go);
        }
    }

    // 풀에서 비활성 오브젝트를 꺼내줍니다.
    // 다 사용 중이면 새로 하나 더 만들어서 줍니다 (자동 확장).
    private GameObject GetFromPool(List<GameObject> pool, GameObject prefab)
    {
        // 풀에서 비활성화된(사용 중이 아닌) 오브젝트 찾기
        foreach (GameObject go in pool)
        {
            if (!go.activeInHierarchy)
                return go; // 찾았으면 반환 (위치·활성화는 호출한 쪽에서 처리)
        }

        // 풀이 꽉 찼을 때 → 하나 더 만들어서 풀에 추가 후 반환
        Debug.Log("[ObjectPoolManager] 풀 확장! 새 오브젝트 생성");
        GameObject newGo = Instantiate(prefab);
        newGo.SetActive(false);
        pool.Add(newGo);
        return newGo;
    }

    // ─── 외부에서 호출하는 꺼내기 메서드 ───────────────────────────

    // Enemy A를 풀에서 꺼내서 반환 (위치·활성화는 LoadTestMain에서 처리)
    public GameObject GetEnemyA() => GetFromPool(poolA, enemyAPrefab);

    // Enemy B를 풀에서 꺼내서 반환
    public GameObject GetEnemyB() => GetFromPool(poolB, enemyBPrefab);

    // Enemy C를 풀에서 꺼내서 반환
    public GameObject GetEnemyC() => GetFromPool(poolC, enemyCPrefab);
}
