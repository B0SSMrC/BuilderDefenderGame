using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance{get;private set;}

    public event EventHandler OnResurceAmountChanged;


    [SerializeField] private List<ResourceAmount> startingResourceAmountList;
    private Dictionary<ResourceTypeSO,int> resourceAmountDictionary;

    private void Awake()
    {
        Instance = this;
        resourceAmountDictionary = new Dictionary<ResourceTypeSO, int>();

        ResourceTypeListSO resourceTypeList = Resources.Load<ResourceTypeListSO>(typeof(ResourceTypeListSO).Name);


        foreach(ResourceTypeSO resourceType in resourceTypeList.list)
        {
            resourceAmountDictionary[resourceType] = 0;
        }

        foreach(ResourceAmount resourceAmount in startingResourceAmountList)
        {
            AddResource(resourceAmount.resourceType,resourceAmount.amount);
        }
    }

    

    public void AddResource(ResourceTypeSO resourceType,int amount)
    {
        resourceAmountDictionary[resourceType] += amount;

        OnResurceAmountChanged?.Invoke(this,EventArgs.Empty);

        
    }

    public int GetResourceAmount(ResourceTypeSO resourceType)
    {
        return resourceAmountDictionary[resourceType];
    }

    public bool CanAfford(ResourceAmount[] resourceAmountArray)
    {
        foreach(ResourceAmount resourceAmount in resourceAmountArray)
        {
            if(GetResourceAmount(resourceAmount.resourceType) >= resourceAmount.amount)
            {
                
            }
            else
            {
                return false;
            }
        }
        return true;
    }

    public void SpendResources(ResourceAmount[] resourceAmountArray)
    {
        foreach(ResourceAmount resourceAmount in resourceAmountArray)
        {
            resourceAmountDictionary[resourceAmount.resourceType] -= resourceAmount.amount;
            
        }
         
    }

    //导出资源用于存档
    public List<ResourceSaveData> GetResourceSaveData()
    {
        List<ResourceSaveData> saveList = new List<ResourceSaveData>();
        foreach (var kvp in resourceAmountDictionary)
        {
            saveList.Add(new ResourceSaveData { resourceName = kvp.Key.name, amount = kvp.Value });
        }
        return saveList;
    }

    // 【新增】读档时覆盖资源数据
    public void LoadResourceData(List<ResourceSaveData> saveList)
    {
        ResourceTypeListSO resourceTypeList = Resources.Load<ResourceTypeListSO>(typeof(ResourceTypeListSO).Name);
        
        foreach (ResourceSaveData saveData in saveList)
        {
            // 通过名字找到对应的 SO
            ResourceTypeSO typeSO = resourceTypeList.list.Find(so => so.name == saveData.resourceName);
            if (typeSO != null)
            {
                resourceAmountDictionary[typeSO] = saveData.amount;
            }
        }
        OnResurceAmountChanged?.Invoke(this, EventArgs.Empty);
    }
}
