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
            
            // 设置检测范围和巡逻范围
            detectionRange = 10f;
            patrolRange = 20f;  // 敌人的巡逻范围较大
            
            // 寻找城堡
            FindCastle();
            
            // 如果找不到城堡，尝试向场景中心移动
            if (castleTransform == null)
            {
                // 创建一个空物体作为默认目标
                GameObject defaultTarget = new GameObject("DefaultTarget");
                defaultTarget.transform.position = Vector3.zero; // 场景中心
                castleTransform = defaultTarget.transform;
                targetTransform = castleTransform;
            }
        }
        
        /// <summary>
        /// 更新AI逻辑
        /// </summary>
        public override void UpdateAI()
        {
            if (!aiEnabled)
                return;
            
            if (entity == null)
                return;
            
            if (!entity.IsAlive)
                return;
            
            // 按照决策间隔执行AI决策
            if (Time.time >= lastDecisionTime + decisionInterval)
            {
                MakeDecision();
                lastDecisionTime = Time.time;
            }
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
                
                if (newTarget != null)
                {
                    targetTransform = newTarget;
                }
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
            // 查找所有GameEntity
            GameEntity[] allEntities = GameObject.FindObjectsOfType<GameEntity>();
            
            // 首先尝试查找Castle类型的实体
            foreach (GameEntity entity in allEntities)
            {
                if (entity.Type == GameEntity.EntityType.Castle)
                {
                    castleTransform = entity.transform;
                    targetTransform = castleTransform;
                    return;
                }
            }
            
            // 如果找不到Castle类型，尝试查找Player类型的实体
            foreach (GameEntity entity in allEntities)
            {
                if (entity.Type == GameEntity.EntityType.Player)
                {
                    castleTransform = entity.transform;
                    targetTransform = castleTransform;
                    return;
                }
            }
            
            // 如果还是找不到，尝试查找任何玩家队伍的实体
            foreach (GameEntity entity in allEntities)
            {
                if (entity.TeamSystem.Team == TeamSystem.TeamType.Player)
                {
                    castleTransform = entity.transform;
                    targetTransform = castleTransform;
                    return;
                }
            }
        }
    }
} 