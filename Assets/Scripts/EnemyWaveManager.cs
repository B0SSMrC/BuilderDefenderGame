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
    }

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
        state = State.WaitingToSpawnNextWave;
        spawnPosition = spawnPositionTransformList[UnityEngine.Random.Range(0,spawnPositionTransformList.Count)].position;
        nextWavePositionTransform.position = spawnPosition;
        nextWaveSpawnTimer = 3f;
        
    }

    private void Update()
    {
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
                        nextEnemySpawnTimer = UnityEngine.Random.Range(0f,.2f);
                        Enemy.Create(spawnPosition + UtilsClass.GetRandomDir() * UnityEngine.Random.Range(0f,10f));
                        remainingEnemySpawnAmount--;
                        if(remainingEnemySpawnAmount <= 0)
                        {
                            // 怪物全部生成完毕，进入等待下一波的状态
                            state = State.WaitingToSpawnNextWave;
                            spawnPosition = spawnPositionTransformList[UnityEngine.Random.Range(0,spawnPositionTransformList.Count)].position;
                            nextWavePositionTransform.position = spawnPosition;
                            nextWaveSpawnTimer = 10f;
                        }
                    }
                }
                break;
        }
    }

    private void SpawnWave()
    {
        
        
        remainingEnemySpawnAmount = 3 + 3 * waveNumber;
        state = State.SpawningWave;
        waveNumber++;
        OnWaveNumberChanged?.Invoke(this, EventArgs.Empty);
    }

    public int GetWaveNumber()
    {
        return waveNumber;
    }

    public float GetNextWaveSpawnTimer()
    {
        return nextWaveSpawnTimer;
    }

    public Vector3 GetSpawnPosition()
    {
        return spawnPosition;
    }

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
