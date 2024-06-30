using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "LiquidClass",menuName = "Terraria/LiquidClass")]
public class LiquidClass : TileClass
{
    [field:SerializeField]public float FlowSpeed { get; private set; }

    public IEnumerator CalculatePhysics(int x, int y)
    {
        yield return new WaitForSeconds(1f / FlowSpeed);

        if (y - 1 >= 0 && TerrainManager.Instance.GetTileClass(2,x,y - 1) == null 
                       && TerrainManager.Instance.GetTileClass(3,x,y - 1) == null)
        {
            TerrainManager.Instance.PlaceTile(this,x,y - 1,this.coverRange);
        }
        else
        {
            if(x - 1 >= 0 && TerrainManager.Instance.GetTileClass(2,x - 1,y) == null 
                          && TerrainManager.Instance.GetTileClass(3,x - 1,y) == null)
            {
                TerrainManager.Instance.PlaceTile(this,x - 1,y,coverRange);
            }
        
            if(x + 1 <= TerrainManager.Instance.terrainSetting.WorldSize.x && TerrainManager.Instance.GetTileClass(2,x + 1,y) == null 
                                                                           && TerrainManager.Instance.GetTileClass(3,x + 1,y) == null)
            {
                TerrainManager.Instance.PlaceTile(this,x + 1,y,coverRange);
            }
        }

        
        
        
    }
    
    
    
}
