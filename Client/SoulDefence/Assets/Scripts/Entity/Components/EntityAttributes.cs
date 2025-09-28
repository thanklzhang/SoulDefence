using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulDefence.Entity
{
    /// <summary>
    /// 实体属性系统
    /// 管理实体的所有属性数据
    /// </summary>
    [System.Serializable]
    public class EntityAttributes
    {
        [Header("属性配置")]
        [SerializeField] private EntityAttributesData attributesData;  // 属性数据配置

        [Header("当前状态属性")]
        [SerializeField] private float currentHealth;           // 当前生命值
        [SerializeField] private int currentLevel = 1;          // 当前等级
        [SerializeField] private float currentExp = 0f;         // 当前经验值

        /// <summary>
        /// 初始化属性
        /// </summary>
        public void Initialize()
        {
            if (attributesData != null)
            {
                currentHealth = attributesData.maxHealth;
                currentLevel = attributesData.baseLevel;
                currentExp = 0f;
            }
            else
            {
                Debug.LogError("EntityAttributes: attributesData is null!");
                currentHealth = 100f; // 默认值
                currentLevel = 1;
                currentExp = 0f;
            }
        }

        /// <summary>
        /// 设置属性数据配置
        /// </summary>
        public void SetAttributesData(EntityAttributesData data)
        {
            attributesData = data;
            Initialize();
        }

        #region 基础属性访问器
        
        public float MaxHealth 
        { 
            get => attributesData != null ? attributesData.maxHealth : 100f; 
            set 
            {
                if (attributesData != null)
                {
                    // 这里我们不直接修改ScriptableObject，而是创建一个新的实例
                    EntityAttributesData newData = ScriptableObject.Instantiate(attributesData);
                    newData.maxHealth = Mathf.Max(0f, value);
                    attributesData = newData;
                }
            }
        }
        
        public float AttackPower 
        { 
            get => attributesData != null ? attributesData.attackPower : 10f; 
            set 
            {
                if (attributesData != null)
                {
                    EntityAttributesData newData = ScriptableObject.Instantiate(attributesData);
                    newData.attackPower = Mathf.Max(0f, value);
                    attributesData = newData;
                }
            }
        }
        
        public float Defense 
        { 
            get => attributesData != null ? attributesData.defense : 5f; 
            set 
            {
                if (attributesData != null)
                {
                    EntityAttributesData newData = ScriptableObject.Instantiate(attributesData);
                    newData.defense = Mathf.Max(0f, value);
                    attributesData = newData;
                }
            }
        }
        
        public float AttackSpeed 
        { 
            get => attributesData != null ? attributesData.attackSpeed : 1f; 
            set 
            {
                if (attributesData != null)
                {
                    EntityAttributesData newData = ScriptableObject.Instantiate(attributesData);
                    newData.attackSpeed = Mathf.Max(0f, value);
                    attributesData = newData;
                }
            }
        }
        
        public float MoveSpeed 
        { 
            get => attributesData != null ? attributesData.moveSpeed : 5f; 
            set 
            {
                if (attributesData != null)
                {
                    EntityAttributesData newData = ScriptableObject.Instantiate(attributesData);
                    newData.moveSpeed = Mathf.Max(0f, value);
                    attributesData = newData;
                }
            }
        }
        
        public float AttackRange 
        { 
            get => attributesData != null ? attributesData.attackRange : 2f; 
            set 
            {
                if (attributesData != null)
                {
                    EntityAttributesData newData = ScriptableObject.Instantiate(attributesData);
                    newData.attackRange = Mathf.Max(0f, value);
                    attributesData = newData;
                }
            }
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
        /// 获取属性数据配置
        /// </summary>
        public EntityAttributesData GetAttributesData()
        {
            return attributesData;
        }

        #endregion
    }
} 