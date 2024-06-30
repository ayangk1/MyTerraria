using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemAtlas",menuName = "Atlas/ItemAtlas")]
public class ItemAtlas : ScriptableObject
{
    public Item[] allItems;
    [Header("Weapon")]
    public Item sword;
    public Item pickax;
    public Item axe;
    public Item Hammer;
    [Header("Build")]
    public Item dirtBlock;
    [Header("FallItem")]
    public Item gelItem;

    public Item torchItem;
}
