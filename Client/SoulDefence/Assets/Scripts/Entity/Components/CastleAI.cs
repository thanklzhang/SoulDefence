using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulDefence.Skill;

namespace SoulDefence.Entity
{
    /// <summary>
    /// 城堡AI控制器
    /// 实现城堡的自动攻击功能
    /// </summary>
    [System.Serializable]
    public class CastleAI : EntityAI
    {
        [Header("城堡AI设置")]
        [SerializeField] private float targetUpdateInterval = 0.5f; // 目标更新间隔
        [SerializeField] private float attackInterval = 1.0f;      // 攻击尝试间隔

        private float lastTargetUpdateTime = 0f;
        private float lastAttackAttemptTime = 0f;                  // 上次尝试攻击的时间

        /// <summary>
        /// 初始化城堡AI
        /// </summary>
        public override void Initialize(Transform entityTransform, GameEntity gameEntity, TeamSystem team)
        {
            base.Initialize(entityTransform, gameEntity, team);
            
            // 城堡AI默认启用
            // aiEnabled = true;
            
            // 设置检测范围和巡逻范围
            // detectionRange = 15f;
            patrolRange = 6.0f;  // 城堡的检测范围较大
        }

        /// <summary>
        /// 城堡AI决策逻辑
        /// </summary>
        protected override void MakeDecision()
        {
            // 定期更新目标
            if (Time.time >= lastTargetUpdateTime + targetUpdateInterval)
            {
                targetTransform = FindTarget();
                lastTargetUpdateTime = Time.time;
            }

            // 如果有目标，且在攻击范围内，则攻击
            if (targetTransform != null)
            {
                float distanceToTarget = Vector3.Distance(transform.position, targetTransform.position);
                float attackRange = GetAttackRange();
                
                if (distanceToTarget <= attackRange)
                {
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
        /// 城堡不移动，重写移动方法为空
        /// </summary>
        protected new void MoveToTarget(Transform target, float stopDistance)
        {
            // 城堡不移动，所以此方法为空
        }
    }
} 