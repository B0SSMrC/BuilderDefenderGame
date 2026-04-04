using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Services.Authentication;
public class LeaderboardUI : MonoBehaviour
{
    [Header("UI 引用")]
    public Transform contentContainer; // Content
    public GameObject entryPrefab;     // EntryTemplate 预制体
    public TextMeshProUGUI statusText; //  StatusText ("正在加载...")
    public Button closeButton;         // 关闭按钮

    private void Awake()
    {
        // 绑定关闭按钮
        closeButton.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });
    }

    // 每次打开面板时调用此方法
    public async void ShowLeaderboard()
    {
        gameObject.SetActive(true);
        statusText.gameObject.SetActive(true);
        statusText.text = "正在向服务器请求数据...";

        // 1. 清空上次残留的旧数据
        foreach (Transform child in contentContainer)
        {
            Destroy(child.gameObject);
        }

        // 2. 向后端请求数据（等待网络返回）
        var data = await CloudLeaderboardManager.Instance.GetLeaderboardDataAsync();

        // 3. 处理数据结果
        if (data == null || data.Results.Count == 0)
        {
            statusText.text = "当前排行榜暂无数据！快去打一把吧！";
            return;
        }

        // 4. 有数据了，隐藏提示文字
        statusText.gameObject.SetActive(false);

        // 5. 遍历生成每一行排行榜 UI
        foreach (var entry in data.Results)
        {
            GameObject entryObj = Instantiate(entryPrefab, contentContainer);
            LeaderboardEntryUI entryUI = entryObj.GetComponent<LeaderboardEntryUI>();
            
            if (entryUI != null)
            {
                // 截取 # 前面的纯净名字
                string cleanName = entry.PlayerName;
                if (cleanName.Contains("#"))
                {
                    cleanName = cleanName.Split('#')[0];
                }

                // 传入清洗后的名字
                entryUI.SetData(entry.Rank + 1, cleanName, entry.Score);

                // 【核心高亮逻辑】
                // 获取当前设备登录的唯一账号 ID
                string myPlayerId = AuthenticationService.Instance.PlayerId;
                
                // 判断当前生成的这一行，是不是我自己的数据？
                bool isMyRank = (entry.PlayerId == myPlayerId);
                
                // 告诉单行 UI 是否需要高亮自己
                entryUI.SetHighlight(isMyRank);
            }
        }
    }
}