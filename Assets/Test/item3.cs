using UnityEngine;

public class item3 : MonoBehaviour
{
    [Header("아이템 설정")]
    public string itemName;      // 아이템 이름 (Coin, Boom, Power 등)
    public float moveSpeed = 3f; // 아래로 내려가는 속도

    // Update는 매 프레임(매우 짧은 시간)마다 실행됩니다.
    void Update()
    {
        // 1. 매 순간 아래쪽(Vector3.down) 방향으로 이동하라!
        transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);

        // 2. (보너스) 화면 밖으로 너무 멀리 내려가면 스스로 삭제 (메모리 관리)
        if (transform.position.y < -10f)
        {
            Destroy(gameObject);
        }
    }

    // 플레이어와 부딪혔을 때 플레이어가 호출할 함수
    public void Collected()
    {
        Debug.Log($"{itemName}을(를) 획득했습니다!");
        
        // 먹었으니까 화면에서 제거
        Destroy(gameObject);
    }
}