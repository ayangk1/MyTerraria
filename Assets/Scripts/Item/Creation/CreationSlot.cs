using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CreationSlot : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IPointerClickHandler
{
    public int Index;
    public Item item;
    public bool IsSelected
    {
        get => isSelected;
        set
        {
            isSelected = value;
            if (value)
            {
                transform.GetComponent<Image>().sprite = AtlasManager.Instance.textureAtlas.inventoryHighLight;
                CreationBar.Instance.curSlot = GetComponent<CreationSlot>();
                CreationBar.Instance.RefrashRaw();
            }
            else
                transform.GetComponent<Image>().sprite = AtlasManager.Instance.textureAtlas.inventoryNormal;
        }
    }
    [SerializeField] private bool isSelected;
    private int count;

    public GameObject canvas;
    
    public void Init(Item newItem,int newCount)
    {
        item = newItem;
        transform.name = item.tileName;
        
        transform.GetChild(0).GetComponent<Image>().sprite = item.image;
        transform.GetChild(0).GetComponent<Image>().SetNativeSize();

        count = newCount;

        if (newItem.stackable)
            transform.GetChild(1).GetComponent<Text>().text = count.ToString();
        else
            transform.GetChild(1).GetComponent<Text>().text = "";
        
        if (CreationBar.Instance.transform.childCount == 1)
        {
            Index = 0;
            IsSelected = true;
        }
        else
            Index = CreationBar.Instance.transform.GetChild(CreationBar.Instance.transform.childCount - 2)
                .GetComponent<CreationSlot>().Index - 1;

        GetComponent<RectTransform>().anchoredPosition = new Vector2(0, Index * 70);
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (IsSelected)
        {
            CreationSystem.Instance.createdObjTimes++;
            if (CreationBar.Instance.PreCreation(item,CreationSystem.Instance.createdObjTimes))
            {
                if (CreationSystem.Instance.createdObj == null)
                {
                    var obj = Instantiate(AtlasManager.Instance.prefabsAtlas.creationItem,transform.root);
                    Image img = obj.GetComponent<Image>();
                    
                    CreationSystem.Instance.createdObj = obj;
                    obj.GetComponent<CreationItem>().Init(item);
                    img.sprite = item.image;
                    img.SetNativeSize();
                    img.raycastTarget = false;
                }
                CreationSystem.Instance.createdCount += count;
                if (CreationSystem.Instance.createdObj.GetComponent<CreationItem>().item.stackable)
                    CreationSystem.Instance.createdObj.GetComponentInChildren<Text>().text =
                        CreationSystem.Instance.createdCount.ToString();
                else
                    CreationSystem.Instance.createdObj.GetComponentInChildren<Text>().text = "";

            }
            return;
        }
        
        CreationBar.Instance.ClickSlot(GetComponent<CreationSlot>());
    }
    
}
