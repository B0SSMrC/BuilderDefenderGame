using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public static Enemy Create(Vector3 position)
    {
       return EnemyPool.Instance.Get(position);
    }
    private Rigidbody2D rigidbody2d;
    private Transform targetTransform;
    private HealthSystem healthSystem;
    private float lookForTargetTimer;
    private float lookForTargetTimerMax = 0.2f;

    private void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        healthSystem = GetComponent<HealthSystem>();
        healthSystem.OnDamaged += HealthSystem_OnDamaged;
        healthSystem.OnDied += HealthSystem_OnDied;

    }

    public void Init()
    {
        //重置目标
        if(BuildingManager.Instance.GetHQBuilding() != null)
        {
            targetTransform = BuildingManager.Instance.GetHQBuilding().transform;
        }
        else
        {
            targetTransform = null;
        }

        //满血复活
        healthSystem.HealFull();

        //重置物理速度和计时器
        rigidbody2d.velocity = Vector2.zero;
        lookForTargetTimer = Random.Range(0f,lookForTargetTimerMax);
    }
    

    private void Update()
    {
        
        HandleMovement();
        HandleTargeting();
    }

    private void HealthSystem_OnDamaged(object sender, System.EventArgs e)
    {
        SoundManager.Instance.PlaySound(SoundManager.Sound.EnemyHit);
    }

    private void HealthSystem_OnDied(object sender, System.EventArgs e)
    {
        SoundManager.Instance.PlaySound(SoundManager.Sound.EnemyDie);
        Instantiate(GameAssets.Instance.pfEnemyDieParticles, transform.position,Quaternion.identity);
        ChromaticAberrationEffect.Instance.SetWeight(0.4f);
        //回收入池
        EnemyPool.Instance.Release(this);
    }

    

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Building building = collision.gameObject.GetComponent<Building>();
        if(building != null)
        {
            HealthSystem healthSystem = building.GetComponent<HealthSystem>();
            healthSystem.Damage(10);
            this.healthSystem.Damage(999);
        }
    }

    private void HandleMovement()
    {
        if(targetTransform != null)
        {   
            Vector3 moveDir = (targetTransform.position - transform.position).normalized;

            float moveSpeed = 5.5f;
            rigidbody2d.velocity = moveDir * moveSpeed;
        }
        else
        {
            rigidbody2d.velocity = Vector2.zero;
        }
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

    private void LookForTargets()
    {
        float targetMaxRadius = 10f;
        Collider2D[] collider2DArray = Physics2D.OverlapCircleAll(transform.position , targetMaxRadius);

        foreach(Collider2D collider2D in collider2DArray)
        {
            Building building = collider2D.GetComponent<Building>();
            if(building != null)
            {
                if(targetTransform == null)
                {
                    targetTransform = building.transform;
                }
                else
                {
                    if(Vector3.Distance(transform.position,building.transform.position) <
                       Vector3.Distance(transform.position, targetTransform.position))
                    {
                        targetTransform = building.transform;
                    }
                }
            }
        }
        if(targetTransform == null)
        {
            if(BuildingManager.Instance.GetHQBuilding() != null)
            {
                targetTransform = BuildingManager.Instance.GetHQBuilding().transform;
            }
        }
    }
}
