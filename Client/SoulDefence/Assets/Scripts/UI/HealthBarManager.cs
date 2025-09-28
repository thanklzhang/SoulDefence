using System.Collections.Generic;
using UnityEngine;
using SoulDefence.Entity;

namespace SoulDefence.UI
{
    /// <summary>
    /// 血条管理器
    /// 自动为场景中的实体创建和管理血条
    /// </summary>
    public class HealthBarManager : MonoBehaviour
    {
        [Header("血条配置")]
        [SerializeField] private GameObject healthBarPrefab;                    // 血条预制体
        [SerializeField] private Transform healthBarContainer;                  // 血条容器
        [SerializeField] private bool autoCreateHealthBars = true;              // 是否自动创建血条
        [SerializeField] private bool showPlayerHealthBar = false;              // 是否显示玩家血条
        [SerializeField] private bool showEnemyHealthBar = true;                // 是否显示敌人血条
        [SerializeField] private bool showCastleHealthBar = true;               // 是否显示城堡血条
        
        [Header("更新设置")]
        [SerializeField] private float scanInterval = 1f;                      // 扫描间隔（秒）
        [SerializeField] private int maxHealthBars = 50;                       // 最大血条数量
        
        // 血条池
        private List<HealthBarUI> healthBarPool = new List<HealthBarUI>();
        
        // 实体到血条的映射
        private Dictionary<GameEntity, HealthBarUI> entityToHealthBar = new Dictionary<GameEntity, HealthBarUI>();
        
