using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulDefence.Entity
{
    /// <summary>
    /// 玩家AI控制器
    /// 实现玩家的自动寻怪和移动功能
    /// </summary>
    [System.Serializable]
    public class PlayerAI : EntityAI
    {
        [Header("玩家AI设置")]
        [SerializeField] private float targetUpdateInterval = 1.0f;  // 目标更新间隔
        [SerializeField] private float preferredCombatDistance = 1.5f; // 战斗距离

        private float lastTargetUpdateTime = 0f;

        /// <summary>
        /// 初始化玩家AI
        /// </summary>
        public override void Initialize(Transform entityTransform, GameEntity gameEntity, TeamSystem team)
        {
            base.Initialize(entityTransform, gameEntity, team);
            
            // 玩家AI默认不启用
            aiEnabled = false;
            
            // 设置检测范围
            detectionRange = 15f;
        }

        /// <summary>
        /// 玩家AI决策逻辑
        /// </summary>
        protected override void MakeDecision()
        {
            // 定期更新目标
            if (Time.time >= lastTargetUpdateTime + targetUpdateInterval)
            {
                targetTransform = FindTarget();
                lastTargetUpdateTime = Time.time;
            }

            // 如果有目标，向目标移动
            if (targetTransform != null)
            {
                // 计算到目标的距离
                float distanceToTarget = Vector3.Distance(transform.position, targetTransform.position);
                float attackRange = GetAttackRange();
                
                // 如果在攻击范围内，停止移动并攻击
                if (distanceToTarget <= attackRange)
                {
                    entity.StopMovement();
                    AttackTarget(targetTransform);
                }
                // 否则移动到战斗距离
                else
                {
                    MoveToTarget(targetTransform, preferredCombatDistance);
                }
            }
            else
            {
                // 没有目标，停止移动
                entity.StopMovement();
            }
        }

        /// <summary>
        /// 切换AI模式
        /// </summary>
        /// <param name="enabled">是否启用AI</param>
        public void ToggleAI(bool enabled)
        {
            aiEnabled = enabled;
            
            if (!aiEnabled)
            {
                // 关闭AI时停止移动
                if (entity != null)
                {
                    entity.StopMovement();
                }
            }
            
            Debug.Log($"玩家AI模式: {(aiEnabled ? "已启用" : "已禁用")}");
        }
    }
} 