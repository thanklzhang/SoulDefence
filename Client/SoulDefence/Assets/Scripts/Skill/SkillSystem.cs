using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulDefence.Entity;
using SoulDefence.Buff;

namespace SoulDefence.Skill
{
    /// <summary>
    /// 技能系统核心类
    /// 负责技能的释放和伤害计算
    /// </summary>
    public class SkillSystem : MonoBehaviour
    {
        // 单例
        private static SkillSystem instance;
        public static SkillSystem Instance => instance;
        
        [Header("调试设置")]
        [SerializeField] private bool showDebugInfo = true;   // 显示调试信息
        [SerializeField] private bool drawGizmos = true;      // 绘制Gizmos

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            if (showDebugInfo)
            {
                Debug.Log("SkillSystem初始化完成");
            }
        }

        /// <summary>
        /// 使用技能
        /// </summary>
        /// <param name="caster">技能施放者</param>
        /// <param name="skill">技能数据</param>
        /// <param name="targetPosition">目标位置(远程技能使用)</param>
        /// <param name="targetDirection">目标方向(近战技能使用)</param>
        /// <returns>是否成功释放</returns>
        public bool UseSkill(GameEntity caster, SkillData skill, Vector3 targetPosition, Vector3 targetDirection)
        {
            if (caster == null || skill == null)
            {
                if (showDebugInfo)
                {
                    Debug.LogError("UseSkill失败: caster或skill为空");
                }
                return false;
            }

            if (showDebugInfo)
            {
                Debug.Log($"{caster.name}使用技能: {skill.skillName}, 类型: {skill.skillType}");
            }
            
            // 根据技能类型调用不同的释放方法
            switch (skill.skillType)
            {
                case SkillType.Melee:
                    return UseMeleeSkill(caster, skill, targetDirection);
                case SkillType.Ranged:
                    return UseRangedSkill(caster, skill, targetPosition);
                default:
                    if (showDebugInfo)
                    {
                        Debug.LogError($"未知的技能类型: {skill.skillType}");
                    }
                    return false;
            }
        }

        /// <summary>
        /// 使用近战技能
        /// </summary>
        private bool UseMeleeSkill(GameEntity caster, SkillData skill, Vector3 targetDirection)
        {
            // 获取实体的攻击距离作为技能攻击范围
            float attackRange = GetEntityAttackRange(caster, skill);
            
            if (showDebugInfo)
            {
                Debug.Log($"使用近战技能: 范围={attackRange}, 角度={skill.arcAngle}, 方向={targetDirection}");
            }
            
            // 获取扇形区域内的目标
            List<GameEntity> targets = GetTargetsInArc(caster, attackRange, skill.arcAngle, targetDirection, skill.targetCount);
            
            if (showDebugInfo)
            {
                Debug.Log($"找到目标数量: {targets.Count}");
                foreach (var target in targets)
                {
                    Debug.Log($"目标: {target.name}");
                }
            }
            
            // 对每个目标应用伤害和Buff
            foreach (var target in targets)
            {
                ApplyDamage(caster, target, skill.damage);
                
                // 应用技能的Buff效果
                ApplySkillBuffs(caster, target, skill);
            }
            
            // 对自己应用Buff
            if (skill.buffToSelf != null)
            {
                caster.AddBuff(skill.buffToSelf, caster);
            }
            
            return targets.Count > 0;
        }

        /// <summary>
        /// 使用远程技能
        /// </summary>
        private bool UseRangedSkill(GameEntity caster, SkillData skill, Vector3 targetPosition)
        {
            // 检查是否有投掷物预制体
            if (skill.projectilePrefab == null)
            {
                if (showDebugInfo)
                {
                    Debug.LogError("远程技能缺少投掷物预制体");
                }
                return false;
            }
                
            // 计算发射方向
            Vector3 direction = (targetPosition - caster.transform.position).normalized;
            
            if (showDebugInfo)
            {
                Debug.Log($"发射投掷物: 方向={direction}, 目标位置={targetPosition}");
            }
            
            // 创建投掷物
            GameObject projectileObj = Instantiate(
                skill.projectilePrefab, 
                caster.transform.position + direction * 1f, // 稍微偏移起始位置
                Quaternion.LookRotation(direction)
            );
            
            // 设置投掷物参数
            Projectile projectile = projectileObj.GetComponent<Projectile>();
            if (projectile != null)
            {
                // 获取实体的攻击距离
                float attackRange = GetEntityAttackRange(caster, skill);
                
                // 初始化投掷物，传递实体的攻击距离
                projectile.Initialize(caster, skill, direction, attackRange);
            }
            else
            {
                if (showDebugInfo)
                {
                    Debug.LogError("投掷物预制体缺少Projectile组件");
                }
                
                // 如果没有Projectile组件，添加一个简单的移动脚本
                var simpleMove = projectileObj.AddComponent<Projectile>();
                float range = GetEntityAttackRange(caster, skill);
                simpleMove.Initialize(caster, skill, direction, range);
                
                // 设置销毁时间
                Destroy(projectileObj, skill.projectileLifetime);
            }
            
            return true;
        }

