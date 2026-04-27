using UnityEngine;

public class Player3 : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 부딪힌 물체에서 item3 스크립트가 있는지 확인
        item3 item = other.GetComponent<item3>();

        if (item != null)
        {
            item.Collected();
        }
    }
}