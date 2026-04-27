using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    // 싱글톤 설정을 통해 어디서든 UIManager.instance로 접근 가능하게 합니다
    public static UIManager instance;

    public Image[] lifeImages;
    public GameObject gameOverPanel;
    public TextMeshProUGUI scoreText;

    private void Awake()
    {
        instance = this;

        // Inspector에 lifeImages가 할당되지 않았으면 이름으로 자동 탐색
        if (lifeImages == null || lifeImages.Length == 0)
        {
            var list = new System.Collections.Generic.List<Image>();
            GameObject g0 = GameObject.Find("Life_0");
            GameObject g1 = GameObject.Find("Life_1");
            GameObject g2 = GameObject.Find("Life_2");
            if (g0 != null) list.Add(g0.GetComponent<Image>());
            if (g1 != null) list.Add(g1.GetComponent<Image>());
            if (g2 != null) list.Add(g2.GetComponent<Image>());
            if (list.Count > 0) lifeImages = list.ToArray();
        }

        // Inspector에 gameOverPanel이 할당되지 않았으면 이름으로 자동 탐색
        if (gameOverPanel == null)
            gameOverPanel = GameObject.Find("GameOverPanel");
    }

    public void InitializeUI(int score, int currentLives)
    {
        UpdateScore(score);

        for (int i = 0; i < lifeImages.Length; i++)
        {
            Color color = lifeImages[i].color;
            color.a = i < currentLives ? 1f : 0f;
            lifeImages[i].color = color;
        }

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    // 점수 텍스트 업데이트 (N0: 세자리 콤마)
    public void UpdateScore(int score)
    {
        if (scoreText != null)
            scoreText.text = score.ToString("N0");
    }

    // 목숨 이미지 업데이트 (Alpha 제어)
    public void UpdateLife(int currentLives)
    {
        // 목숨이 깎일 때 해당 인덱스의 이미지 알파를 0으로
        if (currentLives >= 0 && currentLives < lifeImages.Length)
        {
            Color color = lifeImages[currentLives].color;
            color.a = 0f;
            lifeImages[currentLives].color = color;
        }
    }

    // 게임오버 창 맴우기 (일시정지)
    public void ShowGameOver()
    {
        Time.timeScale = 0f;
        gameOverPanel.SetActive(true);
    }

    // Retry 버튼에 연결할 함수
    public void RetryGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}