        /// <summary>
        /// 获取实体的攻击距离
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="skill">技能数据</param>
        /// <returns>攻击距离</returns>
        private float GetEntityAttackRange(GameEntity entity, SkillData skill)
        {
            // 如果实体有攻击距离属性，则使用实体的攻击距离
            if (entity != null && entity.Attributes != null)
            {
                return entity.Attributes.AttackRange;
            }
            
            // 否则使用技能配置的攻击范围
            return 1;
        }

        /// <summary>
        /// 获取扇形区域内的目标
        /// </summary>
        private List<GameEntity> GetTargetsInArc(GameEntity caster, float range, float arcAngle, Vector3 direction, int maxTargets)
        {
            List<GameEntity> targets = new List<GameEntity>();
            
            // 获取可能的目标
            Collider[] colliders = Physics.OverlapSphere(caster.transform.position, range);
            
            if (showDebugInfo)
            {
                Debug.Log($"OverlapSphere找到碰撞体数量: {colliders.Length}");
            }
            
            // 计算扇形角度的一半(弧度)
            float halfAngleRad = arcAngle * 0.5f * Mathf.Deg2Rad;
            
            // 标准化方向
            Vector3 forward = direction.normalized;
            
            // 检查每个碰撞体
            foreach (var collider in colliders)
            {
                // 忽略自身
                if (collider.gameObject == caster.gameObject)
                    continue;
                
                // 获取目标实体
                GameEntity target = collider.GetComponent<GameEntity>();
                
                // 检查是否是有效目标
                if (target != null && target != caster && target.IsAlive && IsHostile(caster, target))
                {
                    // 计算方向向量
                    Vector3 dirToTarget = (target.transform.position - caster.transform.position).normalized;
                    
                    // 计算夹角
                    float angle = Vector3.Angle(forward, dirToTarget) * Mathf.Deg2Rad;
                    
                    // 检查是否在扇形内
                    if (angle <= halfAngleRad)
                    {
                        if (showDebugInfo)
                        {
                            Debug.Log($"目标 {target.name} 在扇形内，角度: {angle * Mathf.Rad2Deg}度");
                        }
                        
                        targets.Add(target);
                        
                        // 如果达到最大目标数，停止搜索
                        if (targets.Count >= maxTargets)
                            break;
                    }
                    else if (showDebugInfo)
                    {
                        Debug.Log($"目标 {target.name} 不在扇形内，角度: {angle * Mathf.Rad2Deg}度");
                    }
                }
            }
            
            return targets;
        }

        /// <summary>
        /// 应用伤害
        /// </summary>
        public void ApplyDamage(GameEntity attacker, GameEntity target, float baseDamage)
        {
            if (attacker == null || target == null || !target.IsAlive)
            {
                if (showDebugInfo)
                {
                    // Debug.LogError($"ApplyDamage失败: attacker={attacker}, target={target}, target.IsAlive={target?.IsAlive}");
                }
                return;
            }
                
            // 计算实际伤害
            float actualDamage = CalculateDamage(attacker, target, baseDamage);
            
            if (showDebugInfo)
            {
                // Debug.Log($"{attacker.name}对{target.name}造成{actualDamage}点伤害");
            }
            
            // 应用伤害
            target.TakeDamage(actualDamage);
        }
        
        /// <summary>
        /// 应用技能的Buff效果
        /// </summary>
        private void ApplySkillBuffs(GameEntity caster, GameEntity target, SkillData skill)
        {
            if (skill == null || !skill.HasBuffEffect)
                return;

            // 对目标应用Buff
            if (skill.buffToTarget != null && target != null && target.IsAlive)
            {
                target.AddBuff(skill.buffToTarget, caster);
                
                if (showDebugInfo)
                {
                    Debug.Log($"[技能Buff] {caster.name} 的技能 {skill.skillName} 给 {target.name} 添加Buff: {skill.buffToTarget.buffName}");
                }
            }
        }

        /// <summary>
        /// 计算伤害
        /// </summary>
        private float CalculateDamage(GameEntity attacker, GameEntity target, float baseDamage)
        {
            // 基于攻击者属性和目标防御计算实际伤害
            float attackPower = attacker.Attributes.AttackPower;
            float defense = target.Attributes.Defense;
            
            // 简单的伤害公式：基础伤害 * (1 + 攻击力/100) - 防御/2
            // float damage = baseDamage * (1 + attackPower / 100) - defense / 2;
            float damage = attackPower - defense / 2;
            
            // 确保伤害至少为1
            return Mathf.Max(1, damage);
        }

        /// <summary>
        /// 判断两个实体是否敌对
        /// </summary>
        private bool IsHostile(GameEntity entity1, GameEntity entity2)
        {
            // 使用队伍系统判断
            bool isHostile = entity1.TeamSystem.IsHostile(entity2.TeamSystem);
            
            if (showDebugInfo)
            {
                Debug.Log($"{entity1.name}和{entity2.name}是否敌对: {isHostile}");
            }
            
            return isHostile;
        }
        
        /// <summary>
        /// 绘制调试信息
        /// </summary>
        private void OnDrawGizmos()
        {
            if (!drawGizmos || !Application.isPlaying)
                return;
                
            // 可以在这里添加全局的调试绘制
        }
    }
} 