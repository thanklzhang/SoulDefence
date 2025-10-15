using UnityEngine;

namespace SoulDefence.Item
{
    /// <summary>
    /// 血瓶实例
    /// 管理运行时的冷却状态
    /// </summary>
    [System.Serializable]
    public class HealthPotionInstance
    {
        private HealthPotionData potionData;    // 血瓶配置数据
        private float cooldownRemaining;        // 剩余冷却时间

        /// <summary>
        /// 构造函数
        /// </summary>
        public HealthPotionInstance(HealthPotionData data)
        {
            potionData = data;
            cooldownRemaining = data.startOnCooldown ? data.cooldownTime : 0f;
        }

        /// <summary>
        /// 获取血瓶配置数据
        /// </summary>
        public HealthPotionData Data => potionData;

        /// <summary>
        /// 是否在冷却中
        /// </summary>
        public bool IsOnCooldown => cooldownRemaining > 0f;

        /// <summary>
        /// 获取冷却剩余时间
        /// </summary>
        public float CooldownRemaining => cooldownRemaining;

        /// <summary>
        /// 获取冷却百分比（0-1）
        /// </summary>
        public float CooldownPercent
        {
            get
            {
                if (potionData == null || potionData.cooldownTime <= 0f)
                    return 0f;
                return cooldownRemaining / potionData.cooldownTime;
            }
        }

        /// <summary>
        /// 更新冷却
        /// </summary>
        public void UpdateCooldown(float deltaTime)
        {
            if (cooldownRemaining > 0f)
            {
                cooldownRemaining -= deltaTime;
                if (cooldownRemaining < 0f)
                {
                    cooldownRemaining = 0f;
                }
            }
        }

        /// <summary>
        /// 开始冷却
        /// </summary>
        public void StartCooldown()
        {
            if (potionData != null)
            {
                cooldownRemaining = potionData.cooldownTime;
            }
        }

        /// <summary>
        /// 重置冷却
        /// </summary>
        public void ResetCooldown()
        {
            cooldownRemaining = 0f;
        }
    }
}

