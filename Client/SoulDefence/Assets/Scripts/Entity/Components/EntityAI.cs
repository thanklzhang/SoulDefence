using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulDefence.Entity
{
    /// <summary>
    /// 实体AI基类
    /// 提供基础的AI控制功能
    /// </summary>
    [System.Serializable]
    public abstract class EntityAI
    {
        [Header("AI基础设置")]
        [SerializeField] protected bool aiEnabled = true;           // AI是否启用
        //[SerializeField] protected float detectionRange = 10f;       // 检测范围
        [SerializeField] protected float patrolRange = 5f;          // 巡逻范围
        [SerializeField] protected float decisionInterval = 0.40f;    // AI决策间隔（秒）

        // 组件引用
        protected Transform transform;
        protected GameEntity entity;
        protected TeamSystem teamSystem;

        // 状态变量
        protected float lastDecisionTime = 0f;
        protected Transform targetTransform;
        
        /// <summary>
        /// 初始化AI
        /// </summary>
        /// <param name="entityTransform">实体Transform</param>
        /// <param name="gameEntity">实体引用</param>
        /// <param name="team">队伍系统</param>
        public virtual void Initialize(Transform entityTransform, GameEntity gameEntity, TeamSystem team)
        {
            transform = entityTransform;
            entity = gameEntity;
            teamSystem = team;
            lastDecisionTime = 0f;
            
            Debug.Log($"AI基类初始化: transform={transform!=null}, entity={entity!=null}, team={team!=null}");
        }

        /// <summary>
        /// 更新AI逻辑
        /// </summary>
        public virtual void UpdateAI()
        {
            if (!aiEnabled || entity == null || !entity.IsAlive)
                return;

            // 按照决策间隔执行AI决策
            if (Time.time >= lastDecisionTime + decisionInterval)
            {
                MakeDecision();
                lastDecisionTime = Time.time;
            }
        }

        /// <summary>
        /// AI决策逻辑（由子类实现）
        /// </summary>
        protected abstract void MakeDecision();

        /// <summary>
        /// 获取攻击范围
        /// </summary>
        /// <returns>实体的攻击范围</returns>
        protected float GetAttackRange()
        {
            if (entity != null && entity.Attributes != null)
            {
                return entity.Attributes.AttackRange;
            }
            return 2f; // 默认攻击范围
        }

        /// <summary>
        /// 寻找范围内的目标
        /// </summary>
        /// <returns>找到的目标Transform，如果没找到则为null</returns>
        protected Transform FindTarget()
        {
            if (transform == null)
            {
                Debug.LogError("AI的Transform为空，无法寻找目标");
                return null;
            }
            
            // 在实际项目中，应该使用更高效的方式查找目标
            // 例如使用Physics.OverlapSphere或对象池
            Debug.Log($"开始寻找目标，位置: {transform.position}, 巡逻范围: {patrolRange}");
            Collider[] colliders = Physics.OverlapSphere(transform.position, patrolRange);
            Debug.Log($"在范围内找到 {colliders.Length} 个碰撞体");
            
            Transform closestTarget = null;
            float closestDistance = float.MaxValue;
            
            foreach (var collider in colliders)
            {
                // 尝试获取GameEntity组件
                GameEntity targetEntity = collider.GetComponent<GameEntity>();
                if (targetEntity == null)
                {
                    // Debug.Log($"物体 {collider.name} 不是GameEntity");
                    continue;
                }

                // 获取目标的队伍系统
                TeamSystem targetTeam = targetEntity.TeamSystem;
                if (targetTeam == null)
                {
                    Debug.Log($"物体 {collider.name} 没有TeamSystem组件");
                    continue;
                }

                // 检查是否为敌对队伍
                if (teamSystem.IsHostile(targetTeam))
                {
                    float distance = Vector3.Distance(transform.position, collider.transform.position);
                    // Debug.Log($"发现敌对实体: {collider.name}, 距离: {distance}");
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestTarget = collider.transform;
                        // Debug.Log($"更新最近目标为: {collider.name}");
                    }
                }
                else
                {
                    // Debug.Log($"物体 {collider.name} 不是敌对队伍");
                }
            }
            
            if (closestTarget != null)
            {
                // Debug.Log($"找到最近的敌对目标: {closestTarget.name}, 距离: {closestDistance}");
            }
            else
            {
                // Debug.Log("没有找到任何敌对目标");
            }
            
            return closestTarget;
        }

        /// <summary>
        /// 向目标移动
        /// </summary>
        /// <param name="target">目标Transform</param>
        /// <param name="stopDistance">停止距离</param>
        protected void MoveToTarget(Transform target, float stopDistance)
        {
            if (target == null)
            {
                Debug.LogError("移动目标为空");
                return;
            }
            
            if (entity == null)
            {
                Debug.LogError("实体引用为空，无法移动");
                return;
            }
            
            if (transform == null)
            {
                Debug.LogError("AI的Transform为空，无法计算距离");
                return;
            }

            // 计算到目标的距离
            float distance = Vector3.Distance(transform.position, target.position);
            Debug.Log($"向目标 {target.name} 移动，当前距离: {distance}, 停止距离: {stopDistance}");
            
            // 如果距离大于停止距离，则移动向目标
            if (distance > stopDistance)
            {
                Vector3 direction = (target.position - transform.position).normalized;
                Debug.Log($"设置移动方向: {direction}");
                entity.SetMoveDirection(direction);
            }
            else
            {
                // 到达目标位置，停止移动
                Debug.Log("已达到目标位置，停止移动");
                entity.StopMovement();
            }
        }

        /// <summary>
        /// 攻击目标（使用技能系统）
        /// </summary>
        /// <param name="target">攻击目标</param>
        protected void AttackTarget(Transform target)
        {
            if (target == null)
            {
                Debug.LogError("攻击目标为空");
                return;
            }

            GameEntity targetEntity = target.GetComponent<GameEntity>();
            if (targetEntity != null && targetEntity.IsAlive)
            {
                // 使用默认技能攻击目标
                bool skillUsed = entity.UseDefaultSkill(target.position);
                
                if (skillUsed)
                {
                    Debug.Log($"{entity.name} 使用技能攻击了 {targetEntity.name}");
                }
                else
                {
                    // 如果技能使用失败（可能是冷却中），可以考虑其他行为
                    Debug.Log($"{entity.name} 尝试攻击 {targetEntity.name}，但技能使用失败");
                    
                    // 可以在这里添加备用行为，比如移动到更好的位置
                    // 或者等待技能冷却结束
                }
            }
            else
            {
                Debug.LogWarning($"攻击目标 {target.name} 不是有效的GameEntity或已死亡");
            }
        }

        #region 属性访问器

        /// <summary>
        /// AI是否启用
        /// </summary>
        public bool AIEnabled
        {
            get => aiEnabled;
            set => aiEnabled = value;
        }

        // /// <summary>
        // /// 检测范围
        // /// </summary>
        // public float DetectionRange
        // {
        //     get => detectionRange;
        //     set => detectionRange = Mathf.Max(0f, value);
        // }
        //
        // /// <summary>
        // /// 巡逻范围
        // /// </summary>
        // public float PatrolRange
        // {
        //     get => patrolRange;
        //     set => patrolRange = Mathf.Max(0f, value);
        // }

        #endregion
    }
} 