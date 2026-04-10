using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    public static EnemyPool Instance{get;private set;}

    private Queue<Enemy> pool = new Queue<Enemy>();

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// 从对象池中获取一个敌人
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public Enemy Get(Vector3 position)
    {
        Enemy enemy;

        if(pool.Count > 0)
        {
            enemy = pool.Dequeue();
            enemy.transform.position = position;
            enemy.gameObject.SetActive(true);
        }
        else
        {
            Transform enemyTransform = Instantiate(GameAssets.Instance.pfEnemy,position,Quaternion.identity);
            enemy = enemyTransform.GetComponent<Enemy>();
        }

        enemy.Init();

        return enemy;
    }

    /// <summary>
    /// 敌人死亡时，回收进对象池
    /// </summary>
    /// <param name="enemy"></param>
    public void Release(Enemy enemy)
    {
        enemy.gameObject.SetActive(false);
        pool.Enqueue(enemy);
    }
}
