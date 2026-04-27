using UnityEngine;
using UnityEngine.UI;

public class Test3Main : MonoBehaviour
{
    public Button attackBtn;
    public Enemy3 targetEnemy;

    void Start()
    {
        // 1. 대리자 연결: 적이 죽으면 매니저의 아이템 생성 함수를 실행하도록 예약
        if (targetEnemy != null)
        {
            targetEnemy.OnDie += () => {
                Test3GameManager.Instance.SpawnItem(targetEnemy.transform.position);
            };
        }

        // 2. 버튼 클릭 이벤트: 적에게 데미지 5 주기
        attackBtn.onClick.AddListener(() => {
            if (targetEnemy != null)
            {
                targetEnemy.TakeDamage(5);
            }
            else
            {
                Debug.Log("적이 이미 없습니다.");
            }
        });
    }
}