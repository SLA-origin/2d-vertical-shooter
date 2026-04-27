using UnityEngine;

public class Test3GameManager : MonoBehaviour
{
    // 어디서든 접근 가능한 싱글톤
    public static Test3GameManager Instance;

    // 인스펙터에서 코인, 붐, 파워 프리팹 3개를 넣어주세요
    public GameObject[] itemPrefabs;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    // 적이 죽은 위치에 랜덤 아이템 생성
    public void SpawnItem(Vector3 position)
    {
        int rand = Random.Range(0, itemPrefabs.Length);
        GameObject item = Instantiate(itemPrefabs[rand], position, Quaternion.identity);
        
        // 이름 뒤에 (Clone)이 붙지 않게 원본 이름으로 설정
        item.name = itemPrefabs[rand].name;
    }
}