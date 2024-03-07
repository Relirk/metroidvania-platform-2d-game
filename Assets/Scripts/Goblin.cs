using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class Goblin : MonoBehaviour
{
    public float speed;
    public float maxVision;
    public Transform point;
    public Transform behind;
    public bool isRight;
    public int health = 3;
    public float stopDistance;
    private bool isFront;
    private bool isDead;
    private Animator anim;
    private Vector2 direction;
    private Rigidbody2D rig;
    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        if (isRight) {
                transform.eulerAngles = new UnityEngine.Vector2(0,0);
                direction = Vector2.right;
            } else {
                transform.eulerAngles = new Vector2(0,180);
                direction = Vector2.left;
           }
    }

    void OnMove() {
        if(isFront && !isDead) {
            anim.SetInteger("transition", 1);

            if (isRight) {
                transform.eulerAngles = new UnityEngine.Vector2(0,0);
                direction = Vector2.right;
                rig.velocity = new Vector2(speed, rig.velocity.y);
            } else {
                transform.eulerAngles = new Vector2(0,180);
                direction = Vector2.left;
                rig.velocity = new Vector2(-speed, rig.velocity.y);
            }
        }
    }

    void FixedUpdate() {
        Getplayer();
        OnMove();
    }

    void Getplayer(){
        RaycastHit2D hit = Physics2D.Raycast(point.position, direction, maxVision);

        if(hit.collider != null && !isDead) {
            if(hit.transform.CompareTag("Player")){
                isFront = true;
                float distance = Vector2.Distance(transform.position, hit.transform.position);
                
                if (distance <= stopDistance) { // distãncia para atacar
                    isFront = false;
                    rig.velocity = Vector2.zero;
                    anim.SetInteger("transition", 2);

                    // chama varias vezes mas o player bloqueia pelo recovery time
                    hit.transform.GetComponent<Player>().OnHit();
                }
            } else {
                isFront = false;
                rig.velocity = Vector2.zero;
                anim.SetInteger("transition", 0);
            }
        }

        RaycastHit2D behindHit = Physics2D.Raycast(behind.position, -direction, maxVision);
        if (behindHit.collider != null)
        {
            if(behindHit.transform.CompareTag("Player")) {
                // player ta atrás do inimigo
                isRight = !isRight;
                isFront = true;
            }
        }
    }

    public void OnHit()
    {
        anim.SetTrigger("hit");
        health--;
        
        if(health <= 0)
        {
            isDead = true;
            speed = 0;
            anim.SetTrigger("dead");
            Destroy(gameObject, 1f);
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.DrawRay(point.position, direction * maxVision);
    }

    private void OnDrawGizmos() {
        Gizmos.DrawRay(behind.position, -direction * maxVision);
    }
}
