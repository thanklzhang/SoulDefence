using UnityEngine;
using UnityEngine.UI;
using SoulDefence.Entity;

namespace SoulDefence.UI
{
    /// <summary>
    /// 血条UI组件
    /// 显示单个实体的生命值，跟随实体移动
    /// </summary>
    public class HealthBarUI : MonoBehaviour
    {
        [Header("UI组件")]
        [SerializeField] private Slider healthSlider;           // 生命值进度条
        [SerializeField] private Image healthFill;              // 生命值填充图片
        [SerializeField] private Text healthText;               // 生命值文本
        [SerializeField] private CanvasGroup canvasGroup;       // 用于控制透明度
        
        [Header("跟随设置")]
        [SerializeField] private Vector3 worldOffset = new Vector3(0, 2f, 0);  // 世界坐标偏移
        [SerializeField] private bool lookAtCamera = true;                      // 是否面向摄像机
        [SerializeField] private bool hideWhenFull = false;                     // 满血时是否隐藏
        [SerializeField] private bool hideWhenDead = true;                      // 死亡时是否隐藏
        
        [Header("显示设置")]
        [SerializeField] private bool showHealthText = true;                    // 是否显示生命值文本
        [SerializeField] private bool showPercentage = false;                   // 是否显示百分比
        [SerializeField] private float fadeDistance = 20f;                     // 淡出距离
        
        [Header("颜色配置")]
        [SerializeField] private Color healthHighColor = Color.green;           // 生命值高时的颜色
        [SerializeField] private Color healthMidColor = Color.yellow;           // 生命值中等时的颜色
        [SerializeField] private Color healthLowColor = Color.red;              // 生命值低时的颜色
        
        // 目标实体
        private GameEntity targetEntity;
        
        // 主摄像机引用
        private Camera mainCamera;
        
        // UI更新状态
        private bool isActive = true;
        
        private void Awake()
        {
            // 获取组件引用
            if (healthSlider == null)
                healthSlider = GetComponentInChildren<Slider>();
            
            if (healthFill == null && healthSlider != null)
                healthFill = healthSlider.fillRect?.GetComponent<Image>();
            
            if (healthText == null)
                healthText = GetComponentInChildren<Text>();
            
            if (canvasGroup == null)
                canvasGroup = GetComponent<CanvasGroup>();
            
            // 获取主摄像机
            mainCamera = Camera.main;
            if (mainCamera == null)
                mainCamera = FindObjectOfType<Camera>();
        }
        
        private void Start()
        {
            // 初始化UI状态
            UpdateHealthBar();
        }
        
        private void Update()
        {
            if (isActive && targetEntity != null)
            {
                // 更新位置
                UpdatePosition();
                
                // 更新血条显示
                UpdateHealthBar();
                
                // 更新透明度
                UpdateFade();
            }
        }
        
        /// <summary>
        /// 设置目标实体
        /// </summary>
        /// <param name="entity">目标实体</param>
        public void SetTargetEntity(GameEntity entity)
        {
            targetEntity = entity;
            
            if (targetEntity != null)
            {
                // 立即更新一次
                UpdatePosition();
                UpdateHealthBar();
                isActive = true;
                gameObject.SetActive(true);
            }
            else
            {
                isActive = false;
                gameObject.SetActive(false);
            }
        }
        
        /// <summary>
        /// 更新位置
        /// </summary>
        private void UpdatePosition()
        {
            if (targetEntity != null && mainCamera != null)
            {
                // 将世界位置转换为屏幕位置
                Vector3 worldPosition = targetEntity.transform.position + worldOffset;
                Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);
                
                // 检查是否在摄像机前方
                if (screenPosition.z > 0)
                {
                    // 设置UI位置（屏幕空间）
                    RectTransform rectTransform = transform as RectTransform;
                    if (rectTransform != null)
                    {
                        rectTransform.position = screenPosition;
                    }
                    
                    gameObject.SetActive(true);
                }
                else
                {
                    // 在摄像机后方，隐藏血条
                    gameObject.SetActive(false);
                }
            }
        }
        
        /// <summary>
        /// 更新血条显示
        /// </summary>
        private void UpdateHealthBar()
        {
            if (targetEntity == null || !targetEntity.IsAlive)
            {
                // 实体死亡或不存在
                if (hideWhenDead)
                {
                    gameObject.SetActive(false);
                    return;
                }
            }
            
            if (targetEntity == null) return;
            
            var attributes = targetEntity.Attributes;
            float currentHealth = attributes.CurrentHealth;
            float maxHealth = attributes.MaxHealth;
            float healthPercentage = attributes.HealthPercentage;
            
            // 检查是否满血且需要隐藏
            if (hideWhenFull && healthPercentage >= 1f)
            {
                gameObject.SetActive(false);
                return;
            }
            
            gameObject.SetActive(true);
            
            // 更新进度条
            if (healthSlider != null)
            {
                healthSlider.value = healthPercentage;
            }
            
            // 更新颜色
            if (healthFill != null)
            {
                Color healthColor = GetHealthColor(healthPercentage);
                healthFill.color = healthColor;
            }
            
            // 更新文本
            if (healthText != null && showHealthText)
            {
                if (showPercentage)
                {
                    healthText.text = $"{(healthPercentage * 100):F0}%";
                }
                else
                {
                    healthText.text = $"{currentHealth:F0}/{maxHealth:F0}";
                }
            }
        }
        
        /// <summary>
        /// 更新淡出效果
        /// </summary>
        private void UpdateFade()
        {
            if (canvasGroup == null || mainCamera == null || targetEntity == null)
                return;
            
            // 计算距离
            float distance = Vector3.Distance(mainCamera.transform.position, targetEntity.transform.position);
            
            // 计算透明度
            float alpha = 1f;
            if (distance > fadeDistance)
            {
                alpha = Mathf.Clamp01(1f - (distance - fadeDistance) / fadeDistance);
            }
            
            canvasGroup.alpha = alpha;
        }
        
        /// <summary>
        /// 根据生命值百分比获取颜色
        /// </summary>
        /// <param name="healthPercentage">生命值百分比</param>
        /// <returns>对应颜色</returns>
        private Color GetHealthColor(float healthPercentage)
        {
            if (healthPercentage > 0.6f)
            {
                return Color.Lerp(healthMidColor, healthHighColor, (healthPercentage - 0.6f) / 0.4f);
            }
            else if (healthPercentage > 0.3f)
            {
                return Color.Lerp(healthLowColor, healthMidColor, (healthPercentage - 0.3f) / 0.3f);
            }
            else
            {
                return healthLowColor;
            }
        }
        
        /// <summary>
        /// 获取目标实体
        /// </summary>
        /// <returns>目标实体</returns>
        public GameEntity GetTargetEntity()
        {
            return targetEntity;
        }
        
        /// <summary>
        /// 设置是否激活
        /// </summary>
        /// <param name="active">是否激活</param>
        public void SetActive(bool active)
        {
            isActive = active;
            gameObject.SetActive(active);
        }
        
        /// <summary>
        /// 检查血条是否激活
        /// </summary>
        /// <returns>是否激活</returns>
        public bool IsActive()
        {
            return isActive && gameObject.activeInHierarchy;
        }
    }
} 