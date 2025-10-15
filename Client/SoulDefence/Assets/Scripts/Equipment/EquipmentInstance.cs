using UnityEngine;
using System.Collections.Generic;
using SoulDefence.Entity;
using SoulDefence.Core;

namespace SoulDefence.Equipment
{
    /// <summary>
    /// 装备实例
    /// 表示一个实际的装备对象，包含当前品质和技能树点数
    /// </summary>
    [System.Serializable]
    public class EquipmentInstance
    {
        [SerializeField] private EquipmentData equipmentData;    // 装备配置数据
        [SerializeField] private Quality currentQuality;         // 当前品质
        [SerializeField] private Dictionary<SkillTreeType, int> skillTreeLevels; // 技能树等级

        /// <summary>
        /// 构造函数
        /// </summary>
        public EquipmentInstance(EquipmentData data)
        {
            equipmentData = data;
            currentQuality = data.initialQuality;
            skillTreeLevels = new Dictionary<SkillTreeType, int>();

            // 初始化技能树等级
            foreach (var tree in data.skillTrees)
            {
                if (tree != null)
                {
                    skillTreeLevels[tree.treeType] = 0;
                }
            }
        }

        /// <summary>
        /// 获取装备配置数据
        /// </summary>
        public EquipmentData Data => equipmentData;

        /// <summary>
        /// 获取当前品质
        /// </summary>
        public Quality CurrentQuality => currentQuality;

        /// <summary>
        /// 获取装备类型
        /// </summary>
        public EquipmentType EquipmentType => equipmentData.equipmentType;

        /// <summary>
        /// 升级品质
        /// </summary>
        public bool UpgradeQuality()
        {
            if (currentQuality < Quality.Red)
            {
                currentQuality++;
                Debug.Log($"装备 {equipmentData.equipmentName} 品质提升至 {currentQuality}");
                return true;
            }
            Debug.LogWarning($"装备 {equipmentData.equipmentName} 已达到最高品质");
            return false;
        }

        /// <summary>
        /// 升级技能树
        /// </summary>
        public bool UpgradeSkillTree(SkillTreeType treeType)
        {
            if (!skillTreeLevels.ContainsKey(treeType))
            {
                Debug.LogError($"技能树类型 {treeType} 不存在");
                return false;
            }

            int currentLevel = skillTreeLevels[treeType];
            int maxLevel = (int)currentQuality + 1; // 品质决定最大等级

            if (currentLevel < maxLevel && currentLevel < 5)
            {
                skillTreeLevels[treeType]++;
                Debug.Log($"技能树 {treeType} 升级至 {skillTreeLevels[treeType]} 级");
                return true;
            }

            Debug.LogWarning($"技能树 {treeType} 已达到当前品质允许的最大等级");
            return false;
        }

        /// <summary>
        /// 获取技能树等级
        /// </summary>
        public int GetSkillTreeLevel(SkillTreeType treeType)
        {
            return skillTreeLevels.ContainsKey(treeType) ? skillTreeLevels[treeType] : 0;
        }

        /// <summary>
        /// 计算当前总属性加成
        /// </summary>
        public EntityAttributesData CalculateTotalAttributes()
        {
            // 获取当前品质的基础属性
            EntityAttributesData baseAttributes = equipmentData.GetBonusForQuality(currentQuality);
            
            if (baseAttributes == null)
            {
                Debug.LogWarning($"装备 {equipmentData.equipmentName} 品质 {currentQuality} 没有配置属性");
                return ScriptableObject.CreateInstance<EntityAttributesData>();
            }

            // 克隆基础属性，避免修改原始配置
            EntityAttributesData totalAttributes = baseAttributes.Clone();

            // TODO: 根据技能树等级添加额外加成（当被动技能系统完善后）

            return totalAttributes;
        }

        /// <summary>
        /// 获取所有技能树信息
        /// </summary>
        public Dictionary<SkillTreeType, int> GetAllSkillTreeLevels()
        {
            return new Dictionary<SkillTreeType, int>(skillTreeLevels);
        }

        /// <summary>
        /// 检查是否可以升级品质
        /// </summary>
        public bool CanUpgradeQuality()
        {
            return currentQuality < Quality.Red;
        }

        /// <summary>
        /// 检查技能树是否可以升级
        /// </summary>
        public bool CanUpgradeSkillTree(SkillTreeType treeType)
        {
            if (!skillTreeLevels.ContainsKey(treeType))
            {
                return false;
            }

            int currentLevel = skillTreeLevels[treeType];
            int maxLevel = (int)currentQuality + 1;
            return currentLevel < maxLevel && currentLevel < 5;
        }
    }
}

