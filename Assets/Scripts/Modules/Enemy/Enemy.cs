using UnityEngine;

public class Enemy : MonoBehaviour
{
    public static Enemy Create(Vector3 position)
    {
       return EnemyPool.Instance.Get(position);
    }

    [Header("索敌设置")]
    [SerializeField] private float targetMaxRadius = 10f;
    [SerializeField] private LayerMask buildingLayerMask;
    private Collider2D[] colliderBuffer = new Collider2D[50];

    private Rigidbody2D rigidbody2d;
    private Transform targetTransform;
    private HealthSystem healthSystem;
    private float lookForTargetTimer;
    private float lookForTargetTimerMax = 0.2f;

    private void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        healthSystem = GetComponent<HealthSystem>();
        

    }

    private void OnEnable()
    {
        healthSystem.OnDamaged += HealthSystem_OnDamaged;
        healthSystem.OnDied += HealthSystem_OnDied;
    }

    private void OnDisable()
    {
        healthSystem.OnDamaged -= HealthSystem_OnDamaged;
        healthSystem.OnDied -= HealthSystem_OnDied;
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
    // 1. 使用 NonAlloc 版本：把扫到的结果填入 colliderBuffer，而不是创建新数组。
    // hitCount 返回的是实际扫到了几个物体
    int hitCount = Physics2D.OverlapCircleNonAlloc(transform.position, targetMaxRadius, colliderBuffer, buildingLayerMask);

    targetTransform = null; // 每次索敌前清空目标
    float closestDistanceSqr = float.MaxValue; // 记录当前发现的最短距离
    Vector3 currentPosition = transform.position; // 缓存自身坐标

    // 2. 只遍历实际扫到的数量
    for (int i = 0; i < hitCount; i++)
    {
        Collider2D collider2D = colliderBuffer[i];
        
        Building building = collider2D.GetComponent<Building>();
        if (building != null)
        {
            // 3. 计算新目标的距离
            float distanceToNewSqr = (currentPosition - building.transform.position).sqrMagnitude;

            // 4. 如果比记录的最短距离还要短，就更新目标和最短距离
            if (distanceToNewSqr < closestDistanceSqr)
            {
                closestDistanceSqr = distanceToNewSqr;
                targetTransform = building.transform;
            }
        }
    }

    // 5. 兜底逻辑：如果范围内没找到任何建筑，就去打大本营
    if (targetTransform == null)
    {
        Building hqBuilding = BuildingManager.Instance.GetHQBuilding();
        if (hqBuilding != null)
        {
            targetTransform = hqBuilding.transform;
        }
    }
}


}