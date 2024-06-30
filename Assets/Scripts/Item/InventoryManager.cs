using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : SingletonMonoBehaviour<InventoryManager>,ISaveable
{
    public ItemEventSO addItemEventSO;//添加item事件
    
    public int inventorySize;
    public GameObject inventoryBar;
    public InventorySlot[] inventorySlots;
    public PrefabsAtlas prefabsAtlas;
    public ItemAtlas itemAtlas;

    private void OnEnable()
    {
        ISaveable saveable = this;
        saveable.RegisterSaveData();
        //addItemEventSO.OnEventRaised += OnAddItem;
        
    }

    private void OnDisable()
    {
        ISaveable saveable = this;
        saveable.UnRegisterSaveData();
        //addItemEventSO.OnEventRaised -= OnAddItem;
    }

    private void Start()
    {
        inventorySlots = new InventorySlot[inventorySize];
        for (int i = 0; i < inventorySize; i++)
        {
            var obj = Instantiate(prefabsAtlas.inventorySlot, inventoryBar.transform);
            inventorySlots[i] = obj.GetComponent<InventorySlot>();
            obj.GetComponent<InventorySlot>().index = i;
            if (i > 8) obj.SetActive(false);
        }

        inventorySlots[0].IsSelected = true;
        
        AddItem(itemAtlas.sword);
        AddItem(itemAtlas.pickax);
        AddItem(itemAtlas.axe);
        AddItem(itemAtlas.Hammer);
        AddItem(itemAtlas.torchItem);
        AddItemByCount(itemAtlas.torchItem,20);
    }

    public void AddItemToPos(Item item,Transform toSlot)
    {
        InventoryItem itemInSlot = toSlot.GetComponentInChildren<InventoryItem>();
        if (itemInSlot == null)
        {
            SpawnNewItem(item,toSlot.GetComponent<InventorySlot>(), CreationSystem.Instance.createdCount);
            CreationBar.Instance.RefrashCreation();
        }
        else
        {
            if (item.stackable && itemInSlot.item == item)
            {
                itemInSlot.Count += CreationSystem.Instance.createdCount;
                itemInSlot.RefreshCount();
                Storage.Instance.items[itemInSlot.item] = itemInSlot.Count;
                CreationBar.Instance.RefrashCreation();
            }
        }
    }

    public void OnAddItem(Item item)
    {
        
    }
    
    public void RemoveCreatedItemRaw(Item createdItem)
    {
        for (int i = 0; i < CreationBar.Instance.recipes.recipes.Length; i++)
        {
            var canCreation = CreationBar.Instance.recipes.recipes[i].Split('-');
            
            if (CreationSystem.Instance.GetItemByName(canCreation[0]) == createdItem)
            {
                for (int j = 2; j < canCreation.Length; j += 2)
                {
                    if (CreationSystem.Instance.GetItemByName(canCreation[j]).stackable)
                    {
                        for (int k = 0; k < inventorySlots.Length; k++)
                        {
                            InventorySlot slot = inventorySlots[k];
                            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
                            if (itemInSlot != null && itemInSlot.item == CreationSystem.Instance.GetItemByName(canCreation[j]))
                            {
                                itemInSlot.Count -= CreationSystem.Instance.createdCount;
                                Storage.Instance.items[itemInSlot.item] = itemInSlot.Count;
                                itemInSlot.RefreshCount();
                                if (Storage.Instance.items[itemInSlot.item] == 0) Storage.Instance.items.Remove(itemInSlot.item);
                                CreationBar.Instance.RefrashCreation();
                            }
                        }
                    }
                    else
                    {
                        for (int k = 0; k < inventorySlots.Length; k++)
                        {
                            InventorySlot slot = inventorySlots[k];
                            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
                            if (itemInSlot != null)
                            {
                                Storage.Instance.items.Remove(itemInSlot.item);
                                CreationBar.Instance.RefrashCreation();
                            }
                        }
                    }
                }
            }
        }
        
    }

    public void RefrashAllItem()
    {
        for (int i = 0; i < inventorySize; i++)
            if (inventorySlots[i].GetItemInSlot() != null)
                inventorySlots[i].GetItemInSlot().RefreshCount();
    }
    
    public void AddItem(Item item)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null && item.stackable && itemInSlot.item == item)
            {
                itemInSlot.Count++;
                itemInSlot.RefreshCount();
                Storage.Instance.items[itemInSlot.item] = itemInSlot.Count;
                CreationBar.Instance.RefrashCreation();
                return;
            }
        }
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot == null)
            {
                SpawnNewItem(item,slot,1);
                CreationBar.Instance.RefrashCreation();
                return;
            }
        }
    }

    public void AddItemByCount(Item item,int count)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null && item.stackable && itemInSlot.item == item)
            {
                itemInSlot.Count += count;
                itemInSlot.RefreshCount();
                Storage.Instance.items[itemInSlot.item] = itemInSlot.Count;
                CreationBar.Instance.RefrashCreation();
                return;
            }
        }
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot == null)
            {
                SpawnNewItem(item,slot,count);
                CreationBar.Instance.RefrashCreation();
                return;
            }
        }
    }

    /// <summary>
    /// 是否能够创造物体
    /// </summary>
    public bool IsGetItem(string itemName, int count)
    {
        for (int i = 0; i < Storage.Instance.items.Count; i++)
        {
            if (Storage.Instance.items.ContainsKey(CreationSystem.Instance.GetItemByName(itemName)) && 
                Storage.Instance.items[CreationSystem.Instance.GetItemByName(itemName)] >=  count)
                return true;
        }
        return false;
    }

    public bool IsInventoryFull(Item item)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot == null || (itemInSlot != null && item.stackable && itemInSlot.item == item))
                return false;
        }
        return true;
    }

    public void SpawnNewItem(Item item, InventorySlot slot,int count)
    {
        GameObject newItem = Instantiate(prefabsAtlas.inventoryItem, slot.transform);
        InventoryItem inventoryItem = newItem.GetComponent<InventoryItem>();
        inventoryItem.InitItem(item,count);
        if (!Storage.Instance.items.TryAdd(item, count))
            Storage.Instance.items[item] += count;
    }

    public DataDefination GetDataID()
    {
        return GetComponent<DataDefination>();
    }

    public Data GetSaveData(Data data)
    {
        object[] itemNameObjs = new object[inventorySlots.Length];
        object[] intObjs = new object[inventorySlots.Length];
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            itemNameObjs[i] = inventorySlots[i].GetComponentInChildren<InventoryItem>()?.item.tileName;
            intObjs[i] = inventorySlots[i].GetComponentInChildren<InventoryItem>()?.Count;
        }
        data.objectSaveData[GetDataID().ID + "item"] = itemNameObjs;
        data.objectSaveData[GetDataID().ID + "int"] = intObjs;
        
        return data;
    }

    public void LoadData(Data data)
    {
        object[] itemNameObjs = new object[inventorySlots.Length];
        object[] intObjs = new object[inventorySlots.Length];
        if (data.objectSaveData.ContainsKey(GetDataID().ID + "item"))
            itemNameObjs = data.objectSaveData[GetDataID().ID + "item"];
        if (data.objectSaveData.ContainsKey(GetDataID().ID + "int"))
            intObjs = data.objectSaveData[GetDataID().ID + "int"];

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (itemNameObjs[i] != null && CreationSystem.Instance.GetItemByName(Convert.ToString(itemNameObjs[i])) != null 
                                        && inventorySlots[i].transform.childCount == 0)
            {
                SpawnNewItem(CreationSystem.Instance.GetItemByName(Convert.ToString(itemNameObjs[i]))
                    , inventorySlots[i],  Convert.ToInt32(intObjs[i]));
            }
                
        }
    }
}
