using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField] private float shootTimerMax;
    private float shootTimer;
    private Enemy targetEnemy;
    
    private float lookForTargetTimer;
    private float lookForTargetTimerMax = 0.1f;
    private Vector3 projectileSpawnPosition;

    private void Awake()
    {
        projectileSpawnPosition = transform.Find("projectileSpawnPosition").position;
    }
    private void Update()
    {
        HandleTargeting();
        HandleShooting();
    }
    private void HandleTargeting()
    {
        lookForTargetTimer -= Time.deltaTime;
        if(lookForTargetTimer < 0f)
        {
            lookForTargetTimer += lookForTargetTimerMax;
            LookForTargets();
        }
    }

    private void HandleShooting()
    {
        shootTimer -= Time.deltaTime;
        if(shootTimer <= 0f)
        {
            shootTimer += shootTimerMax;
            if(targetEnemy != null && targetEnemy.gameObject.activeSelf)
            {
                ArrowProjectile.Create(projectileSpawnPosition,targetEnemy);
            }
        }
        
    }
    private void LookForTargets()
    {
        float targetMaxRadius = 40f;
        Collider2D[] collider2DArray = Physics2D.OverlapCircleAll(transform.position , targetMaxRadius);

        // 如果当前瞄准的目标已经被回收了，清空目标
        if (targetEnemy != null && !targetEnemy.gameObject.activeSelf)
        {
            targetEnemy = null;
        }

        foreach(Collider2D collider2D in collider2DArray)
        {
            Enemy enemy = collider2D.GetComponent<Enemy>();
            if(enemy != null && enemy.gameObject.activeSelf)
            {
                if(targetEnemy == null)
                {
                    targetEnemy = enemy;
                }
                else
                {
                    if(Vector3.Distance(transform.position,enemy.transform.position) <
                       Vector3.Distance(transform.position, targetEnemy.transform.position))
                    {
                        targetEnemy = enemy;
                    }
                }
            }
        }
    }
}
