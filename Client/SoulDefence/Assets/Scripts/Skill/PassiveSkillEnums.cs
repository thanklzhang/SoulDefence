namespace SoulDefence.Skill
{
    /// <summary>
    /// 被动技能触发类型
    /// </summary>
    public enum PassiveTriggerType
    {
        None = 0,               // 无触发（永久生效，如属性加成）
        OnAttack = 1,           // 攻击时触发
        OnHit = 2,              // 受击时触发
        OnKill = 3,             // 击杀时触发
        Interval = 4,           // 定时触发
        OnHealthLow = 5,        // 生命值低时触发
        OnAttackCount = 6,      // 攻击次数触发（如每N次攻击）
    }

    /// <summary>
    /// 被动效果类型
    /// </summary>
    public enum PassiveEffectType
    {
        // 属性加成类
        AttributeBonus = 0,         // 属性加成（攻击力、防御力等）
        
        // 伤害修改类
        DamageMultiplier = 10,      // 伤害倍率（造成伤害时）
        ExtraDamage = 11,           // 额外伤害
        CriticalHit = 12,           // 暴击
        
        // 防御修改类
        DamageReduction = 20,       // 伤害减免（受到伤害时）
        Shield = 21,                // 护盾
        
        // 恢复类
        HealthRegen = 30,           // 生命恢复
        
        // 特殊效果类
        Stun = 40,                  // 眩晕
        Slow = 41,                  // 减速
        Custom = 99,                // 自定义效果
    }
}

