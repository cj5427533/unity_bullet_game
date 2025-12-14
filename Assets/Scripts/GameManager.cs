using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; // TextMeshPro 사용을 위한 네임스페이스 추가

public class GameManager : MonoBehaviour
{
    // 게임 오브젝트
    public GameObject gameoverText;  // 게임 오버 텍스트

    // 텍스트
    public Text scoreText;  // 실시간 점수를 표시할 텍스트
    public Text recordText;  // 최고 점수를 표시할 텍스트
    public Text timeText;  // 실제 생존 시간을 표시할 텍스트
    public Text timeText1;  // 최고 생존 시간을 표시할 텍스트
    public Text invincibilityText; // 무적 텍스트
    public TextMeshProUGUI phaseText; // 페이즈 전환 문구 (TextMeshPro)

    // 점수 및 시간
    private float surviveTime;  // 생존 시간
    private float score;  // 점수 + 추가점수
    private float timeBasedScore; // 기본점수
    private bool isGameover; // 게임 오버 상태

    // 프리팹
    public GameObject heartPrefab;   // 하트 프리팹
    public GameObject healthItemPrefab; // 체력 아이템 프리팹
    public GameObject invincibilityItemPrefab; // 무적 아이템 프리팹
    public GameObject scoreItemPrefab; // 점수 아이템 프리팹

    // 생명
    public int maxLife = 5; // 최대 생명 수치
    private int currentLife; // 현재 생명 수치

    // 생명 디자인
    private GameObject[] hearts;     // 하트 오브젝트 배열
    public Transform lifeContainer;  // 하트 UI 부모 컨테이너

    // 페이즈
    public float phaseDuration = 10f; // 페이즈 전환 간격
    private float timeSinceLastPhase; // 마지막 페이즈 시간
    private int currentPhase = 1; // 현재 페이즈

    // 아이템 (생명)
    public float maxHealthItemSpawnInterval = 12f; // 생명 아이템 생성 간격 최대값
    public float minHealthItemSpawnInterval = 8f; // 생명 아이템 생성 간격 최소값
    public float initialHealthItemSpawnDelay = 4f; // 생명 아이템 최초 생성 지연 시간
    // 아이템 (무적)
    public float maxInvincibilityItemSpawnInterval = 20f; // 무적 아이템 생성 간격 최대값
    public float minInvincibilityItemSpawnInterval = 15f; // 무적 아이템 생성 간격 최소값
    public float initialInvincibilityItemSpawnDelay = 10f; // 무적 아이템 최초 생성 지연 시간
    public float invincibilityDuration = 3f; // 무적 지속 시간
    private bool isInvincible = false; // 무적 상태 여부
    // 아이템 (점수)
    public float scoreItemSpawnInterval = 5f; // 점수 아이템 생성 간격

    // 맵
    public float mapRadius = 10f; // 원형 맵의 반경

    void Start()
    {
        surviveTime = 0f;                   // 게임 시작 시 생존 시간 초기화
        score = 0f;                               // 게임 시작 시 점수 초기화
        timeSinceLastPhase = 0f;     // 게임 시작 시 생명 초기화
        isGameover = false;               // 게임 시작 시 게임 오버 False 업데이트

        gameoverText?.SetActive(false);
        phaseText?.gameObject.SetActive(false); // 페이즈 문구 숨기기

        // DifficultyManager에서 난이도를 기반으로 시작 생명 설정
        if (DifficultyManager.instance != null)
        {
            maxLife = DifficultyManager.instance.GetStartingLives(); // 난이도에 따라 최대 생명갯수 지정
        }
        else
        {
            maxLife = 3; // 기본값
        }

        currentLife = maxLife;   // 게임 시작 시 생명 초기화
        InitializeHearts();          // 게임 시작 시 하트 UI 초기화
        UpdateLifeUI();              // 게임 시작 시 UI 초기화

        // 게임 오버 텍스트 비활성화
        if (gameoverText != null)
        {
            gameoverText.SetActive(false);
        }
        else
        {
            Debug.LogError("GameManager: gameoverText가 할당되지 않았습니다.");
        }

        if (recordText == null)
        {
            Debug.LogError("GameManager: recordText가 할당되지 않았습니다.");
        }
        if (timeText1 == null)
        {
            Debug.LogError("GameManager: timeText1가 할당되지 않았습니다.");
        }
        StartCoroutine(SpawnHealthItems());             // 생명 아이템 생성 시작 (초기 지연 후 생성)
        StartCoroutine(SpawnInvincibilityItems());   // 무적 아이템 생성 시작 (초기 지연 후 생성)
        StartCoroutine(SpawnScoreItems());               // 점수 아이템 생성 시작 (초기 지연 후 생성)
    }

