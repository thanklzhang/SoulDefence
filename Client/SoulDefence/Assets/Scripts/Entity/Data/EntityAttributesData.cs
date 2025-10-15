using UnityEngine;

namespace SoulDefence.Entity
{
    /// <summary>
    /// 实体属性数据配置
    /// 通用属性组，可用于实体基础属性、装备属性等
    /// </summary>
    [CreateAssetMenu(fileName = "New Entity Attributes", menuName = "GameConfig/Entity/Attributes")]
    public class EntityAttributesData : ScriptableObject
    {
        [Header("基础属性")]
        public float maxHealth = 100f;        // 最大生命值
        public float attackPower = 10f;       // 攻击力
        public float defense = 5f;            // 防御力
        public float attackSpeed = 1f;        // 攻击速度
        public float moveSpeed = 5f;          // 移动速度
        public float attackRange = 2f;        // 攻击距离
        
        [Header("战斗属性")]
        public float criticalRate = 0f;       // 暴击率（百分比）
        public float criticalDamage = 150f;   // 暴击伤害（百分比）
        public float damageReduction = 0f;    // 伤害减免（百分比）
        
        /// <summary>
        /// 克隆属性数据
        /// </summary>
        public EntityAttributesData Clone()
        {
            EntityAttributesData clone = CreateInstance<EntityAttributesData>();
            clone.maxHealth = this.maxHealth;
            clone.attackPower = this.attackPower;
            clone.defense = this.defense;
            clone.attackSpeed = this.attackSpeed;
            clone.moveSpeed = this.moveSpeed;
            clone.attackRange = this.attackRange;
            clone.criticalRate = this.criticalRate;
            clone.criticalDamage = this.criticalDamage;
            clone.damageReduction = this.damageReduction;
            return clone;
        }
        
        /// <summary>
        /// 添加属性（用于属性叠加）
        /// </summary>
        public static EntityAttributesData operator +(EntityAttributesData a, EntityAttributesData b)
        {
            if (a == null) return b;
            if (b == null) return a;
            
            EntityAttributesData result = CreateInstance<EntityAttributesData>();
            result.maxHealth = a.maxHealth + b.maxHealth;
            result.attackPower = a.attackPower + b.attackPower;
            result.defense = a.defense + b.defense;
            result.attackSpeed = a.attackSpeed + b.attackSpeed;
            result.moveSpeed = a.moveSpeed + b.moveSpeed;
            result.attackRange = a.attackRange + b.attackRange;
            result.criticalRate = a.criticalRate + b.criticalRate;
            result.criticalDamage = a.criticalDamage + b.criticalDamage;
            result.damageReduction = a.damageReduction + b.damageReduction;
            return result;
        }
    }
} 