using UnityEngine;

namespace SoulDefence.Item
{
    /// <summary>
    /// 血瓶配置数据
    /// 使用ScriptableObject存储血瓶配置
    /// </summary>
    [CreateAssetMenu(fileName = "New Health Potion", menuName = "GameConfig/Item/Health Potion")]
    public class HealthPotionData : ScriptableObject
    {
        [Header("基础信息")]
        public int potionId;                    // 血瓶ID
        public string potionName;               // 血瓶名称

        [Header("恢复配置")]
        [Range(0f, 100f)]
        public float healPercentage = 33f;      // 恢复百分比（0-100）

        [Header("冷却配置")]
        public float cooldownTime = 15f;        // 冷却时间（秒）
        public bool startOnCooldown = false;    // 是否初始冷却

        [Header("使用限制")]
        public bool allowUseWhenFull = false;   // 是否允许满血使用
    }
}

