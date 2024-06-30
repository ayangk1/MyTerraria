using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public enum DayTimeState
{
    morning,//9     
    nooning,//15
    afternoon,//9
    night,//3
    evening,//0
}
//固定光源 移动光源 环境光
public class LightHandler : SingletonMonoBehaviour<LightHandler>
{
    public TerrainSetting terrainSettings;
    public TerrainManager terrainManager;
    public float[,] lightValues;
    [SerializeField]public float[,] sunLightValues;
    public bool[,] isFixLightValues;
    public float[,] moveLightValues;
    public readonly float MaxlightLevel = 15f;
    public float sunLight = 15f;
    private Texture2D lightTex;
    public Material lightMap;
    private static readonly int LightMap = Shader.PropertyToID("_LightMap");
    public Queue<Vector2Int> updates = new Queue<Vector2Int>();
    public bool updating;
    public float normalLightDeclineWall = 2f;
    public float normalLightDecline = 1f;
    public float fixLightDeclineWall = 2f;
    public float fixLightDecline = 1f;

    public float dayLength;
    public TimeSpan currTime;
    public event EventHandler<TimeSpan> WorldTimeChanged; //世界时间改变事件
    public float minuteLength => dayLength / 1440;

    private void OnEnable()
    {
        
        
        
    }

    private void Start()
    {
        
        
    }

    private bool isPerDay;

    private void OnWorldTimeChanged(object sender, TimeSpan e)
    {
        int range = 5;
        var playerPos = GameManager.Instance.player.position;
        int nx1 = Mathf.Clamp(terrainManager.tilemaps[0].WorldToCell(playerPos).x - (int)MaxlightLevel * range, 0, terrainSettings.WorldSize.x - 1);
        int nx2 = Mathf.Clamp(terrainManager.tilemaps[0].WorldToCell(playerPos).x + (int)MaxlightLevel * range, 0, terrainSettings.WorldSize.x - 1);
        int ny1 = Mathf.Clamp(terrainManager.tilemaps[0].WorldToCell(playerPos).y - (int)MaxlightLevel * range, 0, terrainSettings.WorldSize.y - 1);
        int ny2 = Mathf.Clamp(terrainManager.tilemaps[0].WorldToCell(playerPos).y + (int)MaxlightLevel * range, 0, terrainSettings.WorldSize.y - 1);
        
        for (int x = nx1; x < nx2; x++)
        for (int y = ny1; y < ny2; y++)
        {
            if (terrainManager.GetTileClass(1, x, y) == null && terrainManager.GetTileClass(2, x, y) == null)
            {
                if (PercentOfDay(e) >= 0.5f)
                    sunLightValues[x, y] = PercentOfHalfDay(e) * MaxlightLevel;
                else
                    sunLightValues[x, y] = (1 - PercentOfHalfDay(e)) * MaxlightLevel;
            }
                
        }
    }

    private float PercentOfHalfDay(TimeSpan timeSpan)
    {
        return (float)timeSpan.TotalMinutes % 720 / 720;
    }
    private float PercentOfDay(TimeSpan timeSpan)
    {
        return (float)timeSpan.TotalMinutes % 1440 / 1440;
    }


    private IEnumerator AddMinute()
    {
        currTime += TimeSpan.FromMinutes(1);
        
        WorldTimeChanged?.Invoke(this, currTime);



        // if (GameManager.Instance != null && GameManager.Instance.player != null)
        // {
            var playerPos = GameManager.Instance.player.position;
            RefrashLight(terrainManager.tilemaps[0].WorldToCell(playerPos).x, terrainManager.tilemaps[0].WorldToCell(playerPos).y, 
                normalLightDecline, normalLightDeclineWall, false,(int)MaxlightLevel * 5);
        // }
        
            
        
        
        
        yield return new WaitForSeconds(minuteLength);
        
        StartCoroutine(AddMinute());
    }
    
