using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody playerRigidbody;
    public float speed = 8f;
    public bool wDown;
    public bool wDie;

    Animator animator;
    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();
    }

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (playerRigidbody && !wDie)
        {
            float xInput = Input.GetAxis("Horizontal");
            float zInput = Input.GetAxis("Vertical");

            float xSpeed = xInput * speed;
            float zSpeed = zInput * speed;
            Vector3 newVelocity = new Vector3(xSpeed, 0f, zSpeed);
            playerRigidbody.velocity = newVelocity;


            animator.SetBool("isWalk", newVelocity != Vector3.zero);
            animator.SetBool("isWalk", wDown);

            transform.LookAt(transform.position + newVelocity);
        }
    }
    public void Die()
    {
        animator.SetTrigger("isDie");
        wDie = true;
        GameManager gameManager = FindObjectOfType<GameManager>();
        gameManager.EndGame();
        
    }
}
