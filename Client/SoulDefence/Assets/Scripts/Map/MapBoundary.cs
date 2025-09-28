using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulDefence.Map
{
    /// <summary>
    /// 地图边界管理器
    /// 限制实体在地图边界内移动
    /// </summary>
    public class MapBoundary : MonoBehaviour
    {
        [Header("边界设置")]
        [SerializeField] private float minX = -50f;
        [SerializeField] private float maxX = 50f;
        [SerializeField] private float minZ = -50f;
        [SerializeField] private float maxZ = 50f;
        [SerializeField] private bool enforceYLimit = false;
        [SerializeField] private float minY = 0f;
        [SerializeField] private float maxY = 10f;

        [Header("边界类型")]
        [SerializeField] private BoundaryType boundaryType = BoundaryType.Box;  // 边界类型

        [Header("可视化设置")]
        [SerializeField] private bool showBoundary = true;        // 是否显示边界
        [SerializeField] private Color boundaryColor = Color.red; // 边界颜色
        [SerializeField] private float boundaryLineWidth = 2f;    // 边界线宽度

        // 单例实例
        private static MapBoundary instance;

        /// <summary>
        /// 边界类型枚举
        /// </summary>
        public enum BoundaryType
        {
            Box,        // 矩形边界
            Circle      // 圆形边界
        }

        /// <summary>
        /// 单例访问器
        /// </summary>
        public static MapBoundary Instance
        {
            get
            {
                if (instance == null)
                {
                    // 尝试在场景中查找MapBoundary实例
                    instance = FindObjectOfType<MapBoundary>();

                    // 如果场景中没有，创建一个新的实例
                    if (instance == null)
                    {
                        GameObject boundaryObject = new GameObject("MapBoundary");
                        instance = boundaryObject.AddComponent<MapBoundary>();
                        Debug.Log("MapBoundary: 自动创建了一个新的MapBoundary实例");
                    }
                }
                return instance;
            }
        }

        private void Awake()
        {
            // 确保场景中只有一个MapBoundary实例
            if (instance != null && instance != this)
            {
                Debug.LogWarning("场景中存在多个MapBoundary实例，销毁重复的实例");
                Destroy(gameObject);
                return;
            }

            instance = this;
            Debug.Log($"MapBoundary初始化完成: 边界范围 X:({minX},{maxX}) Z:({minZ},{maxZ})");
        }

        private void OnEnable()
        {
            // 确保实例引用正确
            if (instance == null)
            {
                instance = this;
            }
        }

        private void OnDestroy()
        {
            // 如果销毁的是当前实例，清除静态引用
            if (instance == this)
            {
                instance = null;
            }
        }

        /// <summary>
        /// 将位置限制在边界内
        /// </summary>
        /// <param name="position">原始位置</param>
        /// <returns>限制后的位置</returns>
        public Vector3 ClampPositionToBounds(Vector3 position)
        {
            if (boundaryType == BoundaryType.Box)
            {
                // 限制X和Z坐标
                float x = Mathf.Clamp(position.x, minX, maxX);
                float z = Mathf.Clamp(position.z, minZ, maxZ);
                
                // 如果启用Y轴限制，则限制Y坐标
                float y = enforceYLimit ? Mathf.Clamp(position.y, minY, maxY) : position.y;
                
                return new Vector3(x, y, z);
            }
            else // Circle boundary
            {
                // 计算中心点
                Vector2 center = new Vector2((minX + maxX) * 0.5f, (minZ + maxZ) * 0.5f);
                // 计算半径
                float radius = Mathf.Min(maxX - minX, maxZ - minZ) * 0.5f;
                
                // 获取位置的水平坐标
                Vector2 positionXZ = new Vector2(position.x, position.z);
                
                // 计算到中心的距离
                float distance = Vector2.Distance(positionXZ, center);
                
                // 如果在圆内，直接返回
                if (distance <= radius)
                {
                    return position;
                }
                
                // 否则限制在圆上
                Vector2 direction = (positionXZ - center).normalized;
                Vector2 clampedXZ = center + direction * radius;
                
                // 如果启用Y轴限制，则限制Y坐标
                float y = enforceYLimit ? Mathf.Clamp(position.y, minY, maxY) : position.y;
                
                return new Vector3(clampedXZ.x, y, clampedXZ.y);
            }
        }

        /// <summary>
        /// 检查位置是否在边界内
        /// </summary>
        /// <param name="position">检查的位置</param>
        /// <returns>是否在边界内</returns>
        public bool IsPositionInBounds(Vector3 position)
        {
            if (boundaryType == BoundaryType.Box)
            {
                bool xInBounds = position.x >= minX && position.x <= maxX;
                bool zInBounds = position.z >= minZ && position.z <= maxZ;
                bool yInBounds = !enforceYLimit || (position.y >= minY && position.y <= maxY);
                
                return xInBounds && zInBounds && yInBounds;
            }
            else // Circle boundary
            {
                // 计算中心点
                Vector2 center = new Vector2((minX + maxX) * 0.5f, (minZ + maxZ) * 0.5f);
                // 计算半径
                float radius = Mathf.Min(maxX - minX, maxZ - minZ) * 0.5f;
                
                // 获取位置的水平坐标
                Vector2 positionXZ = new Vector2(position.x, position.z);
                
                // 计算到中心的距离
                float distance = Vector2.Distance(positionXZ, center);
                
                // 检查Y轴
                bool yInBounds = !enforceYLimit || (position.y >= minY && position.y <= maxY);
                
                return distance <= radius && yInBounds;
            }
        }

        /// <summary>
        /// 获取地图边界信息
        /// </summary>
        /// <returns>边界信息元组</returns>
        public (Vector3 center, Vector3 size, float radius, BoundaryType type) GetBoundsInfo()
        {
            Vector3 center = new Vector3((minX + maxX) * 0.5f, (minY + maxY) * 0.5f, (minZ + maxZ) * 0.5f);
            Vector3 size = new Vector3(maxX - minX, maxY - minY, maxZ - minZ);
            float radius = Mathf.Min(size.x, size.z) * 0.5f;  // 估算半径为最小边长的一半
            
            return (center, size, radius, boundaryType);
        }

        #region 边界可视化

        /// <summary>
        /// 在Scene视图中绘制边界
        /// </summary>
        private void OnDrawGizmos()
        {
            if (!showBoundary)
                return;

            Gizmos.color = boundaryColor;

            if (boundaryType == BoundaryType.Box)
            {
                DrawBoxBoundary();
            }
            else
            {
                DrawCircleBoundary();
            }
        }

        /// <summary>
        /// 绘制矩形边界
        /// </summary>
        private void DrawBoxBoundary()
        {
            // 计算边界的8个顶点
            Vector3 v1 = new Vector3(minX, minY, minZ);
            Vector3 v2 = new Vector3(maxX, minY, minZ);
            Vector3 v3 = new Vector3(maxX, minY, maxZ);
            Vector3 v4 = new Vector3(minX, minY, maxZ);
            Vector3 v5 = new Vector3(minX, maxY, minZ);
            Vector3 v6 = new Vector3(maxX, maxY, minZ);
            Vector3 v7 = new Vector3(maxX, maxY, maxZ);
            Vector3 v8 = new Vector3(minX, maxY, maxZ);

            // 绘制底部矩形
            Gizmos.DrawLine(v1, v2);
            Gizmos.DrawLine(v2, v3);
            Gizmos.DrawLine(v3, v4);
            Gizmos.DrawLine(v4, v1);

            // 如果启用Y轴限制，绘制顶部矩形和连接线
            if (enforceYLimit)
            {
                // 绘制顶部矩形
                Gizmos.DrawLine(v5, v6);
                Gizmos.DrawLine(v6, v7);
                Gizmos.DrawLine(v7, v8);
                Gizmos.DrawLine(v8, v5);

                // 绘制连接线
                Gizmos.DrawLine(v1, v5);
                Gizmos.DrawLine(v2, v6);
                Gizmos.DrawLine(v3, v7);
                Gizmos.DrawLine(v4, v8);
            }
            else
            {
                // 如果不限制Y轴，绘制向上的指示线
                float lineLength = 2f;
                Gizmos.DrawLine(v1, v1 + Vector3.up * lineLength);
                Gizmos.DrawLine(v2, v2 + Vector3.up * lineLength);
                Gizmos.DrawLine(v3, v3 + Vector3.up * lineLength);
                Gizmos.DrawLine(v4, v4 + Vector3.up * lineLength);
            }

            // 绘制中心点
            Vector3 center = new Vector3((minX + maxX) * 0.5f, (minY + maxY) * 0.5f, (minZ + maxZ) * 0.5f);
            Gizmos.DrawSphere(center, 0.5f);
        }

        /// <summary>
        /// 绘制圆形边界
        /// </summary>
        private void DrawCircleBoundary()
        {
            // 计算中心点
            Vector3 center = new Vector3((minX + maxX) * 0.5f, minY, (minZ + maxZ) * 0.5f);
            // 计算半径
            float radius = Mathf.Min(maxX - minX, maxZ - minZ) * 0.5f;

            // 绘制圆形
            DrawCircle(center, radius, 32);

            // 如果启用Y轴限制，绘制上部圆形
            if (enforceYLimit)
            {
                Vector3 topCenter = new Vector3(center.x, maxY, center.z);
                DrawCircle(topCenter, radius, 32);

                // 绘制连接线
                for (int i = 0; i < 4; i++)
                {
                    float angle = i * Mathf.PI / 2;
                    Vector3 bottomPoint = center + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
                    Vector3 topPoint = topCenter + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
                    Gizmos.DrawLine(bottomPoint, topPoint);
                }
            }
            else
            {
                // 如果不限制Y轴，绘制向上的指示线
                float lineLength = 2f;
                for (int i = 0; i < 4; i++)
                {
                    float angle = i * Mathf.PI / 2;
                    Vector3 point = center + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
                    Gizmos.DrawLine(point, point + Vector3.up * lineLength);
                }
            }

            // 绘制中心点
            Gizmos.DrawSphere(center, 0.5f);
        }

        /// <summary>
        /// 绘制圆形
        /// </summary>
        private void DrawCircle(Vector3 center, float radius, int segments)
        {
            float angleStep = 2 * Mathf.PI / segments;
            Vector3 prevPoint = center + new Vector3(radius, 0, 0);

            for (int i = 1; i <= segments; i++)
            {
                float angle = i * angleStep;
                Vector3 newPoint = center + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
                Gizmos.DrawLine(prevPoint, newPoint);
                prevPoint = newPoint;
            }
        }

        #endregion

        #region 属性访问器

        /// <summary>
        /// 获取或设置X轴最小值
        /// </summary>
        public float MinX
        {
            get => minX;
            set => minX = value;
        }

        /// <summary>
        /// 获取或设置X轴最大值
        /// </summary>
        public float MaxX
        {
            get => maxX;
            set => maxX = value;
        }

        /// <summary>
        /// 获取或设置Z轴最小值
        /// </summary>
        public float MinZ
        {
            get => minZ;
            set => minZ = value;
        }

        /// <summary>
        /// 获取或设置Z轴最大值
        /// </summary>
        public float MaxZ
        {
            get => maxZ;
            set => maxZ = value;
        }

        /// <summary>
        /// 获取或设置是否启用Y轴限制
        /// </summary>
        public bool EnforceYLimit
        {
            get => enforceYLimit;
            set => enforceYLimit = value;
        }

        /// <summary>
        /// 获取或设置Y轴最小值
        /// </summary>
        public float MinY
        {
            get => minY;
            set => minY = value;
        }

        /// <summary>
        /// 获取或设置Y轴最大值
        /// </summary>
        public float MaxY
        {
            get => maxY;
            set => maxY = value;
        }

        /// <summary>
        /// 获取或设置边界类型
        /// </summary>
        public BoundaryType Type
        {
            get => boundaryType;
            set => boundaryType = value;
        }

        /// <summary>
        /// 获取或设置是否显示边界
        /// </summary>
        public bool ShowBoundary
        {
            get => showBoundary;
            set => showBoundary = value;
        }

        /// <summary>
        /// 获取或设置边界颜色
        /// </summary>
        public Color BoundaryColor
        {
            get => boundaryColor;
            set => boundaryColor = value;
        }

        #endregion
    }
} 