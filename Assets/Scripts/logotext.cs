using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextAnimation : MonoBehaviour
{
    public Text coverText;  // UI Text를 연결할 변수
    public float animationDuration = 3f;  // 애니메이션 지속 시간 (3초)
    public Vector3 targetScale = new Vector3(1.2f, 1.2f, 1f);  // 확대할 크기
    private Vector3 originalScale;  // 원래 크기

    private const string AnimationCompletedKey = "TextAnimationCompleted";  // PlayerPrefs 키

    void Start()
    {
        // coverText가 null인지 체크
        if (coverText == null)
        {
            Debug.LogError("coverText가 null입니다! UI Text 컴포넌트를 연결해주세요.");
            return;  // coverText가 null이면 애니메이션을 실행하지 않음
        }

        // 원래 크기 저장 (첫 실행시 애니메이션을 실행하기 위해)
        originalScale = coverText.transform.localScale;

        // 애니메이션이 이미 완료되었는지 확인
        if (PlayerPrefs.GetInt(AnimationCompletedKey, 0) == 1)
        {
            // 애니메이션이 이미 완료되었으면 바로 큰 텍스트로 표시
            coverText.transform.localScale = targetScale;
            return;  // 애니메이션 실행을 건너뜁니다.
        }

        // 애니메이션을 실행하고, 끝나면 PlayerPrefs에 완료 상태를 기록
        StartCoroutine(ScaleText());  // 애니메이션 시작
    }

    private IEnumerator ScaleText()
    {
        float timeElapsed = 0f;

        // 확대 애니메이션
        while (timeElapsed < animationDuration)
        {
            // 비율을 timeElapsed / animationDuration로 계산하여 보간
            float scaleLerpFactor = timeElapsed / animationDuration;
            coverText.transform.localScale = Vector3.Lerp(originalScale, targetScale, scaleLerpFactor);

            // 시간 경과
            timeElapsed += Time.deltaTime;

            yield return null;  // 프레임마다 기다림
        }

        // 정확한 크기로 설정 (애니메이션 끝났을 때 최종 크기)
        coverText.transform.localScale = targetScale;

        // 애니메이션이 완료되었음을 PlayerPrefs에 저장
        PlayerPrefs.SetInt(AnimationCompletedKey, 1);
        PlayerPrefs.Save();  // 변경사항 저장

        // 디버그 로그로 확인
        Debug.Log("애니메이션 완료: " + coverText.transform.localScale);
    }
}
