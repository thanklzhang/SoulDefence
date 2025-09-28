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
        private Transform playerTransform;                          // 玩家位置
        private float lastTargetUpdateTime = 0f;

        /// <summary>
        /// 初始化敌人AI
        /// </summary>
        public override void Initialize(Transform entityTransform, GameEntity gameEntity, TeamSystem team)
        {
            base.Initialize(entityTransform, gameEntity, team);
            
            // 设置检测范围和巡逻范围
            patrolRange = 4.0f;  // 敌人的巡逻范围较大
            
            // 寻找城堡和玩家
            FindTargets();
            
            // 如果找不到任何目标，尝试向场景中心移动
            if (castleTransform == null && playerTransform == null)
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
                // 更新城堡和玩家的位置
                FindTargets();
                
                // 选择最优目标
                SelectBestTarget();
                
                lastTargetUpdateTime = Time.time;
            }

            // 检查当前目标是否有效
            if (targetTransform == null || !IsValidTarget(targetTransform))
            {
                // 目标无效，重新选择目标
                FindTargets();
                SelectBestTarget();
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
                // 没有目标，尝试重新寻找目标
                FindTargets();
                SelectBestTarget();
                
                // 如果仍然没有目标，停止移动
                if (targetTransform == null)
                {
                    entity.StopMovement();
                }
            }
        }

        /// <summary>
        /// 检查目标是否有效
        /// </summary>
        private bool IsValidTarget(Transform target)
        {
            if (target == null)
                return false;
                
            // 检查目标是否存在
            if (target.gameObject == null || !target.gameObject.activeInHierarchy)
                return false;
                
            // 检查目标是否有GameEntity组件
            GameEntity targetEntity = target.GetComponent<GameEntity>();
            if (targetEntity == null || !targetEntity.IsAlive)
                return false;
                
            return true;
        }

        /// <summary>
        /// 寻找城堡和玩家
        /// </summary>
        private void FindTargets()
        {
            castleTransform = null;
            playerTransform = null;
            
            // 查找所有GameEntity
            GameEntity[] allEntities = GameObject.FindObjectsOfType<GameEntity>();
            
            // 查找敌方城堡和玩家
            foreach (GameEntity targetEntity in allEntities)
            {
                // 跳过自己
                if (targetEntity == entity)
                    continue;
                    
                // 跳过非活跃或已死亡的实体
                if (!targetEntity.gameObject.activeInHierarchy || !targetEntity.IsAlive)
                    continue;
                
                // 检查是否为敌对队伍
                if (teamSystem.IsHostile(targetEntity.TeamSystem))
                {
                    // 根据实体类型分类
                    if (targetEntity.Type == GameEntity.EntityType.Castle && castleTransform == null)
                    {
                        castleTransform = targetEntity.transform;
                    }
                    else if (targetEntity.Type == GameEntity.EntityType.Player && playerTransform == null)
                    {
                        playerTransform = targetEntity.transform;
                    }
                }
            }
        }
        
        /// <summary>
        /// 选择最优目标
        /// </summary>
        private void SelectBestTarget()
        {
            // 默认目标为null
            targetTransform = null;
            
            // 首先检查是否有玩家在巡逻范围内
            if (playerTransform != null)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
                
                // 如果玩家在巡逻范围内，选择玩家为目标
                if (distanceToPlayer <= patrolRange)
                {
                    targetTransform = playerTransform;
                    return;
                }
            }
            
            // 如果没有玩家或玩家超出范围，选择城堡为目标
            if (castleTransform != null)
            {
                targetTransform = castleTransform;
                return;
            }
            
            // 如果既没有玩家也没有城堡，尝试查找任何敌对实体
            Collider[] colliders = Physics.OverlapSphere(transform.position, patrolRange);
            float closestDistance = float.MaxValue;
            Transform closestTarget = null;
            
            foreach (var collider in colliders)
            {
                if (collider == null || !collider.gameObject.activeInHierarchy)
                    continue;
                    
                GameEntity targetEntity = collider.GetComponent<GameEntity>();
                if (targetEntity != null && targetEntity != entity && targetEntity.IsAlive)
                {
                    // 检查是否为敌对队伍
                    if (teamSystem.IsHostile(targetEntity.TeamSystem))
                    {
                        float distance = Vector3.Distance(transform.position, collider.transform.position);
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestTarget = collider.transform;
                        }
                    }
                }
            }
            
            // 如果找到了敌对实体，选择它为目标
            if (closestTarget != null)
            {
                targetTransform = closestTarget;
            }
        }
    }
} 