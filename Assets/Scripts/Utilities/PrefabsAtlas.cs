using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "PrefabsAtlas",menuName = "Atlas/PrefabsAtlas")]
public class PrefabsAtlas : ScriptableObject
{
    [Header("Inventory")] 
    public GameObject inventoryItem;
    public GameObject inventorySlot;

    [Header("Creation")] 
    public GameObject creationItem;
    public GameObject creationRaw;
    public GameObject creationSlot;
    
    [Header("Item")] 
    public GameObject pickupItem;
    public GameObject attackItem;

    [Header("Enemy")] 
    public GameObject slimeUmbrella;

}
