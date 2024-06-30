using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler
{
    public Item item;
    public Image image;
    public Transform parentAfterDrag;

    public int Count
    {
        get { return count;} 
        set
        {
            count = value;
            Storage.Instance.items[item] = value;
            RefreshCount();
            if (count == 0)
            {
                Storage.Instance.items.Remove(item);
                Destroy(gameObject);
            }
        }
    }
    private int count;
    private Text text;

    public void RefreshCount()
    {
        if (!transform.parent.gameObject.activeSelf) return;
        if (!item.stackable ) text.text = "";
        else text.text = count.ToString();
    }
    
    public void InitItem(Item newItem,int newCount)
    {
        text = transform.GetChild(1).GetComponent<Text>();
        image = transform.GetChild(0).GetComponent<Image>();
        item = newItem;
        image.sprite = item.image;
        image.SetNativeSize();
        gameObject.name = item.tileName;
        count = newCount;
        parentAfterDrag = transform.parent;
        RefreshCount();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        image.raycastTarget = false;
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        image.raycastTarget = true;
        transform.SetParent(parentAfterDrag);
    }
}
