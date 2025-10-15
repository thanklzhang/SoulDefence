using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulDefence.Entity
{
    /// <summary>
    /// 实体属性系统
    /// 管理实体的所有属性数据
    /// 属性 = 基础配置属性 + 装备属性 + 其他加成属性
    /// </summary>
    [System.Serializable]
    public class EntityAttributes
    {
        [Header("基础属性配置")]
        [SerializeField] private EntityAttributesData baseAttributesData;  // 基础属性数据配置
        
        [Header("等级配置")]
        [SerializeField] private EntityLevelData levelData;                // 等级数据配置

        [Header("当前状态")]
        [SerializeField] private float currentHealth;           // 当前生命值
        [SerializeField] private int currentLevel = 1;          // 当前等级
        [SerializeField] private float currentExp = 0f;         // 当前经验值

        // 计算后的总属性（缓存）
        private EntityAttributesData cachedTotalAttributes;
        
        // 装备系统引用
        private GameEntity owner;

        /// <summary>
        /// 初始化属性
        /// </summary>
        public void Initialize(GameEntity entity = null)
        {
            owner = entity;
            
            if (levelData != null)
            {
                currentLevel = levelData.baseLevel;
                currentExp = 0f;
            }
            else
            {
                currentLevel = 1;
                currentExp = 0f;
            }
            
            // 重新计算总属性
            RecalculateAttributes();
            
            // 设置当前生命值为最大值
            currentHealth = MaxHealth;
        }

        /// <summary>
        /// 设置基础属性数据配置
        /// </summary>
        public void SetBaseAttributesData(EntityAttributesData data)
        {
            baseAttributesData = data;
            RecalculateAttributes();
        }
        
        /// <summary>
        /// 设置等级数据配置
        /// </summary>
        public void SetLevelData(EntityLevelData data)
        {
            levelData = data;
        }
        
        /// <summary>
        /// 重新计算总属性
        /// 总属性 = 基础属性 + 装备属性 + 其他属性
        /// </summary>
        public void RecalculateAttributes()
        {
            // 从基础属性开始
            if (baseAttributesData != null)
            {
                cachedTotalAttributes = baseAttributesData.Clone();
            }
            else
            {
                cachedTotalAttributes = ScriptableObject.CreateInstance<EntityAttributesData>();
                Debug.LogWarning("EntityAttributes: baseAttributesData is null!");
            }
            
            // 添加装备属性
            if (owner != null && owner.EquipmentSystem != null)
            {
                EntityAttributesData equipmentAttr = owner.EquipmentSystem.GetTotalEquipmentAttributes();
                if (equipmentAttr != null)
                {
                    cachedTotalAttributes = cachedTotalAttributes + equipmentAttr;
                }
            }
            
            // TODO: 添加其他属性加成（Buff、技能等）
            
            Debug.Log($"属性已重新计算 - 最大生命: {cachedTotalAttributes.maxHealth}, 攻击力: {cachedTotalAttributes.attackPower}");
        }
        
        /// <summary>
        /// 获取当前总属性
        /// </summary>
        private EntityAttributesData GetTotalAttributes()
        {
            if (cachedTotalAttributes == null)
            {
                RecalculateAttributes();
            }
            return cachedTotalAttributes;
        }

        #region 基础属性访问器（只读，从总属性中获取）
        
        public float MaxHealth 
        { 
            get => GetTotalAttributes()?.maxHealth ?? 100f;
        }
        
        public float AttackPower 
        { 
            get => GetTotalAttributes()?.attackPower ?? 10f;
        }
        
        public float Defense 
        { 
            get => GetTotalAttributes()?.defense ?? 5f;
        }
        
        public float AttackSpeed 
        { 
            get => GetTotalAttributes()?.attackSpeed ?? 1f;
        }
        
        public float MoveSpeed 
        { 
            get => GetTotalAttributes()?.moveSpeed ?? 5f;
        }
        
        public float AttackRange 
        { 
            get => GetTotalAttributes()?.attackRange ?? 2f;
        }
        
        public float CriticalRate
        {
            get => GetTotalAttributes()?.criticalRate ?? 0f;
        }
        
        public float CriticalDamage
        {
            get => GetTotalAttributes()?.criticalDamage ?? 150f;
        }
        
        public float DamageReduction
        {
            get => GetTotalAttributes()?.damageReduction ?? 0f;
        }

        #endregion

        #region 当前状态属性访问器
        
        public float CurrentHealth 
        { 
            get => currentHealth; 
            set => currentHealth = Mathf.Clamp(value, 0f, MaxHealth); 
        }
        
        public int CurrentLevel 
        { 
            get => currentLevel; 
            set => currentLevel = Mathf.Max(1, value); 
        }
        
        public float CurrentExp 
        { 
            get => currentExp; 
            set => currentExp = Mathf.Max(0f, value); 
        }

        #endregion

        #region 实用方法

        /// <summary>
        /// 是否存活
        /// </summary>
        public bool IsAlive => currentHealth > 0f;

        /// <summary>
        /// 生命值百分比
        /// </summary>
        public float HealthPercentage => MaxHealth > 0f ? currentHealth / MaxHealth : 0f;

        /// <summary>
        /// 造成伤害
        /// </summary>
        /// <param name="damage">伤害值</param>
        /// <returns>实际造成的伤害</returns>
        public float TakeDamage(float damage)
        {
            float actualDamage = Mathf.Max(0f, damage - Defense);
            float oldHealth = currentHealth;
            CurrentHealth -= actualDamage;
            return oldHealth - currentHealth;
        }

        /// <summary>
        /// 恢复生命值
        /// </summary>
        /// <param name="healAmount">恢复量</param>
        /// <returns>实际恢复量</returns>
        public float Heal(float healAmount)
        {
            float oldHealth = currentHealth;
            CurrentHealth += healAmount;
            return currentHealth - oldHealth;
        }

        /// <summary>
        /// 获取基础属性数据配置
        /// </summary>
        public EntityAttributesData GetBaseAttributesData()
        {
            return baseAttributesData;
        }
        
        /// <summary>
        /// 获取计算后的总属性
        /// </summary>
        public EntityAttributesData GetTotalAttributesData()
        {
            return GetTotalAttributes();
        }
        
        /// <summary>
        /// 获取等级数据配置
        /// </summary>
        public EntityLevelData GetLevelData()
        {
            return levelData;
        }

        #endregion
    }
} 