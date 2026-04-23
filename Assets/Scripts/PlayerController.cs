using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject bulletPrefab;

    void Start()
    {
        Application.targetFrameRate = 60;
    }

    void Update()
    {
        // 비행기=플레이어 이동하기
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            transform.Translate(-1.5f, 0, 0); // 이동 거리 조절 가능
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            transform.Translate(1.5f, 0, 0);
        }

        // 총알 발사
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 현재 위치(transform.position)에서 총알 생성
            Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            Debug.Log("총알 발사!");
        }
    }
}