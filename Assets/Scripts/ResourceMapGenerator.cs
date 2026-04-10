using System.Collections.Generic;
using UnityEngine;

public class ResourceMapGenerator : MonoBehaviour
{
    [System.Serializable]
    public class ResourceSpawnConfig
    {
        public string resourceName;      
        public GameObject resourcePrefab; 
        
        [Header("矿脉簇 (Cluster) 聚集设置")]
        public int clusterCount = 3; 
        public int minResourcesPerCluster = 6;
        public int maxResourcesPerCluster = 8;
        public float clusterRadius = 3f; 
        public float spawnRadius = 1.0f; 
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

    private void Start()
    {
        GenerateResources();
    }

    private void GenerateResources()
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