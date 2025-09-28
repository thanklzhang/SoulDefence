using UnityEngine;
using TMPro;

namespace SoulDefence.UI
{
    /// <summary>
    /// UI系统初始化器
    /// 负责初始化和启动整个UI系统
    /// </summary>
    public class UIInitializer : MonoBehaviour
    {
        [Header("UI面板预制体")]
        [SerializeField] private GameObject battleUIPrefab;         // 战斗界面预制体
        
        [Header("血条系统")]
        [SerializeField] private GameObject healthBarPrefab;        // 血条预制体
        [SerializeField] private bool enableHealthBarSystem = true; // 是否启用血条系统
        
        [Header("启动设置")]
        [SerializeField] private bool openBattleUIOnStart = true;   // 游戏开始时是否打开战斗界面
        [SerializeField] private bool autoInitialize = true;        // 是否自动初始化
        
        private void Start()
        {
            if (autoInitialize)
            {
                Initialize();
            }
        }
        
        /// <summary>
        /// 初始化UI系统
        /// </summary>
        public void Initialize()
        {
            Debug.Log("开始初始化UI系统...");
            
            // 初始化UI管理器
            InitializeUIManager();
            
            // 初始化血条系统
            if (enableHealthBarSystem)
            {
                InitializeHealthBarSystem();
            }
            
            // 创建和注册UI面板
            CreateAndRegisterPanels();
            
            // 打开初始界面
            if (openBattleUIOnStart)
            {
                OpenBattleUI();
            }
            
            Debug.Log("UI系统初始化完成！");
        }
        
        /// <summary>
        /// 初始化UI管理器
        /// </summary>
        private void InitializeUIManager()
        {
            // 获取或创建UIManager实例
            UIManager uiManager = UIManager.Instance;
            
            Debug.Log("UIManager 已初始化");
        }
        
        /// <summary>
        /// 初始化血条系统
        /// </summary>
        private void InitializeHealthBarSystem()
        {
            // 获取或创建HealthBarManager实例
            HealthBarManager healthBarManager = HealthBarManager.Instance;
            
            // 设置血条预制体
            if (healthBarPrefab != null)
            {
                healthBarManager.SetHealthBarPrefab(healthBarPrefab);
            }
            
            Debug.Log("血条系统已初始化");
        }
        
        /// <summary>
        /// 创建和注册UI面板
        /// </summary>
        private void CreateAndRegisterPanels()
        {
            UIManager uiManager = UIManager.Instance;
            
            // 创建战斗界面
            if (battleUIPrefab != null)
            {
                GameObject battleUIGO = Instantiate(battleUIPrefab);
                BattleUI battleUI = battleUIGO.GetComponent<BattleUI>();
                if (battleUI == null)
                {
                    battleUI = battleUIGO.AddComponent<BattleUI>();
                }
                uiManager.RegisterPanel(battleUI);
                Debug.Log("战斗界面已创建并注册");
            }
            else
            {
                // 如果没有预制体，创建默认的战斗界面
                CreateDefaultBattleUI();
            }
        }
        
        /// <summary>
        /// 创建默认战斗界面
        /// </summary>
        private void CreateDefaultBattleUI()
        {
            GameObject battleUIGO = new GameObject("BattleUI");
            BattleUI battleUI = battleUIGO.AddComponent<BattleUI>();
            
            // 创建基本的UI结构
            CreateDefaultBattleUILayout(battleUIGO);
            
            // 注册到UI管理器
            UIManager.Instance.RegisterPanel(battleUI);
            
            Debug.Log("创建了默认战斗界面");
        }
        
        /// <summary>
        /// 创建默认战斗界面布局
        /// </summary>
        /// <param name="parent">父对象</param>
        private void CreateDefaultBattleUILayout(GameObject parent)
        {
            // 确保有RectTransform组件
            if (parent.GetComponent<RectTransform>() == null)
            {
                parent.AddComponent<RectTransform>();
            }
            
            // 创建玩家状态面板
            CreatePlayerStatusPanel(parent);
            
            // 创建属性面板
            CreateAttributesPanel(parent);
        }
        
