using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private PlayerAudio playerAudio;
    private Rigidbody2D rig;
    private bool isJumping;
    private bool isAttacking;
    private bool doubleJump;
    private bool recovery;
    private Health healthSystem;
    public float recoveryTime;
    public Animator anim;
    public float speed;
    public float jumpForce;
    public Transform point;
    public float radius;
    public LayerMask enemyLayer;
    
    public static Player instance;

    [Header("UI")]
    public Text scoreText;
    public GameObject gameOver;

    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else if( instance != this){
            Destroy(instance.gameObject);
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        playerAudio = GetComponent<PlayerAudio>();
        healthSystem = GetComponent<Health>();
    }

    void Update() {
        Jump();
        Attack();
    }

    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        //se nao pressionar nada, retorna 0. Se pressionar direita, retorna 1. Se for esquerda, retorna -1.
        float movement = Input.GetAxis("Horizontal");

        rig.velocity = new Vector2(movement * speed, rig.velocity.y);

        if (movement > 0)
        {
            if (!isJumping && !isAttacking)
            {
                anim.SetInteger("transition", 1);
            }

            transform.eulerAngles = new Vector3(0, 0, 0);
        }

        if (movement < 0)
        {
            if (!isJumping && !isAttacking)
            {
                anim.SetInteger("transition", 1);
            }

            transform.eulerAngles = new Vector3(0, 180, 0);
        }

        if (movement == 0 && !isJumping && !isAttacking)
        {
            anim.SetInteger("transition", 0);
        }

    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (!isJumping)
            {
                anim.SetInteger("transition", 2);
                rig.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                isJumping = true;
                doubleJump = true;
                playerAudio.PlaySFX(playerAudio.jumpSound);
            }
            else if (doubleJump)
            {
                anim.SetInteger("transition", 2);
                rig.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                doubleJump = false;
                playerAudio.PlaySFX(playerAudio.jumpSound);
            }
        }
    }

    void Attack()
    {
        if (Input.GetButtonDown("Fire1")) {
            isAttacking = true;
            anim.SetInteger("transition", 3);

            Collider2D hit = Physics2D.OverlapCircle(point.position, radius, enemyLayer);
            playerAudio.PlaySFX(playerAudio.hitSound);

            if (hit != null) {
                if(hit.GetComponent<Slime>()){
                    hit.GetComponent<Slime>().OnHit();
                }

                if(hit.GetComponent<Goblin>()){
                    hit.GetComponent<Goblin>().OnHit();
                }
            }

            StartCoroutine(OnAttack());
        }
    }

    IEnumerator OnAttack() {
        yield return new WaitForSeconds(0.33f);
        isAttacking = false;
    }

    public void OnHit() {
        if(!recovery){
            anim.SetTrigger("hit");
            healthSystem.health--;

            if(healthSystem.health <= 0) {
                recovery = true;
                anim.SetTrigger("dead");
                GameController.instance.ShowGameOver();
            } else {
                StartCoroutine(Recover());
            }
        }  
    }

    private IEnumerator Recover() {
        recovery = true;
        yield return new WaitForSeconds(recoveryTime);
        recovery = false;
    }
    

    void OnDrawGizmos() {
        Gizmos.DrawWireSphere(point.position, radius);    
    }

    private void OnCollisionEnter2D(Collision2D collider) {
        if(collider.gameObject.layer == 3 || collider.gameObject.layer == 10) {
            isJumping = false;
            doubleJump = false;
        }

        if(collider.gameObject.layer == 9) {
            PlayerPos.instance.Checkpoint();
        }
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.layer == 6) {
            OnHit();
        }

        if(collision.CompareTag("Coin")){
            playerAudio.PlaySFX(playerAudio.coinSound);
            collision.GetComponent<Animator>().SetTrigger("hit");
            GameController.instance.GetCoin();
            Destroy(collision.gameObject, .5f);
        }
    }
}
