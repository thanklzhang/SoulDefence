using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 单波怪物配置
/// </summary>
[System.Serializable]
public class WaveConfig
{
    [Header("波次基本信息")]
    public int waveNumber = 1;                  // 波次编号
    public string waveName = "第1波";           // 波次名称
    
    [Header("怪物配置")]
    public int monsterCount = 5;                // 怪物数量
    public string monsterType = "SmallEnemy";   // 怪物类型
    
    [Header("时间配置")]
    public float waveDelay = 0f;                // 波次开始延迟时间（秒）
    public float spawnInterval = 1f;            // 怪物生成间隔时间（秒）
    
    [Header("可选配置")]
    public string description = "";             // 波次描述
}

/// <summary>
/// 关卡配置
/// </summary>
[CreateAssetMenu(fileName = "NewLevelConfig", menuName = "GameConfig/Level Config")]
public class LevelConfig : ScriptableObject
{
    [Header("关卡基本信息")]
    public int levelId = 1;                     // 关卡编号
    public string levelName = "关卡1";          // 关卡名称
    public string levelDescription = "";        // 关卡描述
    
    [Header("波次设置")]
    [SerializeField] private int waveCount = 5;                    // 波次数量（可配置）
    [SerializeField] private List<WaveConfig> waves = new List<WaveConfig>();
    
    void OnValidate()
    {
        // 确保波次数量至少为1
        waveCount = Mathf.Max(1, waveCount);
        
        // 根据设定的波次数量调整waves列表
        while (waves.Count < waveCount)
        {
            WaveConfig newWave = new WaveConfig();
            newWave.waveNumber = waves.Count + 1;
            newWave.waveName = $"第{newWave.waveNumber}波";
            waves.Add(newWave);
        }
        
        // 如果波次过多，移除多余的
        if (waves.Count > waveCount)
        {
            waves.RemoveRange(waveCount, waves.Count - waveCount);
        }
        
        // 更新波次编号和名称
        for (int i = 0; i < waves.Count; i++)
        {
            waves[i].waveNumber = i + 1;
            if (string.IsNullOrEmpty(waves[i].waveName) || waves[i].waveName == $"第{i}波")
            {
                waves[i].waveName = $"第{i + 1}波";
            }
        }
    }
    
    /// <summary>
    /// 获取所有波次配置
    /// </summary>
    public List<WaveConfig> GetWaves()
    {
        return new List<WaveConfig>(waves);
    }
    
    /// <summary>
    /// 获取指定波次配置
    /// </summary>
    /// <param name="waveIndex">波次索引（0-4）</param>
    public WaveConfig GetWave(int waveIndex)
    {
        if (waveIndex >= 0 && waveIndex < waves.Count)
        {
            return waves[waveIndex];
        }
        return null;
    }
    
    /// <summary>
    /// 获取总波数
    /// </summary>
    public int GetWaveCount()
    {
        return waves.Count;
    }
    
    /// <summary>
    /// 设置波次数量
    /// </summary>
    /// <param name="count">波次数量</param>
    public void SetWaveCount(int count)
    {
        waveCount = Mathf.Max(1, count);
        OnValidate(); // 触发验证以更新波次列表
    }
    
    /// <summary>
    /// 获取配置的波次数量
    /// </summary>
    public int GetConfiguredWaveCount()
    {
        return waveCount;
    }
    
    /// <summary>
    /// 获取关卡总怪物数量
    /// </summary>
    public int GetTotalMonsterCount()
    {
        int total = 0;
        foreach (var wave in waves)
        {
            total += wave.monsterCount;
        }
        return total;
    }
} 