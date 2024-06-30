using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISaveable
{
    DataDefination GetDataID();
    public void RegisterSaveData() => DataManager.Instance.RegisterSaveData(this);
    public void UnRegisterSaveData() => DataManager.Instance.UnRegisterSaveData(this);
    Data GetSaveData(Data data);
    void LoadData(Data data);
}
