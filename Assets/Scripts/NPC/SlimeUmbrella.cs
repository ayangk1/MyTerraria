using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeUmbrella : Enemy
{
    public override string m_name => "slimeUmbrella";
    public float jumpForce;
    public bool isGround;
    public float jumpInterval;
    private float timer;
    private readonly RaycastHit2D[] raycastHits = new RaycastHit2D[5];
    
    protected override void OnEnable()
    {
        timer = jumpInterval;
        if (GameManager.Instance != null)
            target = GameManager.Instance.player;
        
        base.OnEnable();
    }

    private void Update()
    {
        Move();
        isGround = Collider2D.Cast(Vector2.down, raycastHits, 0.25f) > 0;
    }

    private void Move()
    {
        if (target == null)
        {
            target = GameManager.Instance.player;
            return;
        }
        if (isGround && timer < 0)
        {
            Vector2 dir;
            
            if (target.position.x >= transform.position.x )dir = new Vector2(1, 10);
            else dir = new Vector2(-1, 10);
            
            rigidbody2D.AddForce(dir * jumpForce,ForceMode2D.Impulse);
            timer = jumpInterval;
        }
        if (isGround) timer -= Time.deltaTime;
    }

    public override void Dead()
    {
        var obj = Instantiate(AtlasManager.Instance.prefabsAtlas.pickupItem);
        obj.GetComponentInChildren<ItemActive>().Init(AtlasManager.Instance.itemAtlas.gelItem,transform.position);
        
        ObjectPool.Instance.ReturnPool(transform.gameObject,4);
        base.Dead();
    }

    
}
