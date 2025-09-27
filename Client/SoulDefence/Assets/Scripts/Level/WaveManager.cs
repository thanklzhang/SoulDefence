using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 波次管理器
/// 控制关卡中可配置数量波次的怪物生成流程
/// </summary>
public class WaveManager : MonoBehaviour
{
    [Header("当前状态")]
    [SerializeField] private int currentWaveIndex = -1;     // 当前波次索引（-1表示未开始）
    [SerializeField] private bool isWaveActive = false;     // 当前是否有波次在进行
    [SerializeField] private bool isLevelComplete = false;  // 关卡是否完成
    
    // 组件引用
    private SpawnEntityController spawnController;
    private LevelConfig levelConfig;
    
    // 当前运行的协程
    private Coroutine currentWaveCoroutine;
    
    // 事件
    public System.Action<int, WaveConfig> OnWaveStart;      // 波次开始 (波次索引, 波次配置)
    public System.Action<int, WaveConfig> OnWaveComplete;   // 波次完成
    public System.Action<int> OnWaveCountdown;              // 波次倒计时 (剩余秒数)
    public System.Action OnAllWavesComplete;                // 所有波次完成
    
    void Awake()
    {
        spawnController = GetComponent<SpawnEntityController>();
        if (spawnController == null)
        {
            spawnController = gameObject.AddComponent<SpawnEntityController>();
        }
    }
    
    /// <summary>
    /// 开始关卡
    /// </summary>
    /// <param name="config">关卡配置</param>
    public void StartLevel(LevelConfig config)
    {
        if (config == null)
        {
            Debug.LogError("[WaveManager] 关卡配置为空！");
            return;
        }
        
        levelConfig = config;
        currentWaveIndex = -1;
        isWaveActive = false;
        isLevelComplete = false;
        
        Debug.Log($"[WaveManager] 开始关卡: {config.levelName}");
        
        // 开始第一波
        StartNextWave();
    }
    
    /// <summary>
    /// 开始下一波
    /// </summary>
    public void StartNextWave()
    {
        if (isLevelComplete)
        {
            Debug.LogWarning("[WaveManager] 关卡已完成，无法开始下一波");
            return;
        }
        
        if (isWaveActive)
        {
            Debug.LogWarning("[WaveManager] 当前有波次正在进行，无法开始下一波");
            return;
        }
        
        currentWaveIndex++;
        
        // 检查是否所有波次都完成了
        if (currentWaveIndex >= levelConfig.GetWaveCount())
        {
            CompleteLevelAllWaves();
            return;
        }
        
        // 获取当前波次配置
        WaveConfig waveConfig = levelConfig.GetWave(currentWaveIndex);
        if (waveConfig == null)
        {
            Debug.LogError($"[WaveManager] 无法获取第{currentWaveIndex + 1}波配置");
            return;
        }
        
        // 开始波次协程
        currentWaveCoroutine = StartCoroutine(WaveCoroutine(waveConfig));
    }
    
    /// <summary>
    /// 波次协程
    /// </summary>
    private IEnumerator WaveCoroutine(WaveConfig waveConfig)
    {
        isWaveActive = true;
        
        // 波次延迟倒计时
        if (waveConfig.waveDelay > 0)
        {
            Debug.Log($"[WaveManager] {waveConfig.waveName} 延迟 {waveConfig.waveDelay} 秒");
            
            int remainingSeconds = Mathf.CeilToInt(waveConfig.waveDelay);
            float timer = waveConfig.waveDelay;
            
            while (timer > 0)
            {
                int currentSeconds = Mathf.CeilToInt(timer);
                if (currentSeconds != remainingSeconds)
                {
                    remainingSeconds = currentSeconds;
                    OnWaveCountdown?.Invoke(remainingSeconds);
                }
                
                timer -= Time.deltaTime;
                yield return null;
            }
        }
        
        // 触发波次开始事件
        OnWaveStart?.Invoke(currentWaveIndex, waveConfig);
        Debug.Log($"[WaveManager] 开始 {waveConfig.waveName}");
        
        // 开始生成怪物
        yield return spawnController.SpawnWave(waveConfig);
        
        // 波次完成
        OnWaveComplete?.Invoke(currentWaveIndex, waveConfig);
        Debug.Log($"[WaveManager] {waveConfig.waveName} 完成");
        
        isWaveActive = false;
        
        // 自动开始下一波
        StartNextWave();
    }
    
    /// <summary>
    /// 完成所有波次
    /// </summary>
    private void CompleteLevelAllWaves()
    {
        isLevelComplete = true;
        OnAllWavesComplete?.Invoke();
        Debug.Log($"[WaveManager] 关卡 {levelConfig.levelName} 所有{levelConfig.GetWaveCount()}波完成！");
    }
    
    /// <summary>
    /// 停止当前关卡
    /// </summary>
    public void StopLevel()
    {
        if (currentWaveCoroutine != null)
        {
            StopCoroutine(currentWaveCoroutine);
            currentWaveCoroutine = null;
        }
        
        spawnController.StopAllSpawning();
        isWaveActive = false;
        
        Debug.Log("[WaveManager] 停止当前关卡");
    }
    
    /// <summary>
    /// 重置波次管理器
    /// </summary>
    public void Reset()
    {
        StopLevel();
        currentWaveIndex = -1;
        isLevelComplete = false;
        levelConfig = null;
        
        spawnController.Reset();
        
        Debug.Log("[WaveManager] 重置波次管理器");
    }
    
    /// <summary>
    /// 强制开始下一波（调试用）
    /// </summary>
    [ContextMenu("Force Next Wave")]
    public void ForceNextWave()
    {
        if (isWaveActive && currentWaveCoroutine != null)
        {
            StopCoroutine(currentWaveCoroutine);
            currentWaveCoroutine = null;
            isWaveActive = false;
        }
        
        StartNextWave();
    }
    
    #region 状态查询
    
    /// <summary>
    /// 获取当前波次索引
    /// </summary>
    public int GetCurrentWaveIndex()
    {
        return currentWaveIndex;
    }
    
    /// <summary>
    /// 获取当前波次编号（从1开始）
    /// </summary>
    public int GetCurrentWaveNumber()
    {
        return currentWaveIndex + 1;
    }
    
    /// <summary>
    /// 是否有波次正在进行
    /// </summary>
    public bool IsWaveActive()
    {
        return isWaveActive;
    }
    
    /// <summary>
    /// 关卡是否完成
    /// </summary>
    public bool IsLevelComplete()
    {
        return isLevelComplete;
    }
    
    /// <summary>
    /// 获取当前关卡配置
    /// </summary>
    public LevelConfig GetCurrentLevelConfig()
    {
        return levelConfig;
    }
    
    /// <summary>
    /// 获取当前波次配置
    /// </summary>
    public WaveConfig GetCurrentWaveConfig()
    {
        if (levelConfig != null && currentWaveIndex >= 0)
        {
            return levelConfig.GetWave(currentWaveIndex);
        }
        return null;
    }
    
    /// <summary>
    /// 获取生成控制器
    /// </summary>
    public SpawnEntityController GetSpawnController()
    {
        return spawnController;
    }
    
    #endregion
} 