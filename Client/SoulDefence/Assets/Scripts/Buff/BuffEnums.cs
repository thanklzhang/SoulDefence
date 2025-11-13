namespace SoulDefence.Buff
{
    /// <summary>
    /// Buff类型
    /// </summary>
    public enum BuffType
    {
        Positive = 0,       // 正面Buff（增益）
        Negative = 1,       // 负面Buff（减益）
        Neutral = 2,        // 中性Buff
    }

    /// <summary>
    /// Buff效果类型
    /// </summary>
    public enum BuffEffectType
    {
        // 属性修改类
        AttributeModifier = 0,      // 属性修改（攻击力、防御力等）
        
        // 持续伤害/恢复类
        DamageOverTime = 10,        // 持续伤害（DOT）
        HealOverTime = 11,          // 持续治疗（HOT）
        
        // 周围效果类
        AreaDamage = 20,            // 对周围敌人造成伤害
        AreaHeal = 21,              // 对周围友军恢复
        
        // 计数触发类
        StackCounter = 30,          // 计数器（满层触发效果）
        
        // 控制类
        Stun = 40,                  // 眩晕
        Slow = 41,                  // 减速
        Silence = 42,               // 沉默（无法释放技能）
        
        // 护盾类
        Shield = 50,                // 护盾
        
        // 攻击特效类
        LifeSteal = 60,             // 吸血（攻击时回血）
        
        // 复合效果类
        MultiAttributeModifier = 70, // 多属性修改
        
        // 特殊类
        Custom = 99,                // 自定义效果
    }

    /// <summary>
    /// Buff叠加类型
    /// </summary>
    public enum BuffStackType
    {
        None = 0,           // 不可叠加（同类型只能存在一个）
        Duration = 1,       // 叠加持续时间
        Count = 2,          // 叠加层数
        Independent = 3,    // 独立叠加（可以有多个相同Buff）
    }

    /// <summary>
    /// Buff更新频率
    /// </summary>
    public enum BuffTickRate
    {
        None = 0,           // 不需要Tick
        PerSecond = 1,      // 每秒
        PerHalfSecond = 2,  // 每0.5秒
        PerFrame = 3,       // 每帧
    }
}

