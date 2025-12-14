using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;  // UI 관련
using System.Collections;  // IEnumerator 사용을 위한 네임스페이스 추가

#if UNITY_EDITOR
using UnityEditor;  // Unity 에디터 관련 기능을 사용하려면 필요
#endif

public class StartButtonHandler : MonoBehaviour
{
    // 버튼 크기 변화에 사용할 값
    public Vector3 scaleOnClick = new Vector3(1.2f, 1.2f, 1f);  // 클릭 시 버튼 크기 (확대 비율)
    public Vector3 originalScale = new Vector3(1f, 1f, 1f);     // 원래 크기

    // 버튼 클릭 시 호출되는 메서드들
    public void OnStartButtonClicked() => OnButtonClicked("StartScene");
    public void OnRuleButtonClicked() => OnButtonClicked("RuleScene");
    public void OnOptionClicked() => OnButtonClicked("OptionScene");
    public void OnBackClicked() => OnButtonClicked("MainScene");

    // 게임 종료 버튼 클릭 시
    public void OnExitButtonClicked()
    {
        // 버튼 크기 확대 애니메이션 시작
        StartCoroutine(ScaleButton(transform, scaleOnClick, 0.1f));

        // 게임 종료
        ExitGame();
    }

    // 버튼 클릭 시 크기 변경 및 씬 전환
    private void OnButtonClicked(string sceneName)
    {
        // 버튼 크기 확대 애니메이션 시작
        StartCoroutine(ScaleButton(transform, scaleOnClick, 0.1f));

        // 씬 전환
        SceneManager.LoadScene(sceneName);
    }

    // 게임 종료 함수
    private void ExitGame()
    {
        // 게임 종료
        Debug.Log("게임을 종료합니다.");  // 게임 종료 시 로그 출력

        // 빌드된 게임에서는 게임 종료
        Application.Quit();

        // 에디터에서 실행 중일 때는 종료되지 않으므로, 에디터에서 테스트하려면 아래 코드 사용
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
        Debug.Log("게임이 종료되었습니다. (에디터에서 테스트 중)");
#endif
    }

    // 버튼 크기를 확대하고 원래 크기로 돌아오게 하는 코루틴
    private IEnumerator ScaleButton(Transform buttonTransform, Vector3 targetScale, float duration)
    {
        Vector3 initialScale = buttonTransform.localScale;  // 초기 크기
        float timeElapsed = 0f;

        // 크기 확대
        while (timeElapsed < duration)
        {
            buttonTransform.localScale = Vector3.Lerp(initialScale, targetScale, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        buttonTransform.localScale = targetScale;  // 정확한 크기 설정

        // 잠시 후 원래 크기로 돌아옴
        yield return new WaitForSeconds(0.1f);

        // 원래 크기로 돌아가는 애니메이션
        timeElapsed = 0f;
        while (timeElapsed < duration)
        {
            buttonTransform.localScale = Vector3.Lerp(targetScale, originalScale, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        buttonTransform.localScale = originalScale;  // 정확한 원래 크기 설정
    }
}
