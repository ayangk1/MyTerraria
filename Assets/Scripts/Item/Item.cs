using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public enum ItemType
{
    Interact,
    BuildItem,
    Material,
    Other
}

public enum CreatedStructureType
{
    Null,
    WorkBench,
    Anvil,
    Furnace
}

[CreateAssetMenu(fileName = "Item",menuName = "Item/new Item")]
public class Item : ScriptableObject
{
    public TileClass tile;
    public TileClass buildingTile;
    public string tileName;
    public string chineseName;
    public Sprite image;
    public ItemType type;
    public CreatedStructureType needStructureType;
    public Vector2Int range = new Vector2Int(5, 4);
    public bool stackable;
    public GameObject prefab;
    public bool isPlace;
    

}
