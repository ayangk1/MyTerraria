using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

//Addons,Background,Ground,Liquid
public class TerrainManager : SingletonMonoBehaviour<TerrainManager>,ISaveable
{
    
    public TerrainSetting terrainSetting;
    public TileClass[,,] tileDatas;
    public Tilemap[] tilemaps;
    public TileAtlas tileAtlas;
    public GameObject BackGround;
    public bool isGenerateOver;
    public string[] biomeName; 
    public float[,,] demagedCondition;

    public TileBase highline;
    public Vector3Int lastHighline;

    private void OnEnable()
    {
        Init();
        
    }

    private void OnDisable()
    {
        ISaveable saveable = this;
        saveable.UnRegisterSaveData();
    }

    public void Init()
    {
        ISaveable saveable = this;
        saveable.RegisterSaveData();
        terrainSetting.Init();
        terrainSetting.InitCaves();
        tileDatas = new TileClass[5, terrainSetting.WorldSize.x, terrainSetting.WorldSize.y];
        biomeName = new string[terrainSetting.biomes.Length];
        demagedCondition = new float[5, terrainSetting.WorldSize.x, terrainSetting.WorldSize.y];
        Generate();
        InitBackGround();
        
        isGenerateOver = true;
    }
    
    
    public void InitBackGround()
    {
        for (float x = 0; x < terrainSetting.WorldSize.x / 2f; x += 20.3f)
        {
            GameObject go = Instantiate(BackGround, new Vector3(x, 33, 0), Quaternion.identity,GameObject.Find("BackGround").transform);
            if (go.transform.position.x > terrainSetting.WorldSize.x)
                break;
        }
    }

    private void Generate()
    {
        List<BiomeClass> biomes = new List<BiomeClass>();
        for (int i = 0; i < terrainSetting.biomes.Length; i++)
        {
            biomes.Insert(Random.Range(0,biomes.Count),terrainSetting.biomes[i]);
        }

        for (int i = 0; i < biomes.Count; i++)
        {
            if (biomes[i] == terrainSetting.biomes[0])
            {
                BiomeClass biome = biomes[i];
                biomes.RemoveAt(i);
                biomes.Insert((biomes.Count + 1)/ 2,biome);
                break;
            }
        }

        for (int i = 0; i < biomes.Count; i++)
        {
            biomeName[i] = biomes[i].biomeName;
        }
        
        int[] biomeLengths = new int[terrainSetting.biomes.Length];
        BiomeClass[] chunkBiomes= new BiomeClass[terrainSetting.ChunkSize.x];
        int start = 0;
        for (int i = 0; i < biomeLengths.Length; i++)
        {
            biomeLengths[i] = terrainSetting.ChunkSize.x / biomeLengths.Length;
            if (biomes[i] == terrainSetting.biomes[0])
            {
                biomeLengths[i] += terrainSetting.ChunkSize.x % biomeLengths.Length;
            }

            for (int j = 0; j < biomeLengths[i]; j++)
            {
                chunkBiomes[start + j] = biomes[i];
            }

            start += biomeLengths[i];
        }
        
        
        //生成tile
        for (int x = 0; x < terrainSetting.WorldSize.x; x++)
        {
            BiomeClass biome = chunkBiomes[x / terrainSetting.ChunkScale];
            
            
            int height = terrainSetting.GetHeight(x);
            terrainSetting.heights[x] = height;
            for (int y = 0; y < height; y++)
            {
                TileClass tileToPlace;
                if (y > height - Random.Range(3, 5))
                    tileToPlace = biome.tileAtlas.surfaceBlock;
                else if (y > height -30)
                    tileToPlace = biome.tileAtlas.underSurfaceBlock;
                else
                    tileToPlace = biome.tileAtlas.stoneBlock;

                //生成矿物
                foreach (var ore in biome.ores)
                {
                    if (Mathf.PerlinNoise((x + ore.Offset) * ore.OreFrequency,(y + ore.Offset) * ore.OreFrequency) < ore.OreRadius)
                    {
                        tileToPlace = ore;
                        break;
                    }
                }

                if (!terrainSetting.CavePoints[x,y])
                {
                    PlaceTile(tileToPlace, x, y,tileToPlace.coverRange);
                }
                    
                
                //生成墙壁
                if (y > height - 40 && y < height - 2)
                    PlaceTile(biome.tileAtlas.surfaceWall,x,y,biome.tileAtlas.surfaceWall.coverRange);
                else if (y <= height - 40)
                    PlaceTile(biome.tileAtlas.stoneWall,x,y,biome.tileAtlas.stoneWall.coverRange);
            }
        }
        //生成植物
        for (int x = 0; x < terrainSetting.WorldSize.x; x++)
        {
            BiomeClass biome = chunkBiomes[x / 16];
            int height = terrainSetting.GetHeight(x);
            for (int y = 0; y < height; y++)
            {
                if (y == height - 1 && (tileDatas[(int)Layers.Ground,x,y] == biome.tileAtlas.surfaceBlock 
                                        || tileDatas[(int)Layers.Ground,x,y] == biome.tileAtlas.underSurfaceBlock))
                {
                    if (Mathf.PerlinNoise((x + terrainSetting.Seed) * biome.plantsFrequncy,
                            (y + terrainSetting.Seed) * biome.plantsFrequncy) > biome.plantsThreshold)
                    {
                        PlaceTile(biome.tileAtlas.plants,x,y+1,biome.tileAtlas.plants.coverRange);
                    }
                    else if (Mathf.PerlinNoise((x + terrainSetting.Seed) * biome.treeFrequncy,
                                 (y + terrainSetting.Seed) * biome.treeFrequncy) > biome.treeThreshold)
                    {
                        if (x > 2 && tileDatas[(int)Layers.Addons,x - 2 ,terrainSetting.GetHeight(x - 2)] != biome.tileAtlas.tree 
                                  && tileDatas[(int)Layers.Addons,x - 1 ,terrainSetting.GetHeight(x - 1)] != biome.tileAtlas.tree
                                  && tileDatas[(int)Layers.Addons,x - 3 ,terrainSetting.GetHeight(x - 3)] != biome.tileAtlas.tree)
                        {
                            SpawnTree(x , y + 1,biome);
                        }
                    }
                }
            }
        }
        
        //光照
        LightHandler.Instance.Init();
        isGenerateOver = true;
        
    }

