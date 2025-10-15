using UnityEngine;
using SoulDefence.Entity;
using SoulDefence.Core;

namespace SoulDefence.Equipment
{
    /// <summary>
    /// 装备系统测试脚本
    /// 挂载到GameEntity上，用于测试装备系统功能
    /// </summary>
    public class EquipmentSystemTest : MonoBehaviour
    {
        private GameEntity entity;

        void Start()
        {
            entity = GetComponent<GameEntity>();
            if (entity == null)
            {
                Debug.LogError("需要GameEntity组件！");
            }
        }

        [ContextMenu("测试/显示当前属性")]
        void TestShowCurrentAttributes()
        {
            if (entity == null) return;

            Debug.Log("=== 当前属性 ===");
            Debug.Log($"最大生命值: {entity.Attributes.MaxHealth}");
            Debug.Log($"攻击力: {entity.Attributes.AttackPower}");
            Debug.Log($"防御力: {entity.Attributes.Defense}");
            Debug.Log($"攻击速度: {entity.Attributes.AttackSpeed}");
            Debug.Log($"移动速度: {entity.Attributes.MoveSpeed}");
            Debug.Log($"攻击范围: {entity.Attributes.AttackRange}");
            Debug.Log($"暴击率: {entity.Attributes.CriticalRate}%");
            Debug.Log($"暴击伤害: {entity.Attributes.CriticalDamage}%");
            Debug.Log($"伤害减免: {entity.Attributes.DamageReduction}%");
        }

        [ContextMenu("测试/显示装备信息")]
        void TestShowEquipmentInfo()
        {
            if (entity == null || entity.EquipmentSystem == null) return;

            Debug.Log("=== 装备信息 ===");
            
            // 武器信息
            if (entity.EquipmentSystem.Weapon != null)
            {
                Debug.Log($"武器: {entity.EquipmentSystem.Weapon.Data.equipmentName}");
                Debug.Log($"武器品质: {entity.EquipmentSystem.GetWeaponQuality()}");
                
                var weaponAttr = entity.EquipmentSystem.GetWeaponAttributes();
                Debug.Log($"  - 攻击力加成: +{weaponAttr.attackPower}");
                Debug.Log($"  - 攻击速度加成: +{weaponAttr.attackSpeed}");
                Debug.Log($"  - 暴击率加成: +{weaponAttr.criticalRate}%");
            }
            
            // 防具信息
            if (entity.EquipmentSystem.Armor != null)
            {
                Debug.Log($"防具: {entity.EquipmentSystem.Armor.Data.equipmentName}");
                Debug.Log($"防具品质: {entity.EquipmentSystem.GetArmorQuality()}");
                
                var armorAttr = entity.EquipmentSystem.GetArmorAttributes();
                Debug.Log($"  - 生命值加成: +{armorAttr.maxHealth}");
                Debug.Log($"  - 防御力加成: +{armorAttr.defense}");
                Debug.Log($"  - 伤害减免加成: +{armorAttr.damageReduction}%");
            }
        }

        [ContextMenu("测试/升级武器品质")]
        void TestUpgradeWeaponQuality()
        {
            if (entity == null || entity.EquipmentSystem == null) return;

            Debug.Log("=== 升级武器品质 ===");
            Debug.Log($"升级前品质: {entity.EquipmentSystem.GetWeaponQuality()}");
            Debug.Log($"升级前攻击力: {entity.Attributes.AttackPower}");
            
            bool success = entity.EquipmentSystem.UpgradeWeaponQuality();
            
            if (success)
            {
                Debug.Log($"升级后品质: {entity.EquipmentSystem.GetWeaponQuality()}");
                Debug.Log($"升级后攻击力: {entity.Attributes.AttackPower}");
            }
            else
            {
                Debug.Log("升级失败（可能已达到最高品质）");
            }
        }

