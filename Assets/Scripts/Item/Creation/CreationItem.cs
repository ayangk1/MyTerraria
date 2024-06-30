using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CreationItem : MonoBehaviour
{
    public Item item;

    public void Init(Item newItem)
    {
        item = newItem;
    }

    private void Update()
    {
        transform.position = Input.mousePosition;
        
        Mouse mouse = Mouse.current;
        if (mouse.rightButton.wasPressedThisFrame && CreationSystem.Instance.createdObj != null)
        {
            CreationSystem.Instance.createdObj = null;
            Destroy(transform.gameObject);
        }
        
        
        
    }
}
