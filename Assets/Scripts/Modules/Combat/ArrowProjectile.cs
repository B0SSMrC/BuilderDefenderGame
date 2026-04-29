using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
    // 静态生成方法，现在不再直接 Instantiate，而是向池子要人！
    public static ArrowProjectile Create(Vector3 position, Enemy enemy)
    {
        return ArrowPool.Instance.Get(position, enemy);
    }

    private Enemy targetEnemy;
    private Vector3 lastMoveDir;
    
    // 用于对象池复用的核心计时器，每次 Init 重置
    private float timeToDie;
    private const float TIME_TO_DIE_MAX = 2.5f; // 箭矢最大存活时间（略微增长防止提前消失）

    // 初始化状态，每次从池子里拿出来时调用
    public void Init(Enemy targetEnemy)
    {
        this.targetEnemy = targetEnemy;
        this.timeToDie = TIME_TO_DIE_MAX; // 重置存活计时器
        
        if(targetEnemy != null)
        {
            lastMoveDir = (targetEnemy.transform.position - transform.position).normalized;
        }
        else
        {
            lastMoveDir = Vector3.up; // 兜底方向
        }
    }

    private void Update()
    {
        Vector3 moveDir;
        if(targetEnemy != null && targetEnemy.gameObject.activeSelf) // 检查组件存在且非活跃状态（未死亡/回收）
        {
            moveDir = (targetEnemy.transform.position - transform.position).normalized;
            lastMoveDir = moveDir;
        }
        else
        {
            moveDir = lastMoveDir;
        }
        
        float moveSpeed = 40f;
        transform.position += moveDir * moveSpeed * Time.deltaTime;
        transform.eulerAngles = new Vector3(0,0,UtilsClass.GetAngleFromVector(moveDir));

        timeToDie -= Time.deltaTime;
        if(timeToDie < 0f)
        {
            //回收进池子
            ReleaseProjectile();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        Enemy enemy = collision.GetComponent<Enemy>();
        
        // 只有当撞到的物体确实是敌人，且敌人还活着时
        if(enemy != null && enemy.gameObject.activeSelf) 
        {
            int damageAmount = 15;
            enemy.GetComponent<HealthSystem>().Damage(damageAmount);
            ReleaseProjectile();
        }
    }

    // 内部调用，用于安全地回收箭矢
    private void ReleaseProjectile()
    {
        // 只有当前物体处于活跃状态才去回收，防止竞态条件下重复呼叫
        if (gameObject != null && gameObject.activeSelf)
        {
            ArrowPool.Instance.Release(this);
        }
    }
}