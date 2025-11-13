using System.Collections;
using System.Collections.Generic;
using SoulDefence.Entity;
using UnityEngine;

/// <summary>
/// 玩家控制器
/// 处理玩家输入并控制玩家实体
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("玩家实体")]
    [SerializeField] private GameEntity playerEntity;       // 玩家实体引用
    
    [Header("控制设置")]
    [SerializeField] private bool enableInput = true;       // 是否启用输入
    [SerializeField] private LayerMask groundLayer = -1;    // 地面层（用于鼠标射线检测）
    
    private Camera mainCamera;                              // 主摄像机
    
    void Start()
    {
        // 获取主摄像机
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("[PlayerController] 未找到主摄像机！");
        }
        
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
        // 处理移动输入
        HandleMovementInput();
        
        // 处理鼠标朝向
        HandleMouseRotation();
        
        // 处理战斗输入
        HandleCombatInput();
    }
    
    /// <summary>
    /// 处理移动输入（WASD）
    /// </summary>
    private void HandleMovementInput()
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
    /// 处理鼠标朝向控制
    /// </summary>
    private void HandleMouseRotation()
    {
        if (mainCamera == null) return;
        
        // 从鼠标位置发射射线
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        // 检测射线是否击中地面
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
        {
            // 计算从玩家到鼠标点击位置的方向
            Vector3 directionToMouse = hit.point - playerEntity.transform.position;
            directionToMouse.y = 0; // 保持在水平面
            
            // 如果方向有效，设置玩家朝向
            if (directionToMouse.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToMouse);
                playerEntity.transform.rotation = targetRotation;
            }
        }
    }
    
    /// <summary>
    /// 处理战斗输入（攻击和技能）
    /// </summary>
    private void HandleCombatInput()
    {
        // 鼠标左键 - 普通攻击
        if (Input.GetMouseButtonDown(0))
        {
            playerEntity.UseDefaultSkill();
        }
        
        // F键 - 技能小招（技能1）
        if (Input.GetKeyDown(KeyCode.F))
        {
            playerEntity.UseSkill(0);
        }
        
        // V键 - 技能大招（技能2）
        if (Input.GetKeyDown(KeyCode.V))
        {
            playerEntity.UseSkill(1);
        }
        
        // Space键 - 滑行技能（技能3）
        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerEntity.UseSkill(2);
        }
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