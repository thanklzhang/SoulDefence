using UnityEngine;

namespace SoulDefence.Skill
{
    /// <summary>
    /// 被动技能配置数据
    /// </summary>
    [System.Serializable]
    public class PassiveSkillData
    {
        [Header("被动技能基础")]
        public bool enabled = true;                    // 是否启用被动技能
        public string passiveName = "被动技能";         // 被动技能名称
        [TextArea(2, 4)]
        public string description = "";                 // 被动技能描述

        [Header("触发条件")]
        public PassiveTriggerType triggerType;          // 触发类型
        
        [Header("触发条件参数")]
        [Tooltip("攻击次数触发：每N次攻击触发一次")]
        public int attackCountThreshold = 3;            // 攻击次数阈值
        
        [Tooltip("定时触发：间隔秒数")]
        public float intervalTime = 5f;                 // 定时触发间隔
        
        [Tooltip("生命值触发：低于百分比时触发(0-100)")]
        [Range(0f, 100f)]
        public float healthThreshold = 30f;             // 生命值阈值百分比

        [Header("效果配置")]
        public PassiveEffectType effectType;            // 效果类型
        
        [Header("效果参数")]
        [Tooltip("属性加成：增加的属性值")]
        public float attributeBonusValue = 0f;          // 属性加成值
        
        [Tooltip("属性加成类型：0=攻击力, 1=防御力, 2=攻击速度, 3=移动速度")]
        public int attributeType = 0;                   // 属性类型
        
        [Tooltip("伤害倍率：1.0=100%, 1.5=150%")]
        public float damageMultiplier = 1.5f;           // 伤害倍率
        
        [Tooltip("额外伤害：固定增加的伤害值")]
        public float extraDamage = 10f;                 // 额外伤害
        
        [Tooltip("暴击倍率：1.5=150%伤害")]
        public float criticalMultiplier = 2.0f;         // 暴击倍率
        
        [Tooltip("伤害减免：0.2=减免20%伤害")]
        [Range(0f, 1f)]
        public float damageReductionPercent = 0.2f;     // 伤害减免百分比
        
        [Tooltip("护盾值：吸收伤害的护盾量")]
        public float shieldAmount = 50f;                // 护盾值
        
        [Tooltip("生命恢复：每次恢复的生命值")]
        public float healAmount = 20f;                  // 生命恢复量
        
        [Tooltip("效果持续时间：秒数，0=瞬时效果")]
        public float effectDuration = 0f;               // 效果持续时间

        [Header("冷却配置")]
        [Tooltip("是否有冷却时间")]
        public bool hasCooldown = false;                // 是否有冷却
        [Tooltip("冷却时间：秒数")]
        public float cooldownTime = 10f;                // 冷却时间

        /// <summary>
        /// 是否是永久效果（无触发条件）
        /// </summary>
        public bool IsPermanentEffect => triggerType == PassiveTriggerType.None;
    }
}

