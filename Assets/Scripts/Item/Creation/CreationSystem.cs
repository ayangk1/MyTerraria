using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class CreationSystem : SingletonMonoBehaviour<CreationSystem>
{
    public GameObject createdObj;
    public int createdCount;
    public int createdObjTimes;
    //通过名字获得物品
    public Item GetItemByName(string itemName)
    {
        for (int i = 0; i < AtlasManager.Instance.itemAtlas.allItems.Length; i++)
        {
            if (AtlasManager.Instance.itemAtlas.allItems[i].tileName == itemName)
                return AtlasManager.Instance.itemAtlas.allItems[i];
        }
        return null;
    }

    private void Update()
    {
        Mouse mouse = Mouse.current;
        if (mouse.rightButton.wasPressedThisFrame && createdObj != null)
        {
            createdCount = 0;
            createdObjTimes = 0;
            Destroy(createdObj);
        }
    }
}
