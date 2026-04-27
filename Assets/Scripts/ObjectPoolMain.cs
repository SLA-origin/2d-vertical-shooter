using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolMain : MonoBehaviour
{
    // 어디서든 ObjectPoolMain.Instance로 접근 가능하게 만듭니다.
    public static ObjectPoolMain Instance { get; private set; }

    public GameObject playerBullet0Prefabs;
    private List<GameObject> playerBullet0List = new List<GameObject>();

    [SerializeField] private int initialPoolSize = 10;

    private void Awake()
    {
        // 싱글톤 설정
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // 초기 풀 생성
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateNewBullet();
        }
    }

    private GameObject CreateNewBullet()
    {
        GameObject go = Instantiate(playerBullet0Prefabs);
        go.SetActive(false);
        playerBullet0List.Add(go);
        return go;
    }

    public GameObject GetPlayerBullet0(Vector3 position)
    {
        for (int i = 0; i < playerBullet0List.Count; i++)
        {
            if (!playerBullet0List[i].activeInHierarchy)
            {
                GameObject bullet = playerBullet0List[i];
                bullet.transform.position = position; // 발사 위치 설정
                bullet.SetActive(true);
                return bullet;
            }
        }

        // 만약 풀에 남는 총알이 없다면 새로 생성해서 반환
        GameObject newBullet = CreateNewBullet();
        newBullet.transform.position = position;
        newBullet.SetActive(true);
        return newBullet;
    }

    public void ReleaseBullet(GameObject bulletGo)
    {
        bulletGo.SetActive(false);
    }
}