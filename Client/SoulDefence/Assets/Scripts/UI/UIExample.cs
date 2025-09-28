using UnityEngine;

namespace SoulDefence.UI
{
    /// <summary>
    /// UI使用示例
    /// 展示如何使用UI系统的各项功能
    /// </summary>
    public class UIExample : MonoBehaviour
    {
        [Header("测试设置")]
        [SerializeField] private KeyCode openBattleUIKey = KeyCode.B;       // 打开战斗界面的按键
        [SerializeField] private KeyCode closeBattleUIKey = KeyCode.N;      // 关闭战斗界面的按键
        [SerializeField] private KeyCode toggleHealthBarsKey = KeyCode.H;   // 切换血条显示的按键
        
        private void Update()
        {
            HandleInput();
        }
        
        /// <summary>
        /// 处理输入
        /// </summary>
        private void HandleInput()
        {
            // 打开战斗界面
            if (Input.GetKeyDown(openBattleUIKey))
            {
                OpenBattleUI();
            }
            
            // 关闭战斗界面
            if (Input.GetKeyDown(closeBattleUIKey))
            {
                CloseBattleUI();
            }
            
            // 切换血条显示
            if (Input.GetKeyDown(toggleHealthBarsKey))
            {
                ToggleHealthBars();
            }
        }
        
        /// <summary>
        /// 打开战斗界面
        /// </summary>
        public void OpenBattleUI()
        {
            UIManager.Instance.OpenPanel<BattleUI>();
            Debug.Log("打开战斗界面 (按键: " + openBattleUIKey + ")");
        }
        
        /// <summary>
        /// 关闭战斗界面
        /// </summary>
        public void CloseBattleUI()
        {
            UIManager.Instance.ClosePanel<BattleUI>();
            Debug.Log("关闭战斗界面 (按键: " + closeBattleUIKey + ")");
        }
        
        /// <summary>
        /// 切换血条显示
        /// </summary>
        public void ToggleHealthBars()
        {
            HealthBarManager healthBarManager = HealthBarManager.Instance;
            
            // 这里可以添加切换血条显示的逻辑
            // 例如：切换显示设置、清理所有血条等
            
            Debug.Log("切换血条显示 (按键: " + toggleHealthBarsKey + ")");
            
            // 示例：打印当前血条数量
            Debug.Log("当前UI管理器打开的面板数量: " + UIManager.Instance.GetOpenPanelCount());
        }
        
        /// <summary>
        /// 检查UI系统状态
        /// </summary>
        [ContextMenu("检查UI系统状态")]
        public void CheckUISystemStatus()
        {
            Debug.Log("=== UI系统状态 ===");
            
            // 检查UIManager
            if (UIManager.Instance != null)
            {
                Debug.Log("UIManager: 正常运行");
                Debug.Log("当前打开的面板数量: " + UIManager.Instance.GetOpenPanelCount());
                Debug.Log("战斗界面是否打开: " + UIManager.Instance.IsPanelOpen<BattleUI>());
            }
            else
            {
                Debug.LogWarning("UIManager: 未初始化");
            }
            
            // 检查HealthBarManager
            if (HealthBarManager.Instance != null)
            {
                Debug.Log("HealthBarManager: 正常运行");
            }
            else
            {
                Debug.LogWarning("HealthBarManager: 未初始化");
            }
            
            // 检查场景中的实体数量
            var entities = FindObjectsOfType<SoulDefence.Entity.GameEntity>();
            Debug.Log("场景中的实体数量: " + entities.Length);
            
            Debug.Log("=== 检查完成 ===");
        }
        
        /// <summary>
        /// 初始化UI系统（手动调用）
        /// </summary>
        [ContextMenu("手动初始化UI系统")]
        public void ManualInitializeUISystem()
        {
            UIInitializer initializer = FindObjectOfType<UIInitializer>();
            if (initializer != null)
            {
                initializer.Initialize();
                Debug.Log("手动初始化UI系统完成");
            }
            else
            {
                Debug.LogWarning("未找到UIInitializer组件");
            }
        }
    }
} 