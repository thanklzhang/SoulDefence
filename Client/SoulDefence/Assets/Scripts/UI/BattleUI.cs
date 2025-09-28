using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SoulDefence.Entity;

namespace SoulDefence.UI
{
    /// <summary>
    /// 战斗界面
    /// 显示玩家实体的生命值、进度条和各项属性
    /// </summary>
    public class BattleUI : UIBasePanel
    {
        [Header("玩家状态显示")]
        [SerializeField] private TextMeshProUGUI playerHealthText;          // 玩家生命值文本
        [SerializeField] private Slider playerHealthSlider;                 // 玩家生命值进度条
        [SerializeField] private Image playerHealthFill;                    // 生命值填充图片
        
        [Header("属性显示")]
        [SerializeField] private TextMeshProUGUI attackPowerText;           // 攻击力文本
        [SerializeField] private TextMeshProUGUI defenseText;               // 防御力文本
        [SerializeField] private TextMeshProUGUI attackSpeedText;           // 攻击速度文本
        [SerializeField] private TextMeshProUGUI moveSpeedText;             // 移动速度文本
        [SerializeField] private TextMeshProUGUI attackRangeText;           // 攻击距离文本
        [SerializeField] private TextMeshProUGUI levelText;                 // 等级文本
        [SerializeField] private TextMeshProUGUI expText;                   // 经验值文本
        
        [Header("颜色配置")]
        [SerializeField] private Color healthHighColor = Color.green;    // 生命值高时的颜色
        [SerializeField] private Color healthMidColor = Color.yellow;    // 生命值中等时的颜色
        [SerializeField] private Color healthLowColor = Color.red;       // 生命值低时的颜色
        
        // 当前跟踪的玩家实体
        private GameEntity playerEntity;
        
        protected override void OnInitialize()
        {
            base.OnInitialize();
            
            // 查找UI组件（如果没有在Inspector中设置）
            if (playerHealthText == null)
                playerHealthText = FindComponent<TextMeshProUGUI>("PlayerHealth/HealthText");
            if (playerHealthSlider == null)
                playerHealthSlider = FindComponent<Slider>("PlayerHealth/HealthSlider");
            if (playerHealthFill == null && playerHealthSlider != null)
                playerHealthFill = playerHealthSlider.fillRect?.GetComponent<Image>();
            
            if (attackPowerText == null)
                attackPowerText = FindComponent<TextMeshProUGUI>("Attributes/AttackPower/ValueText");
            if (defenseText == null)
                defenseText = FindComponent<TextMeshProUGUI>("Attributes/Defense/ValueText");
            if (attackSpeedText == null)
                attackSpeedText = FindComponent<TextMeshProUGUI>("Attributes/AttackSpeed/ValueText");
            if (moveSpeedText == null)
                moveSpeedText = FindComponent<TextMeshProUGUI>("Attributes/MoveSpeed/ValueText");
            if (attackRangeText == null)
                attackRangeText = FindComponent<TextMeshProUGUI>("Attributes/AttackRange/ValueText");
            if (levelText == null)
                levelText = FindComponent<TextMeshProUGUI>("PlayerInfo/LevelText");
            if (expText == null)
                expText = FindComponent<TextMeshProUGUI>("PlayerInfo/ExpText");
            
            // 初始化UI状态
            UpdateUI();
        }
        
        protected override void OnOpen(object data = null)
        {
            base.OnOpen(data);
            
            // 尝试找到玩家实体
            FindPlayerEntity();
            
            // 开始更新UI
            InvokeRepeating(nameof(UpdateUI), 0f, 0.1f); // 每0.1秒更新一次
        }
        
        protected override void OnClose()
        {
            base.OnClose();
            
            // 停止更新UI
            CancelInvoke(nameof(UpdateUI));
        }
        
        /// <summary>
        /// 查找玩家实体
        /// </summary>
        private void FindPlayerEntity()
        {
            // 查找场景中的玩家实体
            GameEntity[] entities = FindObjectsOfType<GameEntity>();
            foreach (var entity in entities)
            {
                // 检查是否为玩家类型的实体
                if (IsPlayerEntity(entity))
                {
                    playerEntity = entity;
                    Debug.Log($"找到玩家实体: {entity.name}");
                    break;
                }
            }
            
            if (playerEntity == null)
            {
                Debug.LogWarning("未找到玩家实体");
            }
        }
        
