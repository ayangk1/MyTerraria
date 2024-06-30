using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "BiomeTileAtlas",menuName = "Terraria/BiomeTileAtlas")]
public class BiomeTileAtlas : ScriptableObject
{
    public TileClass surfaceBlock;
    public TileClass underSurfaceBlock;
    public TileClass stoneBlock;
    public TileClass surfaceWall;
    public TileClass stoneWall;
    public TileClass plants;
    public TileClass tree;
}
