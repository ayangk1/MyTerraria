using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ItemManager : SingletonMonoBehaviour<ItemManager>
{
    public float gatherInterval = 0.2f;
    private Dictionary<Item,int> tempGatherDic = new Dictionary<Item,int>();

    public void GatherBegin(ItemActive itemActive,Item item,int count)
    {
        if(!tempGatherDic.ContainsKey(item))
        {
            tempGatherDic.Add(item, count);
            StartCoroutine(Gather(itemActive));
        }
        else
            tempGatherDic[item] += count;
        
    }

    private IEnumerator Gather(ItemActive itemActive)
    {
        yield return new WaitForSeconds(gatherInterval);
        foreach(var item in tempGatherDic.Keys)
        {
            InventoryManager.Instance.AddItemByCount(item,tempGatherDic[item]);

            GameObject go = ObjectPool.Instance.GetFormPool(5);
            go.transform.SetParent(UIManager.Instance.canvas.transform);
            go.GetComponent<ItemActivePrompt>().Init(itemActive.transform.position);
            go.GetComponent<TextMeshProUGUI>().text = item.chineseName + '(' + tempGatherDic[item] + ')';
        }
        tempGatherDic.Clear();
    }

    public void HoldTorch()
    {
        var cellPoint = TerrainManager.Instance.tilemaps[2].WorldToCell(GameObject.FindWithTag("Player").transform.position);
       // LightHandler.Instance.MovingLight(cellPoint.x,cellPoint.y,AtlasManager.Instance.tileAtlas.torch.lightLevel);
    }
    
    public void UnHoldTorch()
    {
        var cellPoint = TerrainManager.Instance.tilemaps[2].WorldToCell(GameObject.FindWithTag("Player").transform.position);
    //    LightHandler.Instance.MovingLight(cellPoint.x,cellPoint.y,0);
    }
    
    
}
