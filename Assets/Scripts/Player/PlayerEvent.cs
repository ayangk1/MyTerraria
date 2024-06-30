using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEvent : MonoBehaviour
{
    private Character character;
    private List<GameObject> bodyMasks;
    private CharacterEvent characterEvent;

    private void OnEnable()
    {
        character = GetComponent<Character>();
        characterEvent = GetComponent<CharacterEvent>();
        bodyMasks = new List<GameObject>();

        characterEvent.hitEvent.OnEventRaised += GetHit;
    }

    private void OnDisable()
    {
        characterEvent.hitEvent.OnEventRaised -= GetHit;
    }

    public void GetHit()
    {
        Transform mask = transform.GetChild(transform.childCount - 1);
        for (int i = 0; i < mask.childCount; i++)
            mask.GetChild(i).GetComponent<SpriteRenderer>().color = new Color(1,0,0,1);
        StartCoroutine(HitOver());
    }

    private IEnumerator HitOver()
    {
        yield return new WaitForSeconds(character.invincibleTime);
        Transform mask = transform.GetChild(transform.childCount - 1);
        for (int i = 0; i < mask.childCount; i++)
            mask.GetChild(i).GetComponent<SpriteRenderer>().color = new Color(1,0,0,0);
    }

}
