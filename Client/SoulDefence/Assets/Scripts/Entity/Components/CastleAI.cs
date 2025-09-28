using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        private float lastTargetUpdateTime = 0f;

        /// <summary>
        /// 初始化城堡AI
        /// </summary>
        public override void Initialize(Transform entityTransform, GameEntity gameEntity, TeamSystem team)
        {
            base.Initialize(entityTransform, gameEntity, team);
            
            // 城堡AI默认启用
            // aiEnabled = true;
            
            // 设置检测范围和巡逻范围
            detectionRange = 15f;
            patrolRange = 20f;  // 城堡的检测范围较大
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
                    AttackTarget(targetTransform);
                }
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