using UnityEngine;
using UnityEditor;
public class LevelEditorWindow : EditorWindow
{
    private LevelConfigSO currentConfig;
    private Vector2 scrollPosition;

    [MenuItem("Tools/塔防关卡编辑器")]
    public static void ShowWindow()
    {
        GetWindow<LevelEditorWindow>("关卡配置中心");
    }

    private void OnGUI()
    {
        GUILayout.Label("塔防关卡数据管理面板", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        currentConfig = (LevelConfigSO)EditorGUILayout.ObjectField("当前关卡配置", currentConfig,typeof(LevelConfigSO),false);

        if(currentConfig != null)
        {
            //横线间隔
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            //修改基础配置
            currentConfig.levelName = EditorGUILayout.TextField("关卡名称", currentConfig.levelName);
            currentConfig.initialWaitTime = EditorGUILayout.FloatField("开局准备时间(秒)",currentConfig.initialWaitTime);

            EditorGUILayout.Space();
            GUILayout.Label($"总波次:{currentConfig.waves.Count} 波", EditorStyles.helpBox);
            //面板滚动
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            for(int i = 0; i < currentConfig.waves.Count; i++)
            {
                EditorGUILayout.BeginVertical("box");

                GUILayout.Label($"--- 第 {i + 1} 波 ---", EditorStyles.boldLabel);
                currentConfig.waves[i].enemyCount = EditorGUILayout.IntField("怪物数量", currentConfig.waves[i].enemyCount);
                currentConfig.waves[i].spawnInterval = EditorGUILayout.FloatField("生成间隔", currentConfig.waves[i].spawnInterval);
                currentConfig.waves[i].timeToNextWave = EditorGUILayout.FloatField("休息时间", currentConfig.waves[i].timeToNextWave);

                // 提供一个删除波次的按钮（标红显示）
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("删除此波次"))
                {
                    currentConfig.waves.RemoveAt(i);
                }
                GUI.backgroundColor = Color.white;

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }

            EditorGUILayout.EndScrollView();
            // 新增波次的按钮（绿色显示）
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("+++ 新增一波 +++", GUILayout.Height(30)))
            {
                currentConfig.waves.Add(new WaveData { enemyCount = 5, spawnInterval = 0.1f, timeToNextWave = 10f });
            }
            GUI.backgroundColor = Color.white;

            // 如果发生修改，通知 Unity 保存资产
            if (GUI.changed)
            {
                EditorUtility.SetDirty(currentConfig);
            }
        }
        else
        {
            EditorGUILayout.HelpBox("请将 LevelConfigSO 文件拖入上方槽位，或在 Project 面板创建一个。", MessageType.Warning);
        }
    }
}
