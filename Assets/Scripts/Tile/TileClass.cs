using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "TileClass",menuName = "Terraria/new TileClass")]
public class TileClass : ScriptableObject
{
    public TileBase tile;
    public Layers layer;
    public int blockID;
    public Item item;
    public float lightLevel;
    public CreatedStructureType structureType;
    public bool structure;
    public bool tree;
    public bool torch;
    [Range(0,100)]public int droprate;
    [Tooltip("是否可覆盖")]public bool isCover;
    [Tooltip("占用格数")]public Vector2Int coverRange = Vector2Int.one;
    [Tooltip("是否连续")] public bool isCollapse;
    [Tooltip("是否发光")] public bool isIlluminate;
}
