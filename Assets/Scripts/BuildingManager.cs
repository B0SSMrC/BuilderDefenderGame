using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.EventSystems;
public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance {get;private set;}

    public event EventHandler<OnActiveBuildingTypeChangedEventArgs> OnActiveBuildingTypeChanged;

    public class OnActiveBuildingTypeChangedEventArgs : EventArgs
    {
        public BuildingTypeSO activeBuildingType;
    }
    [SerializeField] private Building hqBuilding;
    private Camera mainCamera;
    private BuildingTypeListSO buildingTypeList;
    private BuildingTypeSO activeBuildingType;


    private void Awake()
    {
        Instance = this;
        //可用建筑列表
        buildingTypeList = Resources.Load<BuildingTypeListSO>(typeof(BuildingTypeListSO).Name);
        
    }

    private void Start()
    {
        mainCamera = Camera.main;

        hqBuilding.GetComponent<HealthSystem>().OnDied += HQ_OnDied;
    }

    private void HQ_OnDied(object sender, EventArgs e)
    {
        SoundManager.Instance.PlaySound(SoundManager.Sound.GameOver);
        GameOverUI.Instance.Show();
    }

    private void Update()
    {   
        //按下鼠标左键且未点击在UI上
        if(Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {   
            //选中某种建筑
            if(activeBuildingType != null ){
                //空间规则检测
                if(CanSpawnBuilding(activeBuildingType,UtilsClass.GetMouseWorldPosition(), out string errorMessage))
                {
                    //资源数量不足
                    if(ResourceManager.Instance.CanAfford(activeBuildingType.constructionResourceCostArray))
                    {   
                        //扣除资源
                        ResourceManager.Instance.SpendResources(activeBuildingType.constructionResourceCostArray);
                        //Instantiate(activeBuildingType.prefab,UtilsClass.GetMouseWorldPosition(),quaternion.identity);
                        BuildingConstruction.Create(UtilsClass.GetMouseWorldPosition(),activeBuildingType);
                        SoundManager.Instance.PlaySound(SoundManager.Sound.BuildingPlaced);
                    }
                    else
                    {
                        TooltipUI.Instance.Show("付不起: " + activeBuildingType.GetConstructionResourceCostString(),
                            new TooltipUI.TooltipTimer{timer = 2f});
                    }
                }
                else
                {
                    TooltipUI.Instance.Show(errorMessage,new TooltipUI.TooltipTimer{timer = 2f});
                }
            }
        }  
    }

    

    public void SetActiveBuildingType(BuildingTypeSO buildingType)
    {
        activeBuildingType = buildingType;
        //广播：当前选中建筑类型发生变化
        OnActiveBuildingTypeChanged?.Invoke(this,new OnActiveBuildingTypeChangedEventArgs{activeBuildingType = activeBuildingType});
    }

    public BuildingTypeSO GetActiveBuildingType()
    {
        return activeBuildingType;
    }

    /// <summary>
    /// 检测当前鼠标位置是否允许建造该建筑
    /// </summary>
    /// <param name="buildingType">建筑类型</param>
    /// <param name="position">鼠标位置</param>
    /// <param name="error_Message">输出不允许建造原因</param>
    /// <returns></returns>
    private bool CanSpawnBuilding(BuildingTypeSO buildingType, Vector3 position,out string error_Message)
    {
        BoxCollider2D boxCollider2D = buildingType.prefab.GetComponent<BoxCollider2D>();

        //检测1：建筑不能和任何现有的物体（石头、树木、其他建筑）重叠
        Collider2D[] collider2DArray = Physics2D.OverlapBoxAll(position + (Vector3)boxCollider2D.offset,boxCollider2D.size,0);
        
        bool isAreaClear = collider2DArray.Length == 0;
        if(!isAreaClear) 
        {
            error_Message = "区域未清空!";
            return false;
        }

        // 检测二：同类建筑不能靠得太近（比如伐木场之间要有一定距离）
        collider2DArray = Physics2D.OverlapCircleAll(position,buildingType.minConstructionRadius);   
        foreach(Collider2D collider2D in collider2DArray)
        {
            BuildingTypeHolder buildingTypeHolder = collider2D.GetComponent<BuildingTypeHolder>();
            if(buildingTypeHolder != null)
            {
                if(buildingTypeHolder.buildingType == buildingType)
                {
                    error_Message = "此区域已存在相同类型建筑!";
                    return false;
                }
            }
        }


        // 检测三：新建筑不能建在荒郊野外，必须挨着已有的其他建筑（扩大领地）
        float maxConstructionRadius = 50;
        collider2DArray = Physics2D.OverlapCircleAll(position,maxConstructionRadius);   
        foreach(Collider2D collider2D in collider2DArray)
        {
            BuildingTypeHolder buildingTypeHolder = collider2D.GetComponent<BuildingTypeHolder>();
            if(buildingTypeHolder != null)
            {
                error_Message = "";
               return true;
            }
        }

        error_Message = "距离其他建筑过远!";
        return false;
    }

    public Building GetHQBuilding()
    {
        return hqBuilding;
    }
}
