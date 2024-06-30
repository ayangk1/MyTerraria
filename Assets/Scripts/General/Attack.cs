using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// public enum AttackType
// {
//     enemy,
//     weapon
// }
public class Attack : MonoBehaviour
{
    // public AttackType type;

    public float damage;
    public float repelNum;//击退

    public int axeIntensity;//斧力
    public int pickaxeIntensity;//稿力
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // if(type != AttackType.weapon) return;
            
        other.GetComponent<Character>()?.TakeDamage(this);
    }
    // private void OnCollisionEnter2D(Collision2D other)
    // {
    //      other.gameObject.GetComponent<Character>()?.TakeDamage(this);
    // }
    

    
}
