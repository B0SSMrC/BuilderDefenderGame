using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CameraHandler : MonoBehaviour
{
    public static CameraHandler Instance{get; private set;}
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
    
    private float orthographicSize;
    private float targetOrthographicSize;
    private bool edgeScrolling;

    private void Awake()
    {
        Instance = this;

        edgeScrolling = PlayerPrefs.GetInt("edgeScrolling", 0) == 1;
    }
    private void Start()
    {
        orthographicSize = cinemachineVirtualCamera.m_Lens.OrthographicSize;
        targetOrthographicSize = orthographicSize;
    }
    private void Update()
    {
        HandleMovement();
        HandleZoom();
        
    }

    private void HandleMovement()
    {
        float x =Input.GetAxisRaw("Horizontal");
        float y =Input.GetAxisRaw("Vertical");
        if(edgeScrolling)
        {
            float edgeScrollingSize  = 30;
            if(Input.mousePosition.x > Screen.width - edgeScrollingSize)
            {
                x = +1f;
            }
            if(Input.mousePosition.x < edgeScrollingSize)
            {
                x = -1f;
            } 
        
            if(Input.mousePosition.y > Screen.height - edgeScrollingSize)
            {
                y = +1f;
            }
            if(Input.mousePosition.y < edgeScrollingSize)
            {
                y = -1f;
            }
        }

        Vector3 moveDir = new Vector3(x,y).normalized;
        float moveSpeed = 80f;

        transform.position += moveDir * moveSpeed * Time.deltaTime;
    }
    private void HandleZoom()
    {
        float zoomAmount = 2f;
        targetOrthographicSize += -Input.mouseScrollDelta.y * zoomAmount;

        float minOrthographicSize =5;
        float maxOrthographicSize =50;
        targetOrthographicSize = Mathf.Clamp(targetOrthographicSize,minOrthographicSize,maxOrthographicSize);

        float zoomSpeed = 6f;
        orthographicSize = Mathf.Lerp(orthographicSize,targetOrthographicSize,Time.deltaTime * zoomSpeed);
        cinemachineVirtualCamera.m_Lens.OrthographicSize =orthographicSize;
    }

    public void SetEdgeScrolling()
    {
        if (edgeScrolling)
        {
            edgeScrolling = false;
            TooltipUI.Instance.Show("已关闭边缘滚动功能" , new TooltipUI.TooltipTimer{timer = 1.5f});
        }
        else
        {
            edgeScrolling = true;
            TooltipUI.Instance.Show("已开启边缘滚动功能" , new TooltipUI.TooltipTimer{timer = 1.5f});
        }
        PlayerPrefs.SetInt("edgeScrolling", edgeScrolling ? 1 : 0);
    }
    public bool GetEdgeScrolling()
    {
        return edgeScrolling;
    }
}
