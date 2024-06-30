using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "CustomTile",menuName = "Terraria/new CustomTile")]
public class CustomTile : RuleTile<CustomTile.Neighbor>
{
    public TileBase[] specifiedBlocks;
    public class Neighbor : RuleTile.TilingRule.Neighbor
    {
        public const int Any = 3;
        public const int Specified = 4;
        public const int NotSpecified = 5;
        public const int Air = 6;
        public const int NotThisOrAir = 7;
    }
    
    
    
    public override bool RuleMatch(int neighbor, TileBase other)
    {
        if (neighbor == 3)
        {
            return CheckAny(other);
        }
        if (neighbor == 4)
        {
            return CheckSpecified(other);
        }
        if (neighbor == 5)
        {
            return CheckNotSpecified(other);
        }
        if (neighbor == 6)
        {
            return CheckAir(other);
        }
        if (neighbor == 7)
        {
            return CheckNotThisOrAir(other);
        }
        
        return base.RuleMatch(neighbor, other);
    }
    
    private bool CheckNotThisOrAir(TileBase other)
    {
        
        if (other != this || other == null)
        {
            return true;
        }
        return false;
    }

    private bool CheckSpecified(TileBase other)
    {
        if (specifiedBlocks.Contains(other))
        {
            return true;
        }
        return false;
    }
    private bool CheckAny(TileBase other)
    {
        if (specifiedBlocks.Contains(other) || other == this)
        {
            return true;
            
        }
        return false;
    }
    private bool CheckNotSpecified(TileBase other)
    {
        if (!specifiedBlocks.Contains(other) || other == this)
        {
            return true;
        }
        return false;
    }
    private bool CheckAir(TileBase other)
    {
        if (other == null)
        {
            return true;
        }
        return false;
    }
}
