using System.Collections.Generic;
using UnityEngine;

public class ResourceMapGenerator : MonoBehaviour
{
    public static ResourceMapGenerator Instance { get; private set; }
    [System.Serializable]
    public class ResourceSpawnConfig
    {
        public string resourceName;      
        public GameObject resourcePrefab; 
        
        [Header("矿脉簇 (Cluster) 聚集设置")]
        public int clusterCount = 3; //簇数量
        public int minResourcesPerCluster = 6;//每簇最少资源数量
        public int maxResourcesPerCluster = 8;//每簇最多资源数量
        public float clusterRadius = 3f;//簇半径，资源会在这个范围内随机分布
        public float spawnRadius = 1.0f; //资源本身的碰撞半径（用于防重叠检测）
    }

    [Header("生成区域设置")]
    public Vector2 mapCenter = Vector2.zero;
    public Vector2 mapSize = new Vector2(50f, 50f);

    [Header("大本营避让设置")]
    [Tooltip("大本营周边的保护半径，资源不会生成在这个范围内")]
    public float hqSafeRadius = 10f; 

    [Header("碰撞检测设置")]
    public LayerMask obstacleLayer;

    [Header("资源配置")]
    public List<ResourceSpawnConfig> spawnConfigs;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (!SaveManager.IsLoadingGame)
        {
            GenerateResources();
        }
    }

    public void GenerateResources()
    {
        foreach (var config in spawnConfigs)
        {
            if (config.resourcePrefab == null) continue;

            for (int i = 0; i < config.clusterCount; i++)
            {
                // 1. 获取矿堆中心点
                Vector2 clusterCenter = GetValidClusterCenter();
                
                int resourcesInThisCluster = Random.Range(config.minResourcesPerCluster, config.maxResourcesPerCluster + 1);
                int spawnedAmount = 0;
                int attempts = 0;
                int maxAttempts = resourcesInThisCluster * 20;

                while (spawnedAmount < resourcesInThisCluster && attempts < maxAttempts)
                {
                    attempts++;

                    Vector2 randomOffset = Random.insideUnitCircle * config.clusterRadius;
                    Vector2 spawnPosition = clusterCenter + randomOffset;

                    // 2. 核心判定：不仅要检测地图边界，还要检测是否在 HQ 避让范围内
                    if (!IsPositionInsideMap(spawnPosition) || IsPositionInSafeZone(spawnPosition))
                    {
                        continue;
                    }

                    // 3. 物理防重叠检测
                    Collider2D overlappingCollider = Physics2D.OverlapCircle(spawnPosition, config.spawnRadius, obstacleLayer);

                    if (overlappingCollider == null)
                    {
                        Instantiate(config.resourcePrefab, spawnPosition, Quaternion.identity, transform);
                        spawnedAmount++;
                    }
                }
            }
        }
    }

    // 获取一个不在避让区内的矿堆中心点
    private Vector2 GetValidClusterCenter()
    {
        Vector2 pos;
        int attempts = 0;
        do
        {
            float randomX = Random.Range(mapCenter.x - mapSize.x / 2f, mapCenter.x + mapSize.x / 2f);
            float randomY = Random.Range(mapCenter.y - mapSize.y / 2f, mapCenter.y + mapSize.y / 2f);
            pos = new Vector2(randomX, randomY);
            attempts++;
        } while (IsPositionInSafeZone(pos) && attempts < 100);

        return pos;
    }

    public void RestoreResourceNodes(List<ResourceNodeSaveData> savedNodes)
    {
        // 清空当前所有资源节点（包括 Start 时可能已经生成的）
        ResourceNode[] existing = GetComponentsInChildren<ResourceNode>();
        foreach (var node in existing)
        {
            Destroy(node.gameObject);
        }

        // 按存档逐个还原
        foreach (var data in savedNodes)
        {
            GameObject prefab = GetPrefabByResourceName(data.resourceTypeName);
            if (prefab != null)
            {
                Instantiate(prefab, data.position, Quaternion.identity, transform);
            }
        }
    }

    private GameObject GetPrefabByResourceName(string resourceName)
    {
        foreach (var config in spawnConfigs)
        {
            if (config.resourceName == resourceName)
            {
                return config.resourcePrefab;
            }
        }
        return null;
    }

    // 检查是否在避让范围内
    private bool IsPositionInSafeZone(Vector2 position)
    {
        return Vector2.Distance(position, mapCenter) < hqSafeRadius;
    }

    // 检查是否在地图范围内
    private bool IsPositionInsideMap(Vector2 position)
    {
        return position.x >= mapCenter.x - mapSize.x / 2f && position.x <= mapCenter.x + mapSize.x / 2f &&
               position.y >= mapCenter.y - mapSize.y / 2f && position.y <= mapCenter.y + mapSize.y / 2f;
    }

    private void OnDrawGizmosSelected()
    {
        // 绘制地图边界
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(mapCenter, mapSize);

        // 绘制大本营避让范围（黄色圆圈）
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(mapCenter, hqSafeRadius);
    }
}