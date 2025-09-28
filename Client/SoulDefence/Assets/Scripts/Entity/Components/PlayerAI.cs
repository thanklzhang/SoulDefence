using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulDefence.Skill;

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
        [SerializeField] private float attackInterval = 0.3f;       // 攻击尝试间隔

        private float lastTargetUpdateTime = 0f;
        private float lastAttackAttemptTime = 0f;                   // 上次尝试攻击的时间

        /// <summary>
        /// 初始化玩家AI
        /// </summary>
        public override void Initialize(Transform entityTransform, GameEntity gameEntity, TeamSystem team)
        {
            base.Initialize(entityTransform, gameEntity, team);
            
            // 玩家AI默认不启用
            // aiEnabled = false;
            
            // 设置检测范围
            // detectionRange = 15f;
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
                    
                    // 检查是否可以攻击（攻击间隔和技能冷却）
                    if (CanAttack())
                    {
                        // 面向目标
                        LookAtTarget(targetTransform);
                        
                        // 攻击目标
                        AttackTarget(targetTransform);
                        lastAttackAttemptTime = Time.time;
                    }
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
        /// 检查是否可以攻击（基于攻击间隔和技能冷却）
        /// </summary>
        private bool CanAttack()
        {
            // 检查攻击间隔
            if (Time.time < lastAttackAttemptTime + attackInterval)
                return false;
            
            // 获取默认技能
            SkillData defaultSkill = entity.GetDefaultSkill();
            if (defaultSkill == null)
                return false;
                
            // 检查技能是否在冷却中
            if (entity.IsSkillOnCooldown(defaultSkill))
                return false;
                
            return true;
        }
        
        /// <summary>
        /// 使实体面向目标
        /// </summary>
        private void LookAtTarget(Transform target)
        {
            if (target == null || transform == null)
                return;
                
            Vector3 direction = (target.position - transform.position).normalized;
            direction.y = 0; // 保持在水平面上
            
            if (direction != Vector3.zero)
            {
                // 计算朝向目标的旋转
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                
                // 设置实体的朝向
                transform.rotation = targetRotation;
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