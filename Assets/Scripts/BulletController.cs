using UnityEngine;

public class BulletController : MonoBehaviour
{
    void Update()
    {
        // 총알을 위로 이동
        transform.Translate(0, 0.15f, 0);

        // 화면 밖(y = 6.0 이상)으로 나가면 제거하기
        if (transform.position.y > 6.0f)
        {
            Destroy(gameObject);
            Debug.Log("총알 제거됨");
        }
    }
}