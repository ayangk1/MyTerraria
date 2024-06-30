using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Bringer : Enemy
{
    public float outHateTime = 5;
    public float skillIntervalTime;
    public float attackIntervalTime;
    public GameObject spell;
    public LayerMask layer;
    public bool canAttack;
    public GameObject target;
    public bool isFaceRight;
    public Vector3 viewDir => isFaceRight ? Vector2.right : Vector2.left;
    // protected override void Start()
    // {
    //     base.Start();
    // }

    private void Update()
    {
       // base.Update();
        if (transform.localScale.x == 1)
        {
           // isFaceRight = false;
        }
        else if (transform.localScale.x == -1)
        {
           // isFaceRight = true;
        }
    }

    // protected override void Movement()
    // {
    //     base.Movement();
    //     if (state == State.Chase || state == State.Skill || state == State.Attack)
    //     {
    //         outHateTime -= Time.deltaTime;
    //         skillIntervalTime -= Time.deltaTime;
    //         attackIntervalTime -= Time.deltaTime;
    //         if (outHateTime <= 0)
    //         {
    //             target = null;
    //             Animator.Play("Walk");
    //         }
    //     }
    //     switch (state)
    //     {
    //         case State.Idle:
    //             break;
    //         case State.Attack:
    //             view.SetActive(false);
    //             rigidbody.velocity = new Vector2(transform.localScale.x * attackMoveSpeed, rigidbody.velocity.y);
    //             break;
    //         case State.Hit:
    //             break;
    //         case State.Walk:
    //             view.SetActive(true);
    //             Walk();
    //             break;
    //         case State.Chase:
    //             view.SetActive(true);
    //             if (skillIntervalTime <= 0)
    //             {
    //                 Animator.Play("Cast");
    //             }
    //             if (attackIntervalTime <= 0)
    //             {
    //                 canAttack = true;
    //             }
    //             if (target != null)
    //             {
    //                 Vector2 dir = new Vector2(target.transform.position.x, 0) - new Vector2(transform.position.x, 0);
    //                 if (dir.normalized.x > 0)
    //                 {
    //                     transform.localScale = new Vector3(-1, 1, 1);
    //                 }
    //                 else transform.localScale = new Vector3(1, 1, 1);
    //                 if (Mathf.Abs(dir.x) > 1.5f)
    //                 {
    //                     Animator.Play("Chase");
    //                     rigidbody.velocity = new Vector2(moveSpeed * dir.normalized.x, rigidbody.velocity.y);
    //                 }
    //                 else
    //                     Animator.Play("AttackIdle");
    //
    //             }
    //             Attack();
    //             break;
    //         case State.Skill:
    //             break;
    //         case State.Dead:
    //             break;
    //         default:
    //             break;
    //     }
    // }
    public void Skill()
    {
        GameObject sp = Instantiate(spell, target.transform.position + new Vector3(0, -1, 0), Quaternion.identity);
        Destroy(sp, 1);
    }
    public void SkillOver()
    {
        skillIntervalTime = 20;
        if (target == null)
            Animator.Play("Walk");
        else Animator.Play("Chase");
    }
    public void AttackOver()
    {
        attackIntervalTime = 5;
        if (target == null)
            Animator.Play("Walk");
        else Animator.Play("Chase");
    }
    public void Attack()
    {
        RaycastHit2D ray = Physics2D.Raycast(transform.position + new Vector3(0, 0.5f), new Vector3(viewDir.x, 0), 4, layer);
        if (ray.transform != null)
        {
            if (ray.transform.CompareTag("Player") && canAttack)
            {
                canAttack = false;
                Animator.Play("Attack");
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 5);
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position + new Vector3(0, 1), new Vector3(viewDir.x, 0) * 4);
    }

    // public override void GetHit(Vector2 direction, float demage)
    // {
    //     //base.GetHit(-direction, demage);
    //
    //     //GameObject num = Instantiate(hitNumber, transform.position + new Vector3(direction.x * Random.Range(0.6f, 0.8f), Random.Range(0.9f, 1.1f)), Quaternion.identity);
    //     //num.GetComponentInChildren<TextMeshProUGUI>().text = demage.ToString();
    //     //Destroy(num, 1);
    // }

    // private void OnTriggerEnter2D(Collider2D other)
    // {
    //     if (other.CompareTag("Player"))
    //     {
    //         switch (state)
    //         {
    //             case State.Idle:
    //                 break;
    //             case State.Attack:
    //                 if (transform.localScale.x < 0)
    //                     other.GetComponent<PlayerController>().GetHit(Vector2.right);
    //                 else if (transform.localScale.x > 0)
    //                     other.GetComponent<PlayerController>().GetHit(Vector2.left);
    //                 break;
    //             case State.Hit:
    //                 break;
    //             case State.Walk:
    //                 target = other.gameObject;
    //                 Animator.Play("Chase");
    //                 outHateTime = 10;
    //                 break;
    //             case State.Chase:
    //                 break;
    //             case State.Skill:
    //                 break;
    //             case State.Dead:
    //                 break;
    //             default:
    //                 break;
    //         }
    //     }
    // }
    // private void OnTriggerStay2D(Collider2D other)
    // {
    //     if (other.CompareTag("Player"))
    //     {
    //         switch (state)
    //         {
    //             case State.Idle:
    //                 break;
    //             case State.Attack:
    //                 target = other.gameObject;
    //                 outHateTime = 10;
    //                 break;
    //             case State.Hit:
    //                 break;
    //             case State.Walk:
    //                 break;
    //             case State.Chase:
    //                 target = other.gameObject;
    //                 outHateTime = 10;
    //                 break;
    //             case State.Skill:
    //                 target = other.gameObject;
    //                 outHateTime = 10;
    //                 break;
    //             case State.Dead:
    //                 break;
    //             default:
    //                 break;
    //         }
    //     }
    // }
    // private void OnTriggerExit2D(Collider2D other)
    // {
    //     if (other.CompareTag("Player"))
    //     {
    //         switch (state)
    //         {
    //             case State.Idle:
    //                 break;
    //             case State.Attack:
    //                 break;
    //             case State.Hit:
    //                 break;
    //             case State.Walk:
    //                 break;
    //             case State.Chase:
    //                 break;
    //             case State.Skill:
    //                 break;
    //             case State.Dead:
    //                 break;
    //             default:
    //                 break;
    //         }
    //     }
    // }
}
