using UnityEngine;
using SoulDefence.Entity;

namespace SoulDefence.Skill
{
    /// <summary>
    /// 被动技能实例
    /// 管理被动技能的运行时状态
    /// </summary>
    public class PassiveSkillInstance
    {
        private PassiveSkillData passiveData;       // 被动技能配置
        private GameEntity owner;                   // 所有者
        
        // 运行时状态
        private int attackCount;                    // 攻击计数
        private float intervalTimer;                // 定时器
        private float cooldownRemaining;            // 冷却剩余时间
        private bool isHealthLowTriggered;          // 生命值低是否已触发

        /// <summary>
        /// 构造函数
        /// </summary>
        public PassiveSkillInstance(PassiveSkillData data, GameEntity owner)
        {
            this.passiveData = data;
            this.owner = owner;
            this.attackCount = 0;
            this.intervalTimer = data.intervalTime;
            this.cooldownRemaining = 0f;
            this.isHealthLowTriggered = false;
        }

        /// <summary>
        /// 获取被动技能数据
        /// </summary>
        public PassiveSkillData Data => passiveData;

        /// <summary>
        /// 是否在冷却中
        /// </summary>
        public bool IsOnCooldown => cooldownRemaining > 0f;

        /// <summary>
        /// 更新被动技能（每帧调用）
        /// </summary>
        public void Update(float deltaTime)
        {
            // 更新冷却
            if (cooldownRemaining > 0f)
            {
                cooldownRemaining -= deltaTime;
                if (cooldownRemaining < 0f)
                    cooldownRemaining = 0f;
            }

            // 定时触发
            if (passiveData.triggerType == PassiveTriggerType.Interval)
            {
                intervalTimer -= deltaTime;
                if (intervalTimer <= 0f)
                {
                    TriggerEffect();
                    intervalTimer = passiveData.intervalTime;
                }
            }

            // 生命值低触发
            if (passiveData.triggerType == PassiveTriggerType.OnHealthLow)
            {
                float healthPercent = owner.Attributes.HealthPercentage * 100f;
                if (healthPercent <= passiveData.healthThreshold && !isHealthLowTriggered)
                {
                    TriggerEffect();
                    isHealthLowTriggered = true;
                }
                else if (healthPercent > passiveData.healthThreshold)
                {
                    isHealthLowTriggered = false;
                }
            }
        }

        /// <summary>
        /// 攻击时触发检测
        /// </summary>
        public void OnAttack()
        {
            if (passiveData.triggerType == PassiveTriggerType.OnAttack)
            {
                TriggerEffect();
            }
            else if (passiveData.triggerType == PassiveTriggerType.OnAttackCount)
            {
                attackCount++;
                if (attackCount >= passiveData.attackCountThreshold)
                {
                    TriggerEffect();
                    attackCount = 0;
                }
            }
        }

        /// <summary>
        /// 受击时触发检测
        /// </summary>
        public void OnHit()
        {
            if (passiveData.triggerType == PassiveTriggerType.OnHit)
            {
                TriggerEffect();
            }
        }

        /// <summary>
        /// 击杀时触发检测
        /// </summary>
        public void OnKill()
        {
            if (passiveData.triggerType == PassiveTriggerType.OnKill)
            {
                TriggerEffect();
            }
        }

        /// <summary>
        /// 触发效果
        /// </summary>
        private void TriggerEffect()
        {
            // 检查冷却
            if (passiveData.hasCooldown && IsOnCooldown)
            {
                return;
            }

            // 应用效果
            ApplyEffect();

            // 开始冷却
            if (passiveData.hasCooldown)
            {
                cooldownRemaining = passiveData.cooldownTime;
            }
        }

        /// <summary>
        /// 应用效果
        /// </summary>
        private void ApplyEffect()
        {
            if (owner == null || !owner.IsAlive)
                return;

            switch (passiveData.effectType)
            {
                case PassiveEffectType.ExtraDamage:
                    // 额外伤害在攻击时由外部处理
                    Debug.Log($"[被动技能] {passiveData.passiveName} 触发：额外伤害 {passiveData.extraDamage}");
                    break;

                case PassiveEffectType.Shield:
                    // TODO: 需要护盾系统支持
                    Debug.Log($"[被动技能] {passiveData.passiveName} 触发：护盾 {passiveData.shieldAmount}");
                    break;

                case PassiveEffectType.HealthRegen:
                    owner.Heal(passiveData.healAmount);
                    Debug.Log($"[被动技能] {passiveData.passiveName} 触发：恢复生命 {passiveData.healAmount}");
                    break;

                case PassiveEffectType.DamageReduction:
                    // 伤害减免在受击时由外部处理
                    Debug.Log($"[被动技能] {passiveData.passiveName} 触发：伤害减免 {passiveData.damageReductionPercent * 100}%");
                    break;

                default:
                    Debug.Log($"[被动技能] {passiveData.passiveName} 触发");
                    break;
            }
        }

        /// <summary>
        /// 获取伤害修正（攻击时调用）
        /// </summary>
        public float GetDamageModifier(float baseDamage)
        {
            float modifier = 0f;

            switch (passiveData.effectType)
            {
                case PassiveEffectType.DamageMultiplier:
                    modifier = baseDamage * (passiveData.damageMultiplier - 1f);
                    break;

                case PassiveEffectType.ExtraDamage:
                    modifier = passiveData.extraDamage;
                    break;

                case PassiveEffectType.CriticalHit:
                    // 暴击判定（可以加入随机或条件）
                    modifier = baseDamage * (passiveData.criticalMultiplier - 1f);
                    break;
            }

            return modifier;
        }

        /// <summary>
        /// 获取受击伤害修正（受击时调用）
        /// </summary>
        public float GetDamageReductionModifier(float incomingDamage)
        {
            if (passiveData.effectType == PassiveEffectType.DamageReduction)
            {
                return incomingDamage * passiveData.damageReductionPercent;
            }
            return 0f;
        }

        /// <summary>
        /// 获取属性加成值
        /// </summary>
        public float GetAttributeBonus()
        {
            if (passiveData.effectType == PassiveEffectType.AttributeBonus)
            {
                return passiveData.attributeBonusValue;
            }
            return 0f;
        }

        /// <summary>
        /// 重置状态
        /// </summary>
        public void Reset()
        {
            attackCount = 0;
            intervalTimer = passiveData.intervalTime;
            cooldownRemaining = 0f;
            isHealthLowTriggered = false;
        }
    }
}

