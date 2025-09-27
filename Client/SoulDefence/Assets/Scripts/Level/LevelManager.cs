using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 关卡管理器
/// 关卡流程系统的统一入口
/// </summary>
public class LevelManager : MonoBehaviour
{
    [Header("关卡配置")]
    [SerializeField] private LevelConfig currentLevelConfig;    // 当前关卡配置
    [SerializeField] private bool autoStartOnAwake = false;     // 是否自动开始关卡
    
    [Header("调试设置")]
    [SerializeField] private bool showDebugInfo = true;         // 是否显示调试信息
    
    // 组件引用
    private WaveManager waveManager;
    
    // 静态实例
    public static LevelManager Instance { get; private set; }
    
    // 关卡事件
    public System.Action<LevelConfig> OnLevelStart;             // 关卡开始
    public System.Action<LevelConfig> OnLevelComplete;          // 关卡完成
    public System.Action<int, WaveConfig> OnWaveStart;          // 波次开始
    public System.Action<int, WaveConfig> OnWaveComplete;       // 波次完成
    public System.Action<Vector3, Quaternion, string> OnMonsterSpawn; // 怪物生成
    
    void Awake()
    {
        // 设置单例
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("场景中存在多个LevelManager实例！");
            Destroy(gameObject);
            return;
        }
        
        // 获取或创建WaveManager
        waveManager = GetComponent<WaveManager>();
        if (waveManager == null)
        {
            waveManager = gameObject.AddComponent<WaveManager>();
        }
        
        // 订阅事件
        SubscribeToEvents();
    }
    
    void Start()
    {
        if (autoStartOnAwake && currentLevelConfig != null)
        {
            StartLevel(currentLevelConfig);
        }
    }
    
    void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
    
    /// <summary>
    /// 订阅事件
    /// </summary>
    private void SubscribeToEvents()
    {
        if (waveManager != null)
        {
            waveManager.OnWaveStart += HandleWaveStart;
            waveManager.OnWaveComplete += HandleWaveComplete;
            waveManager.OnAllWavesComplete += HandleLevelComplete;
            
            var spawnEntityController = waveManager.GetSpawnController();
            if (spawnEntityController != null)
            {
                spawnEntityController.OnMonsterSpawn += HandleMonsterSpawn;
            }
        }
    }
    
    /// <summary>
    /// 取消订阅事件
    /// </summary>
    private void UnsubscribeFromEvents()
    {
        if (waveManager != null)
        {
            waveManager.OnWaveStart -= HandleWaveStart;
            waveManager.OnWaveComplete -= HandleWaveComplete;
            waveManager.OnAllWavesComplete -= HandleLevelComplete;
            
            var spawnEntityController = waveManager.GetSpawnController();
            if (spawnEntityController != null)
            {
                spawnEntityController.OnMonsterSpawn -= HandleMonsterSpawn;
            }
        }
    }
    
    /// <summary>
    /// 开始关卡
    /// </summary>
    /// <param name="levelConfig">关卡配置</param>
    public void StartLevel(LevelConfig levelConfig)
    {
        if (levelConfig == null)
        {
            Debug.LogError("[LevelManager] 关卡配置为空！");
            return;
        }
        
        currentLevelConfig = levelConfig;
        
        if (showDebugInfo)
        {
            Debug.Log($"[LevelManager] 开始关卡: {levelConfig.levelName}");
            Debug.Log($"[LevelManager] 总怪物数量: {levelConfig.GetTotalMonsterCount()}");
        }
        
        // 触发关卡开始事件
        OnLevelStart?.Invoke(levelConfig);
        
        // 开始波次管理
        waveManager.StartLevel(levelConfig);
    }
    
    /// <summary>
    /// 停止当前关卡
    /// </summary>
    public void StopLevel()
    {
        waveManager?.StopLevel();
        
        if (showDebugInfo)
        {
            Debug.Log("[LevelManager] 停止关卡");
        }
    }
    
    /// <summary>
    /// 重置关卡管理器
    /// </summary>
    public void ResetLevel()
    {
        waveManager?.Reset();
        
        if (showDebugInfo)
        {
            Debug.Log("[LevelManager] 重置关卡");
        }
    }
    
    /// <summary>
    /// 重新开始当前关卡
    /// </summary>
    public void RestartLevel()
    {
        if (currentLevelConfig != null)
        {
            ResetLevel();
            StartLevel(currentLevelConfig);
        }
        else
        {
            Debug.LogWarning("[LevelManager] 没有当前关卡配置，无法重新开始");
        }
    }
    
    #region 事件处理
    
    private void HandleWaveStart(int waveIndex, WaveConfig waveConfig)
    {
        if (showDebugInfo)
        {
            Debug.Log($"[LevelManager] 波次开始: {waveConfig.waveName} ({waveConfig.monsterCount}只{waveConfig.monsterType})");
        }
        
        OnWaveStart?.Invoke(waveIndex, waveConfig);
    }
    
    private void HandleWaveComplete(int waveIndex, WaveConfig waveConfig)
    {
        if (showDebugInfo)
        {
            Debug.Log($"[LevelManager] 波次完成: {waveConfig.waveName}");
        }
        
        OnWaveComplete?.Invoke(waveIndex, waveConfig);
    }
    
    private void HandleLevelComplete()
    {
        if (showDebugInfo)
        {
            Debug.Log($"[LevelManager] 关卡完成: {currentLevelConfig?.levelName}");
        }
        
        OnLevelComplete?.Invoke(currentLevelConfig);
    }
    
    private void HandleMonsterSpawn(Vector3 position, Quaternion rotation, string monsterType)
    {
        if (showDebugInfo)
        {
            Debug.Log($"[LevelManager] 生成怪物: {monsterType} at {position}");
        }
        
        // 这里可以实际创建怪物实例
        // 目前只是触发事件，让其他系统处理实际的怪物创建
        OnMonsterSpawn?.Invoke(position, rotation, monsterType);
    }
    
    #endregion
    
    #region 状态查询
    
    /// <summary>
    /// 获取当前关卡配置
    /// </summary>
    public LevelConfig GetCurrentLevelConfig()
    {
        return currentLevelConfig;
    }
    
    /// <summary>
    /// 获取当前波次编号
    /// </summary>
    public int GetCurrentWaveNumber()
    {
        return waveManager?.GetCurrentWaveNumber() ?? 0;
    }
    
    /// <summary>
    /// 是否有波次正在进行
    /// </summary>
    public bool IsWaveActive()
    {
        return waveManager?.IsWaveActive() ?? false;
    }
    
    /// <summary>
    /// 关卡是否完成
    /// </summary>
    public bool IsLevelComplete()
    {
        return waveManager?.IsLevelComplete() ?? false;
    }
    
    /// <summary>
    /// 获取波次管理器
    /// </summary>
    public WaveManager GetWaveManager()
    {
        return waveManager;
    }
    
    #endregion
    
    #region 调试功能
    
    [ContextMenu("开始测试关卡")]
    public void StartTestLevel()
    {
        if (currentLevelConfig != null)
        {
            StartLevel(currentLevelConfig);
        }
        else
        {
            Debug.LogWarning("[LevelManager] 没有设置关卡配置");
        }
    }
    
    [ContextMenu("停止关卡")]
    public void StopTestLevel()
    {
        StopLevel();
    }
    
    [ContextMenu("重置关卡")]
    public void ResetTestLevel()
    {
        ResetLevel();
    }
    
    [ContextMenu("强制下一波")]
    public void ForceNextWave()
    {
        waveManager?.ForceNextWave();
    }
    
    #endregion
} 