using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 出怪点配置
/// </summary>
[System.Serializable]
public class SpawnPoint
{
    [Header("出怪点基本信息")]
    public string pointName = "SpawnPoint";         // 出怪点名称
    public Vector3 position = Vector3.zero;         // 出怪点位置
    public Vector3 rotation = Vector3.zero;         // 出怪点朝向（欧拉角）
    
    [Header("出怪点设置")]
    public bool isActive = true;                    // 是否激活
    public Color pointColor = Color.red;            // 出怪点颜色
    public Color centerBallColor = Color.white;     // 中心小球颜色
    
    [Header("可选设置")]
    [Range(0.5f, 5f)]
    public float pointSize = 1f;                    // 出怪点显示大小
    public string description = "";                 // 出怪点描述
    
    /// <summary>
    /// 获取旋转四元数
    /// </summary>
    public Quaternion GetRotation()
    {
        return Quaternion.Euler(rotation);
    }
}

/// <summary>
/// 出怪点管理器
/// 管理场景中的所有出怪点
/// </summary>
public class SpawnPointManager : MonoBehaviour
{
    [Header("出怪点设置")]
    [SerializeField] private bool showSpawnPoints = true;           // 是否显示出怪点
    [SerializeField] private List<SpawnPoint> spawnPoints = new List<SpawnPoint>();  // 出怪点列表

    [Header("拖拽限制设置")]
    [SerializeField] private bool lockYAxis = false;                // 是否锁定Y轴
    [SerializeField] private float fixedYValue = 0f;                // 固定的Y轴值

    // 静态实例，方便其他脚本访问
    public static SpawnPointManager Instance { get; private set; }

