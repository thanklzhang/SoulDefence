using UnityEngine;
using SoulDefence.Entity;

namespace SoulDefence.Item
{
    /// <summary>
    /// 血瓶系统
    /// 管理实体的血瓶使用和冷却
    /// </summary>
    [System.Serializable]
    public class HealthPotionSystem
    {
        [Header("血瓶配置")]
        [SerializeField] private HealthPotionData initialPotion;  // 初始血瓶配置

        // 血瓶实例（内部管理，不在Inspector中显示）
        private HealthPotionInstance potionInstance;
        private GameEntity owner;                                  // 所有者

        /// <summary>
        /// 初始化血瓶系统
        /// </summary>
        public void Initialize(GameEntity entity)
        {
            owner = entity;

            // 初始化血瓶实例
            if (initialPotion != null)
            {
                potionInstance = new HealthPotionInstance(initialPotion);
                Debug.Log($"血瓶系统已初始化: {initialPotion.potionName}");
            }
            else
            {
                Debug.Log($"实体 {entity.name} 没有配置血瓶");
            }
        }

        /// <summary>
        /// 更新血瓶系统（冷却计时）
        /// </summary>
        public void Update(float deltaTime)
        {
            if (potionInstance != null)
            {
                potionInstance.UpdateCooldown(deltaTime);
            }
        }

        /// <summary>
        /// 使用血瓶
        /// </summary>
        public bool UsePotion()
        {
            // 检查是否有血瓶
            if (potionInstance == null || potionInstance.Data == null)
            {
                Debug.LogWarning("没有可用的血瓶");
                return false;
            }

            // 检查所有者
            if (owner == null || !owner.IsAlive)
            {
                Debug.LogWarning("所有者不存在或已死亡");
                return false;
            }

            // 检查是否可以使用
            if (!CanUse())
            {
                return false;
            }

            // 计算恢复量
            float healAmount = owner.Attributes.MaxHealth * (potionInstance.Data.healPercentage / 100f);

            // 应用恢复
            float actualHeal = owner.Heal(healAmount);

            // 开始冷却
            potionInstance.StartCooldown();

            Debug.Log($"使用血瓶: 恢复 {actualHeal} 生命值 ({potionInstance.Data.healPercentage}%)");

            return true;
        }

        /// <summary>
        /// 检查是否可以使用血瓶
        /// </summary>
        public bool CanUse()
        {
            // 没有血瓶
            if (potionInstance == null || potionInstance.Data == null)
            {
                return false;
            }

            // 所有者不存在或已死亡
            if (owner == null || !owner.IsAlive)
            {
                return false;
            }

            // 在冷却中
            if (potionInstance.IsOnCooldown)
            {
                return false;
            }

            // 生命值已满且不允许满血使用
            if (!potionInstance.Data.allowUseWhenFull)
            {
                if (owner.Attributes.CurrentHealth >= owner.Attributes.MaxHealth)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 获取冷却剩余时间
        /// </summary>
        public float GetCooldownRemaining()
        {
            return potionInstance?.CooldownRemaining ?? 0f;
        }

        /// <summary>
        /// 获取冷却百分比（0-1）
        /// </summary>
        public float GetCooldownPercent()
        {
            return potionInstance?.CooldownPercent ?? 0f;
        }

        /// <summary>
        /// 是否在冷却中
        /// </summary>
        public bool IsOnCooldown()
        {
            return potionInstance?.IsOnCooldown ?? false;
        }

        /// <summary>
        /// 是否有血瓶
        /// </summary>
        public bool HasPotion()
        {
            return potionInstance != null && potionInstance.Data != null;
        }

        /// <summary>
        /// 获取血瓶实例
        /// </summary>
        public HealthPotionInstance PotionInstance => potionInstance;
    }
}