    public DayTimeState DayState
    {
        get { return dayState; }
        set
        {
            if (value != dayState)
            {
                switch (value)
                {
                    case DayTimeState.morning:
                    sunLight = 10f;
                    break;
                    case DayTimeState.nooning:
                    sunLight = 15f;
                    break;
                    case DayTimeState.afternoon:
                    sunLight = 10f;
                    break;
                    case DayTimeState.night:
                    sunLight = 5f;
                    break;
                    case DayTimeState.evening:
                    sunLight = 1f;
                    break;
                } 
            }
            dayState = value;
        }
    }
    [SerializeField]
    private DayTimeState dayState;

    public void Init()
    {
        lightValues = new float[terrainSettings.WorldSize.x, terrainSettings.WorldSize.y];
        sunLightValues = new float[terrainSettings.WorldSize.x, terrainSettings.WorldSize.y];
        moveLightValues = new float[terrainSettings.WorldSize.x, terrainSettings.WorldSize.y];
        isFixLightValues = new bool[terrainSettings.WorldSize.x, terrainSettings.WorldSize.y];
        lightTex = new Texture2D(terrainSettings.WorldSize.x, terrainSettings.WorldSize.y);
        transform.localScale = new Vector3((float)terrainSettings.WorldSize.x / 4, (float)terrainSettings.WorldSize.y / 4,1);
        transform.localPosition = new Vector3((float)terrainSettings.WorldSize.x / 8, (float)terrainSettings.WorldSize.y / 8, 0);
        lightMap.SetTexture(LightMap,lightTex);
        updates = new Queue<Vector2Int>();
        DayState = DayTimeState.nooning;
        dayLength = 1440;
        
        WorldTimeChanged += OnWorldTimeChanged;
        InitLight();
    }
    

    public void SwitchDayState(DayTimeState state)
    {
        DayState = state;
    }

    private bool begin;
    private void Update()
    {
        if (!begin && GameManager.Instance != null && GameManager.Instance.player != null)
        {
            StartCoroutine(AddMinute());
            begin = true;
        }
            
        if (!updating && updates.Count > 0)
        {
            updating = true;
            StartCoroutine(LightFixUpdate(updates.Dequeue()));
        }
        
    }
    public void InitLight()
    {
        for (int x = 0; x < terrainSettings.WorldSize.x; x++)
        for (int y = 0; y < terrainSettings.WorldSize.y; y++)
            sunLightValues[x, y] = sunLight;

        RefrashLight(0, 0, normalLightDecline, normalLightDeclineWall, true,0);
    }
    
    public void RefrshWay(int x,int y,float decline,float declineWall)
    {
        float lightValue;
        
        int nx1 = Mathf.Clamp(x - 1, 0, terrainSettings.WorldSize.x - 1);
        int nx2 = Mathf.Clamp(x + 1, 0, terrainSettings.WorldSize.x - 1);
        int ny1 = Mathf.Clamp(y - 1, 0, terrainSettings.WorldSize.y - 1);
        int ny2 = Mathf.Clamp(y + 1, 0, terrainSettings.WorldSize.y - 1);
        
        float max = Mathf.Max(lightValues[x ,ny1], lightValues[x ,ny2], lightValues[nx1 ,y], lightValues[nx2 ,y]);

        if (terrainManager.GetLightValue(x,y) != 0)//获得发光方块
            lightValue = terrainManager.GetLightValue(x, y);
        else
        {
            lightValue = max;
            
            if (terrainManager.GetTileClass(2,x,y) == null)
                lightValue -= decline;
            else
                lightValue -= declineWall;
        }
        
        lightValue = Mathf.Clamp(lightValue, 0f, MaxlightLevel);
        
        if (terrainManager.GetTileClass(2, x, y) == null && terrainManager.GetTileClass(1, x, y) == null 
                                                         && lightValue <= sunLightValues[x, y]) 
            lightValues[x, y] = sunLightValues[x, y];
        else
            lightValues[x, y] = lightValue;
        
            
    }
    
