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
        }



        /// <summary>
        /// 执行移动和转向
        /// </summary>
        public void Move()
        {
            if (transform == null || attributes == null)
                return;

            // 如果有移动方向，执行移动和转向
            if (moveDirection.magnitude > 0.1f)
            {
                // 计算移动向量
                Vector3 movement = moveDirection * attributes.MoveSpeed * Time.deltaTime;
                Vector3 newPosition = transform.position + movement;

                // 检查地图边界限制
                if (MapBoundary.Instance != null)
                {
                    // 如果新位置超出边界，将其限制在边界内
                    newPosition = MapBoundary.Instance.ClampPositionToBounds(newPosition);
                }

                // 应用移动
                transform.position = newPosition;

                // 只有在移动时才执行转向
                HandleRotation();
                
                // 记录最后的移动方向
                lastMoveDirection = moveDirection;
            }
            // 如果没有移动输入，停止转向（保持当前朝向）
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
            moveDirection = direction.normalized;
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