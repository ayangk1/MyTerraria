using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public enum CharacterEnum
{
    player,
    enemy
}
public class Character : MonoBehaviour
{
    public CharacterEnum characterEnum;
    private CharacterEvent characterEvent;
    private new Rigidbody2D rigidbody2D;

    public float maxHealth;
    public float currHealth;
    
    public float recovery;
    public float recoveryTime;

    public bool isInvincible;
    public float invincibleTime;

    public bool canMove;
    
    public UnityEvent<Transform> OnTakeDemage;
    public UnityEvent<Character> OnHealthChange;
    
    private void OnEnable()
    {
        characterEvent = GetComponent<CharacterEvent>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        currHealth = maxHealth;
        canMove = true;
        invincibleTime = 0.2f;
        switch (characterEnum)
        {
            case CharacterEnum.player:
                StartCoroutine(Recovery());
                OnHealthChange?.Invoke(this);
                break;
            case CharacterEnum.enemy:
                break;
        }
    }

    public void TakeDamage(Attack attacker)
    {
        if (isInvincible) return;
        StartCoroutine(Invincible());
        
        currHealth -= attacker.damage;
        OnTakeDemage?.Invoke(attacker.transform);
        
        canMove = false;

        switch (characterEnum)
        {
            case CharacterEnum.player:
                characterEvent.hitEvent.RaiseEvent();
                break;
            case CharacterEnum.enemy:
                break;
        }

        // var dir = attacker.transform.position.x - transform.position.x;
        // rigidbody2D.drag = 1;
        // if(dir > 0) rigidbody2D.AddForce(Vector2.left  * attacker.repelNum,ForceMode2D.Impulse);
        // else rigidbody2D.AddForce(Vector2.right  * attacker.repelNum,ForceMode2D.Impulse);
        // rigidbody2D.bodyType = RigidbodyType2D.Kinematic;

        OnHealthChange?.Invoke(this);
        
        if (currHealth <= 0) Dead();
    }

    public void Dead()
    {
        switch (characterEnum)
        {
            case CharacterEnum.player:
                break;
            case CharacterEnum.enemy:
                GetComponent<Enemy>().Dead();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    private IEnumerator Recovery()
    {
        yield return new WaitForSeconds(recoveryTime);
        if (currHealth + recovery <= maxHealth)
            currHealth += recovery;
        else
            currHealth = maxHealth;
        OnHealthChange?.Invoke(this);
        StartCoroutine(Recovery());
    }
    
    private IEnumerator Invincible()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibleTime);
        isInvincible = false;
        rigidbody2D.drag = 0;
        canMove = true;
        rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
    }

}
