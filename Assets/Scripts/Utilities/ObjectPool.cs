using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PoolPrefab
{
    Item,
    CreationSlot,
    ItemAttack
}
public class ObjectPool : SingletonMonoBehaviour<ObjectPool>
{
    private Dictionary<int, Queue<GameObject>> m_Pool;
    // 0-Item 1-CreationSlot
    public GameObject[] prefab;
    public GameObject[] prefabParent;
    public int objCount;

    private void OnEnable()
    {
        prefabParent = new GameObject[prefab.Length];
        m_Pool = new Dictionary<int, Queue<GameObject>>();
        FillPool(-1);//为-1时则为初始化
    }

    public void FillPool(int index)
    {
        if (index < -1)return;
        if (index == -1)
        {
            for (int i = 0; i < prefab.Length; i++)
            {
                if (prefabParent[i] == null)
                {
                    prefabParent[i] = new GameObject(prefab[i].name + "Parent");
                    prefabParent[i].transform.SetParent(transform);
                    m_Pool.Add(i,new Queue<GameObject>());
                }
                
                for (int j = 0; j < objCount; j++)
                {
                    var obj = Instantiate(prefab[i], prefabParent[i].transform,true);
                    ReturnPool(obj,i);
                }
            }
        }
        else
        {
            for (int j = 0; j < objCount; j++)
            {
                var obj = Instantiate(prefab[index], prefabParent[index].transform,true);
                ReturnPool(obj,index);
            }
        }
        
        
        
    }

    public void ReturnPool(GameObject obj,int index)
    {
        if (m_Pool.ContainsKey(index))
        {
            obj.SetActive(false);
            obj.transform.SetParent(prefabParent[index].transform);
            m_Pool[index].Enqueue(obj);
        }
            
    }

    public GameObject GetFormPool(int index)
    {
        if (m_Pool[index].Count == 0) FillPool(index);
        
        var outObj = m_Pool[index].Dequeue();
        outObj.SetActive(true);
        outObj.transform.parent = null;

        return outObj;
    }
    
}
