using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    public GameObject bulletPrefab; // ź , GameObject Ÿ
    public float spawnRateMin = 0.5f; // źֱּ
    public float spawnRateMax = 3f; // ''ִ ''
    public float rotationSpeed = 30f; // ȸϴ ӵ

    private Transform target; // ߻ 󿡰 Ʈ Ʈ Ʈ Ҵ
    private float spawnRate; //ֱ, spawnMin spawnMax ̿ 
    private float timeAfterSpawn; //ֱ  ð,  ź 
                                  //帥ð ǥϴ Ÿ̸
    public bool clockwise = true; //ð

    void Start()
    {
        //ֱ ð 0 ʱȭ
        timeAfterSpawn = 0f;

        // ź  ֱ
        spawnRate = Random.Range(spawnRateMin, spawnRateMax);

        // bulletPrefab이 할당되었는지 확인
        if (bulletPrefab == null)
        {
            Debug.LogWarning("BulletSpawner: bulletPrefab이 할당되지 않았습니다. 자동으로 찾는 중...");
            // 자동으로 Bullet prefab 찾기
            GameObject foundBullet = GameObject.Find("Bullet");
            if (foundBullet != null)
            {
                bulletPrefab = foundBullet;
                Debug.Log("BulletSpawner: Bullet GameObject를 찾았습니다.");
            }
            else
            {
                // Resources 폴더에서 찾기 시도
                bulletPrefab = Resources.Load<GameObject>("Bullet");
                if (bulletPrefab != null)
                {
                    Debug.Log("BulletSpawner: Resources에서 Bullet prefab을 찾았습니다.");
                }
                else
                {
                    Debug.LogError("BulletSpawner: bulletPrefab을 찾을 수 없습니다! Unity Inspector에서 수동으로 할당해주세요.");
                }
            }
        }

        // PlayerController  ӿƮ ã ش
        FindPlayer();
    }

    void FindPlayer()
    {
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            target = playerController.transform;
        }
        else
        {
            Debug.LogWarning("BulletSpawner: PlayerController를 찾을 수 없습니다. 잠시 후 다시 시도합니다.");
            // 잠시 후 다시 시도
            Invoke(nameof(FindPlayer), 0.5f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // ȸϵ
        float rotationDirection = clockwise ? 1f : -1f; //ð/ݽð
        transform.RotateAround(Vector3.zero, Vector3.up, rotationDirection * rotationSpeed * Time.deltaTime);

        //timeAfterSpawn
        timeAfterSpawn += Time.deltaTime;
        //ֱ  ðֱ⺸ ũų 
        if (timeAfterSpawn >= spawnRate)
        {
            // target이 null이면 플레이어를 다시 찾기
            if (target == null)
            {
                FindPlayer();
            }

            // target이 여전히 null이면 총알을 발사하지 않음
            if (target != null && bulletPrefab != null)
            {
                //ð
                timeAfterSpawn = 0f;

                // bulletPrefeb 
                // transform.position ġ transform.ratation ȸ
                GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
                
                if (bullet != null)
                {
                    //  bullet  target ϵ ġ ȸ
                    bullet.transform.LookAt(target);
                    Debug.Log($"총알 생성됨: {bullet.name} at {transform.position}");
                }
                else
                {
                    Debug.LogError("BulletSpawner: 총알 생성 실패!");
                }

                spawnRate = Random.Range(spawnRateMin, spawnRateMax);
            }
            else
            {
                if (target == null)
                {
                    Debug.LogWarning("BulletSpawner: target이 null입니다. 총알을 발사할 수 없습니다.");
                }
                if (bulletPrefab == null)
                {
                    Debug.LogWarning("BulletSpawner: bulletPrefab이 null입니다. 총알을 발사할 수 없습니다.");
                }
            }
        }
    }
}
