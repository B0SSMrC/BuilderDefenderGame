using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField] 
public class GameSaveData 
{
    [Header("波次与敌人数据")]
    public int waveNumber;
    public float nextWaveSpawnTimer;
    public Vector3 nextSpawnPosition;

    [Header("资源数据")]
    // 因为不能存 Dictionary，用 List 来存键值对
    public List<ResourceSaveData> resourceSaveList = new List<ResourceSaveData>();

    [Header("建筑数据")]
    public List<BuildingSaveData> buildingSaveList = new List<BuildingSaveData>();
}

[System.Serializable]
public class ResourceSaveData
{
    public string resourceName; // 使用 ResourceTypeSO 的名字作为唯一ID
    public int amount;
}

[System.Serializable]
public class BuildingSaveData
{
    public string buildingTypeName; // 使用 BuildingTypeSO 的名字
    public Vector3 position;
    public int currentHealth;
}
