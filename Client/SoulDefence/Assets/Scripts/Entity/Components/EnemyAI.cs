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
            // aiEnabled = true;
            
            // 设置检测范围和巡逻范围
            detectionRange = 10f;
            patrolRange = 20f;  // 敌人的巡逻范围较大
            
            // 寻找城堡
            FindCastle();
            
            // 如果找不到城堡，尝试向场景中心移动
            if (castleTransform == null)
            {
                Debug.LogWarning("找不到城堡，将使用场景中心作为默认移动目标");
                // 创建一个空物体作为默认目标
                GameObject defaultTarget = new GameObject("DefaultTarget");
                defaultTarget.transform.position = Vector3.zero; // 场景中心
                castleTransform = defaultTarget.transform;
                targetTransform = castleTransform;
            }
            
            Debug.Log($"敌人AI初始化完成: AI启用={aiEnabled}, 目标={targetTransform?.name ?? "无"}");
        }
        
        /// <summary>
        /// 更新AI逻辑
        /// </summary>
        public override void UpdateAI()
        {
            if (!aiEnabled)
            {
                Debug.LogWarning("敌人AI未启用");
                return;
            }
            
            if (entity == null)
            {
                Debug.LogError("敌人AI的实体引用为空");
                return;
            }
            
            if (!entity.IsAlive)
            {
                Debug.LogWarning("敌人已死亡，AI不执行");
                return;
            }
            
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
                    Debug.Log($"敌人找到新目标: {newTarget.name}");
                    targetTransform = newTarget;
                }
                else if (castleTransform != null)
                {
                    Debug.Log("敌人未找到玩家，目标设为城堡");
                    targetTransform = castleTransform;
                }
                else
                {
                    Debug.LogWarning("敌人没有找到任何目标，包括城堡");
                }
                
                lastTargetUpdateTime = Time.time;
            }

            // 如果有目标，向目标移动
            if (targetTransform != null)
            {
                // 计算到目标的距离
                float distanceToTarget = Vector3.Distance(transform.position, targetTransform.position);
                float attackRange = GetAttackRange();
                
                Debug.Log($"敌人到目标({targetTransform.name})距离: {distanceToTarget}, 攻击范围: {attackRange}");
                
                // 如果在攻击范围内，停止移动并攻击
                if (distanceToTarget <= attackRange)
                {
                    Debug.Log("敌人在攻击范围内，停止移动并攻击");
                    entity.StopMovement();
                    AttackTarget(targetTransform);
                }
                // 否则继续移动
                else
                {
                    Debug.Log($"敌人向目标({targetTransform.name})移动");
                    MoveToTarget(targetTransform, attackRange);
                }
            }
            else
            {
                // 没有目标，尝试重新寻找城堡
                Debug.LogWarning("敌人没有目标，尝试重新寻找城堡");
                FindCastle();
                
                // 如果仍然没有目标，停止移动
                if (targetTransform == null)
                {
                    Debug.LogError("敌人找不到任何目标，停止移动");
                    entity.StopMovement();
                }
            }
        }

        /// <summary>
        /// 寻找城堡
        /// </summary>
        private void FindCastle()
        {
            // 尝试通过标签查找城堡
            GameObject castleObj = GameObject.FindGameObjectWithTag("Castle");
            
            // 如果没找到，尝试通过名称查找
            if (castleObj == null)
            {
                Debug.LogWarning("找不到标签为'Castle'的游戏对象，尝试通过名称查找");
                castleObj = GameObject.Find("Castle");
            }
            
            // 如果还没找到，尝试查找带有"castle"的游戏对象（不区分大小写）
            if (castleObj == null)
            {
                Debug.LogWarning("找不到名称为'Castle'的游戏对象，尝试查找包含'castle'的游戏对象");
                GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
                foreach (GameObject obj in allObjects)
                {
                    if (obj.name.ToLower().Contains("castle"))
                    {
                        castleObj = obj;
                        Debug.Log($"找到包含'castle'的游戏对象: {obj.name}");
                        break;
                    }
                }
            }
            
            // 如果找到了城堡，设置为目标
            if (castleObj != null)
            {
                Debug.Log($"找到城堡: {castleObj.name}");
                castleTransform = castleObj.transform;
                targetTransform = castleTransform;
            }
            else
            {
                Debug.LogError("无法找到任何可能的城堡对象");
                
                // 如果实在找不到，尝试找到玩家队伍的实体作为目标
                GameEntity[] allEntities = GameObject.FindObjectsOfType<GameEntity>();
                foreach (GameEntity entity in allEntities)
                {
                    if (entity.TeamSystem.Team == TeamSystem.TeamType.Player)
                    {
                        Debug.Log($"找到玩家队伍实体作为替代目标: {entity.name}");
                        castleTransform = entity.transform;
                        targetTransform = castleTransform;
                        break;
                    }
                }
            }
        }
    }
} 