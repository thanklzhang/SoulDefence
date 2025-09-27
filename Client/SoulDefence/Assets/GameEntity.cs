using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏实体主类
/// 整合属性系统和移动系统
/// </summary>
public class GameEntity : MonoBehaviour
{
    [Header("实体系统")]
    [SerializeField] private EntityAttributes attributes = new EntityAttributes();
    [SerializeField] private EntityMovement movement = new EntityMovement();

    // Start is called before the first frame update
    void Start()
    {
        InitializeEntity();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateEntity();
    }

    /// <summary>
    /// 初始化实体
    /// </summary>
    private void InitializeEntity()
    {
        // 初始化属性系统
        attributes.Initialize();
        
        // 初始化移动系统
        movement.Initialize(transform, attributes);
    }

    /// <summary>
    /// 更新实体
    /// </summary>
    private void UpdateEntity()
    {
        // 处理移动输入
        movement.HandleMovementInput();
        
        // 执行移动
        movement.Move();
    }

    #region 公共接口

    /// <summary>
    /// 获取实体属性
    /// </summary>
    public EntityAttributes Attributes => attributes;

    /// <summary>
    /// 获取移动系统
    /// </summary>
    public EntityMovement Movement => movement;

    /// <summary>
    /// 设置移动速度（快捷方法）
    /// </summary>
    /// <param name="speed">移动速度</param>
    public void SetMoveSpeed(float speed)
    {
        attributes.MoveSpeed = speed;
    }

    /// <summary>
    /// 设置转向速度（快捷方法）
    /// </summary>
    /// <param name="speed">转向速度</param>
    public void SetRotationSpeed(float speed)
    {
        movement.RotationSpeed = speed;
    }

    /// <summary>
    /// 造成伤害（快捷方法）
    /// </summary>
    /// <param name="damage">伤害值</param>
    /// <returns>实际造成的伤害</returns>
    public float TakeDamage(float damage)
    {
        return attributes.TakeDamage(damage);
    }

    /// <summary>
    /// 恢复生命值（快捷方法）
    /// </summary>
    /// <param name="healAmount">恢复量</param>
    /// <returns>实际恢复量</returns>
    public float Heal(float healAmount)
    {
        return attributes.Heal(healAmount);
    }

    /// <summary>
    /// 是否存活
    /// </summary>
    public bool IsAlive => attributes.IsAlive;

    /// <summary>
    /// 是否正在移动
    /// </summary>
    public bool IsMoving => movement.IsMoving;

    #endregion
}
