using UnityEngine;
using UnityEngine.UI;

public class Test2 : MonoBehaviour
{
    public Button testButton;

    void Start()
    {
        // 버튼을 누르면 딱 한 줄, "Hello World!"만 출력합니다.
        testButton.onClick.AddListener(() => Debug.Log("Hello World!"));
    }
}