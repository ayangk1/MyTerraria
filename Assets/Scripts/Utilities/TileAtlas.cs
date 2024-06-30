using UnityEngine;

[CreateAssetMenu(fileName = "TileAtlas",menuName = "Terraria/TileAtlas")]
public class TileAtlas : ScriptableObject
{
    [field:SerializeField]public TileClass GrassBlock { get; private set; }
    [field:SerializeField]public TileClass DirtBlock { get; private set; }
    [field:SerializeField]public TileClass StoneBlock { get; private set; }
    
    [field:SerializeField]public TileClass DirtWall { get; private set; }
    
    [field:SerializeField]public TileClass StoneWall { get; private set; }
    
    [field:SerializeField]public TileClass Plants { get; private set; }
    [field:SerializeField]public TileClass Trees { get; private set; }
    
    [field:SerializeField]public TileClass Water { get; private set; }
    [field:SerializeField]public TileClass torch { get; private set; }
}
