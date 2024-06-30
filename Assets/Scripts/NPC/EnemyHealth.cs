using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public Character character;
    public Vector2 screenPoint;
    public void Init(Character m_character)
    {
        character = m_character;
        screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, character.transform.position);
        transform.position = screenPoint;
    }

    private void Update()
    {
        screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, character.transform.position);
        transform.position = screenPoint;
    }

    private void OnDisable()
    {
        character = null;
    }
    
}
