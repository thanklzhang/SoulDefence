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
        /// 使用技能（主入口）
        /// </summary>
        /// <param name="caster">技能施放者</param>
        /// <param name="skill">技能数据</param>
        /// <param name="targetPosition">目标位置</param>
        /// <param name="targetDirection">目标方向</param>
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
                Debug.Log($"{caster.name}使用技能: {skill.skillName}, 效果类型: {skill.effectType}");
            }
            
            // 根据技能效果类型执行不同逻辑
            bool success = true;
            
            // 移动型技能
            if (skill.IsMovementSkill)
            {
                success &= ExecuteMovementSkill(caster, skill, targetDirection);
            }
            
            // 伤害型技能
            if (skill.IsDamageSkill)
            {
                success &= ExecuteDamageSkill(caster, skill, targetPosition, targetDirection);
            }
            
            // 状态型技能（Buff）
            if (skill.IsStatusSkill)
            {
                success &= ExecuteStatusSkill(caster, skill, targetPosition, targetDirection);
            }
            
            return success;
        }

        // ========== 技能效果执行方法 ==========
        
        /// <summary>
        /// 执行移动型技能（冲刺、传送）
        /// </summary>
        private bool ExecuteMovementSkill(GameEntity caster, SkillData skill, Vector3 direction)
        {
            if (caster == null) return false;
            
            Vector3 moveDirection = direction.normalized;
            Vector3 targetPos;
            
            if (skill.isTeleport)
            {
                // 传送：瞬间移动到目标位置
                targetPos = caster.transform.position + moveDirection * skill.movementDistance;
                caster.transform.position = targetPos;
                
                if (showDebugInfo)
                {
                    Debug.Log($"{caster.name} 传送到: {targetPos}");
                }
            }
            else
            {
                // 冲刺：在一段时间内移动
                StartCoroutine(DashCoroutine(caster, moveDirection, skill.movementDistance, skill.movementDuration));
                
                if (showDebugInfo)
                {
                    Debug.Log($"{caster.name} 开始冲刺，距离: {skill.movementDistance}, 时间: {skill.movementDuration}");
                }
            }
            
            return true;
        }
        
        /// <summary>
        /// 冲刺协程
        /// </summary>
        private IEnumerator DashCoroutine(GameEntity caster, Vector3 direction, float distance, float duration)
        {
            Vector3 startPos = caster.transform.position;
            Vector3 endPos = startPos + direction * distance;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                caster.transform.position = Vector3.Lerp(startPos, endPos, t);
                yield return null;
            }
            
            caster.transform.position = endPos;
        }
        
        /// <summary>
        /// 执行伤害型技能
        /// </summary>
        private bool ExecuteDamageSkill(GameEntity caster, SkillData skill, Vector3 targetPosition, Vector3 targetDirection)
        {
            // 根据攻击方式选择执行方法
            if (skill.attackType == SkillAttackType.Melee)
            {
                return ExecuteMeleeDamage(caster, skill, targetDirection);
            }
            else if (skill.attackType == SkillAttackType.Ranged)
            {
                return ExecuteRangedDamage(caster, skill, targetPosition);
            }
            
            return false;
        }
        
        /// <summary>
        /// 执行状态型技能（Buff）
        /// </summary>
        private bool ExecuteStatusSkill(GameEntity caster, SkillData skill, Vector3 targetPosition, Vector3 targetDirection)
        {
            // 对自己应用Buff
            if (skill.buffToSelf != null)
            {
                caster.AddBuff(skill.buffToSelf, caster);
                
                if (showDebugInfo)
                {
                    Debug.Log($"[状态技能] {caster.name} 给自己添加Buff: {skill.buffToSelf.buffName}");
                }
            }
            
            // 如果是纯状态型技能（不带伤害），也需要对范围内目标应用Buff
            if (skill.effectType == SkillEffectType.Status && skill.buffToTarget != null)
            {
                // 根据范围类型获取目标
                List<GameEntity> targets = GetTargetsInRange(caster, skill, targetDirection);
                
                foreach (var target in targets)
                {
                    ApplySkillBuffs(caster, target, skill);
                }
                
                return targets.Count > 0;
            }
            
            return true;
        }
        
        // ========== 近战/远程伤害执行方法 ==========
        
        /// <summary>
        /// 执行近战伤害
        /// </summary>
        private bool ExecuteMeleeDamage(GameEntity caster, SkillData skill, Vector3 targetDirection)
        {
            // 获取范围内的目标
            List<GameEntity> targets = GetTargetsInRange(caster, skill, targetDirection);
            
            if (showDebugInfo)
            {
                Debug.Log($"近战技能找到目标数量: {targets.Count}");
            }
            
            // 对每个目标应用伤害和Buff
            foreach (var target in targets)
            {
                ApplyDamage(caster, target, skill.damage);
                ApplySkillBuffs(caster, target, skill);
            }
            
            return targets.Count > 0;
        }

        /// <summary>
        /// 执行远程伤害
        /// </summary>
        private bool ExecuteRangedDamage(GameEntity caster, SkillData skill, Vector3 targetPosition)
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
                
                // 初始化投掷物
                projectile.Initialize(caster, skill, direction, attackRange);
            }
            else
            {
                if (showDebugInfo)
                {
                    Debug.LogError("投掷物预制体缺少Projectile组件");
                }
                
                // 如果没有Projectile组件，添加一个
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

        // ========== 范围检测方法（支持多种范围类型） ==========
        
        /// <summary>
        /// 根据技能配置获取范围内的目标（统一入口）
        /// </summary>
        private List<GameEntity> GetTargetsInRange(GameEntity caster, SkillData skill, Vector3 direction)
        {
            float range = GetEntityAttackRange(caster, skill);
            
            // 根据范围类型选择不同的检测方法
            switch (skill.rangeType)
            {
                case SkillRangeType.Single:
                    return GetSingleTarget(caster, range, direction, skill.targetCount);
                    
                case SkillRangeType.Arc:
                    return GetTargetsInArc(caster, range, skill.rangeSize, direction, skill.targetCount);
                    
                case SkillRangeType.Circle:
                    return GetTargetsInCircle(caster, range, skill.targetCount);
                    
                default:
                    return new List<GameEntity>();
            }
        }
        
        /// <summary>
        /// 获取单个目标（最近的敌人）
        /// </summary>
        private List<GameEntity> GetSingleTarget(GameEntity caster, float range, Vector3 direction, int maxTargets)
        {
            List<GameEntity> targets = new List<GameEntity>();
            Collider[] colliders = Physics.OverlapSphere(caster.transform.position, range);
            
            // 找到最近的敌对目标
            GameEntity closestTarget = null;
            float closestDistance = float.MaxValue;
            
            foreach (var collider in colliders)
            {
                if (collider.gameObject == caster.gameObject) continue;
                
                GameEntity target = collider.GetComponent<GameEntity>();
                if (target != null && target != caster && target.IsAlive && IsHostile(caster, target))
                {
                    float distance = Vector3.Distance(caster.transform.position, target.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestTarget = target;
                    }
                }
            }
            
            if (closestTarget != null)
            {
                targets.Add(closestTarget);
                
                if (showDebugInfo)
                {
                    Debug.Log($"找到单体目标: {closestTarget.name}, 距离: {closestDistance}");
                }
            }
            
            return targets;
        }
        
        /// <summary>
        /// 获取扇形区域内的目标
        /// </summary>
        private List<GameEntity> GetTargetsInArc(GameEntity caster, float range, float arcAngle, Vector3 direction, int maxTargets)
        {
            List<GameEntity> targets = new List<GameEntity>();
            Collider[] colliders = Physics.OverlapSphere(caster.transform.position, range);
            
            if (showDebugInfo)
            {
                Debug.Log($"扇形检测: 范围={range}, 角度={arcAngle}, 找到碰撞体={colliders.Length}");
            }
            
            // 计算扇形角度的一半
            float halfAngle = arcAngle * 0.5f;
            Vector3 forward = direction.normalized;
            
            foreach (var collider in colliders)
            {
                if (collider.gameObject == caster.gameObject) continue;
                
                GameEntity target = collider.GetComponent<GameEntity>();
                if (target != null && target != caster && target.IsAlive && IsHostile(caster, target))
                {
                    Vector3 dirToTarget = (target.transform.position - caster.transform.position).normalized;
                    float angle = Vector3.Angle(forward, dirToTarget);
                    
                    if (angle <= halfAngle)
                    {
                        targets.Add(target);
                        
                        if (showDebugInfo)
                        {
                            Debug.Log($"目标 {target.name} 在扇形内，角度: {angle}度");
                        }
                        
                        if (targets.Count >= maxTargets) break;
                    }
                }
            }
            
            return targets;
        }
        
        /// <summary>
        /// 获取圆形区域内的目标
        /// </summary>
        private List<GameEntity> GetTargetsInCircle(GameEntity caster, float range, int maxTargets)
        {
            List<GameEntity> targets = new List<GameEntity>();
            Collider[] colliders = Physics.OverlapSphere(caster.transform.position, range);
            
            if (showDebugInfo)
            {
                Debug.Log($"圆形检测: 范围={range}, 找到碰撞体={colliders.Length}");
            }
            
            foreach (var collider in colliders)
            {
                if (collider.gameObject == caster.gameObject) continue;
                
                GameEntity target = collider.GetComponent<GameEntity>();
                if (target != null && target != caster && target.IsAlive && IsHostile(caster, target))
                {
                    targets.Add(target);
                    
                    if (showDebugInfo)
                    {
                        Debug.Log($"目标 {target.name} 在圆形范围内");
                    }
                    
                    if (targets.Count >= maxTargets) break;
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