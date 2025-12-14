using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager instance;

    public enum Difficulty { Easy, Medium, Hard }
    public Difficulty currentDifficulty;

    public float bulletSpeed;
    public float bulletLifeTime;
    public float speedIncreasePerPhase; // 페이즈별 속도 증가
    public float[] bestScore = new float[3]; // 각 난이도의 최고 점수 기록
    public float[] bestTimes = new float[3]; // 각 난이도의 최고 생존 시간 기록

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 오브젝트 유지
        }
        else
        {
            Destroy(gameObject); // 이미 인스턴스가 있으면 제거
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "StartScene")
        {
            // StartScene이 로드될 때 버튼에 이벤트를 재연결
            ReconnectButtons();
        }
    }

    private void ReconnectButtons()
    {
        Button easyButton = GameObject.Find("Easybutton")?.GetComponent<Button>();
        Button mediumButton = GameObject.Find("Mediumbutton")?.GetComponent<Button>();
        Button hardButton = GameObject.Find("Hardbutton")?.GetComponent<Button>();

        if (easyButton != null)
        {
            easyButton.onClick.RemoveAllListeners();
            easyButton.onClick.AddListener(() => SetDifficulty(0));
        }
        else
        {
            Debug.LogError("Easybutton을 찾을 수 없습니다.");
        }

        if (mediumButton != null)
        {
            mediumButton.onClick.RemoveAllListeners();
            mediumButton.onClick.AddListener(() => SetDifficulty(1));
        }
        else
        {
            Debug.LogError("Mediumbutton을 찾을 수 없습니다.");
        }

        if (hardButton != null)
        {
            hardButton.onClick.RemoveAllListeners();
            hardButton.onClick.AddListener(() => SetDifficulty(2));
        }
        else
        {
            Debug.LogError("Hardbutton을 찾을 수 없습니다.");
        }
    }

    public void SetDifficulty(int level)
    {
        Debug.Log("SetDifficulty 호출됨: " + level);
        currentDifficulty = (Difficulty)level;

        switch (currentDifficulty)
        {
            case Difficulty.Easy:
                bulletSpeed = 5f;
                bulletLifeTime = 2.5f;
                speedIncreasePerPhase = 0.7f;
                break;

            case Difficulty.Medium:
                bulletSpeed = 5f;
                bulletLifeTime = 2.5f;
                speedIncreasePerPhase = 0.7f;
                break;

            case Difficulty.Hard:
                bulletSpeed = 5f;
                bulletLifeTime = 2.5f;
                speedIncreasePerPhase = 0.7f;
                break;
        }

        // StartScene에서만 InGameScene으로 전환
        if (SceneManager.GetActiveScene().name == "StartScene")
        {
            StartCoroutine(LoadInGameSceneWithDelay());
        }
    }

    private IEnumerator LoadInGameSceneWithDelay()
    {
        yield return new WaitForSeconds(0.1f); // 살짝의 딜레이를 추가하여 씬 전환 문제 해결 시도
        SceneManager.LoadScene("InGameScene");
    }

    public void IncreaseBulletSpeed()
    {
        bulletSpeed += speedIncreasePerPhase;
        Debug.Log($"총알 속도 증가: {bulletSpeed}");
    }

    public void UpdateBestScore(float survivalTime)
    {
        int difficultyIndex = (int)currentDifficulty;
        if (survivalTime > bestScore[difficultyIndex])
        {
            bestScore[difficultyIndex] = survivalTime;
        }
    }
    public void UpdateBestTime(float survivalTime)
    {
        int difficultyIndex = (int)currentDifficulty;
        if (survivalTime > bestTimes[difficultyIndex])
        {
            bestTimes[difficultyIndex] = survivalTime;
        }
    }

    public float GetScore()
    {
        return bestScore[(int)currentDifficulty];
    }
    public float GetBestTime()
    {
        return bestTimes[(int)currentDifficulty];
    }

public int GetStartingLives()
    {
        switch (currentDifficulty)
        {
            case Difficulty.Easy:
                return 5; // 쉬움 난이도: 생명 5개
            case Difficulty.Medium:
                return 3; // 보통 난이도: 생명 3개
            case Difficulty.Hard:
                return 2; // 어려움 난이도: 생명 2개
            default:
                return 3; // 기본값: 생명 3개
        }
    }

}


