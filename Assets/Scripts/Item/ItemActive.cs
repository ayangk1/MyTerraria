using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Random = UnityEngine.Random;

public class ItemActive : MonoBehaviour
{
    private SpriteRenderer sprite;
    public Item item;
    public int count;
    public bool get;
    public bool main;

    private Vector2 lastPos;

    private void OnEnable()
    {
        count = 1;
        get = false;
        sprite = GetComponent<SpriteRenderer>();
        main = true;
        lastPos = transform.position;
    }

    public void Init(Item newitem,Vector3 pos)
    {
        ItemActive[] objs = FindObjectsByType<ItemActive>(FindObjectsInactive.Exclude,FindObjectsSortMode.None);
        foreach (ItemActive obj in objs)
            if(Vector2.Distance(obj.transform.position, transform.position) < 1.6f)
                main = false;

        StartCoroutine(Merge());
        
        transform.position = pos;
        GetComponent<Rigidbody2D>().AddForce(2 * Vector2.up,ForceMode2D.Impulse);
        item = newitem;
        
        sprite.sprite = newitem.image;
    }

    private IEnumerator RelocalPos()
    {
        if(Vector2.Distance(lastPos, transform.position) > 0.01f) main = false;
        yield return new WaitForSeconds(0.2f);
        lastPos = transform.position;
        StartCoroutine(Merge());
    }
    private IEnumerator Merge()
    {
        yield return new WaitForSeconds(1);
        main = true;
        StartCoroutine(RelocalPos());
    }

    public void ReturnPool()
    {
        ObjectPool.Instance.ReturnPool(transform.gameObject,0);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && !InventoryManager.Instance.IsInventoryFull(item) && !get)
        {
            transform.DOMove(other.transform.position, 0.1f).OnComplete(() => ReturnPool());
            get = true;
            ItemManager.Instance.GatherBegin(GetComponent<ItemActive>(),item,count);
        }
        if (other.gameObject.CompareTag("Item") && main)
        {
            if(other.GetComponent<ItemActive>().item == item)
            {
                other.GetComponent<ItemActive>().ReturnPool();
                count += other.GetComponent<ItemActive>().count;
            }
        }
    }
    
}