        /// <summary>
        /// 创建玩家状态面板
        /// </summary>
        /// <param name="parent">父对象</param>
        private void CreatePlayerStatusPanel(GameObject parent)
        {
            // 创建玩家状态容器
            GameObject playerHealthGO = new GameObject("PlayerHealth");
            playerHealthGO.transform.SetParent(parent.transform, false);
            
            var playerHealthRect = playerHealthGO.AddComponent<RectTransform>();
            playerHealthRect.anchorMin = new Vector2(0, 1);
            playerHealthRect.anchorMax = new Vector2(0, 1);
            playerHealthRect.anchoredPosition = new Vector2(20, -20);
            playerHealthRect.sizeDelta = new Vector2(300, 80);
            
            // 创建背景
            var bgImage = playerHealthGO.AddComponent<UnityEngine.UI.Image>();
            bgImage.color = new Color(0, 0, 0, 0.5f);
            
            // 创建生命值文本
            GameObject healthTextGO = new GameObject("HealthText");
            healthTextGO.transform.SetParent(playerHealthGO.transform, false);
            var healthText = healthTextGO.AddComponent<TextMeshProUGUI>();
            healthText.text = "100/100";
            healthText.color = Color.white;
            healthText.fontSize = 16;
            healthText.alignment = TextAlignmentOptions.Center;
            var healthTextRect = healthTextGO.GetComponent<RectTransform>();
            healthTextRect.anchorMin = Vector2.zero;
            healthTextRect.anchorMax = Vector2.one;
            healthTextRect.sizeDelta = Vector2.zero;
            healthTextRect.anchoredPosition = new Vector2(0, 15);
            
            // 创建生命值滑块
            GameObject healthSliderGO = new GameObject("HealthSlider");
            healthSliderGO.transform.SetParent(playerHealthGO.transform, false);
            var healthSlider = healthSliderGO.AddComponent<UnityEngine.UI.Slider>();
            healthSlider.value = 1f;
            var healthSliderRect = healthSliderGO.GetComponent<RectTransform>();
            healthSliderRect.anchorMin = new Vector2(0.1f, 0.2f);
            healthSliderRect.anchorMax = new Vector2(0.9f, 0.5f);
            healthSliderRect.sizeDelta = Vector2.zero;
            healthSliderRect.anchoredPosition = Vector2.zero;
            
            // 创建滑块背景
            GameObject sliderBgGO = new GameObject("Background");
            sliderBgGO.transform.SetParent(healthSliderGO.transform, false);
            var sliderBgImage = sliderBgGO.AddComponent<UnityEngine.UI.Image>();
            sliderBgImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
            var sliderBgRect = sliderBgGO.GetComponent<RectTransform>();
            sliderBgRect.anchorMin = Vector2.zero;
            sliderBgRect.anchorMax = Vector2.one;
            sliderBgRect.sizeDelta = Vector2.zero;
            sliderBgRect.anchoredPosition = Vector2.zero;
            
            // 创建滑块填充区域
            GameObject fillAreaGO = new GameObject("Fill Area");
            fillAreaGO.transform.SetParent(healthSliderGO.transform, false);
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
            
            // 设置滑块引用
            healthSlider.fillRect = fillRect;
        }
        
        /// <summary>
        /// 创建属性面板
        /// </summary>
        /// <param name="parent">父对象</param>
        private void CreateAttributesPanel(GameObject parent)
        {
            // 创建属性容器
            GameObject attributesGO = new GameObject("Attributes");
            attributesGO.transform.SetParent(parent.transform, false);
            
            var attributesRect = attributesGO.AddComponent<RectTransform>();
            attributesRect.anchorMin = new Vector2(0, 1);
            attributesRect.anchorMax = new Vector2(0, 1);
            attributesRect.anchoredPosition = new Vector2(20, -120);
            attributesRect.sizeDelta = new Vector2(250, 200);
            
            // 创建背景
            var bgImage = attributesGO.AddComponent<UnityEngine.UI.Image>();
            bgImage.color = new Color(0, 0, 0, 0.3f);
            
            // 创建属性项
            string[] attributes = { "AttackPower", "Defense", "AttackSpeed", "MoveSpeed", "AttackRange" };
            string[] attributeNames = { "攻击力", "防御力", "攻击速度", "移动速度", "攻击距离" };
            
            for (int i = 0; i < attributes.Length; i++)
            {
                CreateAttributeItem(attributesGO, attributes[i], attributeNames[i], i);
            }
            
            // 创建玩家信息面板
            CreatePlayerInfoPanel(parent);
        }
        
