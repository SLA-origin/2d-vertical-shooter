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
    
    // Collider -> Collider2D / OnTriggerEnter -> OnTriggerEnter2D
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. 충돌한 대상의 태그가 "Player"인지 확인
        if (other.CompareTag("Player"))
        {
            Debug.Log("아이템 획득!");

            // 2. 아이템 획득 로직 (점수 추가 등)
            // ScoreManager.instance.AddScore(10);

            // 3. 아이템 삭제 — Destroy 대신 풀로 반환
            ObjectManager.Instance.Release(gameObject);
        }
    }
    
public class Item2D : MonoBehaviour
{
    // Collider -> Collider2D / OnTriggerEnter -> OnTriggerEnter2D
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("2D 아이템 획득!");
            Destroy(gameObject);
        }
    }
}
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
