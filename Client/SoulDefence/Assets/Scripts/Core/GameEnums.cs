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
}

