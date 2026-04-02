using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private string savePath;

    private void Awake()
    {
        Instance = this;
        // 设定存档绝对路径
        savePath = Application.persistentDataPath + "/savegame.json";
    }

    private void Start()
    {
        // 检查主菜单通过 PlayerPrefs 传来的暗号
        // 默认值设为 0（新游戏），防止在编辑器里直接测试 GameScene 时报错
        if (PlayerPrefs.GetInt("ContinueGame", 0) == 1)
        {
            Debug.Log("收到指令：继续游戏，正在读取存档...");
            LoadGame();
        }
        else
        {
            Debug.Log("收到指令：新游戏，不进行数据读取。");
        }
    }

    private void Update()
    {
        // 测试快捷键：按 K 存档，按 L 读档
        if (Input.GetKeyDown(KeyCode.K)) SaveGame();
        if (Input.GetKeyDown(KeyCode.L)) LoadGame();
    }

    public void SaveGame()
    {
        GameSaveData data = new GameSaveData();

        // 1. 收集波次数据
        data.waveNumber = EnemyWaveManager.Instance.GetWaveNumber();
        data.nextWaveSpawnTimer = EnemyWaveManager.Instance.GetNextWaveSpawnTimer();
        data.nextSpawnPosition = EnemyWaveManager.Instance.GetSpawnPosition();

        // 2. 收集资源数据
        data.resourceSaveList = ResourceManager.Instance.GetResourceSaveData();

        // 3. 收集场上所有建筑数据 (利用 FindObjectsOfType 找到所有建筑)
        Building[] allBuildings = FindObjectsOfType<Building>();
        foreach (Building building in allBuildings)
        {
            BuildingTypeHolder typeHolder = building.GetComponent<BuildingTypeHolder>();
            HealthSystem health = building.GetComponent<HealthSystem>();

            if (typeHolder != null && health != null)
            {
                data.buildingSaveList.Add(new BuildingSaveData
                {
                    buildingTypeName = typeHolder.buildingType.name,
                    position = building.transform.position,
                    currentHealth = health.GetHealthAmount()
                });
            }
        }

        // 转换为 JSON 并写入本地
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log("游戏已保存至: " + savePath);
    }

    public void LoadGame()
    {
        if (!File.Exists(savePath))
        {
            Debug.Log("未找到存档，开始新游戏。");
            return;
        }

        string json = File.ReadAllText(savePath);
        GameSaveData data = JsonUtility.FromJson<GameSaveData>(json);

        // 1. 恢复波次数据
        EnemyWaveManager.Instance.LoadWaveData(data.waveNumber, data.nextWaveSpawnTimer, data.nextSpawnPosition);

        // 2. 恢复资源数据
        ResourceManager.Instance.LoadResourceData(data.resourceSaveList);

        // 3. 恢复建筑数据
        RestoreBuildings(data.buildingSaveList);

        Debug.Log("读档成功！");
        // 通知玩家读取成功
        TooltipUI.Instance.Show("加载存档成功", new TooltipUI.TooltipTimer{timer = 2f});
    }

    private void RestoreBuildings(List<BuildingSaveData> savedBuildings)
    {
        // 获取场景中预先摆好的大本营
        Building hqBuilding = BuildingManager.Instance.GetHQBuilding();

        // 1. 清理当前场上的所有老建筑 (保留大本营)
        Building[] currentBuildings = FindObjectsOfType<Building>();
        foreach (Building b in currentBuildings)
        {
            // 如果是大本营，直接跳过，千万别销毁！
            if (b == hqBuilding)
            {
                continue; 
            }
            Destroy(b.gameObject); 
        }

        BuildingTypeListSO buildingTypeList = Resources.Load<BuildingTypeListSO>(typeof(BuildingTypeListSO).Name);

        // 2. 根据存档重新生成建筑或更新状态
        foreach (BuildingSaveData bData in savedBuildings)
        {
            BuildingTypeSO typeSO = buildingTypeList.list.Find(so => so.name == bData.buildingTypeName);
            if (typeSO != null)
            {
                // 【核心修改】：判断这份存档数据是不是大本营的
                // (假设大本营身上也挂了 BuildingTypeHolder，且类型名字对得上)
                if (hqBuilding != null && hqBuilding.GetComponent<BuildingTypeHolder>().buildingType.name == bData.buildingTypeName)
                {
                    // 这是大本营的数据！不要 Instantiate，只更新血量！
                    HealthSystem hqHealth = hqBuilding.GetComponent<HealthSystem>();
                    if (hqHealth != null)
                    {
                        hqHealth.SetHealthAmount(bData.currentHealth);
                    }
                }
                else
                {
                    // 这是普通建筑（塔、采集器），正常生成并赋值血量
                    Transform newBuilding = Instantiate(typeSO.prefab, bData.position, Quaternion.identity);
                    
                    HealthSystem health = newBuilding.GetComponent<HealthSystem>();
                    if (health != null)
                    {
                        health.SetHealthAmount(bData.currentHealth);
                    }
                }
            }
        }
    }

    // 退出游戏时自动保存
    private void OnApplicationQuit()
    {
        SaveGame();
    }
}
