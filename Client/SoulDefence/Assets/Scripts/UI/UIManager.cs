using System.Collections.Generic;
using UnityEngine;
using System;

namespace SoulDefence.UI
{
    /// <summary>
    /// UI管理器
    /// 负责管理所有UI面板的生命周期、打开、关闭等操作
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [Header("UI配置")]
        [SerializeField] private Transform uiRoot;  // UI根节点
        [SerializeField] private Camera uiCamera;   // UI相机
        
        // 单例实例
        private static UIManager _instance;
        public static UIManager Instance 
        { 
            get 
            { 
                if (_instance == null)
                {
                    _instance = FindObjectOfType<UIManager>();
                    if (_instance == null)
                    {
                        GameObject uiManagerGO = new GameObject("UIManager");
                        _instance = uiManagerGO.AddComponent<UIManager>();
                    }
                }
                return _instance; 
            } 
        }
        
        // 存储所有UI面板
        private Dictionary<Type, UIBasePanel> panels = new Dictionary<Type, UIBasePanel>();
        
        // 面板栈，用于管理面板层级
        private List<UIBasePanel> panelStack = new List<UIBasePanel>();
        
        private void Awake()
        {
            // 确保单例唯一性
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            _instance = this;
            DontDestroyOnLoad(gameObject);
            
            Initialize();
        }
        
        /// <summary>
        /// 初始化UI管理器
        /// </summary>
        private void Initialize()
        {
            // 如果没有设置UI根节点，创建一个
            if (uiRoot == null)
            {
                GameObject uiRootGO = new GameObject("UIRoot");
                uiRootGO.transform.SetParent(transform);
                uiRoot = uiRootGO.transform;
                
                // 添加Canvas组件
                Canvas canvas = uiRootGO.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 100;
                
                // 添加CanvasScaler组件
                var scaler = uiRootGO.AddComponent<UnityEngine.UI.CanvasScaler>();
                scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                scaler.screenMatchMode = UnityEngine.UI.CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                scaler.matchWidthOrHeight = 0.5f;
                
                // 添加GraphicRaycaster组件
                uiRootGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            }
            
            Debug.Log("UIManager 初始化完成");
        }
        
        /// <summary>
        /// 注册UI面板
        /// </summary>
        /// <typeparam name="T">面板类型</typeparam>
        /// <param name="panel">面板实例</param>
        public void RegisterPanel<T>(T panel) where T : UIBasePanel
        {
            Type panelType = typeof(T);
            if (panels.ContainsKey(panelType))
            {
                Debug.LogWarning($"面板 {panelType.Name} 已经注册过了");
                return;
            }
            
            panels[panelType] = panel;
            panel.transform.SetParent(uiRoot, false);
            panel.SetUIManager(this);
            
            Debug.Log($"注册面板: {panelType.Name}");
        }
        
        /// <summary>
        /// 获取UI面板
        /// </summary>
        /// <typeparam name="T">面板类型</typeparam>
        /// <returns>面板实例</returns>
        public T GetPanel<T>() where T : UIBasePanel
        {
            Type panelType = typeof(T);
            if (panels.TryGetValue(panelType, out UIBasePanel panel))
            {
                return panel as T;
            }
            return null;
        }
        
        /// <summary>
        /// 打开UI面板
        /// </summary>
        /// <typeparam name="T">面板类型</typeparam>
        /// <param name="data">传递给面板的数据</param>
        public void OpenPanel<T>(object data = null) where T : UIBasePanel
        {
            T panel = GetPanel<T>();
            if (panel == null)
            {
                Debug.LogError($"面板 {typeof(T).Name} 未注册");
                return;
            }
            
            if (panel.IsOpen)
            {
                Debug.LogWarning($"面板 {typeof(T).Name} 已经打开了");
                return;
            }
            
            // 调用面板的打开方法
            panel.Open(data);
            
            // 添加到面板栈
            if (!panelStack.Contains(panel))
            {
                panelStack.Add(panel);
            }
            
            // 更新面板层级
            UpdatePanelSortingOrder();
            
            Debug.Log($"打开面板: {typeof(T).Name}");
        }
        
        /// <summary>
        /// 关闭UI面板
        /// </summary>
        /// <typeparam name="T">面板类型</typeparam>
        public void ClosePanel<T>() where T : UIBasePanel
        {
            T panel = GetPanel<T>();
            if (panel == null)
            {
                Debug.LogError($"面板 {typeof(T).Name} 未注册");
                return;
            }
            
            if (!panel.IsOpen)
            {
                Debug.LogWarning($"面板 {typeof(T).Name} 已经关闭了");
                return;
            }
            
            // 调用面板的关闭方法
            panel.Close();
            
            // 从面板栈移除
            panelStack.Remove(panel);
            
            Debug.Log($"关闭面板: {typeof(T).Name}");
        }
        
        /// <summary>
        /// 关闭所有面板
        /// </summary>
        public void CloseAllPanels()
        {
            var panelsToClose = new List<UIBasePanel>(panelStack);
            foreach (var panel in panelsToClose)
            {
                panel.Close();
            }
            panelStack.Clear();
            
            Debug.Log("关闭所有面板");
        }
        
        /// <summary>
        /// 更新面板层级顺序
        /// </summary>
        private void UpdatePanelSortingOrder()
        {
            for (int i = 0; i < panelStack.Count; i++)
            {
                var panel = panelStack[i];
                if (panel != null)
                {
                    panel.SetSortingOrder(i);
                }
            }
        }
        
        /// <summary>
        /// 获取UI根节点
        /// </summary>
        public Transform GetUIRoot()
        {
            return uiRoot;
        }
        
        /// <summary>
        /// 获取当前打开的面板数量
        /// </summary>
        public int GetOpenPanelCount()
        {
            return panelStack.Count;
        }
        
        /// <summary>
        /// 检查面板是否打开
        /// </summary>
        /// <typeparam name="T">面板类型</typeparam>
        /// <returns>是否打开</returns>
        public bool IsPanelOpen<T>() where T : UIBasePanel
        {
            T panel = GetPanel<T>();
            return panel != null && panel.IsOpen;
        }
    }
} 