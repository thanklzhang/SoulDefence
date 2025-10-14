using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulDefence.Map;

namespace SoulDefence.Entity
{
    /// <summary>
    /// 实体移动系统
    /// 管理实体的移动行为和转向
    /// </summary>
    [System.Serializable]
    public class EntityMovement
    {
        [Header("转向设置")]
        [SerializeField] private float rotationSpeed = 10f;     // 转向速度

        // 移动相关
        private Vector3 moveDirection = Vector3.zero;
        private Vector3 lastMoveDirection = Vector3.zero;
        
        // 组件引用
        private Transform transform;
        private EntityAttributes attributes;
        
        // 缓存的MapBoundary实例
        private MapBoundary cachedBoundary;

        /// <summary>
        /// 初始化移动系统
        /// </summary>
        /// <param name="entityTransform">实体的Transform</param>
        /// <param name="entityAttributes">实体属性</param>
        public void Initialize(Transform entityTransform, EntityAttributes entityAttributes)
        {
            transform = entityTransform;
            attributes = entityAttributes;
            
            // 尝试获取MapBoundary实例
            cachedBoundary = MapBoundary.Instance;
            
            Debug.Log($"EntityMovement初始化: transform={transform!=null}, attributes={attributes!=null}, boundary={cachedBoundary!=null}");
        }

        /// <summary>
        /// 执行移动和转向
        /// </summary>
        public void Move()
        {
            if (transform == null)
            {
                Debug.LogError("EntityMovement: Transform为空，无法移动");
                return;
            }
            
            if (attributes == null)
            {
                Debug.LogError("EntityMovement: Attributes为空，无法获取移动速度");
                return;
            }

            // 如果有移动方向，执行移动和转向
            if (moveDirection.magnitude > 0.1f)
            {
                // 计算移动向量
                float moveSpeed = attributes.MoveSpeed;
                Vector3 movement = moveDirection * moveSpeed * Time.deltaTime;
                Vector3 newPosition = transform.position + movement;
                
                // 检查地图边界限制
                Vector3 boundedPosition = ApplyBoundaryLimits(newPosition);
                
                // 如果位置被边界限制，输出日志
                if (boundedPosition != newPosition)
                {
                    // Debug.Log($"EntityMovement: 位置被边界限制，原位置={newPosition}, 新位置={boundedPosition}");
                }
                
                // 应用移动
                transform.position = boundedPosition;

                // 只有在移动时才执行转向
                HandleRotation();
                
                // 记录最后的移动方向
                lastMoveDirection = moveDirection;
            }
        }

        /// <summary>
        /// 应用边界限制
        /// </summary>
        /// <param name="position">原始位置</param>
        /// <returns>限制后的位置</returns>
        private Vector3 ApplyBoundaryLimits(Vector3 position)
        {
            // 如果缓存的边界为空，尝试重新获取
            if (cachedBoundary == null)
            {
                cachedBoundary = MapBoundary.Instance;
                
                // 如果仍然为空，使用默认边界
                if (cachedBoundary == null)
                {
                    Debug.LogWarning("EntityMovement: 找不到MapBoundary实例，使用默认边界");
                    return ApplyDefaultBoundary(position);
                }
            }
            
            // 使用缓存的边界限制位置
            return cachedBoundary.ClampPositionToBounds(position);
        }
        
        /// <summary>
        /// 应用默认边界限制
        /// </summary>
        /// <param name="position">原始位置</param>
        /// <returns>限制后的位置</returns>
        private Vector3 ApplyDefaultBoundary(Vector3 position)
        {
            // 默认边界范围
            float minX = -10f;
            float maxX = 10f;
            float minZ = -10f;
            float maxZ = 10f;
            
            // 限制X和Z坐标
            float x = Mathf.Clamp(position.x, minX, maxX);
            float z = Mathf.Clamp(position.z, minZ, maxZ);
            
            return new Vector3(x, position.y, z);
        }

        /// <summary>
        /// 处理缓动转向
        /// </summary>
        private void HandleRotation()
        {
            // 只有在有移动方向时才转向
            if (moveDirection.magnitude < 0.1f)
                return;

            // 计算目标旋转角度
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            
            // 使用Slerp进行平滑转向
            transform.rotation = Quaternion.Slerp(
                transform.rotation, 
                targetRotation, 
                rotationSpeed * Time.deltaTime
            );
        }

        /// <summary>
        /// 设置移动方向（供外部调用）
        /// </summary>
        /// <param name="direction">移动方向</param>
        public void SetMoveDirection(Vector3 direction)
        {
            // 检查方向是否有效
            if (direction == null)
            {
                Debug.LogError("EntityMovement: 设置的移动方向为null");
                return;
            }
            
            // 忽略Y轴方向，保持在水平面移动
            Vector3 horizontalDirection = new Vector3(direction.x, 0, direction.z);
            
            // 如果方向向量太小，可能是无效的
            if (horizontalDirection.magnitude < 0.01f)
            {
                // 如果需要停止移动，应该调用StopMovement方法
                if (direction.magnitude < 0.01f)
                {
                    StopMovement();
                    return;
                }
            }
            
            // 标准化方向向量
            moveDirection = horizontalDirection.normalized;
            
            // 确保移动方向不为零
            if (moveDirection.magnitude < 0.01f)
            {
                Debug.LogError("EntityMovement: 标准化后的移动方向为零，设置默认方向");
                moveDirection = Vector3.forward; // 设置一个默认方向
            }
        }

        /// <summary>
        /// 停止移动
        /// </summary>
        public void StopMovement()
        {
            moveDirection = Vector3.zero;
        }

        /// <summary>
        /// 设置转向速度
        /// </summary>
        /// <param name="speed">转向速度</param>
        public void SetRotationSpeed(float speed)
        {
            rotationSpeed = Mathf.Max(0f, speed);
        }

        #region 属性访问器

        /// <summary>
        /// 当前移动方向
        /// </summary>
        public Vector3 MoveDirection => moveDirection;

        /// <summary>
        /// 是否正在移动
        /// </summary>
        public bool IsMoving => moveDirection.magnitude > 0.1f;

        /// <summary>
        /// 最后的移动方向
        /// </summary>
        public Vector3 LastMoveDirection => lastMoveDirection;

        /// <summary>
        /// 转向速度
        /// </summary>
        public float RotationSpeed 
        { 
            get => rotationSpeed; 
            set => rotationSpeed = Mathf.Max(0f, value); 
        }

        #endregion
    }
} 