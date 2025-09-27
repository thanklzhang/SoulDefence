using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 生成控制器
/// 负责怪物生成位置选择和生成逻辑
/// </summary>
public class SpawnEntityController : MonoBehaviour
{
    [Header("生成设置")]
    [SerializeField] private int avoidSamePointCount = 3;   // 避免连续使用同一出怪点的次数
    
    // 出怪点使用历史
    private Queue<SpawnPoint> recentUsedPoints = new Queue<SpawnPoint>();
    
    // 事件
    public System.Action<Vector3, Quaternion, string> OnMonsterSpawn;
    public System.Action<SpawnPoint> OnSpawnPointSelected;
    
    /// <summary>
    /// 选择出怪点
    /// </summary>
    /// <returns>选中的出怪点，如果没有可用点返回null</returns>
    public SpawnPoint SelectSpawnPoint()
    {
        if (SpawnPointManager.Instance == null)
        {
            Debug.LogWarning("[SpawnController] SpawnPointManager未找到！");
            return null;
        }
        
        // 获取所有激活的出怪点
        List<SpawnPoint> activePoints = SpawnPointManager.Instance.GetActiveSpawnPoints();
        
        if (activePoints.Count == 0)
        {
            Debug.LogWarning("[SpawnController] 没有可用的出怪点！");
            return null;
        }
        
        // 如果只有一个出怪点，直接返回
        if (activePoints.Count == 1)
        {
            return activePoints[0];
        }
        
        // 过滤掉最近使用过的出怪点
        List<SpawnPoint> availablePoints = new List<SpawnPoint>();
        foreach (var point in activePoints)
        {
            if (!recentUsedPoints.Contains(point))
            {
                availablePoints.Add(point);
            }
        }
        
        // 如果没有可用点，清空历史记录重新选择
        if (availablePoints.Count == 0)
        {
            Debug.Log("[SpawnController] 所有出怪点都被使用过，重置历史记录");
            recentUsedPoints.Clear();
            availablePoints = activePoints;
        }
        
        // 随机选择一个可用的出怪点
        SpawnPoint selectedPoint = availablePoints[Random.Range(0, availablePoints.Count)];
        
        // 记录使用历史
        RecordUsedPoint(selectedPoint);
        
        // 触发选中事件
        OnSpawnPointSelected?.Invoke(selectedPoint);
        
        Debug.Log($"[SpawnController] 选中出怪点: {selectedPoint.pointName}");
        return selectedPoint;
    }
    
    /// <summary>
    /// 记录使用过的出怪点
    /// </summary>
    /// <param name="point">使用的出怪点</param>
    private void RecordUsedPoint(SpawnPoint point)
    {
        recentUsedPoints.Enqueue(point);
        
        // 保持队列大小不超过设定值
        while (recentUsedPoints.Count > avoidSamePointCount)
        {
            recentUsedPoints.Dequeue();
        }
    }
    
    /// <summary>
    /// 生成怪物
    /// </summary>
    /// <param name="monsterType">怪物类型</param>
    /// <param name="spawnPoint">生成位置</param>
    public void SpawnMonster(string monsterType, SpawnPoint spawnPoint)
    {
        if (spawnPoint == null)
        {
            Debug.LogError("[SpawnController] 出怪点为空，无法生成怪物！");
            return;
        }
        
        Vector3 spawnPosition = spawnPoint.position;
        Quaternion spawnRotation = spawnPoint.GetRotation();
        
        // 触发生成事件（实际的怪物实例化由其他系统处理）
        OnMonsterSpawn?.Invoke(spawnPosition, spawnRotation, monsterType);
        
        Debug.Log($"[SpawnController] 在 {spawnPoint.pointName} 生成怪物: {monsterType}");
    }
    
    /// <summary>
    /// 生成一波怪物
    /// </summary>
    /// <param name="waveConfig">波次配置</param>
    /// <returns>生成协程</returns>
    public Coroutine SpawnWave(WaveConfig waveConfig)
    {
        return StartCoroutine(SpawnWaveCoroutine(waveConfig));
    }
    
    /// <summary>
    /// 生成波次的协程
    /// </summary>
    private IEnumerator SpawnWaveCoroutine(WaveConfig waveConfig)
    {
        Debug.Log($"[SpawnController] 开始生成 {waveConfig.waveName}，数量: {waveConfig.monsterCount}");
        
        for (int i = 0; i < waveConfig.monsterCount; i++)
        {
            // 选择出怪点
            SpawnPoint spawnPoint = SelectSpawnPoint();
            
            if (spawnPoint != null)
            {
                // 生成怪物
                SpawnMonster(waveConfig.monsterType, spawnPoint);
            }
            else
            {
                Debug.LogWarning($"[SpawnController] 第{i+1}只怪物生成失败：无可用出怪点");
            }
            
            // 等待生成间隔
            if (i < waveConfig.monsterCount - 1) // 最后一只怪物不需要等待
            {
                yield return new WaitForSeconds(waveConfig.spawnInterval);
            }
        }
        
        Debug.Log($"[SpawnController] {waveConfig.waveName} 生成完毕");
    }
    
    /// <summary>
    /// 停止所有生成
    /// </summary>
    public void StopAllSpawning()
    {
        StopAllCoroutines();
        Debug.Log("[SpawnController] 停止所有怪物生成");
    }
    
    /// <summary>
    /// 重置生成控制器
    /// </summary>
    public void Reset()
    {
        StopAllSpawning();
        recentUsedPoints.Clear();
        Debug.Log("[SpawnController] 重置生成控制器");
    }
    
    /// <summary>
    /// 获取最近使用的出怪点数量
    /// </summary>
    public int GetRecentUsedPointCount()
    {
        return recentUsedPoints.Count;
    }
    
    /// <summary>
    /// 设置避免重复使用的次数
    /// </summary>
    /// <param name="count">次数</param>
    public void SetAvoidSamePointCount(int count)
    {
        avoidSamePointCount = Mathf.Max(1, count);
    }
} 