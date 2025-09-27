using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 地图边界系统
/// 管理地图边界和实体移动限制
/// </summary>
public class MapBoundary : MonoBehaviour
{
    [Header("地图边界设置")]
    [SerializeField] private Vector3 mapCenter = Vector3.zero;      // 地图中心点
    [SerializeField] private Vector3 mapSize = new Vector3(20, 10, 20);  // 地图大小 (长, 高, 宽)
    [SerializeField] private bool showBounds = true;                // 是否显示边界
    [SerializeField] private Color boundsColor = Color.green;       // 边界颜色

    [Header("边界类型")]
    [SerializeField] private BoundaryType boundaryType = BoundaryType.Box;  // 边界类型

    [Header("圆形边界设置 (仅当边界类型为Circle时)")]
    [SerializeField] private float circleRadius = 10f;             // 圆形边界半径

    // 静态实例，方便其他脚本访问
    public static MapBoundary Instance { get; private set; }

    // 边界类型枚举
    public enum BoundaryType
    {
        Box,        // 矩形边界
        Circle      // 圆形边界
    }

    void Awake()
    {
        // 设置单例
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("场景中存在多个MapBoundary实例！");
            Destroy(gameObject);
        }
    }

    void Start()
    {
        InitializeBoundary();
    }

    /// <summary>
    /// 初始化边界
    /// </summary>
    private void InitializeBoundary()
    {
        Debug.Log($"地图边界初始化完成 - 边界类型: {boundaryType}, 中心: {mapCenter}, 大小: {mapSize}");
    }

    /// <summary>
    /// 检查位置是否在地图边界内
    /// </summary>
    /// <param name="position">要检查的位置</param>
    /// <returns>是否在边界内</returns>
    public bool IsPositionInBounds(Vector3 position)
    {
        switch (boundaryType)
        {
            case BoundaryType.Box:
                return IsPositionInBoxBounds(position);
            case BoundaryType.Circle:
                return IsPositionInCircleBounds(position);
            default:
                return true;
        }
    }

    /// <summary>
    /// 检查位置是否在矩形边界内
    /// </summary>
    private bool IsPositionInBoxBounds(Vector3 position)
    {
        Vector3 min = mapCenter - mapSize * 0.5f;
        Vector3 max = mapCenter + mapSize * 0.5f;

        return position.x >= min.x && position.x <= max.x &&
               position.y >= min.y && position.y <= max.y &&
               position.z >= min.z && position.z <= max.z;
    }

    /// <summary>
    /// 检查位置是否在圆形边界内
    /// </summary>
    private bool IsPositionInCircleBounds(Vector3 position)
    {
        // 只考虑XZ平面的距离
        Vector3 centerXZ = new Vector3(mapCenter.x, position.y, mapCenter.z);
        Vector3 positionXZ = new Vector3(position.x, position.y, position.z);
        
        float distance = Vector3.Distance(centerXZ, positionXZ);
        return distance <= circleRadius;
    }

    /// <summary>
    /// 将位置限制在边界内
    /// </summary>
    /// <param name="position">原始位置</param>
    /// <returns>限制后的位置</returns>
    public Vector3 ClampPositionToBounds(Vector3 position)
    {
        switch (boundaryType)
        {
            case BoundaryType.Box:
                return ClampPositionToBoxBounds(position);
            case BoundaryType.Circle:
                return ClampPositionToCircleBounds(position);
            default:
                return position;
        }
    }

    /// <summary>
    /// 将位置限制在矩形边界内
    /// </summary>
    private Vector3 ClampPositionToBoxBounds(Vector3 position)
    {
        Vector3 min = mapCenter - mapSize * 0.5f;
        Vector3 max = mapCenter + mapSize * 0.5f;

        return new Vector3(
            Mathf.Clamp(position.x, min.x, max.x),
            Mathf.Clamp(position.y, min.y, max.y),
            Mathf.Clamp(position.z, min.z, max.z)
        );
    }

    /// <summary>
    /// 将位置限制在圆形边界内
    /// </summary>
    private Vector3 ClampPositionToCircleBounds(Vector3 position)
    {
        Vector3 centerXZ = new Vector3(mapCenter.x, position.y, mapCenter.z);
        Vector3 positionXZ = new Vector3(position.x, position.y, position.z);
        
        float distance = Vector3.Distance(centerXZ, positionXZ);
        
        if (distance <= circleRadius)
        {
            return position; // 在边界内，不需要限制
        }
        
        // 超出边界，将位置拉回到边界上
        Vector3 direction = (positionXZ - centerXZ).normalized;
        Vector3 clampedXZ = centerXZ + direction * circleRadius;
        
        return new Vector3(clampedXZ.x, position.y, clampedXZ.z);
    }

    #region 公共接口

    /// <summary>
    /// 设置地图中心
    /// </summary>
    /// <param name="center">新的中心点</param>
    public void SetMapCenter(Vector3 center)
    {
        mapCenter = center;
    }

    /// <summary>
    /// 设置地图大小
    /// </summary>
    /// <param name="size">新的大小</param>
    public void SetMapSize(Vector3 size)
    {
        mapSize = size;
    }

    /// <summary>
    /// 设置圆形边界半径
    /// </summary>
    /// <param name="radius">新的半径</param>
    public void SetCircleRadius(float radius)
    {
        circleRadius = Mathf.Max(0.1f, radius);
    }

    /// <summary>
    /// 设置边界类型
    /// </summary>
    /// <param name="type">边界类型</param>
    public void SetBoundaryType(BoundaryType type)
    {
        boundaryType = type;
    }

    /// <summary>
    /// 获取地图边界信息
    /// </summary>
    public (Vector3 center, Vector3 size, float radius, BoundaryType type) GetBoundsInfo()
    {
        return (mapCenter, mapSize, circleRadius, boundaryType);
    }

    #endregion

    #region 调试和可视化

    void OnDrawGizmos()
    {
        if (!showBounds) return;

        Gizmos.color = boundsColor;
        
        switch (boundaryType)
        {
            case BoundaryType.Box:
                DrawBoxBounds();
                break;
            case BoundaryType.Circle:
                DrawCircleBounds();
                break;
        }
    }

    /// <summary>
    /// 绘制矩形边界
    /// </summary>
    private void DrawBoxBounds()
    {
        Gizmos.DrawWireCube(mapCenter, mapSize);
        
        // 绘制边界标签
        Vector3 labelPos = mapCenter + new Vector3(0, mapSize.y * 0.5f + 1f, 0);
        
        #if UNITY_EDITOR
        UnityEditor.Handles.Label(labelPos, $"Map Bounds\n{mapSize.x}x{mapSize.y}x{mapSize.z}");
        #endif
    }

    /// <summary>
    /// 绘制圆形边界
    /// </summary>
    private void DrawCircleBounds()
    {
        // 绘制XZ平面的圆形
        int segments = 32;
        float angleStep = 360f / segments;
        Vector3 prevPoint = mapCenter + new Vector3(circleRadius, 0, 0);
        
        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 newPoint = mapCenter + new Vector3(
                Mathf.Cos(angle) * circleRadius, 
                0, 
                Mathf.Sin(angle) * circleRadius
            );
            
            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
        }
        
        // 绘制中心点
        Gizmos.DrawWireSphere(mapCenter, 0.5f);
        
        // 绘制边界标签
        Vector3 labelPos = mapCenter + new Vector3(0, 2f, 0);
        
        #if UNITY_EDITOR
        UnityEditor.Handles.Label(labelPos, $"Map Bounds (Circle)\nRadius: {circleRadius}");
        #endif
    }

    void OnDrawGizmosSelected()
    {
        // 选中时显示更详细的信息
        Gizmos.color = Color.yellow;
        
        switch (boundaryType)
        {
            case BoundaryType.Box:
                // 显示边界的各个面
                Vector3 min = mapCenter - mapSize * 0.5f;
                Vector3 max = mapCenter + mapSize * 0.5f;
                
                // 绘制边界的8个角点
                Vector3[] corners = new Vector3[8];
                corners[0] = new Vector3(min.x, min.y, min.z);
                corners[1] = new Vector3(max.x, min.y, min.z);
                corners[2] = new Vector3(max.x, max.y, min.z);
                corners[3] = new Vector3(min.x, max.y, min.z);
                corners[4] = new Vector3(min.x, min.y, max.z);
                corners[5] = new Vector3(max.x, min.y, max.z);
                corners[6] = new Vector3(max.x, max.y, max.z);
                corners[7] = new Vector3(min.x, max.y, max.z);
                
                foreach (var corner in corners)
                {
                    Gizmos.DrawWireSphere(corner, 0.2f);
                }
                break;
                
            case BoundaryType.Circle:
                // 显示半径线
                Gizmos.DrawLine(mapCenter, mapCenter + Vector3.right * circleRadius);
                Gizmos.DrawLine(mapCenter, mapCenter + Vector3.forward * circleRadius);
                Gizmos.DrawLine(mapCenter, mapCenter + Vector3.left * circleRadius);
                Gizmos.DrawLine(mapCenter, mapCenter + Vector3.back * circleRadius);
                break;
        }
    }

    #endregion
} 