using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LeaderboardEntryUI : MonoBehaviour
{
    public TextMeshProUGUI rankText;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI waveText;
    
    [Header("高亮设置")]
    public Image backgroundImage; // 拖入自身挂载的 Image 组件
    public Color normalColor = new Color(0, 0, 0, 0); // 默认透明
    public Color highlightColor = new Color(1f, 0.8f, 0f, 0.4f); // 高亮时的颜色（半透明的金色）

    // 供外部调用的赋值方法
    public void SetData(int rank, string playerName, double wave)
    {
        rankText.text = "#" + rank.ToString();
        nameText.text = playerName;
        waveText.text = wave.ToString() + " 波";
    }

    // 【新增】供外部调用的高亮控制方法
    public void SetHighlight(bool isMe)
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = isMe ? highlightColor : normalColor;
        }
    }
}