using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "TerrainSetting",menuName = "Terraria/new TerrainSetting")]
public class TerrainSetting : ScriptableObject
{
    [HideInInspector]public Vector2Int WorldSize { get; private set; }
    [field:SerializeField]public int Seed { get; private set; }
    [field:SerializeField]public Vector2Int ChunkSize { get; private set; } // 区块大小
    [field:SerializeField]public int ChunkScale { get; private set; }
    [field:SerializeField]public float HeightAddition { get; private set; } // 基准高度
    [field:SerializeField]public float HeightMulti { get; private set; }
    [field:SerializeField,Range(0,1)] public float HeightScale { get; private set; }
    
    [field:SerializeField,Range(0,1)] public float CaveThreshold { get; private set; }
    [field:SerializeField,Range(0,1)] public float CaveScale { get; private set; }
    [field:SerializeField] public bool[,] CavePoints { get; private set; }
    [field:SerializeField] public OreClass[] Ores { get; private set; }
    
    [field:SerializeField,Range(0,1)] public float PlantsThreshold { get; private set; }
    [field:SerializeField,Range(0,1)] public float PlantsFrequency { get; private set; }
    [field:SerializeField,Range(0,1)] public float TreesFrequency { get; private set; }
    [field:SerializeField,Range(0,1)] public float TreesThreshold { get; private set; }

    public int[] heights;
    
    [field:SerializeField] public BiomeClass[] biomes { get; private set; }
    
    
    public void Init()
    {
        if (Seed == 0) Seed = Random.Range(-10000, 10000);
        
        Random.InitState(Seed);
        WorldSize = ChunkSize * ChunkScale;
        heights = new int[WorldSize.x];
        CavePoints = new bool[WorldSize.x, WorldSize.y];
    }
    
    public void InitCaves()
    {
        for (int x = 0; x < WorldSize.x; x++)
        {
            int height = GetHeight(x);
            for (int y = 0; y < height; y++)
            {
                float p = (float)y / height;
                float v = Mathf.PerlinNoise((x + Seed) * CaveScale, (y + Seed) * CaveScale);
                v /= 0.5f + p;
                CavePoints[x, y] = v >= CaveThreshold;
            }
        }
    }


    public int GetHeight(int x)
    {
        return (int)(HeightAddition + HeightMulti * Mathf.PerlinNoise((x + Seed) * HeightScale, Seed));
    }
}