    void Update()
    {
        if (!isGameover)
        {
            // 게임이 진행 중일 때, 시간을 계속 추적하고 점수 계산
            surviveTime += Time.deltaTime;
            timeSinceLastPhase += Time.deltaTime;

            if (timeText != null)
            {
                timeBasedScore = Mathf.FloorToInt(surviveTime * 500);                         // 1초당 점수 500씩 증가 점수 계산
                timeText.text = "Time: " + Mathf.FloorToInt(surviveTime).ToString();    // Time 텍스트 UI에 실제 생존 시간 표시
                scoreText.text = "Score: " + (score + timeBasedScore).ToString();          // Score 텍스트 UI에 1초당 500 증가하는 점수 표시

            }

            // 페이즈 전환 체크
            if (timeSinceLastPhase >= phaseDuration)
            {
                AdvancePhase();
                timeSinceLastPhase = 0f;
            }
        }
        else
        {
            // 게임 종료 후, R 키로 재시작 가능
            if (Input.GetKeyUp(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }

            // T 키로 시작 화면으로 돌아가기
            if (Input.GetKeyUp(KeyCode.T))
            {
                SceneManager.LoadScene("StartScene");
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (!isInvincible && currentLife > 0) // 무적 상태가 아닐 때만 데미지 받음
            currentLife -= damage;
        UpdateLifeUI(); // 데미지를 받을 때마다 생명 UI 업데이트

        PlayerController playerController = FindObjectOfType<PlayerController>();

        if (currentLife <= 0)
        {
            if (playerController != null)
            {
                playerController.Die(); // PlayerController의 Die 호출
            }
            EndGame(); // GameManager의 게임 종료 처리
        }
    }

    void InitializeHearts()
    {
        hearts = new GameObject[maxLife];

        for (int i = 0; i < maxLife; i++)
        {
            GameObject heart = Instantiate(heartPrefab, lifeContainer);
            hearts[i] = heart;
        }
    }

    void UpdateLifeUI()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].SetActive(i < currentLife);
        }
    }

    void AdvancePhase()
    {
        currentPhase++;
        DifficultyManager.instance.IncreaseBulletSpeed();

        // 화면 중앙에 문구 출력
        if (phaseText != null)
        {
            phaseText.text = $"페이즈 {currentPhase}: 총알 속도가 더욱 빨라졌습니다!";
            phaseText.gameObject.SetActive(true);
            Invoke(nameof(HidePhaseText), 3f); // 3초 후 문구 숨기기
        }
    }

    void HidePhaseText()
    {
        if (phaseText != null)
        {
            phaseText.gameObject.SetActive(false);
        }



    }
    IEnumerator SpawnHealthItems()
    {
        // 초기 지연 시간 대기
        yield return new WaitForSeconds(initialHealthItemSpawnDelay);

        while (!isGameover)
        {
            // 원형 맵의 반경 안에서 무작위 위치로 체력 아이템 생성
            Vector2 spawnPosition = Random.insideUnitCircle * mapRadius;
            Instantiate(healthItemPrefab, new Vector3(spawnPosition.x, 2, spawnPosition.y), Quaternion.identity);

            // 다음 아이템 생성 대기 시간을 min, max 사이의 랜덤 값으로 설정
            float currentSpawnInterval = Random.Range(minHealthItemSpawnInterval, maxHealthItemSpawnInterval);
            yield return new WaitForSeconds(currentSpawnInterval);
        }
    }