    public void RefrashLight(int cellx,int celly,float decline,float declineWall,bool isMap,int range)
    {
        if (isMap)
        {
            for (int x = 0; x < terrainSettings.WorldSize.x; x++)
            for (int y = 0; y < terrainSettings.WorldSize.y; y++)
            {
                if (isFixLightValues[x, y])
                {
                      RefrshWay(x, y, fixLightDecline, fixLightDeclineWall);
                }
                else
                    RefrshWay(x, y, decline, declineWall);
            }
            for (int x = terrainSettings.WorldSize.x - 1; x >= 0; x--)
            for (int y = terrainSettings.WorldSize.y - 1; y >= 0; y--)
                if (isFixLightValues[x, y])
                {
                    RefrshWay(x, y, fixLightDecline, fixLightDeclineWall);
                }
                else
                    RefrshWay(x, y, decline, declineWall);
            for (int x = 0; x < terrainSettings.WorldSize.x; x++)
            for (int y = 0; y < terrainSettings.WorldSize.y; y++)
                lightTex.SetPixel(x , y ,new Color(0,0,0, 1f - lightValues[x,y] / MaxlightLevel));
        }
        else
        {
            int px1 = Mathf.Clamp(cellx - range, 0, terrainSettings.WorldSize.x - 1);
            int px2 = Mathf.Clamp(cellx + range, 0, terrainSettings.WorldSize.x - 1);
            int py1 = Mathf.Clamp(celly - range, 0, terrainSettings.WorldSize.y - 1);
            int py2 = Mathf.Clamp(celly + range, 0, terrainSettings.WorldSize.y - 1);
            
            
            for (int x = px1; x <= px2; x++)
            for (int y = py1; y <= py2; y++)
                if (isFixLightValues[x, y])
                {
                    RefrshWay(x, y, fixLightDecline, fixLightDeclineWall);
                }
                else
                    RefrshWay(x, y, decline, declineWall);
            for (int x = px2; x >= px1; x--)
            for (int y = py2; y >= py1; y--)
                if (isFixLightValues[x,y])
                {
                    RefrshWay(x, y, fixLightDecline, fixLightDeclineWall);
                }
                else
                    RefrshWay(x, y, decline, declineWall);
            for (int x = px1; x <= px2; x++)
            for (int y = py1; y <= py2; y++)
                lightTex.SetPixel(x , y ,new Color(0,0,0, 1f - lightValues[x,y] / MaxlightLevel));
        }
        lightTex.Apply();
    }
    
    public void LightFixUpdate(int x,int y)
    {
        updates.Enqueue(new Vector2Int(x,y));
    }
    
    IEnumerator LightFixUpdate(Vector2Int pos)
    {
        int px1 = Mathf.Clamp(pos.x - (int)MaxlightLevel, 0, terrainSettings.WorldSize.x - 1);
        int px2 = Mathf.Clamp(pos.x + (int)MaxlightLevel, 0, terrainSettings.WorldSize.x - 1);
        int py1 = Mathf.Clamp(pos.y - (int)MaxlightLevel, 0, terrainSettings.WorldSize.y - 1);
        int py2 = Mathf.Clamp(pos.y + (int)MaxlightLevel, 0, terrainSettings.WorldSize.y - 1);
        
        for (int x = px1; x <= px2; x++)
        for (int y = py1; y <= py2; y++)
            isFixLightValues[x, y] = true;
        
        for (int x = pos.x; x <= px2; x++)
        for (int y = pos.y; y <= py2; y++)
            RefrshWay(x, y, fixLightDecline, fixLightDeclineWall);
        
        //左上
        for (int x = pos.x; x >= px1; x--)
        for (int y = pos.y; y <= py2; y++)
            RefrshWay(x, y, fixLightDecline, fixLightDeclineWall);
        
        
        //左下
        for (int x = pos.x; x >= px1; x--)
        for (int y = pos.y; y >= py1; y--)
            RefrshWay(x, y, fixLightDecline, fixLightDeclineWall);
        
        
        //右下
        for (int x = pos.x; x <= px2; x++)
        for (int y = pos.y; y >= py1; y--)
            RefrshWay(x, y, fixLightDecline, fixLightDeclineWall);
        
        
        for (int x = px1; x <= px2; x++)
        for (int y = py1; y <= py2; y++)
            lightTex.SetPixel(x, y, new Color(0, 0, 0, 1f - lightValues[x, y] / MaxlightLevel));
        
        lightTex.Apply();
        
        updating = false;
        yield return null;
        
    }
}
