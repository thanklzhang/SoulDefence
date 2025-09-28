using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 关卡调试测试器
/// 用于可视化和测试关卡流程系统
/// </summary>
public class DebugLevelTester : MonoBehaviour
{
    [Header("测试设置")]
    [SerializeField] private bool enableVisualization = true;   // 是否启用可视化
    [SerializeField] private bool createTestMonsters = true;    // 是否创建测试怪物
    [SerializeField] private GameObject testMonsterPrefab;      // 测试怪物预制体
    
    [Header("可视化设置")]
    [SerializeField] private float monsterLifetime = 10f;       // 测试怪物存活时间
    [SerializeField] private Color spawnEffectColor = Color.green; // 生成特效颜色
    
    // 生成的测试怪物列表
    private List<GameObject> spawnedMonsters = new List<GameObject>();
    
    void Start()
    {
        // 订阅LevelManager的事件
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnLevelStart += HandleLevelStart;
            LevelManager.Instance.OnLevelComplete += HandleLevelComplete;
            LevelManager.Instance.OnWaveStart += HandleWaveStart;
            LevelManager.Instance.OnWaveComplete += HandleWaveComplete;
            LevelManager.Instance.OnMonsterSpawn += HandleMonsterSpawn;
        }
        else
        {
            Debug.LogWarning("[DebugLevelTester] 未找到LevelManager实例");
        }
    }
    
    void OnDestroy()
    {
        // 取消订阅事件
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnLevelStart -= HandleLevelStart;
            LevelManager.Instance.OnLevelComplete -= HandleLevelComplete;
            LevelManager.Instance.OnWaveStart -= HandleWaveStart;
            LevelManager.Instance.OnWaveComplete -= HandleWaveComplete;
            LevelManager.Instance.OnMonsterSpawn -= HandleMonsterSpawn;
        }
    }
    
    #region 事件处理
    
    private void HandleLevelStart(LevelConfig levelConfig)
    {
        Debug.Log($"[DebugLevelTester] 关卡开始: {levelConfig.levelName}");
        ClearAllTestMonsters();
    }
    
    private void HandleLevelComplete(LevelConfig levelConfig)
    {
        Debug.Log($"[DebugLevelTester] 关卡完成: {levelConfig.levelName}");
        
        if (enableVisualization)
        {
            // 可以在这里添加关卡完成的特效
            StartCoroutine(ShowLevelCompleteEffect());
        }
    }
    
    private void HandleWaveStart(int waveIndex, WaveConfig waveConfig)
    {
        Debug.Log($"[DebugLevelTester] 波次开始: {waveConfig.waveName}");
    }
    
    private void HandleWaveComplete(int waveIndex, WaveConfig waveConfig)
    {
        Debug.Log($"[DebugLevelTester] 波次完成: {waveConfig.waveName}");
    }
    
    private void HandleMonsterSpawn(Vector3 position, Quaternion rotation, string monsterType)
    {
        Debug.Log($"[DebugLevelTester] 怪物生成: {monsterType} at {position}");
        
        if (enableVisualization)
        {
            // 显示生成特效
            StartCoroutine(ShowSpawnEffect(position));
        }
        
        if (createTestMonsters)
        {
            CreateTestMonster(position, rotation, monsterType);
        }
    }
    
    #endregion
    
    #region 测试怪物创建
    
    /// <summary>
    /// 创建测试怪物
    /// </summary>
    private void CreateTestMonster(Vector3 position, Quaternion rotation, string monsterType)
    {
        GameObject monster = null;
        
        if (testMonsterPrefab != null)
        {
            // 使用指定的预制体
            monster = Instantiate(testMonsterPrefab, position, rotation);
        }
        else
        {
            // 创建简单的立方体作为测试怪物
            monster = GameObject.CreatePrimitive(PrimitiveType.Cube);
            monster.transform.position = position;
            monster.transform.rotation = rotation;
            
            // 根据怪物类型设置颜色
            Renderer renderer = monster.GetComponent<Renderer>();
            if (renderer != null)
            {
                switch (monsterType.ToLower())
                {
                    case "smallenemy":
                        renderer.material.color = Color.red;
                        monster.transform.localScale = Vector3.one * 0.8f;
                        break;
                    case "bigenemy":
                        renderer.material.color = Color.blue;
                        monster.transform.localScale = Vector3.one * 1.5f;
                        break;
                    case "boss":
                        renderer.material.color = Color.black;
                        monster.transform.localScale = Vector3.one * 2f;
                        break;
                    default:
                        renderer.material.color = Color.gray;
                        break;
                }
            }
        }
        
        // 设置名称
        monster.name = $"{monsterType}_{spawnedMonsters.Count + 1}";
        
        // 添加到列表
        spawnedMonsters.Add(monster);
        
        // 设置自动销毁
        if (monsterLifetime > 0)
        {
            //StartCoroutine(DestroyMonsterAfterTime(monster, monsterLifetime));
        }
    }
    
    /// <summary>
    /// 延迟销毁怪物
    /// </summary>
    private IEnumerator DestroyMonsterAfterTime(GameObject monster, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (monster != null)
        {
            spawnedMonsters.Remove(monster);
            Destroy(monster);
        }
    }
    
    /// <summary>
    /// 清除所有测试怪物
    /// </summary>
    public void ClearAllTestMonsters()
    {
        foreach (var monster in spawnedMonsters)
        {
            if (monster != null)
            {
                Destroy(monster);
            }
        }
        spawnedMonsters.Clear();
        Debug.Log("[DebugLevelTester] 清除所有测试怪物");
    }
    
    #endregion
    
    #region 可视化效果
    
    /// <summary>
    /// 显示生成特效
    /// </summary>
    private IEnumerator ShowSpawnEffect(Vector3 position)
    {
        // 创建简单的生成特效
        GameObject effect = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        effect.name = "SpawnEffect";
        effect.transform.position = position + Vector3.up * 0.5f;
        effect.transform.localScale = Vector3.zero;
        
        Renderer renderer = effect.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = spawnEffectColor;
        }
        
        // 移除碰撞器
        Collider collider = effect.GetComponent<Collider>();
        if (collider != null)
        {
            Destroy(collider);
        }
        
        // 缩放动画
        float duration = 0.5f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            float progress = elapsed / duration;
            float scale = Mathf.Sin(progress * Mathf.PI) * 2f;
            effect.transform.localScale = Vector3.one * scale;
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        Destroy(effect);
    }
    
    /// <summary>
    /// 显示关卡完成特效
    /// </summary>
    private IEnumerator ShowLevelCompleteEffect()
    {
        Debug.Log("[DebugLevelTester] 显示关卡完成特效");
        
        // 这里可以添加更复杂的完成特效
        // 目前只是简单的日志输出
        
        yield return new WaitForSeconds(1f);
    }
    
    #endregion
    
    #region 调试功能
    
    [ContextMenu("清除测试怪物")]
    public void ClearTestMonsters()
    {
        ClearAllTestMonsters();
    }
    
    /// <summary>
    /// 获取当前生成的怪物数量
    /// </summary>
    public int GetSpawnedMonsterCount()
    {
        // 清理已销毁的对象
        spawnedMonsters.RemoveAll(monster => monster == null);
        return spawnedMonsters.Count;
    }
    
    /// <summary>
    /// 设置测试怪物预制体
    /// </summary>
    public void SetTestMonsterPrefab(GameObject prefab)
    {
        testMonsterPrefab = prefab;
    }
    
    #endregion
    
    void OnDrawGizmos()
    {
        if (!enableVisualization) return;
        
        // 显示当前生成的怪物数量信息
        if (Application.isPlaying)
        {
            Gizmos.color = Color.white;
            Vector3 textPos = transform.position + Vector3.up * 3f;
            
            // 这里可以用Handles.Label显示更详细的信息
            // 但为了简化，我们只在Scene视图中显示基本信息
        }
    }
} 