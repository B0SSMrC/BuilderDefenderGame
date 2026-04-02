using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class OptionUI : MonoBehaviour
{

    [SerializeField] private SoundManager soundManager;
    [SerializeField] private MusicManager musicManager;

    private TextMeshProUGUI soundVolumeText;
    private TextMeshProUGUI musicVolumeText;

    private void Awake()
    {

        soundVolumeText = transform.Find("soundVolumeText").GetComponent<TextMeshProUGUI>();
        musicVolumeText = transform.Find("musicVolumeText").GetComponent<TextMeshProUGUI>();

        transform.Find("soundIncreaseBtn").GetComponent<Button>().onClick.AddListener(() =>
        {
            soundManager.IncreaseVolume();
            UpdateText();
        });
        transform.Find("soundDecreaseBtn").GetComponent<Button>().onClick.AddListener(() =>
        {
            soundManager.DecreaseVolume();
            UpdateText();
        });
        transform.Find("musicIncreaseBtn").GetComponent<Button>().onClick.AddListener(() =>
        {
            musicManager.IncreaseVolume();
            UpdateText();
        });
        transform.Find("musicDecreaseBtn").GetComponent<Button>().onClick.AddListener(() =>
        {
            musicManager.DecreaseVolume();
            UpdateText();
        });
        transform.Find("mainMenuBtn").GetComponent<Button>().onClick.AddListener(() =>
        {
            // 1. 执行保存
            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.SaveGame();
            }

            // 2. 呼出 TooltipUI 提示
            TooltipUI.Instance.Show("已保存存档", new TooltipUI.TooltipTimer{timer = 1.5f});

            // 3. 开启协程，延迟加载主菜单
            StartCoroutine(LoadMainMenuCoroutine());
        });
        transform.Find("edgeScrollingToggle").GetComponent<Button>().onClick.AddListener(() =>
        {
            CameraHandler.Instance.SetEdgeScrolling();
        });
        
    }

    private void Start()
    {
        UpdateText();
        gameObject.SetActive(false);
        
    }


    private void UpdateText()
    {
        soundVolumeText.SetText(Mathf.RoundToInt(soundManager.GetVolume() * 10).ToString());
        musicVolumeText.SetText(Mathf.RoundToInt(musicManager.GetVolume() * 10).ToString());
    }

    public void ToggleVisible()
    {
        gameObject.SetActive(!gameObject.activeSelf);

        if (gameObject.activeSelf)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    private IEnumerator LoadMainMenuCoroutine()
    {
        // 核心细节：因为打开设置界面时把 Time.timeScale 设为了 0
        // 所以普通的 WaitForSeconds 会卡死！必须使用 WaitForSecondsRealtime（不受时间缩放影响的真实物理时间）
        yield return new WaitForSecondsRealtime(1.5f);
        
        // 等玩家看清提示后，恢复时间缩放并加载场景
        Time.timeScale = 1f;
        GameSceneManager.Load(GameSceneManager.Scene.MainMenuScene);
    }
}
