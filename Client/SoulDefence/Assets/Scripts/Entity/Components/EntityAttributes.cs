using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 实体属性系统
/// 管理实体的所有属性数据
/// </summary>
[System.Serializable]
public class EntityAttributes
{
    [Header("基础属性")]
    [SerializeField] private float maxHealth = 100f;        // 最大生命值
    [SerializeField] private float attackPower = 10f;       // 攻击力
    [SerializeField] private float defense = 5f;            // 防御力
    [SerializeField] private float attackSpeed = 1f;        // 攻击速度
    [SerializeField] private float moveSpeed = 5f;          // 移动速度

    [Header("当前状态属性")]
    [SerializeField] private float currentHealth;           // 当前生命值
    [SerializeField] private int currentLevel = 1;          // 当前等级
    [SerializeField] private float currentExp = 0f;         // 当前经验值

    /// <summary>
    /// 初始化属性
    /// </summary>
    public void Initialize()
    {
        currentHealth = maxHealth;
    }

    #region 基础属性访问器
    
    public float MaxHealth 
    { 
        get => maxHealth; 
        set => maxHealth = Mathf.Max(0f, value); 
    }
    
    public float AttackPower 
    { 
        get => attackPower; 
        set => attackPower = Mathf.Max(0f, value); 
    }
    
    public float Defense 
    { 
        get => defense; 
        set => defense = Mathf.Max(0f, value); 
    }
    
    public float AttackSpeed 
    { 
        get => attackSpeed; 
        set => attackSpeed = Mathf.Max(0f, value); 
    }
    
    public float MoveSpeed 
    { 
        get => moveSpeed; 
        set => moveSpeed = Mathf.Max(0f, value); 
    }

    #endregion

    #region 当前状态属性访问器
    
    public float CurrentHealth 
    { 
        get => currentHealth; 
        set => currentHealth = Mathf.Clamp(value, 0f, maxHealth); 
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
    public float HealthPercentage => maxHealth > 0f ? currentHealth / maxHealth : 0f;

    /// <summary>
    /// 造成伤害
    /// </summary>
    /// <param name="damage">伤害值</param>
    /// <returns>实际造成的伤害</returns>
    public float TakeDamage(float damage)
    {
        float actualDamage = Mathf.Max(0f, damage - defense);
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

    #endregion
} 