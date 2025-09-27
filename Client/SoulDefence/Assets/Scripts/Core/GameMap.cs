using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏地图总管理器
/// 统一管理地图的各个系统
/// </summary>
public class GameMap : MonoBehaviour
{
    [Header("地图系统组件")]
    [SerializeField] private MapBoundary mapBoundary;           // 地图边界系统
    [SerializeField] private SpawnPointManager spawnManager;   // 出怪点管理系统

    // 静态实例，方便其他脚本访问
    public static GameMap Instance { get; private set; }

    void Awake()
    {
        // 设置单例
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("场景中存在多个GameMap实例！");
            Destroy(gameObject);
        }
    }

    void Start()
    {
        InitializeMap();
    }

    /// <summary>
    /// 初始化地图
    /// </summary>
    private void InitializeMap()
    {
        // 自动查找地图系统组件
        if (mapBoundary == null)
        {
            mapBoundary = FindObjectOfType<MapBoundary>();
        }

        if (spawnManager == null)
        {
            spawnManager = FindObjectOfType<SpawnPointManager>();
        }

        // 输出初始化信息
        Debug.Log("=== 游戏地图初始化完成 ===");
        Debug.Log($"边界系统: {(mapBoundary != null ? "已加载" : "未找到")}");
        Debug.Log($"出怪点系统: {(spawnManager != null ? "已加载" : "未找到")}");
        
        if (mapBoundary != null)
        {
            var boundsInfo = mapBoundary.GetBoundsInfo();
            Debug.Log($"地图边界: {boundsInfo.type} - 中心: {boundsInfo.center}");
        }
        
        if (spawnManager != null)
        {
            Debug.Log($"出怪点数量: {spawnManager.GetSpawnPointCount()} (激活: {spawnManager.GetActiveSpawnPointCount()})");
        }
    }

    #region 边界系统快捷访问

    /// <summary>
    /// 检查位置是否在地图边界内
    /// </summary>
    /// <param name="position">要检查的位置</param>
    /// <returns>是否在边界内</returns>
    public bool IsPositionInBounds(Vector3 position)
    {
        if (mapBoundary != null)
        {
            return mapBoundary.IsPositionInBounds(position);
        }
        return true; // 如果没有边界系统，默认允许
    }

    /// <summary>
    /// 将位置限制在边界内
    /// </summary>
    /// <param name="position">原始位置</param>
    /// <returns>限制后的位置</returns>
    public Vector3 ClampPositionToBounds(Vector3 position)
    {
        if (mapBoundary != null)
        {
            return mapBoundary.ClampPositionToBounds(position);
        }
        return position; // 如果没有边界系统，返回原位置
    }

    /// <summary>
    /// 获取边界系统
    /// </summary>
    public MapBoundary GetMapBoundary()
    {
        return mapBoundary;
    }

    #endregion

    #region 出怪点系统快捷访问

    /// <summary>
    /// 获取随机的激活出怪点
    /// </summary>
    /// <returns>随机出怪点，如果没有出怪点系统或激活的出怪点返回null</returns>
    public SpawnPoint GetRandomActiveSpawnPoint()
    {
        if (spawnManager != null)
        {
            return spawnManager.GetRandomActiveSpawnPoint();
        }
        return null;
    }

    /// <summary>
    /// 根据名称获取出怪点
    /// </summary>
    /// <param name="pointName">出怪点名称</param>
    /// <returns>找到的出怪点，如果没找到返回null</returns>
    public SpawnPoint GetSpawnPointByName(string pointName)
    {
        if (spawnManager != null)
        {
            return spawnManager.GetSpawnPointByName(pointName);
        }
        return null;
    }

    /// <summary>
    /// 获取所有激活的出怪点
    /// </summary>
    /// <returns>激活的出怪点列表</returns>
    public List<SpawnPoint> GetActiveSpawnPoints()
    {
        if (spawnManager != null)
        {
            return spawnManager.GetActiveSpawnPoints();
        }
        return new List<SpawnPoint>();
    }

    /// <summary>
    /// 获取出怪点管理器
    /// </summary>
    public SpawnPointManager GetSpawnPointManager()
    {
        return spawnManager;
    }

    #endregion

    #region 公共接口

    /// <summary>
    /// 设置地图边界系统
    /// </summary>
    /// <param name="boundary">边界系统组件</param>
    public void SetMapBoundary(MapBoundary boundary)
    {
        mapBoundary = boundary;
    }

    /// <summary>
    /// 设置出怪点管理系统
    /// </summary>
    /// <param name="manager">出怪点管理器组件</param>
    public void SetSpawnPointManager(SpawnPointManager manager)
    {
        spawnManager = manager;
    }

    /// <summary>
    /// 获取地图系统状态
    /// </summary>
    /// <returns>系统状态信息</returns>
    public (bool hasBoundary, bool hasSpawnManager, int spawnPointCount) GetMapSystemStatus()
    {
        bool hasBoundary = mapBoundary != null;
        bool hasSpawnManager = spawnManager != null;
        int spawnPointCount = hasSpawnManager ? spawnManager.GetSpawnPointCount() : 0;
        
        return (hasBoundary, hasSpawnManager, spawnPointCount);
    }

    #endregion
} 