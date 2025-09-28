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
        [SerializeField] protected bool aiEnabled = false;           // AI是否启用
        [SerializeField] protected float detectionRange = 10f;       // 检测范围
        [SerializeField] protected float patrolRange = 15f;          // 巡逻范围
        [SerializeField] protected float decisionInterval = 0.5f;    // AI决策间隔（秒）

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
            // 在实际项目中，应该使用更高效的方式查找目标
            // 例如使用Physics.OverlapSphere或对象池
            Collider[] colliders = Physics.OverlapSphere(transform.position, patrolRange);
            
            Transform closestTarget = null;
            float closestDistance = float.MaxValue;
            
            foreach (var collider in colliders)
            {
                // 尝试获取GameEntity组件
                GameEntity targetEntity = collider.GetComponent<GameEntity>();
                if (targetEntity == null)
                    continue;

                // 获取目标的队伍系统
                TeamSystem targetTeam = targetEntity.TeamSystem;
                if (targetTeam == null)
                    continue;

                // 检查是否为敌对队伍
                if (teamSystem.IsHostile(targetTeam))
                {
                    float distance = Vector3.Distance(transform.position, collider.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestTarget = collider.transform;
                    }
                }
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
            if (target == null || entity == null)
                return;

            // 计算到目标的距离
            float distance = Vector3.Distance(transform.position, target.position);
            
            // 如果距离大于停止距离，则移动向目标
            if (distance > stopDistance)
            {
                Vector3 direction = (target.position - transform.position).normalized;
                entity.SetMoveDirection(direction);
            }
            else
            {
                // 到达目标位置，停止移动
                entity.StopMovement();
            }
        }

        /// <summary>
        /// 攻击目标（当前仅输出日志）
        /// </summary>
        /// <param name="target">攻击目标</param>
        protected void AttackTarget(Transform target)
        {
            if (target == null)
                return;

            GameEntity targetEntity = target.GetComponent<GameEntity>();
            if (targetEntity != null && targetEntity.IsAlive)
            {
                Debug.Log($"{entity.name} 攻击了 {targetEntity.name}，造成 {entity.Attributes.AttackPower} 伤害");
                // 后续可以实现实际的伤害计算
                // targetEntity.TakeDamage(entity.Attributes.AttackPower);
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

        /// <summary>
        /// 检测范围
        /// </summary>
        public float DetectionRange
        {
            get => detectionRange;
            set => detectionRange = Mathf.Max(0f, value);
        }

        /// <summary>
        /// 巡逻范围
        /// </summary>
        public float PatrolRange
        {
            get => patrolRange;
            set => patrolRange = Mathf.Max(0f, value);
        }

        #endregion
    }
} 