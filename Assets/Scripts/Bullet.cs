using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bullet : MonoBehaviour
{
    public float speed; // Ѿ ӵ
    public float lifeTime; // Ѿ ð
    private Transform playerTransform;
    public int damage = 1; // Ѿ 

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        FindPlayer();
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        // ̵  ӵ
        if (DifficultyManager.instance != null)
        {
            speed = DifficultyManager.instance.bulletSpeed;
            lifeTime = DifficultyManager.instance.bulletLifeTime;
        }
        else
        {
            // DifficultyManager가 없으면 기본값 사용
            speed = 5f;
            lifeTime = 2.5f;
            Debug.LogWarning("Bullet: DifficultyManager.instance가 null입니다. 기본값을 사용합니다.");
        }

        // speed나 lifeTime이 0이면 기본값으로 설정
        if (speed <= 0)
        {
            speed = 5f;
            Debug.LogWarning("Bullet: speed가 0입니다. 기본값 5f로 설정합니다.");
        }
        if (lifeTime <= 0)
        {
            lifeTime = 2.5f;
            Debug.LogWarning("Bullet: lifeTime이 0입니다. 기본값 2.5f로 설정합니다.");
        }

        //ð  Ѿ
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (playerTransform != null && speed > 0)
        {
            transform.LookAt(playerTransform);
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
        else if (playerTransform == null)
        {
            // 플레이어를 찾지 못했으면 다시 시도
            FindPlayer();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {

                // GameManagerνϽ  TakeDamage ȣ
                GameManager gameManager = FindObjectOfType<GameManager>();
                if (gameManager != null)
                {
                    gameManager.TakeDamage(damage);
                }

                Destroy(gameObject); // 浹  Ѿı
            }
        }
    }

    //ε  ȣǴ ޼
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "SampleScene")
        {
            FindPlayer();
        }
        else
        {
            playerTransform = null; // ٸ  Player null
        }
    }

    // Player ã Ҵϴ ޼
    private void FindPlayer()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }
}
