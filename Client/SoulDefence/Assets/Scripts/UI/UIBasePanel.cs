using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SoulDefence.UI
{
    /// <summary>
    /// UI面板基类
    /// 定义统一的界面生命周期和基础功能
    /// </summary>
    public abstract class UIBasePanel : MonoBehaviour
    {
        [Header("面板配置")]
        [SerializeField] protected bool closeOnEscapeKey = false;  // 是否支持ESC键关闭
        [SerializeField] protected bool modal = false;             // 是否为模态窗口
        [SerializeField] protected bool hideOnClose = true;        // 关闭时是否隐藏（否则销毁）
        
        // UI管理器引用
        protected UIManager uiManager;
        
        // 面板状态
        private bool isOpen = false;
        private bool isInitialized = false;
        
        /// <summary>
        /// 面板是否打开
        /// </summary>
        public bool IsOpen => isOpen;
        
        /// <summary>
        /// 面板是否已初始化
        /// </summary>
        public bool IsInitialized => isInitialized;
        
        protected virtual void Awake()
        {
            // 初始化时默认隐藏
            gameObject.SetActive(false);
        }
        
        protected virtual void Start()
        {
            if (!isInitialized)
            {
                Initialize();
            }
        }
        
        protected virtual void Update()
        {
            // 处理ESC键关闭
            if (isOpen && closeOnEscapeKey && Input.GetKeyDown(KeyCode.Escape))
            {
                Close();
            }
        }
        
        /// <summary>
        /// 设置UI管理器引用
        /// </summary>
        /// <param name="manager">UI管理器</param>
        public void SetUIManager(UIManager manager)
        {
            uiManager = manager;
        }
        
        /// <summary>
        /// 初始化面板
        /// </summary>
        protected virtual void Initialize()
        {
            if (isInitialized) return;
            
            OnInitialize();
            isInitialized = true;
            
            Debug.Log($"面板 {GetType().Name} 初始化完成");
        }
        
        /// <summary>
        /// 打开面板
        /// </summary>
        /// <param name="data">传递的数据</param>
        public virtual void Open(object data = null)
        {
            if (isOpen) return;
            
            // 确保已初始化
            if (!isInitialized)
            {
                Initialize();
            }
            
            // 激活GameObject
            gameObject.SetActive(true);
            
            // 设置状态
            isOpen = true;
            
            // 调用子类实现
            OnOpen(data);
            
            Debug.Log($"面板 {GetType().Name} 已打开");
        }
        
        /// <summary>
        /// 关闭面板
        /// </summary>
        public virtual void Close()
        {
            if (!isOpen) return;
            
            // 调用子类实现
            OnClose();
            
            // 设置状态
            isOpen = false;
            
            // 隐藏或销毁
            if (hideOnClose)
            {
                gameObject.SetActive(false);
            }
            else
            {
                Destroy(gameObject);
            }
            
            Debug.Log($"面板 {GetType().Name} 已关闭");
        }
        
        /// <summary>
        /// 设置面板层级
        /// </summary>
        /// <param name="sortingOrder">层级值</param>
        public virtual void SetSortingOrder(int sortingOrder)
        {
            // 通过设置UI层级来控制显示顺序
            // 可以通过设置RectTransform的siblingIndex或其他方式实现
            transform.SetSiblingIndex(sortingOrder);
        }
        
        /// <summary>
        /// 刷新面板数据
        /// </summary>
        public virtual void Refresh()
        {
            if (!isOpen) return;
            OnRefresh();
        }
        
        #region 子类重写方法
        
        /// <summary>
        /// 子类重写：初始化时调用
        /// </summary>
        protected virtual void OnInitialize() { }
        
        /// <summary>
        /// 子类重写：打开时调用
        /// </summary>
        /// <param name="data">传递的数据</param>
        protected virtual void OnOpen(object data = null) { }
        
        /// <summary>
        /// 子类重写：关闭时调用
        /// </summary>
        protected virtual void OnClose() { }
        
        /// <summary>
        /// 子类重写：刷新时调用
        /// </summary>
        protected virtual void OnRefresh() { }
        
        #endregion
        
        #region 工具方法
        
        /// <summary>
        /// 查找子对象组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="path">路径</param>
        /// <returns>组件实例</returns>
        protected T FindComponent<T>(string path) where T : Component
        {
            Transform child = transform.Find(path);
            if (child != null)
            {
                return child.GetComponent<T>();
            }
            
            Debug.LogWarning($"未找到路径为 {path} 的组件 {typeof(T).Name}");
            return null;
        }
        
        /// <summary>
        /// 设置按钮点击事件
        /// </summary>
        /// <param name="path">按钮路径</param>
        /// <param name="callback">回调方法</param>
        protected void SetButtonClick(string path, System.Action callback)
        {
            Button button = FindComponent<Button>(path);
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => callback?.Invoke());
            }
        }
        
        /// <summary>
        /// 设置文本内容
        /// </summary>
        /// <param name="path">文本路径</param>
        /// <param name="text">文本内容</param>
        protected void SetText(string path, string text)
        {
            TextMeshProUGUI textComponent = FindComponent<TextMeshProUGUI>(path);
            if (textComponent != null)
            {
                textComponent.text = text;
            }
        }
        
        /// <summary>
        /// 设置图片填充值
        /// </summary>
        /// <param name="path">图片路径</param>
        /// <param name="fillAmount">填充值（0-1）</param>
        protected void SetImageFillAmount(string path, float fillAmount)
        {
            Image image = FindComponent<Image>(path);
            if (image != null && image.type == Image.Type.Filled)
            {
                image.fillAmount = Mathf.Clamp01(fillAmount);
            }
        }
        
        #endregion
    }
} 