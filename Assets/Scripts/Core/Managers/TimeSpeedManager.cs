using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimeSpeedManager : MonoBehaviour
{
    public static TimeSpeedManager Instance { get; private set; }

    [Header("UI 引用")]
    [Tooltip("拖入你刚才创建的加速按钮")]
    public Button speedButton;
    [Tooltip("拖入按钮下的 Text 子物体")]
    public TextMeshProUGUI speedText;

    private float currentSpeed = 1f; // 记住玩家当前选择的档位
    private bool isGamePaused = false; // 记录是否被设置面板强制暂停了

    private void Awake()
    {
        Instance = this;

        // 给按钮绑定点击事件
        if (speedButton != null)
        {
            speedButton.onClick.AddListener(ToggleSpeed);
        }
    }

    private void ToggleSpeed()
    {
        // 切换逻辑：1倍 -> 2倍 -> 3倍 -> 循环回1倍
        if (currentSpeed == 1f) currentSpeed = 2f;
        else if (currentSpeed == 2f) currentSpeed = 3f;
        else currentSpeed = 1f;

        // 更新按钮上的文字提示
        if (speedText != null)
        {
            speedText.text = "x" + currentSpeed;
        }

        // 【核心】：只有在游戏没有被暂停的情况下，才实际修改底层时间
        if (!isGamePaused)
        {
            ApplyTimeScale(currentSpeed);
        }
    }

    /// <summary>
    /// 供其他系统（如 OptionUI）调用，用来临时暂停或恢复游戏
    /// </summary>
    public void SetGamePaused(bool isPaused)
    {
        this.isGamePaused = isPaused;

        if (isPaused)
        {
            ApplyTimeScale(0f); // 强制暂停
        }
        else
        {
            ApplyTimeScale(currentSpeed); // 恢复到暂停前玩家选择的加速状态！
        }
    }

    /// <summary>
    /// 统一修改时间缩放，并附带物理系统的同步修复
    /// </summary>
    private void ApplyTimeScale(float scale)
    {
        Time.timeScale = scale;
        
        // 如果不跟着改 fixedDeltaTime，在3倍速下敌人的碰撞检测可能会穿墙或者卡顿！
        if (scale > 0)
        {
            Time.fixedDeltaTime = 0.02f * scale; 
        }
    }
}