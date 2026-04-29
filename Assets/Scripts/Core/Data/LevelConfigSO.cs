using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TowerDefense/Level Configuration", fileName = "NewLevelConfig")]
public class LevelConfigSO : ScriptableObject
{
    [Header("关卡基础设置")]
    public string levelName = "默认关卡";
    public float initialWaitTime = 3f; // 游戏开始前给玩家的准备时间

    [Header("波次详细配置")]
    public List<WaveData> waves;
}

[System.Serializable]
public class WaveData
{
    [Tooltip("本波次要生成的敌人总数")]
    public int enemyCount;
    
    [Tooltip("每个敌人生成的间隔时间(秒)")]
    public float spawnInterval;
    
    [Tooltip("打完这波后，距离下一波开始的休息时间(秒)")]
    public float timeToNextWave;

}