        // 单例实例
        private static HealthBarManager _instance;
        public static HealthBarManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<HealthBarManager>();
                    if (_instance == null)
                    {
                        GameObject managerGO = new GameObject("HealthBarManager");
                        _instance = managerGO.AddComponent<HealthBarManager>();
                    }
                }
                return _instance;
            }
        }
        
        private void Awake()
        {
            // 确保单例唯一性
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            _instance = this;
            
            Initialize();
        }
        
        private void Start()
        {
            if (autoCreateHealthBars)
            {
                // 开始定期扫描实体
                InvokeRepeating(nameof(ScanEntities), 0f, scanInterval);
            }
        }
        
        /// <summary>
        /// 初始化血条管理器
        /// </summary>
        private void Initialize()
        {
            // 创建血条容器
            if (healthBarContainer == null)
            {
                // 将血条容器放在UIManager的UI根节点下
                Transform uiRoot = UIManager.Instance.GetUIRoot();
                if (uiRoot != null)
                {
                    GameObject containerGO = new GameObject("HealthBarContainer");
                    containerGO.transform.SetParent(uiRoot, false);
                    healthBarContainer = containerGO.transform;
                    
                    // 添加RectTransform组件
                    RectTransform rectTransform = containerGO.GetComponent<RectTransform>();
                    if (rectTransform == null)
                    {
                        rectTransform = containerGO.AddComponent<RectTransform>();
                    }
                    
                    // 设置为全屏大小
                    rectTransform.anchorMin = Vector2.zero;
                    rectTransform.anchorMax = Vector2.one;
                    rectTransform.sizeDelta = Vector2.zero;
                    rectTransform.anchoredPosition = Vector2.zero;
                }
                else
                {
                    // 如果UIManager还没初始化，创建临时容器
                    GameObject containerGO = new GameObject("HealthBarContainer");
                    containerGO.transform.SetParent(transform);
                    healthBarContainer = containerGO.transform;
                    
                    Debug.LogWarning("UIManager未初始化，血条容器将创建在HealthBarManager下");
                }
            }
            
            // 创建默认血条预制体（如果没有设置）
            if (healthBarPrefab == null)
            {
                CreateDefaultHealthBarPrefab();
            }
            
            Debug.Log("HealthBarManager 初始化完成");
        }
        
        /// <summary>
        /// 创建默认血条预制体
        /// </summary>
        private void CreateDefaultHealthBarPrefab()
        {
            GameObject prefabGO = new GameObject("DefaultHealthBar");
            prefabGO.AddComponent<HealthBarUI>();
            
            // 添加CanvasGroup组件
            prefabGO.AddComponent<CanvasGroup>();
            
            // 添加RectTransform组件
            if (prefabGO.GetComponent<RectTransform>() == null)
            {
                prefabGO.AddComponent<RectTransform>();
            }
            
            // 创建背景
            GameObject backgroundGO = new GameObject("Background");
            backgroundGO.transform.SetParent(prefabGO.transform, false);
            var bgImage = backgroundGO.AddComponent<UnityEngine.UI.Image>();
            bgImage.color = new Color(0, 0, 0, 0.5f);
            var bgRect = backgroundGO.GetComponent<RectTransform>();
            bgRect.sizeDelta = new Vector2(100, 20);
            
            // 创建Slider
            GameObject sliderGO = new GameObject("HealthSlider");
            sliderGO.transform.SetParent(prefabGO.transform, false);
            var slider = sliderGO.AddComponent<UnityEngine.UI.Slider>();
            slider.transition = UnityEngine.UI.Selectable.Transition.None;
            var sliderRect = sliderGO.GetComponent<RectTransform>();
            sliderRect.sizeDelta = new Vector2(90, 10);
            
            // 创建Slider背景
            GameObject sliderBgGO = new GameObject("Background");
            sliderBgGO.transform.SetParent(sliderGO.transform, false);
            var sliderBgImage = sliderBgGO.AddComponent<UnityEngine.UI.Image>();
            sliderBgImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
            var sliderBgRect = sliderBgGO.GetComponent<RectTransform>();
            sliderBgRect.anchorMin = Vector2.zero;
            sliderBgRect.anchorMax = Vector2.one;
            sliderBgRect.sizeDelta = Vector2.zero;
            sliderBgRect.anchoredPosition = Vector2.zero;
            
            // 创建Slider填充区域
            GameObject fillAreaGO = new GameObject("Fill Area");
            fillAreaGO.transform.SetParent(sliderGO.transform, false);
            var fillAreaRect = fillAreaGO.GetComponent<RectTransform>();
            fillAreaRect.anchorMin = Vector2.zero;
            fillAreaRect.anchorMax = Vector2.one;
            fillAreaRect.sizeDelta = Vector2.zero;
            fillAreaRect.anchoredPosition = Vector2.zero;
            
            GameObject fillGO = new GameObject("Fill");
            fillGO.transform.SetParent(fillAreaGO.transform, false);
            var fillImage = fillGO.AddComponent<UnityEngine.UI.Image>();
            fillImage.color = Color.green;
            fillImage.type = UnityEngine.UI.Image.Type.Filled;
            var fillRect = fillGO.GetComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.sizeDelta = Vector2.zero;
            fillRect.anchoredPosition = Vector2.zero;
            
            // 设置Slider引用
            slider.fillRect = fillRect;
            slider.value = 1f;
            
            // 创建文本
            GameObject textGO = new GameObject("HealthText");
            textGO.transform.SetParent(prefabGO.transform, false);
            var text = textGO.AddComponent<UnityEngine.UI.Text>();
            text.text = "100/100";
            text.color = Color.white;
            text.fontSize = 12;
            text.alignment = TextAnchor.MiddleCenter;
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            var textRect = textGO.GetComponent<RectTransform>();
            textRect.sizeDelta = new Vector2(90, 15);
            textRect.anchoredPosition = new Vector2(0, -15);
            
            // 设置血条的RectTransform
            var healthBarRect = prefabGO.GetComponent<RectTransform>();
            healthBarRect.sizeDelta = new Vector2(100, 30);
            
            // 设置预制体
            healthBarPrefab = prefabGO;
            
            Debug.Log("创建了默认血条预制体");
        }
        
        /// <summary>
        /// 扫描场景中的实体
        /// </summary>
        private void ScanEntities()
        {
            GameEntity[] entities = FindObjectsOfType<GameEntity>();
            
            // 为新实体创建血条
            foreach (var entity in entities)
            {
                if (!entityToHealthBar.ContainsKey(entity) && ShouldShowHealthBar(entity))
                {
                    CreateHealthBarForEntity(entity);
                }
            }
            
            // 清理已销毁实体的血条
            var entitiesToRemove = new List<GameEntity>();
            foreach (var kvp in entityToHealthBar)
            {
                if (kvp.Key == null)
                {
                    entitiesToRemove.Add(kvp.Key);
                    ReturnHealthBarToPool(kvp.Value);
                }
            }
            
            foreach (var entity in entitiesToRemove)
            {
                entityToHealthBar.Remove(entity);
            }
        }
        
        /// <summary>
        /// 检查是否应该为实体显示血条
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>是否显示</returns>
        private bool ShouldShowHealthBar(GameEntity entity)
        {
            // 检查是否为玩家
            if (IsPlayerEntity(entity))
                return showPlayerHealthBar;
            
            // 检查是否为城堡
            if (IsCastleEntity(entity))
                return showCastleHealthBar;
            
            // 检查是否为敌人
            if (IsEnemyEntity(entity))
                return showEnemyHealthBar;
            
            return false;
        }
        
        /// <summary>
        /// 检查是否为玩家实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>是否为玩家</returns>
        private bool IsPlayerEntity(GameEntity entity)
        {
            return entity.GetComponent<PlayerController>() != null ||
                   entity.CompareTag("Player") ||
                   entity.name.ToLower().Contains("player");
        }
        
        /// <summary>
        /// 检查是否为城堡实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>是否为城堡</returns>
        private bool IsCastleEntity(GameEntity entity)
        {
            return entity.CompareTag("Castle") ||
                   entity.name.ToLower().Contains("castle");
        }
        
        /// <summary>
        /// 检查是否为敌人实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>是否为敌人</returns>
        private bool IsEnemyEntity(GameEntity entity)
        {
            return entity.CompareTag("Enemy") ||
                   entity.name.ToLower().Contains("enemy") ||
                   entity.name.ToLower().Contains("monster");
        }
        
        /// <summary>
        /// 为实体创建血条
        /// </summary>
        /// <param name="entity">实体</param>
        private void CreateHealthBarForEntity(GameEntity entity)
        {
            if (entityToHealthBar.Count >= maxHealthBars)
            {
                Debug.LogWarning("血条数量已达到上限");
                return;
            }
            
            HealthBarUI healthBar = GetHealthBarFromPool();
            if (healthBar != null)
            {
                healthBar.SetTargetEntity(entity);
                entityToHealthBar[entity] = healthBar;
                
                Debug.Log($"为实体 {entity.name} 创建血条");
            }
        }
        
        /// <summary>
        /// 从对象池获取血条
        /// </summary>
        /// <returns>血条UI</returns>
        private HealthBarUI GetHealthBarFromPool()
        {
            // 查找未激活的血条
            foreach (var healthBar in healthBarPool)
            {
                if (!healthBar.IsActive())
                {
                    return healthBar;
                }
            }
            
            // 如果没有可用的，创建新的
            if (healthBarPrefab != null && healthBarContainer != null)
            {
                GameObject healthBarGO = Instantiate(healthBarPrefab, healthBarContainer);
                HealthBarUI healthBar = healthBarGO.GetComponent<HealthBarUI>();
                if (healthBar == null)
                {
                    healthBar = healthBarGO.AddComponent<HealthBarUI>();
                }
                
                healthBarPool.Add(healthBar);
                return healthBar;
            }
            
            return null;
        }
        
        /// <summary>
        /// 将血条返回对象池
        /// </summary>
        /// <param name="healthBar">血条UI</param>
        private void ReturnHealthBarToPool(HealthBarUI healthBar)
        {
            if (healthBar != null)
            {
                healthBar.SetTargetEntity(null);
                healthBar.SetActive(false);
            }
        }
        
        /// <summary>
        /// 手动为实体创建血条
        /// </summary>
        /// <param name="entity">实体</param>
        public void CreateHealthBar(GameEntity entity)
        {
            if (entity != null && !entityToHealthBar.ContainsKey(entity))
            {
                CreateHealthBarForEntity(entity);
            }
        }
        
        /// <summary>
        /// 移除实体的血条
        /// </summary>
        /// <param name="entity">实体</param>
        public void RemoveHealthBar(GameEntity entity)
        {
            if (entityToHealthBar.TryGetValue(entity, out HealthBarUI healthBar))
            {
                ReturnHealthBarToPool(healthBar);
                entityToHealthBar.Remove(entity);
            }
        }
        
        /// <summary>
        /// 设置血条预制体
        /// </summary>
        /// <param name="prefab">预制体</param>
        public void SetHealthBarPrefab(GameObject prefab)
        {
            healthBarPrefab = prefab;
        }
        
        /// <summary>
        /// 获取实体的血条
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>血条UI</returns>
        public HealthBarUI GetHealthBar(GameEntity entity)
        {
            entityToHealthBar.TryGetValue(entity, out HealthBarUI healthBar);
            return healthBar;
        }
        
        /// <summary>
        /// 清理所有血条
        /// </summary>
        public void ClearAllHealthBars()
        {
            foreach (var healthBar in healthBarPool)
            {
                if (healthBar != null)
                {
                    Destroy(healthBar.gameObject);
                }
            }
            
            healthBarPool.Clear();
            entityToHealthBar.Clear();
        }
    }
} 