        [ContextMenu("测试/升级防具品质")]
        void TestUpgradeArmorQuality()
        {
            if (entity == null || entity.EquipmentSystem == null) return;

            Debug.Log("=== 升级防具品质 ===");
            Debug.Log($"升级前品质: {entity.EquipmentSystem.GetArmorQuality()}");
            Debug.Log($"升级前最大生命值: {entity.Attributes.MaxHealth}");
            
            bool success = entity.EquipmentSystem.UpgradeArmorQuality();
            
            if (success)
            {
                Debug.Log($"升级后品质: {entity.EquipmentSystem.GetArmorQuality()}");
                Debug.Log($"升级后最大生命值: {entity.Attributes.MaxHealth}");
            }
            else
            {
                Debug.Log("升级失败（可能已达到最高品质）");
            }
        }

        [ContextMenu("测试/升级武器技能树(疯刀系)")]
        void TestUpgradeWeaponSkillTree()
        {
            if (entity == null || entity.EquipmentSystem == null) return;

            Debug.Log("=== 升级武器技能树 ===");
            int beforeLevel = entity.EquipmentSystem.Weapon.GetSkillTreeLevel(SkillTreeType.CrazyBlade);
            Debug.Log($"升级前等级: {beforeLevel}");
            
            bool success = entity.EquipmentSystem.UpgradeWeaponSkillTree(SkillTreeType.CrazyBlade);
            
            if (success)
            {
                int afterLevel = entity.EquipmentSystem.Weapon.GetSkillTreeLevel(SkillTreeType.CrazyBlade);
                Debug.Log($"升级后等级: {afterLevel}");
            }
            else
            {
                Debug.Log("升级失败（可能受品质限制或已达最大等级）");
            }
        }

        [ContextMenu("测试/完整测试流程")]
        void TestFullWorkflow()
        {
            if (entity == null || entity.EquipmentSystem == null) return;

            Debug.Log("========== 装备系统完整测试 ==========");
            
            TestShowCurrentAttributes();
            Debug.Log("");
            
            TestShowEquipmentInfo();
            Debug.Log("");
            
            TestUpgradeWeaponQuality();
            Debug.Log("");
            
            TestUpgradeWeaponSkillTree();
            Debug.Log("");
            
            TestShowCurrentAttributes();
            
            Debug.Log("========== 测试完成 ==========");
        }

        [ContextMenu("测试/属性组合验证")]
        void TestAttributeComposition()
        {
            if (entity == null) return;

            Debug.Log("=== 属性组合验证 ===");
            
            // 基础属性
            var baseAttr = entity.Attributes.GetBaseAttributesData();
            if (baseAttr != null)
            {
                Debug.Log($"基础攻击力: {baseAttr.attackPower}");
                Debug.Log($"基础生命值: {baseAttr.maxHealth}");
            }
            
            // 装备属性
            var equipAttr = entity.EquipmentSystem?.GetTotalEquipmentAttributes();
            if (equipAttr != null)
            {
                Debug.Log($"装备攻击力加成: {equipAttr.attackPower}");
                Debug.Log($"装备生命值加成: {equipAttr.maxHealth}");
            }
            
            // 总属性
            Debug.Log($"总攻击力: {entity.Attributes.AttackPower}");
            Debug.Log($"总生命值: {entity.Attributes.MaxHealth}");
            
            // 验证公式
            if (baseAttr != null && equipAttr != null)
            {
                float expectedAttack = baseAttr.attackPower + equipAttr.attackPower;
                float expectedHealth = baseAttr.maxHealth + equipAttr.maxHealth;
                
                Debug.Log($"\n验证: 基础{baseAttr.attackPower} + 装备{equipAttr.attackPower} = {expectedAttack}");
                Debug.Log($"实际总攻击力: {entity.Attributes.AttackPower}");
                Debug.Log($"验证结果: {(Mathf.Approximately(expectedAttack, entity.Attributes.AttackPower) ? "通过✓" : "失败✗")}");
            }
        }
    }
}

