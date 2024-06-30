using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TimeMap : SingletonMonoBehaviour<TimeMap>
{
    public TerrainSetting terrainSettings;
    public TerrainManager terrainManager;
    public float[,] lightValues;
    public readonly float sunlight = 15f;
    private Texture2D timeLightTex;
    public Material timeMap;
    private int TimeMaps = Shader.PropertyToID("_TimeMap");
    public Queue<Vector2Int> updates = new Queue<Vector2Int>();
    public bool updating;

    public void Init()
    {
        lightValues = new float[terrainSettings.WorldSize.x, terrainSettings.WorldSize.y];
        timeLightTex = new Texture2D(terrainSettings.WorldSize.x, terrainSettings.WorldSize.y - (int)terrainSettings.HeightAddition);
        transform.localScale = new Vector3((float)terrainSettings.WorldSize.x / 4, (float)terrainSettings.WorldSize.y / 4,1);
        transform.localPosition = new Vector3((float)terrainSettings.WorldSize.x / 8, (float)terrainSettings.WorldSize.y / 8, 0);
        timeMap.SetTexture(TimeMaps,timeLightTex);
        InitTimeLight();
    }
    
    public void InitTimeLight()
    {
        
        for (int x = 0; x < terrainSettings.WorldSize.x; x++)
        {
            for (int y = 0; y < terrainSettings.WorldSize.y; y++)
            {
                lightValues[x, y] = sunlight;
            }
        }
        for (int x = 0; x < terrainSettings.WorldSize.x; x++)
        {
            for (int y = terrainSettings.WorldSize.y - 10; y < terrainSettings.WorldSize.y; y++)
            {
                lightValues[x, y] = 0;
            }
        }

        lightValues[0, terrainSettings.heights[0]] = 0;
        Debug.Log((int)(terrainSettings.HeightAddition * terrainSettings.HeightMulti) + ":" + terrainSettings.HeightMulti
        +":"+ terrainSettings.WorldSize.x +":"+ terrainSettings.WorldSize.y +":"+ terrainSettings.heights[0]);

        for (int x = 0; x < terrainSettings.WorldSize.x; x++)
            for (int y = 0; y < terrainSettings.WorldSize.y; y++)
                timeLightTex.SetPixel(x , y ,new Color(0,0,0, 1f - lightValues[x,y] / sunlight));
        timeLightTex.Apply();
    }
}