    public void HighTile(int x, int y)
    {
        tilemaps[4].SetTile(lastHighline, null);
        lastHighline = new Vector3Int(x,y);
        tilemaps[4].SetTile(new Vector3Int(x, y, 0), highline);
    }

    public void PlaceTile(TileClass tileClass, int x, int y,Vector2Int size)
    {
        if (x < 0 || x >= terrainSetting.WorldSize.x || y < 0 || y >= terrainSetting.WorldSize.y) return;

        if (tileClass.layer == Layers.Ground) RangeRemoveTile(0, x , y,size);
        
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                tilemaps[(int)tileClass.layer].SetTile(new Vector3Int(x + i, y + j, 0), tileClass.tile);
                demagedCondition[(int)tileClass.layer,x + i,y + j] = 0;
                tileDatas[(int)tileClass.layer, x + i, y + j] = tileClass;
            }
        }
        
        if (tileClass is LiquidClass @class)
            StartCoroutine(@class.CalculatePhysics(x, y));
        if (tileClass.isIlluminate)
            LightHandler.Instance.LightFixUpdate(x,y);
    }
    
    public bool JudgePlaceTile(TileClass tileClass,int x,int y,Vector2Int size)
    {
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                if (tileDatas[(int)Layers.Ground, x + i, y + j] != null || tileDatas[(int)tileClass.layer,x + i, y+ j] != null)
                    return false;
                
                if (tileClass.torch && tileDatas[(int)Layers.Wall,  x + i, y + j] != null 
                                                                        && tileDatas[(int)Layers.Addons,  x + i, y + j ] == null)
                    return true;
                if (tileClass.torch && tileDatas[(int)Layers.Wall,  x + i, y + j] == null 
                                                                        && tileDatas[(int)Layers.Addons,  x + i, y + j ] == null
                                                                        && tileDatas[(int)Layers.Ground,  x - 1 + i, y + j ] != null)
                    return true;

                if (tileClass.layer == Layers.Ground && tileDatas[(int)Layers.Ground, x + 1 + i, y + j] == null 
                                                     && tileDatas[(int)Layers.Ground, x - 1 + i, y + j] == null
                                                     && tileDatas[(int)Layers.Ground, x + i, y + 1 + j] == null
                                                     && tileDatas[(int)Layers.Ground, x + i, y - 1 + j] == null)
                    return false;
            }
        }
        return true;
    }

    public void RangeRemoveTile(int layer,int x,int y,Vector2Int size)
    {
        if (x < 0 || x >= terrainSetting.WorldSize.x || y < 0 || y >= terrainSetting.WorldSize.y) return;
        
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                tilemaps[layer].SetTile(new Vector3Int(x + i, y + j, 0), null);
                tileDatas[layer, x + i, y + j] = null;
            }
        }
    }
    
    //范围删除
    public void RangeRemoveTile(TileClass tileClass, int x, int y,Vector2Int size)
    {
        if (x < 0 || x >= terrainSetting.WorldSize.x || y < 0 || y >= terrainSetting.WorldSize.y) return;

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                tilemaps[(int)tileClass.layer].SetTile(new Vector3Int(x + i, y + j, 0), null);
                if (tileClass.isIlluminate)
                {
                    tileDatas[(int)tileClass.layer, x + i, y + j] = null;
                    LightHandler.Instance.LightFixUpdate(x + i, y + j);
                }
                tileDatas[(int)tileClass.layer, x + i, y + j] = null;
            }
        }

        int rate = Random.Range(0, 100);
        if (tileClass.droprate >= rate)
            GenerateItem(tileClass, 0, x, y);
        
        ResetNearTile(tileClass, x, y);
    }
    //Load
    public void LoadRemoveTile(TileClass tileClass,int x, int y)
    {
        if (x < 0 || x >= terrainSetting.WorldSize.x || y < 0 || y >= terrainSetting.WorldSize.y) return;

        tilemaps[(int)tileClass.layer].SetTile(new Vector3Int(x , y, 0), null);
        tileDatas[(int)tileClass.layer, x, y] = null;
    }
    
    public void StructureRemoveTile(TileClass tileClass,int x, int y)
    {
        if (x < 0 || x >= terrainSetting.WorldSize.x || y < 0 || y >= terrainSetting.WorldSize.y) return;

        tilemaps[(int)tileClass.layer].SetTile(new Vector3Int(x , y, 0), null);
        tileDatas[(int)tileClass.layer, x, y] = null;
        
        ResetNearTile(tileClass, x, y);
    }
    
    public void GenerateItem(TileClass tileClass,int PoolIndex,int cell_x,int cell_y)
    {
        var obj = ObjectPool.Instance.GetFormPool(PoolIndex);
        var worldPos = tilemaps[0].CellToWorld(new Vector3Int(cell_x, cell_y, 0));
        obj.transform.GetComponentInChildren<ItemActive>().Init(tileClass.item,new Vector3(worldPos.x + 0.1f,worldPos.y + 0.1f,0) );
    }
    
    //连带删除
    public void CollapseRemoveTile(TileClass tileClass, int x, int y)
    {
        if (x < 0 || x >= terrainSetting.WorldSize.x || y < 0 || y >= terrainSetting.WorldSize.y) return;

        tilemaps[(int)tileClass.layer].SetTile(new Vector3Int(x , y, 0), null);
        tileDatas[(int)tileClass.layer, x , y] = null;
        
        int rate = Random.Range(0, 100);
        if (tileClass.droprate >= rate)
            GenerateItem(tileClass, 0, x, y);
        
        ResetNearTile(tileClass, x, y);
    }

    

    public TileClass GetTileClass(int layer,int x,int y)
    {
        if (x < 0 || x >= terrainSetting.WorldSize.x || y < 0 || y >= terrainSetting.WorldSize.y) return null;
        return tileDatas[layer,x,y];
    }
    
    //处理临近Tile
    public void ResetNearTile(TileClass tileClass,int x, int y)
    {
        if (tileDatas[0, x, y + 1] == tileAtlas.Plants) RangeRemoveTile(tileAtlas.Plants, x, y + 1,tileAtlas.Plants.coverRange);
        
        if (!tileClass.isCollapse) return;
        //上方
        if (tileDatas[0, x, y + 1] == tileClass && tileClass.structure)
            StructureRemoveTile(tileClass, x, y + 1 );
        else if (tileDatas[0, x, y + 1] == tileClass)
            CollapseRemoveTile(tileClass, x, y + 1 );
        
        //下方
        if (tileDatas[0, x, y - 1] == tileClass && tileClass.structure)
            StructureRemoveTile(tileClass, x, y - 1 );
        else if (tileDatas[0, x, y - 1] == tileClass)
            CollapseRemoveTile(tileClass, x, y - 1);
        
        //左方
        if (tileDatas[0, x - 1, y] == tileClass && tileClass.structure)
            StructureRemoveTile(tileClass, x - 1, y );
        else if (tileDatas[0, x - 1, y] == tileClass)
            CollapseRemoveTile(tileClass, x - 1, y);
        
        //右方
        if (tileDatas[0, x + 1, y] == tileClass && tileClass.structure)
            StructureRemoveTile(tileClass, x + 1, y );
        else if (tileDatas[0, x + 1, y] == tileClass)
            CollapseRemoveTile(tileClass, x + 1, y);
    }
    
    public bool HaveTile(int x,int y)
    {
        if (x < 0 || x >= terrainSetting.WorldSize.x || y < 0 || y >= terrainSetting.WorldSize.y) return false;

        int p = 0;
        for (int i = 0; i < 4; i++)
            if (tileDatas[i, x, y] == null) p++;
        if (p == 4)
            return false;
        return true;
    }

    public float GetLightValue(int x,int y )
    {
        float lightValue = 0;
        for (int i = 0; i < tileDatas.GetLength(0); i++)
        {
            if (tileDatas[i, x, y] == null)
                continue;

            if (tileDatas[i, x, y].lightLevel > lightValue)
                lightValue = tileDatas[i, x, y].lightLevel;
        }
        return lightValue;
    }
    
    public void SpawnTree(int x, int y,BiomeClass biome)
    {
        if (x < 0 || x >= terrainSetting.WorldSize.x || y < 0 || y >= terrainSetting.WorldSize.y) return;

        int h = Random.Range(biome.treeHeight.x, biome.treeHeight.y);
        int maxBranches = Random.Range(3, 10);
        int bCounts = 0;

        for (int ny = y; ny < y + h; ny++)
        {
            PlaceTile(biome.tileAtlas.tree,x,ny,biome.tileAtlas.tree.coverRange);

            if (ny == y)
            {
                if (Random.Range(0,100) < 30)
                {
                    if (x > 0 && tileDatas[(int)Layers.Ground,x - 1, ny - 1] != null && tileDatas[(int)Layers.Ground,x - 1, ny] == null)
                    {
                        PlaceTile(biome.tileAtlas.tree,x - 1, ny,biome.tileAtlas.tree.coverRange);
                    } 
                }
                if (Random.Range(0,100) < 30)
                {
                    if (x < terrainSetting.WorldSize.x - 1 && tileDatas[(int)Layers.Ground,x + 1, ny - 1] != null && tileDatas[(int)Layers.Ground,x + 1, ny] == null)
                    {
                        PlaceTile(biome.tileAtlas.tree,x + 1, ny,biome.tileAtlas.tree.coverRange);
                    }
                }
            }
            else if (ny >= y + 2 && ny <= y + h -3)
            {
                if (bCounts < maxBranches && Random.Range(0,100) < 40)
                {
                    if (x > 0 && tileDatas[(int)Layers.Addons,x - 1, ny ] == null && tileDatas[(int)Layers.Addons,x - 1, ny - 1] != biome.tileAtlas.tree)
                    {
                        PlaceTile(biome.tileAtlas.tree,x - 1, ny,biome.tileAtlas.tree.coverRange);
                        bCounts++;
                    }
                }
                if (bCounts < maxBranches && Random.Range(0,100) < 40)
                {
                    if (x < terrainSetting.WorldSize.x - 1 && tileDatas[(int)Layers.Addons,x + 1, ny ] == null 
                                                           && tileDatas[(int)Layers.Addons,x + 1, ny - 1] != biome.tileAtlas.tree)
                    {
                        PlaceTile(biome.tileAtlas.tree,x + 1, ny,biome.tileAtlas.tree.coverRange);
                        bCounts++;
                    }
                }
            } 
        }
    }

    public DataDefination GetDataID()
    {
        return GetComponent<DataDefination>();
    }

    public Data GetSaveData(Data data)
    {
        object[,,] tileObj = new object[4, terrainSetting.WorldSize.x, terrainSetting.WorldSize.y];
        
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < terrainSetting.WorldSize.x; j++)
            {
                for (int k = 0; k < terrainSetting.WorldSize.y; k++)
                {
                    tileObj[i, j, k] = tileDatas[i, j, k]?.item.tileName;
                }
            }
        }

        data.object3SaveData[GetDataID().ID + "tile"] = tileObj;
        
        return data;
    }

    public void LoadData(Data data)
    {
        object[,,] tileObj = new object[4, terrainSetting.WorldSize.x, terrainSetting.WorldSize.y];
        
        if (data.object3SaveData.ContainsKey(GetDataID().ID + "tile"))
            tileObj = data.object3SaveData[GetDataID().ID + "tile"];
        
        for (int i = 0; i < 4; i++)
        {
            for (int x = 0; x < terrainSetting.WorldSize.x; x++)
            {
                for (int y = 0; y < terrainSetting.WorldSize.y; y++)
                {
                    if (tileDatas[i, x, y] == null && tileObj[i, x, y] != null)
                    {
                        
                        PlaceTile(CreationSystem.Instance.GetItemByName(Convert.ToString(tileObj[i, x, y])).buildingTile
                            ,x,y,CreationSystem.Instance.GetItemByName(Convert.ToString(tileObj[i, x, y])).buildingTile.coverRange);
                    }
                    else if (tileDatas[i, x, y] != null && tileObj[i, x, y] == null)
                    {
                        LoadRemoveTile(tileDatas[i, x, y],x,y);
                    }
                }
            }
        }
        
        
        
    }
}
