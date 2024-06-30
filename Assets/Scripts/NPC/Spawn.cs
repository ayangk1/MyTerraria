using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class Spawn : SingletonMonoBehaviour<Spawn>,ISaveable
{
    public CinemachineVirtualCamera v_camera;
    public GameObject player;
    public VoidEventSO loadDataEvent;
    [Range(0, 100)]
    public float interval = 10;
    public int spawnIndex;

    private void OnEnable()
    {
        ISaveable saveable = this;
        saveable.RegisterSaveData();
        spawnIndex = 0;
        interval = 10;
        if (TerrainManager.Instance.isGenerateOver && !DataManager.Instance.isLoadData)
        {
            for (int i = 0; i < TerrainManager.Instance.biomeName.Length; i++)
            {
                if (TerrainManager.Instance.biomeName[i] == "normal")
                {
                    var x = Random.Range(i * TerrainManager.Instance.terrainSetting.ChunkSize.x
                        , (i + 1) * TerrainManager.Instance.terrainSetting.ChunkSize.x);
                    var y = TerrainManager.Instance.terrainSetting.ChunkSize.y;
                    var obj = Instantiate(player, new Vector3(x, y+1, 0), quaternion.identity, null);
                    SceneManager.MoveGameObjectToScene(obj, SceneManager.GetSceneAt(SceneManager.loadedSceneCount - 1));
                    v_camera.m_Follow = obj.transform;
                    break;
                }
            }
        }
        StartCoroutine(SpawnEnemy());
        
        if (DataManager.Instance.isLoadData)
            loadDataEvent.RaiseEvent();
    }

    private void OnDisable()
    {
        ISaveable saveable = this;
        saveable.UnRegisterSaveData();
    }

    private IEnumerator SpawnEnemy()
    {
        yield return new WaitForSeconds(interval);
        for (int i = 0; i < TerrainManager.Instance.biomeName.Length; i++)
        {
            if (TerrainManager.Instance.biomeName[i] == "normal")
            {
                var x = Random.Range(i * TerrainManager.Instance.terrainSetting.ChunkSize.x
                    , (i + 1) * TerrainManager.Instance.terrainSetting.ChunkSize.x);
                var y = TerrainManager.Instance.terrainSetting.ChunkSize.y;;
            
                var obj = ObjectPool.Instance.GetFormPool(4);
                obj.transform.position = new Vector3(x, y + 1, 0);
                
                obj.GetComponent<DataDefination>().ID += "/" + obj.GetComponent<Enemy>().m_name + "/" + spawnIndex;
                spawnIndex++;
                break;
            }
        }
        StartCoroutine(SpawnEnemy());
    }

    public DataDefination GetDataID()
    {
        return GetComponent<DataDefination>();
    }

    public Data GetSaveData(Data data)
    {
        var playerPos = GameObject.FindWithTag("Player").transform.position;
        PlayerData playerData = new PlayerData(playerPos.x, playerPos.y, playerPos.z,
            GameObject.FindWithTag("Player").GetComponent<Character>().currHealth);
        data.playerSaveData = playerData;
        
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsInactive.Exclude,FindObjectsSortMode.None);
        data.enemyCount = enemies.Length;
        foreach (var value in enemies)
        {
            var pos = value.transform.position;
            EnemyData enemyData = new EnemyData(value.m_name, pos.x, pos.y, pos.z,
                value.GetComponent<Character>().currHealth);
            data.enemiesSaveData.Add(enemyData);
        }
        return data;
    }

    public void LoadData(Data data)
    {
        PlayerData playerData = data.playerSaveData;
        var playerObj = Instantiate(player, new Vector3(playerData.x,playerData.y,playerData.z)
            , quaternion.identity, null);
        playerObj.GetComponent<Character>().currHealth = playerData.health;
        SceneManager.MoveGameObjectToScene(playerObj, SceneManager.GetSceneAt(SceneManager.loadedSceneCount - 1));
        v_camera.m_Follow = playerObj.transform;
        
        for (int i = 0; i < data.enemyCount; i++)
        {
            if (data.enemiesSaveData[i].name == "slimeUmbrella")
            {
                EnemyData enemyData = data.enemiesSaveData[i];
                var obj = ObjectPool.Instance.GetFormPool(4);
                obj.transform.position = new Vector3(enemyData.x,enemyData.y,enemyData.z);
                obj.GetComponent<Character>().currHealth = enemyData.health;
            }
            
        }
    }
}
