using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowPool : MonoBehaviour
{
    public static ArrowPool Instance { get; private set; }

    private Queue<ArrowProjectile> pool = new Queue<ArrowProjectile>();
    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// 从池子中获取一个箭矢
    /// </summary>
    public ArrowProjectile Get(Vector3 position, Enemy targetEnemy)
    {
        ArrowProjectile arrow;

        // 如果仓库里有存货，就拿出来用
        if (pool.Count > 0)
        {
            arrow = pool.Dequeue();
            arrow.transform.position = position;
            arrow.gameObject.SetActive(true); // 唤醒
        }
        else
        {
            // 如果仓库空了，才不得不真正 Instantiate 一个新的
            Transform arrowTransform = Instantiate(GameAssets.Instance.pfArrowProjectile, position, Quaternion.identity);
            arrow = arrowTransform.GetComponent<ArrowProjectile>();
        }

        //拿出来的箭矢必须重置状态
        arrow.Init(targetEnemy); 
        
        return arrow;
    }

    /// <summary>
    /// 箭矢消失或击中时，回收进池子
    /// </summary>
    public void Release(ArrowProjectile arrow)
    {
        // 如果物体已经被销毁（防止极少数情况下的竞态条件），则不处理
        if(arrow == null || !arrow.gameObject.activeSelf) return;

        arrow.gameObject.SetActive(false); // 隐藏
        pool.Enqueue(arrow);               // 放回仓库
    }
}
