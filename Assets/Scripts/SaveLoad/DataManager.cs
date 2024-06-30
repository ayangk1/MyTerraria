using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.IO;
using Newtonsoft.Json;

public class DataManager : SingletonMonoBehaviour<DataManager>
{
    private string jsonFolder;
    public VoidEventSO saveDataEvent;
    public VoidEventSO loadDataEvent;
    private List<ISaveable> saveableList = new List<ISaveable>();
    public Dictionary<string, Data> saveDataDict = new Dictionary<string, Data>();
    public List<string> spawnDataList = new List<string>();
    private Data saveData;
    public bool isLoadData;
    protected override void Awake()
    {
        saveData = new Data();
        jsonFolder = Application.persistentDataPath + "/SAVE/";
        base.Awake();
    }

    public void RegisterSaveData(ISaveable saveable)
    {
        if (!saveableList.Contains(saveable))
        {
            saveableList.Add(saveable);
        }
    }

    private void Update()
    {
        if (Keyboard.current.oKey.wasPressedThisFrame)
        {
            Load();
            //OnStartNewGame();
        }
            
    }

    private void OnEnable()
    {
        saveDataEvent.OnEventRaised += Save;
        loadDataEvent.OnEventRaised += Load;
    }

    private void OnDisable()
    {
        saveDataEvent.OnEventRaised -= Save;
        loadDataEvent.OnEventRaised -= Load;
    }

    public void UnRegisterSaveData(ISaveable saveable)
    {
        saveableList.Remove(saveable);
    }

    private void OnStartNewGame()
    {
        var resultPath = jsonFolder + "data.sav";
        if (File.Exists(resultPath))
        {
            File.Delete(resultPath);
        }
    }

    public void Save()
    {
        saveDataDict.Clear();
        foreach (var saveable in saveableList)
        {
            saveDataDict.Add(saveable.GetDataID().ID,saveable.GetSaveData(saveData));
        }
        var resultPath = jsonFolder + "data.sav";

        var jsonData = JsonConvert.SerializeObject(saveDataDict, Formatting.Indented);

        if (!File.Exists(resultPath))
        {
            Directory.CreateDirectory(jsonFolder);
        }
        
        File.WriteAllText(resultPath,jsonData);
        
    }

    public void Load()
    {
        var resultPath = jsonFolder + "data.sav";
        
        if (!File.Exists(resultPath)) return;

        var stringData = File.ReadAllText(resultPath);
        var jsonData = JsonConvert.DeserializeObject<Dictionary<string, Data>>(stringData);
        
        Debug.Log("Loaded Data");
        
        foreach (var saveable in saveableList)
        {
            saveable.LoadData(jsonData[saveable.GetDataID().ID]);
        }
    }
}
