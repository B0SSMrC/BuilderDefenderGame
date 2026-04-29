using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWaveManager : MonoBehaviour
{   
    public static EnemyWaveManager Instance{get;private set;}
    public event EventHandler OnWaveNumberChanged;
    private enum State
    {
        WaitingToSpawnNextWave,
        SpawningWave,
        LevelCompleted //完成当前波次
    }
    [Header("关卡数据配置")]
    public LevelConfigSO currentLevelConfig;
    [Header("生成点配置")]
    [SerializeField] private List<Transform> spawnPositionTransformList;
    [SerializeField] private Transform nextWavePositionTransform;

    private State state;
    private int waveNumber = 0;
    private float nextWaveSpawnTimer;
    private float nextEnemySpawnTimer;
    private int remainingEnemySpawnAmount;
    private Vector3 spawnPosition;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        if (currentLevelConfig == null)
        {
            Debug.LogError("错误：没有为 EnemyWaveManager 配置 LevelConfigSO！");
            return;
        }
        state = State.WaitingToSpawnNextWave;
        spawnPosition = GetRandomSpawnPosition();
        nextWavePositionTransform.position = spawnPosition;
        nextWaveSpawnTimer = currentLevelConfig.initialWaitTime;
        
    }

    private void Update()
    {
        if(currentLevelConfig == null) return;
        switch (state)
        {
            case State.WaitingToSpawnNextWave:
                nextWaveSpawnTimer -= Time.deltaTime;
                if(nextWaveSpawnTimer < 0f)
                {
                    SpawnWave();
                }
                break;
            case State.SpawningWave:
                if(remainingEnemySpawnAmount > 0)
                {
                    nextEnemySpawnTimer -= Time.deltaTime;
                    if(nextEnemySpawnTimer < 0f)
                    {
                        //读取当前波次生成间隔
                        float currentSpawnInterval = currentLevelConfig.waves[waveNumber -1].spawnInterval;
                        nextEnemySpawnTimer = currentSpawnInterval;
                        Enemy.Create(spawnPosition + UtilsClass.GetRandomDir() * UnityEngine.Random.Range(0f,10f));
                        remainingEnemySpawnAmount--;
                        if(remainingEnemySpawnAmount <= 0)
                        {
                            PrepareNextWave();
                        }
                    }
                }
                break;

            case State.LevelCompleted:
                //TODO:出发胜利UI
                break;
        }
    }

    private void SpawnWave()
    {
        
        
        remainingEnemySpawnAmount = currentLevelConfig.waves[waveNumber].enemyCount;
        state = State.SpawningWave;
        waveNumber++;
        OnWaveNumberChanged?.Invoke(this, EventArgs.Empty);
    }

    private void PrepareNextWave()
    {
        if(waveNumber >= currentLevelConfig.waves.Count)
        {
            Debug.Log("所有波次生成完毕！等待场上怪物被清空即可胜利！");
            state = State.LevelCompleted;
            return;
        }

        nextWaveSpawnTimer = currentLevelConfig.waves[waveNumber - 1].timeToNextWave;

        state = State.WaitingToSpawnNextWave;
        spawnPosition = GetRandomSpawnPosition();
        nextWavePositionTransform.position = spawnPosition;
    }

    private Vector3 GetRandomSpawnPosition()
    {
        return spawnPositionTransformList[UnityEngine.Random.Range(0, spawnPositionTransformList.Count)].position;
    }


    public int GetWaveNumber() => waveNumber;
    public float GetNextWaveSpawnTimer() => nextWaveSpawnTimer;
    public Vector3 GetSpawnPosition() => spawnPosition;

    //用于读档时覆盖波次状态
    public void LoadWaveData(int waveNum, float nextTimer,Vector3 spawnPos)
    {
        this.waveNumber = waveNum;
        this.nextWaveSpawnTimer = nextTimer;
        this.spawnPosition = spawnPos;
        this.nextWavePositionTransform.position = spawnPosition;
        
        // 强制重置为等待状态，避免读档时处于刷怪中导致逻辑错乱
        state = State.WaitingToSpawnNextWave; 
        OnWaveNumberChanged?.Invoke(this, EventArgs.Empty);
    }
}
