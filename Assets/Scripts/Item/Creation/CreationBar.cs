using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CreationBar : SingletonMonoBehaviour<CreationBar>
{
    public CreationRecipes recipes;
    public string[] canCreation;
    public GameObject creationSlot;
    public GameObject creationRaw;
    public Transform creationRawBar;
    public CreationSlot curSlot;

    public bool StructChange
    {
        get {return structChange;}
        set 
        {
            structChange = value;
        }
    }
    private bool structChange;



    public List<CreatedStructureType> nearStructureTypes = new();


    public Transform player;
    private void Start()
    {
        nearStructureTypes.Add(CreatedStructureType.Null);
    }

    private void Update()
    {
        if (transform.childCount == 0)
            ClearRaw();
    }
    private void LateUpdate()
    {
        IsNearStruct();
    }

    public void ClearRaw()
    {
        for (int i = 0; i < creationRawBar.childCount; i++)
        {
            Destroy(creationRawBar.transform.GetChild(i).gameObject);
        }
    }
    
    //清理不创建的物体
    public void ClearNotCreation(Item item)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<CreationSlot>().item == item)
                ObjectPool.Instance.ReturnPool(transform.GetChild(i).gameObject,1);
        }
    }

    public void IsNearStruct()
    {
        if (player == null && GameManager.Instance.player == null) return;
        
        player = GameManager.Instance.player;
        bool[] temp = new bool[3];
        int range = 5;
        int nx1 = Mathf.Clamp(TerrainManager.Instance.tilemaps[0].WorldToCell(player.position).x - range, 0, TerrainManager.Instance.terrainSetting.WorldSize.x - 1);
        int nx2 = Mathf.Clamp(TerrainManager.Instance.tilemaps[0].WorldToCell(player.position).x + range, 0, TerrainManager.Instance.terrainSetting.WorldSize.x - 1);
        int ny1 = Mathf.Clamp(TerrainManager.Instance.tilemaps[0].WorldToCell(player.position).y - range, 0, TerrainManager.Instance.terrainSetting.WorldSize.y - 1);
        int ny2 = Mathf.Clamp(TerrainManager.Instance.tilemaps[0].WorldToCell(player.position).y + range, 0, TerrainManager.Instance.terrainSetting.WorldSize.y - 1);

        for (int i = 0; i < temp.Length; i++)
            temp[i] = false;
        
        for (int x = nx1; x < nx2; x++)
        for (int y = ny1; y < ny2; y++)
        {
            if (TerrainManager.Instance.tileDatas[0,x,y]?.structureType == CreatedStructureType.WorkBench)
            {
                if (!nearStructureTypes.Contains(CreatedStructureType.WorkBench))
                    nearStructureTypes.Add(CreatedStructureType.WorkBench);
                temp[0] = true;
            }
            if (TerrainManager.Instance.tileDatas[0,x,y]?.structureType == CreatedStructureType.Anvil)
            {
                if (!nearStructureTypes.Contains(CreatedStructureType.Anvil))
                    nearStructureTypes.Add(CreatedStructureType.Anvil);
                temp[1] = true;
            }
            if (TerrainManager.Instance.tileDatas[0,x,y]?.structureType == CreatedStructureType.Furnace)
            {
                if (!nearStructureTypes.Contains(CreatedStructureType.Furnace))
                    nearStructureTypes.Add(CreatedStructureType.Furnace);
                temp[2] = true;
            }
        }

        if (!temp[0]) 
        {
            if (nearStructureTypes.Contains(CreatedStructureType.WorkBench)) 
                nearStructureTypes.Remove(CreatedStructureType.WorkBench);
        }
            
        if (!temp[1]) 
        {
            if (nearStructureTypes.Contains(CreatedStructureType.Anvil)) 
                nearStructureTypes.Remove(CreatedStructureType.Anvil);
        }
            
        if (!temp[2]) 
        {
            if (nearStructureTypes.Contains(CreatedStructureType.Furnace)) 
                nearStructureTypes.Remove(CreatedStructureType.Furnace);
        }
            

        RefrashCreation();
    }

    public void RefrashRaw()
    {
        ClearRaw();
        for (int i = 0; i < recipes.recipes.Length; i++)
        {
            canCreation = recipes.recipes[i].Split('-');
            
            if (curSlot != null && curSlot.item == CreationSystem.Instance.GetItemByName(canCreation[0]))
            {
                for (int j = 2; j < canCreation.Length; j += 2)
                {
                    
                    GameObject obj = Instantiate(creationRaw, creationRawBar, true);
                    Image img = obj.transform.GetChild(0).GetComponent<Image>();
                    Text text = obj.transform.GetChild(1).GetComponent<Text>();
                    text.text = canCreation[j + 1];
                    img.sprite = CreationSystem.Instance.GetItemByName(canCreation[j]).image;
                    img.SetNativeSize();
                }
            }
        }
    }

    //判断是否有被选择的
    public bool IsHaveSelect()
    {
        bool haveSelected = false;
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<CreationSlot>().IsSelected)
                haveSelected = true; 
        }
        return haveSelected;
    }

    //创造前判断材料
    public bool PreCreation(Item item,int count)
    {
        for (int i = 0; i < recipes.recipes.Length; i++)
        {
            canCreation = recipes.recipes[i].Split('-');
            if (item.tileName == canCreation[0])
            {
                for (int j = 2; j < canCreation.Length ; j += 2)
                {
                    if (!InventoryManager.Instance.IsGetItem(canCreation[j], int.Parse(canCreation[j + 1]) * count))
                        return false;
                }
            }
        }
        return true;
    }
    
    public void RefrashCreation()
    {
        for (int i = 0; i < recipes.recipes.Length; i++)
        {
            canCreation = recipes.recipes[i].Split('-');
            int count = canCreation.Length / 2 - 1;
            bool enough = true;
            Item outItem = CreationSystem.Instance.GetItemByName(canCreation[0]);
            
            for (int j = 2; j < canCreation.Length ; j += 2)
            {
                if (!IsExistCreation(outItem) && InventoryManager.Instance.IsGetItem(canCreation[j], int.Parse(canCreation[j + 1])))
                    count--;
                if (!InventoryManager.Instance.IsGetItem(canCreation[j], int.Parse(canCreation[j + 1])))
                    enough = false;
            }

            if (count == 0 && !IsExistCreation(outItem) && nearStructureTypes.Contains(outItem.needStructureType)) 
            {
                //生成out);
                var obj = ObjectPool.Instance.GetFormPool(1);
                obj.transform.SetParent(transform);
                obj.GetComponent<CreationSlot>().Init(outItem,int.Parse(canCreation[1]));
            }
            else if (count == 0 && !IsExistCreation(outItem) && outItem.needStructureType == CreatedStructureType.Null) 
            {
                //生成out);
                var obj = ObjectPool.Instance.GetFormPool(1);
                obj.transform.SetParent(transform);
                obj.GetComponent<CreationSlot>().Init(outItem,int.Parse(canCreation[1]));
            }

            //如果存在需要创造的 
            if ((IsExistCreation(outItem) && !enough) || !nearStructureTypes.Contains(outItem.needStructureType))
            {
                //不够材料时清除生成
                if (curSlot != null && curSlot.item == outItem)
                    SlotDownByIndex(curSlot.Index);
                ClearNotCreation(outItem);
                bool haveSelected = IsHaveSelect();
                if(!haveSelected) ClickSlot(curSlot);
            }
            
            
        }
    }

    public void ClickSlot(CreationSlot slot)
    {
        if(slot == null) return;
        int step = slot.Index;
        var slotCount = transform.childCount;
        RectTransform[] slotsRect = new RectTransform[slotCount];
        for (int i = 0; i < slotCount; i++)
            slotsRect[i] = transform.GetChild(i).GetComponent<RectTransform>();
        
        for (int i = 0; i < slotCount; i++)
        {
            slotsRect[i].DOAnchorPosY(slotsRect[i].anchoredPosition.y - step * 70,0.2f);
            slotsRect[i].transform.GetComponent<CreationSlot>().Index -= step;
            transform.GetChild(i).GetComponent<CreationSlot>().IsSelected = false;
            transform.GetChild(i).GetComponent<Image>().sprite = AtlasManager.Instance.textureAtlas.inventoryNormal;
        }
        
        GetComponent<CreationBar>().RefrashRaw();
        slot.IsSelected = true;
    }
    
    //移动Index上方/下方slot
    public void MoveSlotByIndex(int index,bool up)
    {
        var slotCount = transform.childCount;
        RectTransform[] slots = new RectTransform[slotCount];
        for (int i = 0; i < slotCount; i++)
            slots[i] = transform.GetChild(i).GetComponent<RectTransform>();

        if (up)
        {
            for (int j = 0; j < transform.childCount; j++)
            {
                if (transform.GetChild(j).GetComponent<CreationSlot>().Index < index)
                {
                    slots[j].DOAnchorPosY(slots[j].anchoredPosition.y + 70,0.2f);
                    transform.GetChild(j).GetComponent<CreationSlot>().Index++;
                }   
            }
        }
        else
        {
            for (int j = 0; j < transform.childCount; j++)
            {
                if (transform.GetChild(j).GetComponent<CreationSlot>().Index > index)
                {
                    slots[j].DOAnchorPosY(slots[j].anchoredPosition.y - 70,0.2f);
                    transform.GetChild(j).GetComponent<CreationSlot>().Index--;
                }   
            }
        }
        
    }

    //物品消失时选择
    public void SlotDownByIndex(int newIndex)
    {
        if(transform.childCount < 1) return;
        int index = newIndex;
        var slotCount = transform.childCount;
        RectTransform[] slots = new RectTransform[slotCount];
        for (int i = 0; i < slotCount; i++)
            slots[i] = transform.GetChild(i).GetComponent<RectTransform>();
        if(transform.GetChild(0) != null)
            transform.GetChild(0).GetComponent<CreationSlot>().IsSelected = true;

        if (index > 0) MoveSlotByIndex(index, true);
        else if (index < 0) MoveSlotByIndex(index, false);
        else
        {
            bool bigger = false;
            for (int j = 0; j < transform.childCount; j++)
            {
                if (transform.GetChild(j).GetComponent<CreationSlot>().Index > 0)
                {
                    bigger = true;
                    break;
                }
            }
            MoveSlotByIndex(index, !bigger);
        }
    }

    public bool IsExistCreation(Item item)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<CreationSlot>().item == item)
                return true;
        }
        return false;
    }
}
