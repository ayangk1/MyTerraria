using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyAtlas",menuName = "Atlas/EnemyAtlas")]
public class EnemyAtlas : ScriptableObject
{
    public Enemy[] enemies; 
    
    public SlimeUmbrella slimeUmbrella;
}