        /// <summary>
        /// 创建属性项
        /// </summary>
        /// <param name="parent">父对象</param>
        /// <param name="attributeName">属性名称</param>
        /// <param name="displayName">显示名称</param>
        /// <param name="index">索引</param>
        private void CreateAttributeItem(GameObject parent, string attributeName, string displayName, int index)
        {
            // GameObject itemGO = new GameObject(attributeName);
            // itemGO.transform.SetParent(parent.transform, false);
            //
            // var itemRect = itemGO.AddComponent<RectTransform>();
            // itemRect.anchorMin = new Vector2(0, 1);
            // itemRect.anchorMax = new Vector2(1, 1);
            // itemRect.anchoredPosition = new Vector2(0, -10 - index * 35);
            // itemRect.sizeDelta = new Vector2(-10, 30);
            //
            // // 创建标签
            // GameObject labelGO = new GameObject("Label");
            // labelGO.transform.SetParent(itemGO.transform, false);
            // var labelText = labelGO.AddComponent<TextMeshProUGUI>();
            // labelText.text = displayName + ":";
            // labelText.color = Color.white;
            // labelText.fontSize = 14;
            // var labelRect = labelGO.GetComponent<RectTransform>();
            // labelRect.anchorMin = new Vector2(0, 0);
            // labelRect.anchorMax = new Vector2(0.6f, 1);
            // labelRect.sizeDelta = Vector2.zero;
            // labelRect.anchoredPosition = Vector2.zero;
            //
            // // 创建数值文本
            // GameObject valueGO = new GameObject("ValueText");
            // valueGO.transform.SetParent(itemGO.transform, false);
            // var valueText = valueGO.AddComponent<TextMeshProUGUI>();
            // valueText.text = "0";
            // valueText.color = Color.yellow;
            // valueText.fontSize = 14;
            // valueText.alignment = TextAlignmentOptions.MiddleRight;
            // var valueRect = valueGO.GetComponent<RectTransform>();
            // valueRect.anchorMin = new Vector2(0.6f, 0);
            // valueRect.anchorMax = new Vector2(1, 1);
            // valueRect.sizeDelta = Vector2.zero;
            // valueRect.anchoredPosition = Vector2.zero;
        }
        
        /// <summary>
        /// 创建玩家信息面板
        /// </summary>
        /// <param name="parent">父对象</param>
        private void CreatePlayerInfoPanel(GameObject parent)
        {
            GameObject playerInfoGO = new GameObject("PlayerInfo");
            playerInfoGO.transform.SetParent(parent.transform, false);
            
            var playerInfoRect = playerInfoGO.AddComponent<RectTransform>();
            playerInfoRect.anchorMin = new Vector2(0, 1);
            playerInfoRect.anchorMax = new Vector2(0, 1);
            playerInfoRect.anchoredPosition = new Vector2(20, -340);
            playerInfoRect.sizeDelta = new Vector2(250, 60);
            
            // 创建背景
            var bgImage = playerInfoGO.AddComponent<UnityEngine.UI.Image>();
            bgImage.color = new Color(0, 0, 0, 0.3f);
            
            // 创建等级文本
            GameObject levelTextGO = new GameObject("LevelText");
            levelTextGO.transform.SetParent(playerInfoGO.transform, false);
            var levelText = levelTextGO.AddComponent<TextMeshProUGUI>();
            levelText.text = "Lv.1";
            levelText.color = Color.white;
            levelText.fontSize = 16;
            var levelRect = levelTextGO.GetComponent<RectTransform>();
            levelRect.anchorMin = new Vector2(0.05f, 0.5f);
            levelRect.anchorMax = new Vector2(0.95f, 1f);
            levelRect.sizeDelta = Vector2.zero;
            levelRect.anchoredPosition = Vector2.zero;
            
            // 创建经验文本
            GameObject expTextGO = new GameObject("ExpText");
            expTextGO.transform.SetParent(playerInfoGO.transform, false);
            var expText = expTextGO.AddComponent<TextMeshProUGUI>();
            expText.text = "EXP: 0";
            expText.color = Color.cyan;
            expText.fontSize = 14;
            var expRect = expTextGO.GetComponent<RectTransform>();
            expRect.anchorMin = new Vector2(0.05f, 0f);
            expRect.anchorMax = new Vector2(0.95f, 0.5f);
            expRect.sizeDelta = Vector2.zero;
            expRect.anchoredPosition = Vector2.zero;
        }
        
        /// <summary>
        /// 打开战斗界面
        /// </summary>
        public void OpenBattleUI()
        {
            UIManager.Instance.OpenPanel<BattleUI>();
        }
        
        /// <summary>
        /// 关闭战斗界面
        /// </summary>
        public void CloseBattleUI()
        {
            UIManager.Instance.ClosePanel<BattleUI>();
        }
        
        /// <summary>
        /// 设置血条预制体
        /// </summary>
        /// <param name="prefab">预制体</param>
        public void SetHealthBarPrefab(GameObject prefab)
        {
            healthBarPrefab = prefab;
            if (HealthBarManager.Instance != null)
            {
                HealthBarManager.Instance.SetHealthBarPrefab(prefab);
            }
        }
    }
} 