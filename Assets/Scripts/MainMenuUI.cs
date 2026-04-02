using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
public class MainMenuUI : MonoBehaviour
{


    private void Awake()
    {
        // 1. 新游戏按钮 (playBtn)
        transform.Find("playBtn").GetComponent<Button>().onClick.AddListener(() =>
        {
            // 使用 PlayerPrefs 传话：0 代表纯净的新游戏
            PlayerPrefs.SetInt("ContinueGame", 0); 
            GameSceneManager.Load(GameSceneManager.Scene.GameScene);
        });

        // 2. 继续游戏按钮 (continueBtn)
        Transform continueBtnTransform = transform.Find("continueBtn");
        if (continueBtnTransform != null)
        {
            string savePath = Application.persistentDataPath + "/savegame.json";
            
            // 检查硬盘里到底有没有存档
            if (File.Exists(savePath))
            {
                continueBtnTransform.gameObject.SetActive(true); // 有存档则显示
                continueBtnTransform.GetComponent<Button>().onClick.AddListener(() =>
                {
                    // 使用 PlayerPrefs 传话：1 代表需要读取存档
                    PlayerPrefs.SetInt("ContinueGame", 1); 
                    GameSceneManager.Load(GameSceneManager.Scene.GameScene);
                });
            }
            else
            {
                // 如果没有存档，直接把继续游戏按钮隐藏掉
                continueBtnTransform.gameObject.SetActive(false); 
            }
        }

        // 3. 退出按钮 (quitBtn)
        transform.Find("quitBtn").GetComponent<Button>().onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }
}