    IEnumerator SpawnInvincibilityItems()
    {
        // 무적 아이템 초기 지연 시간 대기
        yield return new WaitForSeconds(initialInvincibilityItemSpawnDelay);

        while (!isGameover)
        {
            // 원형 맵의 반경 안에서 무작위 위치로 무적 아이템 생성
            Vector2 spawnPosition = Random.insideUnitCircle * mapRadius;
            Instantiate(invincibilityItemPrefab, new Vector3(spawnPosition.x, 2, spawnPosition.y), Quaternion.identity);

            // 다음 무적 아이템 생성 대기 시간을 min, max 사이의 랜덤 값으로 설정
            float currentSpawnInterval = Random.Range(minInvincibilityItemSpawnInterval, maxInvincibilityItemSpawnInterval);
            yield return new WaitForSeconds(currentSpawnInterval);
        }
    }
    IEnumerator SpawnScoreItems()
    {
        while (!isGameover)
        {
            // 원형 맵의 반경 안에서 무작위 위치로 점수 아이템 생성
            Vector2 spawnPosition = Random.insideUnitCircle * mapRadius;
            Instantiate(scoreItemPrefab, new Vector3(spawnPosition.x, 2, spawnPosition.y), Quaternion.identity);

            yield return new WaitForSeconds(scoreItemSpawnInterval); // 다음 점수 아이템 생성 대기
        }
    }
    public void AddScore(int amount) // 추가 점수
    {
        score += amount;
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score; // 현재 점수에서 아이템 점수를 추가
        }
    }

    public void ActivateInvincibility()
    {
        if (!isInvincible)
        {
            isInvincible = true;
            StartCoroutine(InvincibilityCountdown());
        }
    }

    IEnumerator InvincibilityCountdown()
    {
        if (invincibilityText != null)
        {
            invincibilityText.gameObject.SetActive(true); // 무적 텍스트 활성화

            // 무적 지속 시간 동안 카운트다운 표시
            for (int i = (int)invincibilityDuration; i > 0; i--)
            {
                invincibilityText.text = $"무적: {i}초"; // 줄바꿈 없이 한 줄로 표시
                yield return new WaitForSeconds(1f);
            }

            invincibilityText.gameObject.SetActive(false); // 무적 텍스트 비활성화
            invincibilityText.text = ""; // 텍스트 초기화
        }

        isInvincible = false; // 무적 상태 해제
    }
    IEnumerator InvincibilityDuration()
    {
        // 무적 지속 시간 동안 대기
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false; // 무적 상태 해제
    }

    public void HealPlayer()
    {
        if (currentLife < maxLife)
        {
            currentLife++;
            UpdateLifeUI();
        }
    }
    public void EndGame()
    {
        isGameover = true;

        if (gameoverText != null)
        {
            gameoverText.SetActive(true);
        }

        // 현재 난이도에 따른 최고 기록 갱신
        float bestScore = DifficultyManager.instance.GetScore();
        if (score + timeBasedScore > bestScore)
        {
            bestScore = score + timeBasedScore;
            DifficultyManager.instance.UpdateBestScore(bestScore); // 최고 기록 저장
        }
        // 현재 난이도에 따른 생존 시간 갱신
        float bestTime = DifficultyManager.instance.GetBestTime();
        if (surviveTime > bestTime)
        {
            bestTime = surviveTime;
            DifficultyManager.instance.UpdateBestTime(bestTime);
        }
        // 최고 기록 텍스트 업데이트
        if (recordText != null)
        {
            recordText.text = "최고점수 : " + (int)bestScore + "점";
        }
        if (timeText1 != null)
        {
            timeText1.text = "생존시간 : " + (int)bestTime + "초";
        }
    }

    // 재시작 버튼 클릭 처리
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // 메인 메뉴로 돌아가는 버튼 클릭 처리
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainScene");
    }
}
