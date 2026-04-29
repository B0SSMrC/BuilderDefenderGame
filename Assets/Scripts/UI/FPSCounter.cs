using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class FPSCounter : MonoBehaviour
{
    [Header("UI 组件")]
    public TextMeshProUGUI fpsText;

    [Header("设置")]
    [Tooltip("FPS 刷新频率(秒)。0.2代表每秒刷新5次数字")]
    public float refreshRate = 0.2f; 

    private float timer;

    private void Update()
    {
        // 累加时间（注意：这里必须用 unscaledDeltaTime，不受游戏暂停时间缩放的影响）
        timer += Time.unscaledDeltaTime;

        // 当累加的时间达到我们设定的刷新频率时，才更新一次 UI
        if (timer >= refreshRate)
        {
            // 计算当前的帧率：1 秒除以完成上一帧所花的时间
            int currentFps = Mathf.RoundToInt(1f / Time.unscaledDeltaTime);
            
            // 更新文本显示
            fpsText.text = "FPS: " + currentFps;

            // 根据帧率改变颜色，给予直观的性能警告（可选）
            if (currentFps >= 60)
                fpsText.color = Color.green; // 流畅
            else if (currentFps >= 30)
                fpsText.color = Color.yellow; // 一般
            else
                fpsText.color = Color.red; // 卡顿

            // 重置计时器
            timer = 0f;
        }
    }
}
