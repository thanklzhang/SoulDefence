using UnityEngine;

namespace SoulDefence.Entity
{
    /// <summary>
    /// 实体属性数据配置
    /// 使用ScriptableObject存储实体属性配置
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

        [Header("经验值设置")]
        public int baseLevel = 1;             // 基础等级
        public float expToNextLevel = 100f;   // 升级所需经验
        public float expGainOnDeath = 50f;    // 死亡给予经验
    }
} 