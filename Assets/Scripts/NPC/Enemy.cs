
using UnityEngine;

public enum State
{
    Idle,
    Attack,
    Hit,
    Walk,
    Chase,
    Skill,
    Dead,
}


public class Enemy : MonoBehaviour
{
    public virtual string m_name => "enemy";
    
    public float health;
    protected Transform target;
    protected Animator Animator;
    protected new Rigidbody2D rigidbody2D;
    protected Collider2D Collider2D;
    protected bool IsHit;
    
    protected virtual void OnEnable()
    {
        Animator = GetComponent<Animator>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        if (GetComponent<CircleCollider2D>() != null) Collider2D = GetComponent<CircleCollider2D>();
        else Collider2D = GetComponent<CapsuleCollider2D>();

    }
    
    public void GetHit(Vector2 direction, float demage)
    {
        transform.localScale = new Vector3(-direction.x, 1, 1);
        IsHit = true;
        health -= demage;
        if (health <= 0) Dead();
    }
    
    public virtual void Dead()
    {
        // Destroy(transform.gameObject);
    }

}
