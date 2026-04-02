using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
public class BuildingRepairBtn : MonoBehaviour
{

    [SerializeField] private HealthSystem healthSystem;
    [SerializeField] private ResourceTypeSO goldResourceType;

    private void Awake()
    {
        
        transform.Find("button").GetComponent<Button>().onClick.AddListener(() =>
        {
            int missingHealth = healthSystem.GetHealthAmountMax() - healthSystem.GetHealthAmount();
            int repairCost = missingHealth / 2;
            ResourceAmount[] resourceAmountCost = new ResourceAmount[] {
                new ResourceAmount {resourceType = goldResourceType, amount = repairCost }};
            
            if(ResourceManager.Instance.CanAfford(resourceAmountCost))
            {
                ResourceManager.Instance.SpendResources(resourceAmountCost);
                healthSystem.HealFull();
            }
            else
            {
                TooltipUI.Instance.Show("付不起维修费用！", new TooltipUI.TooltipTimer{timer = 2f});
            }
            
            
        });
    }
}
