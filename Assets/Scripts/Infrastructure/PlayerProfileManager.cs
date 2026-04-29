using System.Threading.Tasks;
using UnityEngine;
using TMPro; 
using Unity.Services.Core;
using Unity.Services.Authentication;

public class PlayerProfileManager : MonoBehaviour
{
    [Header("UI 组件引用")]
    public TMP_InputField nameInputField;
    public GameObject nicknamePanel; 

    private async void Start()
    {
        while (UnityServices.State != ServicesInitializationState.Initialized)
            await Task.Delay(100);

        while (!AuthenticationService.Instance.IsSignedIn)
            await Task.Delay(100); 

        try
        {
            string currentName = await AuthenticationService.Instance.GetPlayerNameAsync();
            
            // 【核心修复】：获取当前测试的 Profile 名字（比如 "Player1" 或 "Player2"）
            string currentProfile = AuthenticationService.Instance.Profile;
            
            // 检查硬盘里是否有“已确认过名字”的标记
            bool hasConfirmedName = PlayerPrefs.GetInt("HasSetCustomName_" + currentProfile, 0) == 1;

            if (!string.IsNullOrEmpty(currentName) && hasConfirmedName)
            {
                // 如果云端有名字，且玩家曾经亲手确认过
                nameInputField.text = currentName.Split('#')[0]; 
                nicknamePanel.SetActive(false); 
            }
            else
            {
                // 如果没确认过（即便是系统分配了随机名字），也强制弹窗让他改！
                if (!string.IsNullOrEmpty(currentName))
                {
                    // 把系统随机给的名字显示在输入框里当做推荐
                    nameInputField.text = currentName.Split('#')[0]; 
                }
                nicknamePanel.SetActive(true);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"获取玩家名字失败: {e.Message}");
        }
    }

    public async void OnSaveNameButtonClicked()
    {
        string newName = nameInputField.text.Trim();

        if (string.IsNullOrEmpty(newName) || newName.Length > 15)
        {
            Debug.LogWarning("名字不合法！");
            return;
        }

        try
        {
            await AuthenticationService.Instance.UpdatePlayerNameAsync(newName);
            Debug.Log($"昵称已成功更新为: {newName}");
            
            // 【核心修复】：保存成功后，在本地打上“已确认”的烙印！
            string currentProfile = AuthenticationService.Instance.Profile;
            PlayerPrefs.SetInt("HasSetCustomName_" + currentProfile, 1);
            PlayerPrefs.Save();

            nicknamePanel.SetActive(false);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"发生了未知错误: {ex.Message}");
        }
    }
}