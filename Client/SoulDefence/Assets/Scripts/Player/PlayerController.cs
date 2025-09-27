using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家控制器
/// 专门处理玩家输入并控制玩家实体
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("玩家实体")]
    [SerializeField] private GameEntity playerEntity;       // 玩家实体引用
    
    [Header("控制设置")]
    [SerializeField] private bool enableInput = true;       // 是否启用输入
    
    void Start()
    {
        // 如果没有设置玩家实体，尝试从当前GameObject获取
        if (playerEntity == null)
        {
            playerEntity = GetComponent<GameEntity>();
        }
        
        if (playerEntity == null)
        {
            Debug.LogError("[PlayerController] 未找到玩家实体！请设置playerEntity引用。");
        }
    }
    
    void Update()
    {
        if (enableInput && playerEntity != null)
        {
            HandlePlayerInput();
        }
    }
    
    /// <summary>
    /// 处理玩家输入
    /// </summary>
    private void HandlePlayerInput()
    {
        float horizontal = 0f;
        float vertical = 0f;

        // WASD按键输入
        if (Input.GetKey(KeyCode.W))
            vertical += 1f;
        if (Input.GetKey(KeyCode.S))
            vertical -= 1f;
        if (Input.GetKey(KeyCode.A))
            horizontal -= 1f;
        if (Input.GetKey(KeyCode.D))
            horizontal += 1f;

        // 计算移动方向并传递给玩家实体
        Vector3 moveDirection = new Vector3(horizontal, 0, vertical).normalized;
        playerEntity.Movement.SetMoveDirection(moveDirection);
    }
    
    /// <summary>
    /// 启用/禁用输入
    /// </summary>
    /// <param name="enable">是否启用</param>
    public void SetInputEnabled(bool enable)
    {
        enableInput = enable;
        
        // 如果禁用输入，停止移动
        if (!enable && playerEntity != null)
        {
            playerEntity.Movement.StopMovement();
        }
    }
    
    /// <summary>
    /// 设置玩家实体
    /// </summary>
    /// <param name="entity">玩家实体</param>
    public void SetPlayerEntity(GameEntity entity)
    {
        playerEntity = entity;
    }
    
    /// <summary>
    /// 获取玩家实体
    /// </summary>
    public GameEntity GetPlayerEntity()
    {
        return playerEntity;
    }
    
    /// <summary>
    /// 输入是否启用
    /// </summary>
    public bool IsInputEnabled()
    {
        return enableInput;
    }
} 