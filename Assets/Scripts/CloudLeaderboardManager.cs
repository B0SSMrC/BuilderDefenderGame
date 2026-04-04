using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Leaderboards;

public class CloudLeaderboardManager : MonoBehaviour
{
    public static CloudLeaderboardManager Instance { get; private set; }
    
    // Unity Dashboard 上创建的排行榜 ID
    private const string LEADERBOARD_ID = "survival_waves_global";

    public string testProfileName;

    private void Awake()
    {
        
        // 防止场景切换（游戏返回主菜单）时重复生成管理器
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // 如果发现已经有一个存在了，立刻销毁自己
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // 设为跨场景不销毁
    }

    private async void Start()
    {
        
        // 只有当 UGS 服务确实还没有初始化时，才去执行
        if (UnityServices.State == ServicesInitializationState.Uninitialized)
        {
            await InitializeAndSignInAsync();
        }
    }

    /// <summary>
    /// 1. 初始化 UGS 并静默登录
    /// </summary>
    private async Task InitializeAndSignInAsync()
    {
        try
        {
            

            InitializationOptions options = new InitializationOptions();
            options.SetProfile(testProfileName);

            // 把 options 传给 InitializeAsync
            await UnityServices.InitializeAsync(options);
            // ===== 核心测试代码结束 =====

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log($"[{testProfileName}] 登录成功！唯一ID: {AuthenticationService.Instance.PlayerId}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"登录发生错误: {e.Message}");
        }
    }

    /// <summary>
    /// 2. 游戏结束时调用：上传生存波次
    /// </summary>
    public async void SubmitWaveScore(int waveNumber)
    {
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            Debug.LogWarning("玩家未登录，无法上传分数！");
            return;
        }

        try
        {
            var scoreResponse = await LeaderboardsService.Instance.AddPlayerScoreAsync(LEADERBOARD_ID, waveNumber);
            Debug.Log($"分数 {waveNumber} 上传成功！");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"上传分数失败: {e.Message}");
        }
    }

    /// <summary>
    /// 3. 获取并打印全球排行榜前 10 名
    /// </summary>
    public async void FetchTopLeaderboard()
    {
        try
        {
            var scoresResponse = await LeaderboardsService.Instance.GetScoresAsync(LEADERBOARD_ID);

            Debug.Log("=== 全球塔防生存榜 ===");
            foreach (var entry in scoresResponse.Results)
            {
                Debug.Log($"第 {entry.Rank + 1} 名 | 玩家: {entry.PlayerName} | 存活波次: {entry.Score}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"获取排行榜失败: {e.Message}");
        }
    }


    /// <summary>
    /// 供 UI 调用的异步获取数据接口
    /// </summary>
    public async Task<Unity.Services.Leaderboards.Models.LeaderboardScoresPage> GetLeaderboardDataAsync()
    {
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            Debug.LogWarning("玩家未登录，无法获取排行榜！");
            return null;
        }

        try
        {
            // 获取数据并直接返回给调用者
            return await LeaderboardsService.Instance.GetScoresAsync(LEADERBOARD_ID);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"获取排行榜数据失败: {e.Message}");
            return null;
        }
    }
}