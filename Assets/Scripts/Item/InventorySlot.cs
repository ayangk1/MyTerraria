using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour,IDropHandler,IPointerClickHandler
{
    public bool IsSelected
    {
        get => isSelected;
        set
        {
            isSelected = value;
            if (value)
            {
                GameObject.Find("GameController").GetComponent<GameController>().currSlot = GetComponent<InventorySlot>();
                GetComponent<Image>().sprite = AtlasManager.Instance.textureAtlas.inventoryHighLight;
            }
            else
                GetComponent<Image>().sprite = AtlasManager.Instance.textureAtlas.inventoryNormal;
        }
    }
    private bool isSelected;
    public int index;
    
    public bool IsPlaceItem()
    {
        InventoryItem item = GetComponentInChildren<InventoryItem>();
        if (item == null || !item.item.isPlace) return false;
        return true;
    }
    
    public InventoryItem GetItemInSlot()
    {
        return GetComponentInChildren<InventoryItem>();
    }
    
    public void OnDrop(PointerEventData eventData)
    {
        InventoryItem drapInventoryItem = eventData.pointerDrag.GetComponent<InventoryItem>();
        InventoryItem thisInventoryItem = GetComponentInChildren<InventoryItem>();
        if (transform.childCount == 0)
            drapInventoryItem.parentAfterDrag = transform;
        else if (transform.childCount != 0 && drapInventoryItem.item.stackable && thisInventoryItem.item == drapInventoryItem.item)
        {
            thisInventoryItem.Count += drapInventoryItem.Count;
            Destroy(eventData.pointerDrag.gameObject);
        }
        else if (transform.childCount != 0 && thisInventoryItem.item != drapInventoryItem.item)
        {
            thisInventoryItem.transform.SetParent(drapInventoryItem.parentAfterDrag);
            drapInventoryItem.parentAfterDrag = transform;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!IsSelected && CreationSystem.Instance.createdObj == null)
            GameObject.Find("GameController").GetComponent<GameController>().ForSwitchInventory(index);
        
        if (CreationSystem.Instance.createdObj != null)
        {
            InventoryManager.Instance.AddItemToPos(CreationSystem.Instance.createdObj.GetComponent<CreationItem>().item,transform);
            InventoryManager.Instance.RemoveCreatedItemRaw(CreationSystem.Instance.createdObj.GetComponent<CreationItem>().item);
            Destroy(CreationSystem.Instance.createdObj);
            CreationSystem.Instance.createdCount = 0;
            CreationSystem.Instance.createdObjTimes = 0;
            if(CreationSystem.Instance != null && CreationSystem.Instance.createdObj != null) CreationSystem.Instance.createdObj = null;
        }
    }
    
}
