using System.Collections.Generic;
using UnityEngine;

public class Data
{
    public Dictionary<string, bool> stringSaveData = new Dictionary<string, bool>();
    public Dictionary<string, float> floatSaveData = new Dictionary<string, float>();
    public Dictionary<string, object[]> objectSaveData = new Dictionary<string,object[]>();
    public Dictionary<string, object[,,]> object3SaveData = new Dictionary<string,object[,,]>();
    public List<EnemyData> enemiesSaveData = new List<EnemyData>();
    public PlayerData playerSaveData;
    public int enemyCount;
}

public class PlayerData
{
    public float x;
    public float y;
    public float z;
    public float health;

    public PlayerData(float m_x,float m_y,float m_z,float m_health)
    {
        x = m_x;
        y = m_y;
        z = m_z;
        health = m_health;
    }
}

public class EnemyData
{
    public string name;
    public float x;
    public float y;
    public float z;
    public float health;

    public EnemyData(string m_name,float m_x,float m_y,float m_z,float m_health)
    {
        name = m_name;
        x = m_x;
        y = m_y;
        z = m_z;
        health = m_health;
    }
}
