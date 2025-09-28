# UI系统使用说明

## 系统概述

这个UI系统为SoulDefence游戏提供了完整的界面管理功能，包括：

1. **UI管理器** - 统一管理所有UI面板的生命周期
2. **战斗界面** - 显示玩家实体的生命值、属性等信息
3. **血条系统** - 为所有实体显示跟随的血条UI
4. **UI初始化器** - 自动设置和启动整个UI系统

## 核心组件

### 1. UIManager（UI管理器）
- **职责**：管理所有UI面板的打开、关闭、层级等
- **特点**：单例模式，支持面板栈管理
- **使用方法**：
  ```csharp
  // 打开面板
  UIManager.Instance.OpenPanel<BattleUI>();
  
  // 关闭面板
  UIManager.Instance.ClosePanel<BattleUI>();
  
  // 检查面板是否打开
  bool isOpen = UIManager.Instance.IsPanelOpen<BattleUI>();
  ```

### 2. UIBasePanel（UI面板基类）
- **职责**：定义统一的界面生命周期和基础功能
- **生命周期**：Initialize → Open → Refresh → Close
- **特点**：支持ESC键关闭、模态窗口、层级管理等
- **设计原则**：每个Panel不包含Canvas组件，所有Panel共享UIManager的Canvas

### 3. BattleUI（战斗界面）
- **职责**：显示玩家实体的生命值、进度条和各项属性
- **功能**：
  - 实时显示当前生命值/最大生命值
  - 生命值进度条（带颜色变化）
  - 显示攻击力、防御力、攻击速度、移动速度、攻击距离
  - 显示等级和经验值
- **自动功能**：
  - 自动查找玩家实体
  - 实时更新数据（每0.1秒）
  - 生命值颜色根据百分比变化

### 4. HealthBarUI（血条UI）
- **职责**：显示单个实体的生命值，跟随实体移动
- **特点**：
  - 屏幕空间UI显示（通过WorldToScreenPoint转换位置）
  - 自动跟随目标实体
  - 距离淡出效果
  - 满血/死亡时可隐藏
  - 自动处理摄像机后方的实体（隐藏血条）

### 5. HealthBarManager（血条管理器）
- **职责**：自动为场景中的实体创建和管理血条
- **功能**：
  - 自动扫描场景实体
  - 对象池管理血条
  - 支持不同实体类型的显示设置
  - 血条容器集成到UIManager的Canvas中
- **配置**：
  - `showPlayerHealthBar`: 是否显示玩家血条
  - `showEnemyHealthBar`: 是否显示敌人血条  
  - `showCastleHealthBar`: 是否显示城堡血条

### 6. UIInitializer（UI初始化器）
- **职责**：初始化和启动整个UI系统
- **功能**：
  - 自动创建UI管理器
  - 初始化血条系统
  - 创建默认UI布局
  - 注册UI面板

## 快速开始

### 1. 基础设置

1. 在场景中创建一个空GameObject
2. 添加`UIInitializer`脚本
3. 配置相关设置（可选，系统会自动创建默认配置）
4. 运行游戏，UI系统会自动初始化

### 2. 使用示例

添加`UIExample`脚本到场景中的任意GameObject上，然后：

- 按 **B** 键打开战斗界面
- 按 **N** 键关闭战斗界面
- 按 **H** 键切换血条显示
- 右键点击脚本选择"检查UI系统状态"查看系统状态

### 3. 手动控制

```csharp
// 打开战斗界面
UIManager.Instance.OpenPanel<BattleUI>();

// 为特定实体创建血条
HealthBarManager.Instance.CreateHealthBar(entity);

// 设置战斗界面显示特定玩家
var battleUI = UIManager.Instance.GetPanel<BattleUI>();
battleUI.SetPlayerEntity(playerEntity);
```

## 系统特性

### ✅ 自动化功能
- 自动查找和跟踪玩家实体
- 自动为场景实体创建血条
- 自动更新UI数据
- 自动管理UI层级

### ✅ 灵活配置
- 支持自定义UI预制体
- 可配置血条显示规则
- 支持不同实体类型的区分显示
- 可调节更新频率和显示参数

### ✅ 性能优化
- 对象池管理血条
- 距离裁剪和淡出
- 合理的更新频率
- 智能的显示/隐藏逻辑

### ✅ 扩展性
- 基于接口的设计
- 统一的面板生命周期
- 支持自定义UI面板
- 模块化架构

## 实体识别规则

系统通过以下方式识别不同类型的实体：

### 玩家实体
- 有`PlayerController`组件
- 标签为"Player"
- 名称包含"player"（不区分大小写）

### 敌人实体
- 标签为"Enemy" 
- 名称包含"enemy"或"monster"（不区分大小写）

### 城堡实体
- 标签为"Castle"
- 名称包含"castle"（不区分大小写）

## 自定义扩展

### 1. 创建新的UI面板

```csharp
public class CustomUI : UIBasePanel
{
    protected override void OnInitialize()
    {
        // 初始化UI组件
    }
    
    protected override void OnOpen(object data = null)
    {
        // 面板打开时的逻辑
    }
    
    protected override void OnClose()
    {
        // 面板关闭时的逻辑
    }
}

// 注册和使用
UIManager.Instance.RegisterPanel(customUI);
UIManager.Instance.OpenPanel<CustomUI>();
```

### 2. 自定义血条预制体

1. 创建包含以下组件的预制体：
   - `HealthBarUI`脚本
   - `Slider`组件（生命值进度条）
   - `Text`组件（生命值文本）
   - `CanvasGroup`组件（透明度控制）

2. 在`UIInitializer`中设置：
   ```csharp
   initializer.SetHealthBarPrefab(yourCustomPrefab);
   ```

## 注意事项

1. **实体要求**：确保游戏实体使用`GameEntity`类并正确配置属性系统
2. **摄像机设置**：血条系统需要主摄像机（Camera.main）
3. **性能考虑**：大量实体时可以调整血条的`maxHealthBars`参数
4. **UI层级**：UI管理器会自动处理面板层级，避免手动修改Canvas的sortingOrder

## 故障排除

### 问题：战斗界面不显示数据
- 检查场景中是否有符合条件的玩家实体
- 确认实体的`EntityAttributes`组件正确配置
- 查看Console是否有"未找到玩家实体"警告

### 问题：血条不显示
- 确认`HealthBarManager`的显示设置
- 检查实体的标签和名称是否符合识别规则
- 查看血条数量是否超过`maxHealthBars`限制

### 问题：UI位置异常
- 检查Canvas的RenderMode设置
- 确认CanvasScaler的配置
- 查看RectTransform的锚点设置

## 版本信息

- 当前版本：1.0
- 兼容Unity版本：2021.3+
- 依赖：Unity UI (uGUI) 