using UnityEngine;

public class Test : MonoBehaviour
{
    public GameObject prefab;    // 복사할 원본 데이터 (붕어빵 틀)
    public Transform parent;     // 생성된 오브젝트를 집어넣을 바구니 (부모 오브젝트)
    
    void Update()
    {
        // 마우스 왼쪽 버튼을 누르는 순간 실행
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("down!"); // 콘솔창에 클릭 신호 출력
            

            // [방식 1] 생성과 동시에 부모를 지정
            // prefab을 복사해서 parent의 자식으로 만듭니다. (하이어라키 구조 생성)
            GameObject go = Instantiate(prefab, parent); 

            // 생성된 go의 위치를 '세상의 중심(0, 0, 0)'으로 강제 이동시킵니다.
            // 주의: 부모가 (5, 5, 5)에 있어도 이 녀석은 월드 좌표 (0, 0, 0)으로 가버립니다.
            go.transform.position = Vector3.zero;    

            // 생성된 go의 회전값을 '회전 없음(0, 0, 0)' 상태로 초기화합니다.
            go.transform.rotation = Quaternion.identity; 

            // --- 아래는 주석 처리된 코드들 해설 ---

            ////GameObject go = Instantiate(prefab); // 부모 없이 그냥 세상에 소환
            //go.transform.SetParent(parent);       // 소환된 이후에 나중에 부모를 설정함


            // [방식 2] 특정 위치와 회전값을 지정해서 소환
            // spawnPoint의 위치에, 회전값은 0(정자세)으로 소환합니다.
            // Instantiate(prefab, spawnPoint.position, Quaternion.identity);
            
            // 현재 오브젝트의 회전 상태를 우리가 읽기 쉬운 각도(0~360도)로 출력합니다.
            // Debug.Log(go.transform.rotation.eulerAngles);


            // [방식 3] 스폰 포인트의 회전값까지 그대로 복사해서 소환
            // Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
            // Debug.Log(go.transform.rotation.eulerAngles);


            // [위치 설정의 두 가지 방법]
            // go.transform.position = Vector3.zero;        // 월드 좌표 0,0,0으로 이동
            // go.transform.position = spawnPoint.position; // 미리 정해둔 스폰 포인트 위치로 이동


            // [최종 데이터 확인]
            // Debug.Log(go.transform.position);               // 최종 결정된 위치 좌표를 콘솔에 출력
            // Debug.Log(go.transform.rotation.eulerAngles);    // 최종 결정된 회전 각도를 콘솔에 출력
        }
    }
}