using UnityEngine;

public class Item : MonoBehaviour
{
    public string type;
    private Rigidbody2D rigid;

    void Awake()
    {
        // Awake는 오브젝트 생성 시 단 한 번만 실행됩니다
        // 컴포넌트 참조만 여기서 취득합니다
        rigid = GetComponent<Rigidbody2D>();
    }

    // OnEnable: 풀에서 꺼낼 때(SetActive true)마다 호출됩니다
    // 속도를 여기서 설정해야 재활용 시에도 아래로 떨어집니다
    private void OnEnable()
    {
        rigid.linearVelocity = Vector2.down * 3; // 낙하 속도 설정
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("아이템 획득!");

            // ObjectManager가 씬에 없을 경우를 대비한 null 체크
            if (ObjectManager.Instance != null)
                ObjectManager.Instance.Release(gameObject);
            else
                Destroy(gameObject);
        }
    }
}