    void Awake()
    {
        // 设置单例
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("场景中存在多个SpawnPointManager实例！");
            Destroy(gameObject);
        }
    }

    void Start()
    {
        InitializeSpawnPoints();
    }

    /// <summary>
    /// 初始化出怪点
    /// </summary>
    private void InitializeSpawnPoints()
    {
        Debug.Log($"出怪点管理器初始化完成 - 出怪点数量: {spawnPoints.Count}");
        
        // 为每个出怪点分配默认名称（如果没有设置）
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            if (string.IsNullOrEmpty(spawnPoints[i].pointName) || spawnPoints[i].pointName == "SpawnPoint")
            {
                spawnPoints[i].pointName = $"SpawnPoint_{i + 1}";
            }
        }
    }

    #region 出怪点管理接口

    /// <summary>
    /// 获取所有出怪点
    /// </summary>
    /// <returns>出怪点列表</returns>
    public List<SpawnPoint> GetAllSpawnPoints()
    {
        return new List<SpawnPoint>(spawnPoints);
    }

    /// <summary>
    /// 获取激活的出怪点
    /// </summary>
    /// <returns>激活的出怪点列表</returns>
    public List<SpawnPoint> GetActiveSpawnPoints()
    {
        return spawnPoints.FindAll(point => point.isActive);
    }

    /// <summary>
    /// 根据名称获取出怪点
    /// </summary>
    /// <param name="pointName">出怪点名称</param>
    /// <returns>找到的出怪点，如果没找到返回null</returns>
    public SpawnPoint GetSpawnPointByName(string pointName)
    {
        return spawnPoints.Find(point => point.pointName == pointName);
    }

    /// <summary>
    /// 根据索引获取出怪点
    /// </summary>
    /// <param name="index">索引</param>
    /// <returns>出怪点，如果索引无效返回null</returns>
    public SpawnPoint GetSpawnPointByIndex(int index)
    {
        if (index >= 0 && index < spawnPoints.Count)
        {
            return spawnPoints[index];
        }
        return null;
    }

    /// <summary>
    /// 获取随机的激活出怪点
    /// </summary>
    /// <returns>随机出怪点，如果没有激活的出怪点返回null</returns>
    public SpawnPoint GetRandomActiveSpawnPoint()
    {
        var activePoints = GetActiveSpawnPoints();
        if (activePoints.Count > 0)
        {
            int randomIndex = Random.Range(0, activePoints.Count);
            return activePoints[randomIndex];
        }
        return null;
    }

    /// <summary>
    /// 添加出怪点
    /// </summary>
    /// <param name="spawnPoint">要添加的出怪点</param>
    public void AddSpawnPoint(SpawnPoint spawnPoint)
    {
        if (spawnPoint != null)
        {
            spawnPoints.Add(spawnPoint);
        }
    }

    /// <summary>
    /// 移除出怪点
    /// </summary>
    /// <param name="pointName">要移除的出怪点名称</param>
    /// <returns>是否成功移除</returns>
    public bool RemoveSpawnPoint(string pointName)
    {
        var point = GetSpawnPointByName(pointName);
        if (point != null)
        {
            spawnPoints.Remove(point);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 设置出怪点激活状态
    /// </summary>
    /// <param name="pointName">出怪点名称</param>
    /// <param name="isActive">是否激活</param>
    /// <returns>是否成功设置</returns>
    public bool SetSpawnPointActive(string pointName, bool isActive)
    {
        var point = GetSpawnPointByName(pointName);
        if (point != null)
        {
            point.isActive = isActive;
            return true;
        }
        return false;
    }

    /// <summary>
    /// 获取出怪点数量
    /// </summary>
    /// <returns>出怪点总数</returns>
    public int GetSpawnPointCount()
    {
        return spawnPoints.Count;
    }

    /// <summary>
    /// 获取激活的出怪点数量
    /// </summary>
    /// <returns>激活的出怪点数量</returns>
    public int GetActiveSpawnPointCount()
    {
        return GetActiveSpawnPoints().Count;
    }

    /// <summary>
    /// 设置是否显示出怪点
    /// </summary>
    /// <param name="show">是否显示</param>
    public void SetShowSpawnPoints(bool show)
    {
        showSpawnPoints = show;
    }

    /// <summary>
    /// 获取是否锁定Y轴
    /// </summary>
    public bool IsYAxisLocked => lockYAxis;

    /// <summary>
    /// 获取固定的Y轴值
    /// </summary>
    public float FixedYValue => fixedYValue;

    /// <summary>
    /// 设置Y轴锁定状态
    /// </summary>
    /// <param name="locked">是否锁定</param>
    public void SetYAxisLocked(bool locked)
    {
        lockYAxis = locked;
    }

    /// <summary>
    /// 设置固定的Y轴值
    /// </summary>
    /// <param name="yValue">Y轴值</param>
    public void SetFixedYValue(float yValue)
    {
        fixedYValue = yValue;
    }

    #endregion

    #region 调试和可视化

    void OnDrawGizmos()
    {
        if (!showSpawnPoints) return;

        DrawSpawnPoints();
    }

    void OnDrawGizmosSelected()
    {
        if (!showSpawnPoints) return;

        DrawSpawnPointsSelected();
    }

    /// <summary>
    /// 绘制出怪点
    /// </summary>
    private void DrawSpawnPoints()
    {
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            var point = spawnPoints[i];
            if (point == null) continue;

            // 设置颜色（激活的更亮，未激活的更暗）
            Color pointColor = point.isActive ? point.pointColor : point.pointColor * 0.5f;
            
            // 绘制出怪点主体（实心球）
            Gizmos.color = pointColor;
            Gizmos.DrawSphere(point.position, point.pointSize * 0.3f);
            
            // 绘制出怪点边框（线框球）
            Gizmos.color = pointColor * 1.2f;
            Gizmos.DrawWireSphere(point.position, point.pointSize * 0.5f);

            // 绘制中心小球装饰
            Gizmos.color = point.isActive ? point.centerBallColor : Color.gray;
            Gizmos.DrawSphere(point.position, point.pointSize * 0.1f);

            // 绘制朝向箭头
            DrawArrow(point.position, point.GetRotation() * Vector3.forward, point.pointSize, pointColor * 0.8f);

            // 绘制出怪点标签（简化版）
            #if UNITY_EDITOR
            Vector3 labelPos = point.position + Vector3.up * (point.pointSize + 1f);
            string statusText = point.isActive ? "" : " (Inactive)";
            string labelText = $"{point.pointName}{statusText}";
            
            // 使用简单的标签显示
            UnityEditor.Handles.Label(labelPos, labelText);
            #endif
        }
    }

    /// <summary>
    /// 绘制选中时的出怪点详细信息
    /// </summary>
    private void DrawSpawnPointsSelected()
    {
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            var point = spawnPoints[i];
            if (point == null) continue;

            // 绘制出怪点范围圈
            Gizmos.color = point.pointColor * 0.3f;
            Gizmos.DrawWireSphere(point.position, point.pointSize);

            // 绘制更大的范围圈
            Gizmos.color = point.pointColor * 0.1f;
            Gizmos.DrawWireSphere(point.position, point.pointSize * 1.5f);

            // 绘制坐标轴
            float axisLength = point.pointSize * 0.8f;
            Quaternion rotation = point.GetRotation();
            
            // X轴 - 红色
            Gizmos.color = Color.red;
            Vector3 rightDir = rotation * Vector3.right * axisLength;
            Gizmos.DrawLine(point.position, point.position + rightDir);
            
            // Y轴 - 绿色
            Gizmos.color = Color.green;
            Vector3 upDir = rotation * Vector3.up * axisLength;
            Gizmos.DrawLine(point.position, point.position + upDir);
            
            // Z轴 - 蓝色（朝向）
            Gizmos.color = Color.blue;
            Vector3 forwardDir = rotation * Vector3.forward * axisLength;
            Gizmos.DrawLine(point.position, point.position + forwardDir);

            #if UNITY_EDITOR
            // 绘制索引号
            Vector3 indexPos = point.position + Vector3.down * (point.pointSize * 0.5f + 0.5f);
            UnityEditor.Handles.Label(indexPos, $"[{i}]");
            
            // 绘制描述信息
            if (!string.IsNullOrEmpty(point.description))
            {
                Vector3 descPos = point.position + Vector3.up * (point.pointSize + 2f);
                UnityEditor.Handles.Label(descPos, point.description);
            }

            // 绘制位置坐标信息
            Vector3 posInfoPos = point.position + Vector3.down * (point.pointSize + 1f);
            string posInfo = $"({point.position.x:F1}, {point.position.y:F1}, {point.position.z:F1})";
            UnityEditor.Handles.Label(posInfoPos, posInfo);
            #endif
        }
    }

    /// <summary>
    /// 绘制箭头
    /// </summary>
    /// <param name="position">起始位置</param>
    /// <param name="direction">方向</param>
    /// <param name="size">大小</param>
    /// <param name="color">颜色</param>
    private void DrawArrow(Vector3 position, Vector3 direction, float size, Color color)
    {
        if (direction.magnitude < 0.001f) return;

        Gizmos.color = color;
        
        Vector3 arrowEnd = position + direction.normalized * size * 0.8f;
        
        // 绘制箭头主线
        Gizmos.DrawLine(position, arrowEnd);
        
        // 计算箭头头部
        Vector3 right = Vector3.Cross(direction.normalized, Vector3.up);
        if (right.magnitude < 0.001f)
        {
            right = Vector3.Cross(direction.normalized, Vector3.forward);
        }
        right = right.normalized;
        
        Vector3 up = Vector3.Cross(right, direction.normalized).normalized;
        
        float arrowHeadSize = size * 0.3f;
        Vector3 arrowLeft = arrowEnd - direction.normalized * arrowHeadSize + right * arrowHeadSize * 0.5f;
        Vector3 arrowRight = arrowEnd - direction.normalized * arrowHeadSize - right * arrowHeadSize * 0.5f;
        Vector3 arrowUp = arrowEnd - direction.normalized * arrowHeadSize + up * arrowHeadSize * 0.5f;
        Vector3 arrowDown = arrowEnd - direction.normalized * arrowHeadSize - up * arrowHeadSize * 0.5f;
        
        // 绘制箭头头部
        Gizmos.DrawLine(arrowEnd, arrowLeft);
        Gizmos.DrawLine(arrowEnd, arrowRight);
        Gizmos.DrawLine(arrowEnd, arrowUp);
        Gizmos.DrawLine(arrowEnd, arrowDown);
        
        // 绘制箭头头部的小圆
        Gizmos.DrawWireSphere(arrowEnd, size * 0.1f);
    }

    #endregion
} 