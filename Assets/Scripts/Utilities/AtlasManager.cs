using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtlasManager : SingletonMonoBehaviour<AtlasManager>
{
    public ItemAtlas itemAtlas;
    public PrefabsAtlas prefabsAtlas;
    public TextureAtlas textureAtlas;
    public TileAtlas tileAtlas;
    public EnemyAtlas enemyAtlas;

    public Enemy GetEnemyByName(string m_name)
    {
        for (int i = 0; i < enemyAtlas.enemies.Length; i++)
        {
            if (enemyAtlas.enemies[i].m_name == m_name)
            {
                return enemyAtlas.enemies[i];
            }
        }

        return null;
    }
}
