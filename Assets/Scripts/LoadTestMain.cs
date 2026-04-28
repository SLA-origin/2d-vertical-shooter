using System.Collections;
using UnityEngine;
using Newtonsoft.Json;

public class LoadTestMain : MonoBehaviour
{
    // stage_data.json의 point 인덱스에 대응하는 스폰 위치 배열
    // Inspector에서 spawnPoint_0, spawnPoint_1, spawnPoint_2 를 여기에 연결합니다
    public Transform[] spawnPoints;

    void Start()
    {
        // Resources 폴더에서 Stage_data.json 로드
        // "Data/Stage_data" = Assets/Resources/Data/Stage_data.json 을 의미합니다
        var ta = Resources.Load<TextAsset>("Data/Stage_data");

        if (ta == null)
        {
            Debug.LogError("[LoadTestMain] JSON 로드 실패! Resources/Data/Stage_data.json 파일을 확인하세요.");
            return;
        }

        // JSON 문자열을 SpawnData 배열로 역직렬화 (번역)
        SpawnData[] arr = JsonConvert.DeserializeObject<SpawnData[]>(ta.text);

        Debug.Log($"<color=cyan>== {arr.Length}개의 스폰 데이터 로드 완료 ==</color>");

        // 코루틴으로 순서대로 적 생성 시작
        StartCoroutine(SpawnRoutine(arr));
    }

    private IEnumerator SpawnRoutine(SpawnData[] datas)
    {
        foreach (SpawnData data in datas)
        {
            // delay(초)만큼 기다린 뒤 다음 줄 실행
            yield return new WaitForSeconds(data.delay);

            // enemyType에 맞는 적기를 오브젝트 풀에서 꺼내기
            GameObject enemyGo = null;
            switch (data.enemyType)
            {
                case Enemy.EnemyType.A:
                    enemyGo = ObjectPoolManager.instance.GetEnemyA();
                    break;
                case Enemy.EnemyType.B:
                    enemyGo = ObjectPoolManager.instance.GetEnemyB();
                    break;
                case Enemy.EnemyType.C:
                    enemyGo = ObjectPoolManager.instance.GetEnemyC();
                    break;
            }

            // 풀에서 오브젝트를 못 가져왔을 경우 이번 항목은 건너뜀
            if (enemyGo == null) continue;

            // point 인덱스에 해당하는 스폰 포인트 위치로 이동
            enemyGo.transform.position = spawnPoints[data.point].position;

            // 위치를 먼저 설정한 뒤 활성화 (OnEnable 실행)
            enemyGo.SetActive(true);

            // 아래 방향으로 이동 시작
            enemyGo.GetComponent<Enemy>().StartMove(Vector2.down);

            Debug.Log($"<color=yellow>적 생성: 타입={data.enemyType}, 포인트={data.point}</color>");
        }

        Debug.Log("<color=green>== 모든 스폰 완료 ==</color>");
    }
}