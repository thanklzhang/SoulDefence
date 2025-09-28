using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulDefence.Entity
{
    /// <summary>
    /// 敌人AI控制器
    /// 实现敌人向城堡移动和攻击功能
    /// </summary>
    [System.Serializable]
    public class EnemyAI : EntityAI
    {
        [Header("敌人AI设置")]
        [SerializeField] private float targetUpdateInterval = 1.0f; // 目标更新间隔

        private Transform castleTransform;                          // 城堡位置
        private float lastTargetUpdateTime = 0f;

        /// <summary>
        /// 初始化敌人AI
        /// </summary>
        public override void Initialize(Transform entityTransform, GameEntity gameEntity, TeamSystem team)
        {
            base.Initialize(entityTransform, gameEntity, team);
            
            // 敌人AI默认启用
            aiEnabled = true;
            
            // 设置检测范围和巡逻范围
            detectionRange = 10f;
            patrolRange = 20f;  // 敌人的巡逻范围较大
            
            // 寻找城堡
            FindCastle();
        }

        /// <summary>
        /// 敌人AI决策逻辑
        /// </summary>
        protected override void MakeDecision()
        {
            // 定期更新目标
            if (Time.time >= lastTargetUpdateTime + targetUpdateInterval)
            {
                // 在巡逻范围内寻找敌对实体
                Transform newTarget = FindTarget();
                
                // 如果找到敌对实体，将其设为目标
                if (newTarget != null)
                {
                    targetTransform = newTarget;
                }
                // 否则默认目标为城堡
                else if (castleTransform != null)
                {
                    targetTransform = castleTransform;
                }
                
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
                // 否则继续移动
                else
                {
                    MoveToTarget(targetTransform, attackRange);
                }
            }
            else
            {
                // 没有目标，尝试重新寻找城堡
                FindCastle();
                
                // 如果仍然没有目标，停止移动
                if (targetTransform == null)
                {
                    entity.StopMovement();
                }
            }
        }

        /// <summary>
        /// 寻找城堡
        /// </summary>
        private void FindCastle()
        {
            // 在实际项目中，可以使用标签或其他方式更高效地查找城堡
            GameObject castleObj = GameObject.FindGameObjectWithTag("Castle");
            
            if (castleObj != null)
            {
                castleTransform = castleObj.transform;
                targetTransform = castleTransform;
            }
        }
    }
} 