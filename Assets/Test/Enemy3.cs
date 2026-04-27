using UnityEngine;
using System; // Action 사용을 위해 필수

public class Enemy3 : MonoBehaviour
{
    public int hp = 10;
    
    // 대리자: "나 죽으면 이거 실행해줘" 명단
    public Action OnDie;

    public void TakeDamage(int damage)
    {
        hp -= damage;
        Debug.Log($"적 현재 HP: {hp}");

        if (hp <= 0)
        {
            // 명단(OnDie)에 등록된 함수들 실행
            OnDie?.Invoke();
            Destroy(gameObject);
        }
    }
}