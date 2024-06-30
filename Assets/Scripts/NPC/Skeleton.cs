using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class Skeleton : Enemy
{
    public float speed;
    private float findDirection;
    private Animator hitAnimator;
    public Vector3 offest;
    [SerializeField]
    private AudioClip SkeletonHit;
    //bool
    public bool isAttack;
    private bool canAttack;
    private bool isDead;
    public GameObject target;
    public bool isFaceRight;
    public Vector3 viewDir => isFaceRight ? Vector2.right : Vector2.left;
    //是否原地待命
    public bool IsIdle
    {
        get { return isIdle; }
        set
        {
            isIdle = value;
            Animator.SetBool("Idle", value);
        }
    }
    private bool isIdle;
    private bool isGetPlayer;
    public bool IsWalk
    {
        get { return isWalk; }
        set
        {
            isWalk = value;
            Animator.SetBool("Walk", value);
        }
    }
    private bool isWalk;
    [SerializeField] private LayerMask layer;
    //攻击间隔 
    public float attackInterval = 2f;
    public float idleInterval = 3f;

    // protected override void Start()
    // {
    //     base.Start();
    //     hitAnimator = transform.GetChild(0).GetComponent<Animator>();
    //    // StartCoroutine(Count(idleInterval, State.Idle));
    //     Init();
    // }
    private void Init()
    {
        health = 50;
        canAttack = true;
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(new Vector3(0, 0, 0), new Vector3(10, 0, 0));
    }
    private void Update()
    {
        if (isDead) return;

        if (transform.localScale.x == 1)
        {
            isFaceRight = true;
        }
        else if (transform.localScale.x == -1)
        {
            isFaceRight = false;
        }
    }
    //行为状态
    // protected override void Movement()
    // {
    //     base.Movement();
    //     switch (state)
    //     {
    //         case State.Idle:
    //             break;
    //         case State.Attack:
    //             //视野关闭
    //             view.SetActive(false);
    //             //攻击时Skeleton速度
    //             rigidbody.velocity = new Vector2(transform.localScale.x * attackMoveSpeed, rigidbody.velocity.y);
    //             break;
    //         case State.Hit:
    //             //视野关闭
    //             view.SetActive(false);
    //             break;
    //         case State.Walk:
    //             //视野关闭
    //             view.SetActive(true);
    //             break;
    //         case State.Chase:
    //             //视野关闭
    //             view.SetActive(false);
    //             AttackPlayer();
    //             rigidbody.velocity = new Vector2(findDirection * moveSpeed, rigidbody.velocity.y);
    //             break;
    //         case State.Dead:
    //             //视野关闭
    //             view.SetActive(false);
    //             break;
    //         default:
    //             break;
    //     }
    //}


    // public override void GetHit(Vector2 direction, float demage)
    // {
    //     base.GetHit(direction, demage);
    //
    //     AudioSource.clip = SkeletonHit;
    //     AudioSource.time = 0.2f;
    //     AudioSource.Play();
    //     
    //     hitAnimator.SetTrigger("Hit");
    //     StartCoroutine(Count(1, State.Hit));
    // }
    // protected override IEnumerator Count(float interval, State name)
    // {
    //     if (isDead)
    //     {
    //         yield return 0;
    //     }
    //     switch (name)
    //     {
    //         case State.Idle:
    //             IsIdle = true;
    //             yield return new WaitForSeconds(interval);
    //             IsWalk = true;
    //             IsIdle = false;
    //             break;
    //         case State.Attack:
    //             isAttack = true;
    //             canAttack = false;
    //             yield return new WaitForSeconds(interval);
    //             canAttack = true;
    //             break;
    //         case State.Hit:
    //             yield return new WaitForSeconds(1);
    //             isAttack = false;
    //             break;
    //         default:
    //             break;
    //     }
    // }
    // public void Dead()
    // {
    //     if (health <= 0)
    //     {
    //         int layer = LayerMask.NameToLayer("Discard");
    //         gameObject.layer = layer;
    //         isDead = true;
    //         Animator.SetTrigger("Dead");
    //         Destroy(gameObject, 3);
    //     }
    // }
    // public void Destroy()
    // {
    //     Destroy(transform, 1);
    // }
    // //大于0是右方
    // public void GetPlayer(float direction)
    // {
    //     if (direction > 0)
    //     {
    //         findDirection = 1;
    //     }
    //     else
    //         findDirection = -1;
    //     transform.localScale = new Vector3(findDirection, 1, 1);
    //     isGetPlayer = true;
    // }
    //
    // public void AttackPlayer()
    // {
    //     RaycastHit2D hit = Physics2D.Raycast(transform.position + offest, viewDir, 1, layer);
    //     if (hit.collider && hit.transform.tag == "Player" && canAttack && !isAttack)
    //     {
    //
    //         StartCoroutine(Count(attackInterval, State.Attack));
    //         Animator.SetTrigger("Attack");
    //     }
    // }
    //
    // private void OnTriggerEnter2D(Collider2D other)
    // {
    //     if (other.CompareTag("Player"))
    //     {
    //         if (!isAttack)
    //         {
    //             Debug.Log("Find Player!!!");
    //             if (!animatorInfo.IsName("Attack"))
    //             {
    //                 float dir = other.transform.position.x - transform.position.x;
    //                 GetPlayer(dir);
    //             }
    //         }
    //         else
    //         {
    //             Debug.Log("Attack Player!!!");
    //             if (transform.localScale.x > 0)
    //                 other.GetComponent<PlayerController>().GetHit(Vector2.right);
    //             else if (transform.localScale.x < 0)
    //                 other.GetComponent<PlayerController>().GetHit(Vector2.left);
    //         }
    //     }
    // }
    //
    // public void AttackOver()
    // {
    //     isAttack = false;
    //     if (!isGetPlayer)
    //     {
    //         StartCoroutine(Count(idleInterval, State.Idle));
    //     }
    // }
    // private void OnTriggerExit2D(Collider2D other)
    // {
    //     if (other.CompareTag("Player"))
    //     {
    //         if (!isAttack || !IsWalk)
    //         {
    //             StartCoroutine(Count(idleInterval, State.Idle));
    //         }
    //         Debug.Log("Lose Player!!!");
    //         isGetPlayer = false;
    //     }
    // }
}
