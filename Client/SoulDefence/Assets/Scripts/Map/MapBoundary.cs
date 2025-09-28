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
        public static MapBoundary Instance => instance;

        private void Awake()
        {
            // 确保场景中只有一个MapBoundary实例
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
        }

        /// <summary>
        /// 将位置限制在边界内
        /// </summary>
        /// <param name="position">原始位置</param>
        /// <returns>限制后的位置</returns>
        public Vector3 ClampPositionToBounds(Vector3 position)
        {
            // 限制X和Z坐标
            float x = Mathf.Clamp(position.x, minX, maxX);
            float z = Mathf.Clamp(position.z, minZ, maxZ);
            
            // 如果启用Y轴限制，则限制Y坐标
            float y = enforceYLimit ? Mathf.Clamp(position.y, minY, maxY) : position.y;
            
            return new Vector3(x, y, z);
        }

        /// <summary>
        /// 检查位置是否在边界内
        /// </summary>
        /// <param name="position">检查的位置</param>
        /// <returns>是否在边界内</returns>
        public bool IsPositionInBounds(Vector3 position)
        {
            bool xInBounds = position.x >= minX && position.x <= maxX;
            bool zInBounds = position.z >= minZ && position.z <= maxZ;
            bool yInBounds = !enforceYLimit || (position.y >= minY && position.y <= maxY);
            
            return xInBounds && zInBounds && yInBounds;
        }

        /// <summary>
        /// 获取地图边界信息
        /// </summary>
        /// <returns>边界信息元组</returns>
        public (Vector3 center, Vector3 size, float radius, BoundaryType type) GetBoundsInfo()
        {
            Vector3 center = new Vector3((minX + maxX) * 0.5f, (minY + maxY) * 0.5f, (minZ + maxZ) * 0.5f);
            Vector3 size = new Vector3(maxX - minX, maxY - minY, maxZ - minZ);
            float radius = Mathf.Max(size.x, size.z) * 0.5f;  // 估算半径为最大边长的一半
            
            return (center, size, radius, boundaryType);
        }

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

        #endregion
    }
} 