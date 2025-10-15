using System.Collections.Generic;
using UnityEngine;
using SoulDefence.Entity;
using SoulDefence.Core;

namespace SoulDefence.Skill
{
    /// <summary>
    /// 被动技能系统
    /// 管理实体的所有被动技能
    /// </summary>
    [System.Serializable]
    public class PassiveSkillSystem
    {
        private GameEntity owner;                                   // 所有者
        private List<PassiveSkillInstance> passiveSkills;          // 被动技能实例列表

        /// <summary>
        /// 初始化被动技能系统
        /// </summary>
        public void Initialize(GameEntity entity, SkillData[] skills)
        {
            owner = entity;
            passiveSkills = new List<PassiveSkillInstance>();

            // 从技能列表中提取被动技能
            if (skills != null)
            {
                foreach (var skill in skills)
                {
                    if (skill != null && skill.HasPassiveSkill)
                    {
                        var passiveInstance = new PassiveSkillInstance(skill.passiveSkill, entity);
                        passiveSkills.Add(passiveInstance);
                        
                        Debug.Log($"[被动技能系统] 初始化被动技能: {skill.passiveSkill.passiveName}");
                        
                        // 如果是永久效果（属性加成），立即应用
                        if (skill.passiveSkill.IsPermanentEffect && 
                            skill.passiveSkill.effectType == PassiveEffectType.AttributeBonus)
                        {
                            ApplyPermanentAttributeBonus(skill.passiveSkill);
                        }
                    }
                }
            }

            Debug.Log($"[被动技能系统] 初始化完成，共 {passiveSkills.Count} 个被动技能");
        }

        /// <summary>
        /// 更新被动技能系统
        /// </summary>
        public void Update(float deltaTime)
        {
            foreach (var passive in passiveSkills)
            {
                passive.Update(deltaTime);
            }
        }

        /// <summary>
        /// 攻击时触发
        /// </summary>
        public void OnAttack()
        {
            foreach (var passive in passiveSkills)
            {
                passive.OnAttack();
            }
        }

        /// <summary>
        /// 受击时触发
        /// </summary>
        public void OnHit()
        {
            foreach (var passive in passiveSkills)
            {
                passive.OnHit();
            }
        }

        /// <summary>
        /// 击杀时触发
        /// </summary>
        public void OnKill()
        {
            foreach (var passive in passiveSkills)
            {
                passive.OnKill();
            }
        }

        /// <summary>
        /// 获取伤害修正（攻击时）
        /// 用于攻击时的被动技能效果
        /// </summary>
        public float GetDamageModifier(float baseDamage, PassiveTriggerType triggerType)
        {
            float totalModifier = 0f;

            foreach (var passive in passiveSkills)
            {
                // 只计算匹配触发类型的被动技能
                if (passive.Data.triggerType == triggerType ||
                    passive.Data.triggerType == PassiveTriggerType.None)
                {
                    totalModifier += passive.GetDamageModifier(baseDamage);
                }
            }

            return totalModifier;
        }

        /// <summary>
        /// 获取受击伤害修正
        /// 用于受击时的被动技能效果
        /// </summary>
        public float GetDamageReductionModifier(float incomingDamage)
        {
            float totalReduction = 0f;

            foreach (var passive in passiveSkills)
            {
                if (passive.Data.triggerType == PassiveTriggerType.OnHit ||
                    passive.Data.triggerType == PassiveTriggerType.None)
                {
                    totalReduction += passive.GetDamageReductionModifier(incomingDamage);
                }
            }

            return totalReduction;
        }

        /// <summary>
        /// 获取总属性加成
        /// </summary>
        public Dictionary<AttributeType, float> GetAttributeBonuses()
        {
            var bonuses = new Dictionary<AttributeType, float>();

            foreach (var passive in passiveSkills)
            {
                if (passive.Data.effectType == PassiveEffectType.AttributeBonus &&
                    passive.Data.IsPermanentEffect)
                {
                    AttributeType attrType = passive.Data.attributeType;
                    float bonus = passive.GetAttributeBonus();
                    
                    if (bonuses.ContainsKey(attrType))
                    {
                        bonuses[attrType] += bonus;
                    }
                    else
                    {
                        bonuses[attrType] = bonus;
                    }
                }
            }

            return bonuses;
        }

        /// <summary>
        /// 应用永久属性加成
        /// </summary>
        private void ApplyPermanentAttributeBonus(PassiveSkillData passiveData)
        {
            if (owner == null || owner.Attributes == null)
                return;

            // 注意：这里的属性加成应该通过属性系统的组合模式来实现
            // 目前先记录，实际应用需要在属性重算时处理
            Debug.Log($"[被动技能] {passiveData.passiveName} 永久加成: " +
                     $"属性类型={passiveData.attributeType}, 加成值={passiveData.attributeBonusValue}");
        }

        /// <summary>
        /// 重置所有被动技能
        /// </summary>
        public void ResetAll()
        {
            foreach (var passive in passiveSkills)
            {
                passive.Reset();
            }
        }

        /// <summary>
        /// 获取被动技能数量
        /// </summary>
        public int GetPassiveSkillCount()
        {
            return passiveSkills != null ? passiveSkills.Count : 0;
        }

        /// <summary>
        /// 添加被动技能
        /// </summary>
        public void AddPassiveSkill(SkillData skill)
        {
            if (skill == null || !skill.HasPassiveSkill || owner == null)
                return;

            var passiveInstance = new PassiveSkillInstance(skill.passiveSkill, owner);
            passiveSkills.Add(passiveInstance);
            
            Debug.Log($"[被动技能系统] 添加被动技能: {skill.passiveSkill.passiveName}");

            // 如果是永久效果，立即应用
            if (skill.passiveSkill.IsPermanentEffect &&
                skill.passiveSkill.effectType == PassiveEffectType.AttributeBonus)
            {
                ApplyPermanentAttributeBonus(skill.passiveSkill);
                // 触发属性重算
                if (owner.Attributes != null)
                {
                    owner.Attributes.RecalculateAttributes();
                }
            }
        }

        /// <summary>
        /// 移除被动技能
        /// </summary>
        public void RemovePassiveSkill(int index)
        {
            if (index >= 0 && index < passiveSkills.Count)
            {
                passiveSkills.RemoveAt(index);
                // 触发属性重算
                if (owner != null && owner.Attributes != null)
                {
                    owner.Attributes.RecalculateAttributes();
                }
            }
        }
    }
}

