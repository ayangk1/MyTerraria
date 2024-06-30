using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAttack : MonoBehaviour
{
    public Attack attack;
    private Vector3 moveSpeed;
    private Vector3 GritySpeed = Vector3.zero;
    public float power;
    private float dTime;
    public float Gravity = -10;
    public Transform attackPos;
    private Vector3 currentAngle;
    public float forceCount;
    public bool isRight;

    public GameObject player;
    private void OnEnable()
    {
        
        if (GameManager.Instance == null) return;
        attack = GetComponent<Attack>();
        attack.damage = 10;
        GritySpeed = Vector3.zero;
        Gravity = -10;
        dTime = 0;
        power = 18f;
        attackPos = GameObject.FindWithTag("AttackPos").transform;
        moveSpeed = attackPos.rotation * -transform.up * power;
        currentAngle = Vector3.zero;
        player = GameObject.FindWithTag("Player");
        if (player.transform.localRotation.y == 0) isRight = true;
    }

    private void FixedUpdate()
    {
        GritySpeed.y = Gravity * (dTime += Time.fixedDeltaTime); 
        //位移模拟轨迹 
        transform.position += (moveSpeed + GritySpeed) * Time.fixedDeltaTime;
        
        if (isRight) currentAngle.z = Mathf.Atan((moveSpeed.y + GritySpeed.y) / moveSpeed.x) * Mathf.Rad2Deg + 90f;
        else currentAngle.z = Mathf.Atan((moveSpeed.y + GritySpeed.y) / moveSpeed.x) * Mathf.Rad2Deg + 270f;
        
        transform.eulerAngles = currentAngle;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ground") || other.CompareTag("Enemy"))
        {
            ObjectPool.Instance.ReturnPool(transform.gameObject,2);
            attackPos = null;
            transform.rotation = Quaternion.Euler(0,0,0);
            isRight = false;
        }
    }
}
