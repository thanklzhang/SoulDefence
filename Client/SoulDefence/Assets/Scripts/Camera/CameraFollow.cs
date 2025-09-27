using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 摄像机跟随系统
/// 实现摄像机对目标的位置跟随，并始终看向目标
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Header("跟随目标")]
    [SerializeField] private Transform target;                  // 跟随目标

    [Header("位置跟随设置")]
    [SerializeField] private Vector3 offset = new Vector3(0, 5, -10);  // 相对目标的偏移
    [SerializeField] private float followSpeed = 5f;           // 位置跟随速度
    [SerializeField] private bool smoothFollow = true;         // 是否平滑跟随

    [Header("视角设置")]
    [SerializeField] private float rotationSpeed = 3f;         // 看向目标的旋转速度

    [Header("边界限制")]
    [SerializeField] private bool useBounds = false;           // 是否使用边界限制
    [SerializeField] private Vector3 minBounds = Vector3.zero; // 最小边界
    [SerializeField] private Vector3 maxBounds = Vector3.zero; // 最大边界

    [Header("高度调整")]
    [SerializeField] private bool maintainHeight = false;      // 是否保持固定高度
    [SerializeField] private float fixedHeight = 5f;           // 固定高度值

    // 内部变量
    private Vector3 targetPosition;
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        InitializeCamera();
    }

    void LateUpdate()
    {
        if (target == null)
            return;

        UpdateCameraPosition();
        UpdateCameraRotation();
    }

    /// <summary>
    /// 初始化摄像机
    /// </summary>
    private void InitializeCamera()
    {
        // 如果没有设置目标，尝试找到GameEntity
        if (target == null)
        {
            GameEntity gameEntity = FindObjectOfType<GameEntity>();
            if (gameEntity != null)
            {
                target = gameEntity.transform;
            }
        }

        // 如果仍然没有目标，输出警告
        if (target == null)
        {
            Debug.LogWarning("CameraFollow: 没有找到跟随目标！请在Inspector中设置target或确保场景中有GameEntity。");
        }
    }

    /// <summary>
    /// 更新摄像机位置
    /// </summary>
    private void UpdateCameraPosition()
    {
        // 计算目标位置（使用固定偏移，不考虑目标旋转）
        targetPosition = target.position + offset;

        // 高度调整
        if (maintainHeight)
        {
            targetPosition.y = fixedHeight;
        }

        // 边界限制
        if (useBounds)
        {
            targetPosition.x = Mathf.Clamp(targetPosition.x, minBounds.x, maxBounds.x);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minBounds.y, maxBounds.y);
            targetPosition.z = Mathf.Clamp(targetPosition.z, minBounds.z, maxBounds.z);
        }

        // 移动摄像机
        if (smoothFollow)
        {
            // 平滑跟随
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, 1f / followSpeed);
        }
        else
        {
            // 直接跟随
            transform.position = targetPosition;
        }
    }

    /// <summary>
    /// 更新摄像机旋转（始终看向目标）
    /// </summary>
    private void UpdateCameraRotation()
    {
        // 始终看向目标
        Vector3 direction = target.position - transform.position;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    #region 公共接口

    /// <summary>
    /// 设置跟随目标
    /// </summary>
    /// <param name="newTarget">新的跟随目标</param>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    /// <summary>
    /// 设置偏移
    /// </summary>
    /// <param name="newOffset">新的偏移值</param>
    public void SetOffset(Vector3 newOffset)
    {
        offset = newOffset;
    }

    /// <summary>
    /// 设置跟随速度
    /// </summary>
    /// <param name="speed">跟随速度</param>
    public void SetFollowSpeed(float speed)
    {
        followSpeed = Mathf.Max(0.1f, speed);
    }

    /// <summary>
    /// 设置旋转速度
    /// </summary>
    /// <param name="speed">旋转速度</param>
    public void SetRotationSpeed(float speed)
    {
        rotationSpeed = Mathf.Max(0.1f, speed);
    }

    /// <summary>
    /// 立即移动到目标位置
    /// </summary>
    public void SnapToTarget()
    {
        if (target == null) return;

        UpdateCameraPosition();
        transform.position = targetPosition;
        
        // 立即看向目标
        Vector3 direction = target.position - transform.position;
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    /// <summary>
    /// 切换跟随模式
    /// </summary>
    /// <param name="smooth">是否平滑跟随</param>
    public void SetSmoothFollow(bool smooth)
    {
        smoothFollow = smooth;
    }

    #endregion

    #region 调试工具

    void OnDrawGizmosSelected()
    {
        if (target == null) return;

        // 绘制目标位置
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(target.position, 0.5f);

        // 绘制摄像机目标位置
        Gizmos.color = Color.blue;
        Vector3 cameraTargetPos = target.position + offset;
        Gizmos.DrawWireSphere(cameraTargetPos, 0.3f);

        // 绘制连接线
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(target.position, cameraTargetPos);

        // 绘制视线方向
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, target.position);

        // 绘制边界
        if (useBounds)
        {
            Gizmos.color = Color.green;
            Vector3 center = (minBounds + maxBounds) * 0.5f;
            Vector3 size = maxBounds - minBounds;
            Gizmos.DrawWireCube(center, size);
        }
    }

    #endregion
} 