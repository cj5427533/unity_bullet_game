using UnityEngine;

public class ItemManager : MonoBehaviour
{
    // 아이템 유형 정의
    public enum ItemType { Health, Invincibility, Score } // Score 유형 추가
    public ItemType itemType; // 이 아이템의 유형 설정

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // 플레이어가 닿으면
        {
            GameManager gameManager = FindObjectOfType<GameManager>();

            if (gameManager != null)
            {
                // 아이템 유형에 따라 효과 적용
                if (itemType == ItemType.Invincibility) // 무적 아이템일 때
                {
                    gameManager.ActivateInvincibility(); // 무적 활성화
                }
                else if (itemType == ItemType.Health) // 체력 아이템일 때
                {
                    gameManager.HealPlayer(); // 체력 회복
                }
                else if (itemType == ItemType.Score) // 점수 아이템일 때
                {
                    gameManager.AddScore(1000); // 점수 1000점 추가
                }
            }

            Destroy(gameObject); // 아이템 파괴
        }
    }
}
