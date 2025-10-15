using UnityEngine;

namespace SoulDefence.Core
{
    /// <summary>
    /// 游戏通用枚举定义
    /// </summary>
    
    /// <summary>
    /// 品质枚举（通用）
    /// 适用于装备、道具、技能等所有有品质概念的内容
    /// </summary>
    public enum Quality
    {
        Green = 0,      // 绿色
        Blue = 1,       // 蓝色
        Purple = 2,     // 紫色
        Orange = 3,     // 橙色
        Red = 4         // 红色
    }

    /// <summary>
    /// 职业类型（通用）
    /// </summary>
    public enum ProfessionType
    {
        None = 0,       // 无限制
        Warrior = 1,    // 战士
        Archer = 2,     // 弓手
        Wizard = 3      // 巫师
    }

    /// <summary>
    /// 实体属性类型（通用）
    /// 用于属性加成、Buff、被动技能等
    /// </summary>
    public enum AttributeType
    {
        AttackPower = 0,        // 攻击力
        Defense = 1,            // 防御力
        AttackSpeed = 2,        // 攻击速度
        MoveSpeed = 3,          // 移动速度
        MaxHealth = 4,          // 最大生命值
        AttackRange = 5,        // 攻击范围
        CriticalRate = 6,       // 暴击率
        CriticalDamage = 7,     // 暴击伤害
        DamageReduction = 8,    // 伤害减免
    }
}

