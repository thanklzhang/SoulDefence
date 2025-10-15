using UnityEngine;
using System.Collections.Generic;
using SoulDefence.Entity;
using SoulDefence.Core;

namespace SoulDefence.Equipment
{
    /// <summary>
    /// 装备系统
    /// 管理实体的武器和防具
    /// </summary>
    [System.Serializable]
    public class EquipmentSystem
    {
        [Header("初始装备配置")]
        [SerializeField] private EquipmentData initialWeapon;  // 初始武器配置
        [SerializeField] private EquipmentData initialArmor;   // 初始防具配置
        
        // 装备实例（内部管理，不在Inspector中显示）
        private EquipmentInstance weapon;      // 武器
        private EquipmentInstance armor;       // 防具

        private GameEntity owner;              // 装备所有者

        /// <summary>
        /// 初始化装备系统
        /// </summary>
        public void Initialize(GameEntity entity)
        {
            owner = entity;

            // 初始化武器
            if (initialWeapon != null)
            {
                weapon = new EquipmentInstance(initialWeapon);
                Debug.Log($"装备武器: {initialWeapon.equipmentName}");
            }
            else
            {
                Debug.LogError("初始武器配置数据为空！");
            }

            // 初始化防具
            if (initialArmor != null)
            {
                armor = new EquipmentInstance(initialArmor);
                Debug.Log($"装备防具: {initialArmor.equipmentName}");
            }
            else
            {
                Debug.LogError("初始防具配置数据为空！");
            }

            // 通知实体重新计算属性
            NotifyAttributesChanged();
        }

        /// <summary>
        /// 获取装备提供的总属性加成
        /// </summary>
        public EntityAttributesData GetTotalEquipmentAttributes()
        {
            EntityAttributesData weaponAttr = weapon?.CalculateTotalAttributes();
            EntityAttributesData armorAttr = armor?.CalculateTotalAttributes();

            // 组合武器和防具属性
            if (weaponAttr != null && armorAttr != null)
            {
                return weaponAttr + armorAttr;
            }
            else if (weaponAttr != null)
            {
                return weaponAttr;
            }
            else if (armorAttr != null)
            {
                return armorAttr;
            }
            else
            {
                return ScriptableObject.CreateInstance<EntityAttributesData>();
            }
        }

        /// <summary>
        /// 通知实体属性已改变，需要重新计算
        /// </summary>
        private void NotifyAttributesChanged()
        {
            if (owner != null)
            {
                owner.Attributes.RecalculateAttributes();
                Debug.Log($"装备属性已更新，实体 {owner.name} 属性已重新计算");
            }
        }

        /// <summary>
        /// 升级武器品质
        /// </summary>
        public bool UpgradeWeaponQuality()
        {
            if (weapon == null)
            {
                Debug.LogError("武器不存在！");
                return false;
            }

            bool success = weapon.UpgradeQuality();
            if (success)
            {
                NotifyAttributesChanged();
            }
            return success;
        }

        /// <summary>
        /// 升级防具品质
        /// </summary>
        public bool UpgradeArmorQuality()
        {
            if (armor == null)
            {
                Debug.LogError("防具不存在！");
                return false;
            }

            bool success = armor.UpgradeQuality();
            if (success)
            {
                NotifyAttributesChanged();
            }
            return success;
        }

        /// <summary>
        /// 升级武器技能树
        /// </summary>
        public bool UpgradeWeaponSkillTree(SkillTreeType treeType)
        {
            if (weapon == null)
            {
                Debug.LogError("武器不存在！");
                return false;
            }

            bool success = weapon.UpgradeSkillTree(treeType);
            if (success)
            {
                NotifyAttributesChanged();
            }
            return success;
        }

        /// <summary>
        /// 升级防具技能树
        /// </summary>
        public bool UpgradeArmorSkillTree(SkillTreeType treeType)
        {
            if (armor == null)
            {
                Debug.LogError("防具不存在！");
                return false;
            }

            bool success = armor.UpgradeSkillTree(treeType);
            if (success)
            {
                NotifyAttributesChanged();
            }
            return success;
        }

        #region 属性访问器

        /// <summary>
        /// 获取武器实例
        /// </summary>
        public EquipmentInstance Weapon => weapon;

        /// <summary>
        /// 获取防具实例
        /// </summary>
        public EquipmentInstance Armor => armor;

        /// <summary>
        /// 获取武器品质
        /// </summary>
        public Quality GetWeaponQuality()
        {
            return weapon?.CurrentQuality ?? Quality.Green;
        }

        /// <summary>
        /// 获取防具品质
        /// </summary>
        public Quality GetArmorQuality()
        {
            return armor?.CurrentQuality ?? Quality.Green;
        }

        /// <summary>
        /// 获取武器属性
        /// </summary>
        public EntityAttributesData GetWeaponAttributes()
        {
            return weapon?.CalculateTotalAttributes() ?? ScriptableObject.CreateInstance<EntityAttributesData>();
        }

        /// <summary>
        /// 获取防具属性
        /// </summary>
        public EntityAttributesData GetArmorAttributes()
        {
            return armor?.CalculateTotalAttributes() ?? ScriptableObject.CreateInstance<EntityAttributesData>();
        }

        #endregion

        #region 奖励接口

        /// <summary>
        /// 处理装备升级奖励
        /// 供奖励系统调用
        /// </summary>
        public void ProcessEquipmentUpgradeReward(EquipmentType type, bool upgradeQuality, SkillTreeType skillTree = SkillTreeType.CrazyBlade)
        {
            if (upgradeQuality)
            {
                // 升级品质
                if (type == EquipmentType.Weapon)
                {
                    UpgradeWeaponQuality();
                }
                else
                {
                    UpgradeArmorQuality();
                }
            }
            else
            {
                // 升级技能树
                if (type == EquipmentType.Weapon)
                {
                    UpgradeWeaponSkillTree(skillTree);
                }
                else
                {
                    UpgradeArmorSkillTree(skillTree);
                }
            }
        }

        #endregion
    }
}

