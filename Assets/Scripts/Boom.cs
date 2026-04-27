using UnityEngine;
public class Boom : MonoBehaviour {
    void Start() {
        Debug.Log("폭발 발생!");
        // 2초 동안 폭발 애니메이션을 재생한 뒤 오브젝트 삭제
        Destroy(gameObject, 2f); 
    }
}