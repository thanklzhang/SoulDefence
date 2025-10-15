using UnityEngine;
using System.Collections.Generic;
using SoulDefence.Entity;
using SoulDefence.Core;

namespace SoulDefence.Equipment
{
    /// <summary>
    /// 装备配置数据
    /// 使用ScriptableObject存储装备基础配置
    /// </summary>
    [CreateAssetMenu(fileName = "New Equipment", menuName = "GameConfig/Equipment/Equipment Data")]
    public class EquipmentData : ScriptableObject
    {
        [Header("基础信息")]
        public int equipmentId;                     // 装备ID
        public string equipmentName;                // 装备名称
        public EquipmentType equipmentType;         // 装备类型（武器/防具）
        public ProfessionType professionRestriction;// 职业限制
        public WeaponType weaponType;               // 武器类型（仅武器有效）

        [Header("初始品质")]
        public Quality initialQuality = Quality.Green;

        [Header("品质属性加成配置")]
        [Tooltip("每个品质对应一个属性加成配置")]
        public EntityAttributesData[] qualityBonuses = new EntityAttributesData[5];

        [Header("技能树配置")]
        public SkillTreeData[] skillTrees = new SkillTreeData[3];

        /// <summary>
        /// 获取指定品质的属性加成
        /// </summary>
        public EntityAttributesData GetBonusForQuality(Quality quality)
        {
            int index = (int)quality;
            if (index >= 0 && index < qualityBonuses.Length && qualityBonuses[index] != null)
            {
                return qualityBonuses[index];
            }
            // 返回空属性或第一个有效的属性
            return qualityBonuses[0];
        }
    }

    /// <summary>
    /// 技能树配置
    /// </summary>
    [System.Serializable]
    public class SkillTreeData
    {
        public SkillTreeType treeType;      // 技能树类型
        public string treeName;             // 技能树名称
        public string description;          // 技能树描述

        [Header("技能树等级配置")]
        [Tooltip("每个等级对应一个品质，最多5级")]
        public SkillTreeLevel[] levels = new SkillTreeLevel[5];
    }

    /// <summary>
    /// 技能树等级配置
    /// </summary>
    [System.Serializable]
    public class SkillTreeLevel
    {
        public int level;                   // 等级（1-5）
        public Quality requiredQuality;     // 需要的装备品质
        public string effectDescription;    // 效果描述

        [Header("等级效果（数值可配置）")]
        public float effectValue;           // 效果数值
        public float effectMultiplier;      // 效果倍率
    }
}

