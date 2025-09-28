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

        /// <summary>
        /// 初始化移动系统
        /// </summary>
        /// <param name="entityTransform">实体的Transform</param>
        /// <param name="entityAttributes">实体属性</param>
        public void Initialize(Transform entityTransform, EntityAttributes entityAttributes)
        {
            transform = entityTransform;
            attributes = entityAttributes;
            Debug.Log($"EntityMovement初始化: transform={transform!=null}, attributes={attributes!=null}");
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
                
                //Debug.Log($"EntityMovement: 移动方向={moveDirection}, 速度={moveSpeed}, 移动量={movement}");

                // 检查地图边界限制
                if (MapBoundary.Instance != null)
                {
                    // 如果新位置超出边界，将其限制在边界内
                    Vector3 boundedPosition = MapBoundary.Instance.ClampPositionToBounds(newPosition);
                    if (boundedPosition != newPosition)
                    {
                        // Debug.Log($"EntityMovement: 位置被边界限制，原位置={newPosition}, 新位置={boundedPosition}");
                        newPosition = boundedPosition;
                    }
                }

                // 应用移动
                transform.position = newPosition;
                // Debug.Log($"EntityMovement: 实体已移动到 {newPosition}");

                // 只有在移动时才执行转向
                HandleRotation();
                
                // 记录最后的移动方向
                lastMoveDirection = moveDirection;
            }
            else
            {
                // 如果没有移动方向，记录日志
                // Debug.Log("EntityMovement: 没有移动方向，保持静止");
            }
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
            
            Debug.Log($"EntityMovement: 实体已转向，面向方向={moveDirection}");
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
                // Debug.LogWarning("EntityMovement: 设置的移动方向太小，可能是无效的");
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
            
            Debug.Log($"EntityMovement: 设置移动方向为 {moveDirection}, 原始方向={direction}");
        }

        /// <summary>
        /// 停止移动
        /// </summary>
        public void StopMovement()
        {
            moveDirection = Vector3.zero;
            // Debug.Log("EntityMovement: 停止移动");
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