        /// <summary>
        /// 检查是否为玩家实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>是否为玩家</returns>
        private bool IsPlayerEntity(GameEntity entity)
        {
            return entity.Type == GameEntity.EntityType.Player;
        }
        
        /// <summary>
        /// 更新UI显示
        /// </summary>
        private void UpdateUI()
        {
            if (playerEntity == null || !playerEntity.IsAlive)
            {
                // 如果玩家实体不存在或已死亡，尝试重新查找
                if (playerEntity == null)
                {
                    FindPlayerEntity();
                }
                
                if (playerEntity == null || !playerEntity.IsAlive)
                {
                    ShowNoPlayerState();
                    return;
                }
            }
            
            // 更新生命值显示
            UpdateHealthDisplay();
            
            // 更新属性显示
            UpdateAttributesDisplay();
            
            // 更新等级和经验显示
            UpdateLevelExpDisplay();
        }
        
        /// <summary>
        /// 更新生命值显示
        /// </summary>
        private void UpdateHealthDisplay()
        {
            if (playerEntity == null) return;
            
            var attributes = playerEntity.Attributes;
            float currentHealth = attributes.CurrentHealth;
            float maxHealth = attributes.MaxHealth;
            float healthPercentage = attributes.HealthPercentage;
            
            // 更新文本
            if (playerHealthText != null)
            {
                playerHealthText.text = $"{currentHealth:F0}/{maxHealth:F0}";
            }
            
            // 更新进度条
            if (playerHealthSlider != null)
            {
                playerHealthSlider.value = healthPercentage;
            }
            
            // 更新颜色
            if (playerHealthFill != null)
            {
                Color healthColor = GetHealthColor(healthPercentage);
                playerHealthFill.color = healthColor;
            }
        }
        
        /// <summary>
        /// 更新属性显示
        /// </summary>
        private void UpdateAttributesDisplay()
        {
            if (playerEntity == null) return;
            
            var attributes = playerEntity.Attributes;
            
            if (attackPowerText != null)
                attackPowerText.text = attributes.AttackPower.ToString("F1");
            
            if (defenseText != null)
                defenseText.text = attributes.Defense.ToString("F1");
            
            if (attackSpeedText != null)
                attackSpeedText.text = attributes.AttackSpeed.ToString("F1");
            
            if (moveSpeedText != null)
                moveSpeedText.text = attributes.MoveSpeed.ToString("F1");
            
            if (attackRangeText != null)
                attackRangeText.text = attributes.AttackRange.ToString("F1");
        }
        
        /// <summary>
        /// 更新等级和经验显示
        /// </summary>
        private void UpdateLevelExpDisplay()
        {
            if (playerEntity == null) return;
            
            var attributes = playerEntity.Attributes;
            
            if (levelText != null)
                levelText.text = $"Lv.{attributes.CurrentLevel}";
            
            if (expText != null)
                expText.text = $"EXP: {attributes.CurrentExp:F0}";
        }
        
        /// <summary>
        /// 显示无玩家状态
        /// </summary>
        private void ShowNoPlayerState()
        {
            if (playerHealthText != null)
                playerHealthText.text = "0/0";
            
            if (playerHealthSlider != null)
                playerHealthSlider.value = 0f;
            
            if (playerHealthFill != null)
                playerHealthFill.color = healthLowColor;
            
            if (attackPowerText != null) attackPowerText.text = "0";
            if (defenseText != null) defenseText.text = "0";
            if (attackSpeedText != null) attackSpeedText.text = "0";
            if (moveSpeedText != null) moveSpeedText.text = "0";
            if (attackRangeText != null) attackRangeText.text = "0";
            if (levelText != null) levelText.text = "Lv.0";
            if (expText != null) expText.text = "EXP: 0";
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
                return healthHighColor;
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
        /// 设置要显示的玩家实体
        /// </summary>
        /// <param name="entity">玩家实体</param>
        public void SetPlayerEntity(GameEntity entity)
        {
            playerEntity = entity;
            UpdateUI();
        }
        
        /// <summary>
        /// 获取当前显示的玩家实体
        /// </summary>
        /// <returns>玩家实体</returns>
        public GameEntity GetPlayerEntity()
        {
            return playerEntity;
        }
    }
} 