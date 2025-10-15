using SoulDefence.Core;

namespace SoulDefence.Equipment
{
    /// <summary>
    /// 装备类型
    /// </summary>
    public enum EquipmentType
    {
        Weapon,         // 武器
        Armor           // 防具
    }

    /// <summary>
    /// 武器类型
    /// </summary>
    public enum WeaponType
    {
        Sword,          // 刀
        Bow,            // 弓箭
        Staff           // 法杖
    }

    /// <summary>
    /// 技能树类型（示例，每个武器/防具有3个技能树）
    /// </summary>
    public enum SkillTreeType
    {
        // 刀类武器技能树
        CrazyBlade = 0,     // 疯刀系
        MagicBlade = 1,     // 魔刀系
        MeleeMaster = 2,    // 混战系

        // 防具技能树
        Tenacity = 10,      // 坚韧系
        Reflection = 11,    // 反伤系
        Recovery = 12       // 恢复系
    }
}

