# 关卡流程系统使用说明

## 系统概述

关卡流程系统实现了策划需求文档中的可配置波次怪物生成功能，包含以下核心组件：

- **LevelConfig** - 关卡配置（ScriptableObject）
- **WaveConfig** - 波次配置（数据结构）
- **LevelManager** - 关卡管理器（主控制器）
- **WaveManager** - 波次管理器（波次流程控制）
- **SpawnEntityController** - 生成控制器（出怪点选择和生成逻辑）
- **DebugLevelTester** - 调试测试器（可视化和测试）

## 快速开始

### 1. 创建关卡配置

1. 在Project窗口中右键 → Create → Game → Level Config
2. 命名为 "Level1Config"
3. 在Inspector中设置波次数量并配置每波怪物的参数

### 2. 设置场景

1. 创建空GameObject，命名为 "LevelManager"
2. 添加 `LevelManager` 脚本
3. 将创建的关卡配置拖入 "Current Level Config" 字段
4. 勾选 "Auto Start On Awake" 来自动开始关卡

### 3. 设置出怪点（必需）

确保场景中有 `SpawnPointManager`：
1. 创建空GameObject，命名为 "SpawnPointManager" 
2. 添加 `SpawnPointManager` 脚本
3. 在Inspector中添加几个出怪点并设置位置

### 4. 添加调试器（可选）

1. 在LevelManager上添加 `DebugLevelTester` 脚本
2. 勾选 "Create Test Monsters" 来生成可视化的测试怪物
3. 运行游戏观察效果

## 详细配置

### 关卡配置 (LevelConfig)

```
关卡ID: 1
关卡名称: "第一关"
关卡描述: "新手关卡"
波次数量: 5 (可配置，支持1-N波)

波次配置 (根据设定数量自动生成):
- 第1波: 5只SmallEnemy，延迟0秒，间隔1秒
- 第2波: 8只SmallEnemy，延迟10秒，间隔0.8秒
- 第3波: 3只BigEnemy，延迟15秒，间隔2秒
- 第4波: 10只SmallEnemy，延迟20秒，间隔1.2秒
- 第5波: 1只Boss，延迟30秒，间隔0秒
```

### 波次配置 (WaveConfig)

每个波次包含以下参数：
- **波次编号/名称** - 自动生成
- **怪物数量** - 这一波生成多少只怪物
- **怪物类型** - 字符串标识（如"SmallEnemy", "BigEnemy", "Boss"）
- **波次延迟** - 开始前等待时间（秒）
- **生成间隔** - 每只怪物之间的间隔时间（秒）

## 系统特性

### 🎯 出怪点分散算法

- 自动避免连续在同一出怪点生成怪物
- 可配置避免重复的次数（默认3次）
- 出怪点用完后自动重置历史记录

### ⏱️ 精确时间控制

- 支持每波独立的延迟时间
- 支持每波独立的生成间隔
- 倒计时事件通知

### 🔧 事件系统

```csharp
// 订阅关卡事件
LevelManager.Instance.OnLevelStart += (config) => { /* 关卡开始 */ };
LevelManager.Instance.OnWaveStart += (index, wave) => { /* 波次开始 */ };
LevelManager.Instance.OnMonsterSpawn += (pos, rot, type) => { /* 怪物生成 */ };
LevelManager.Instance.OnLevelComplete += (config) => { /* 关卡完成 */ };
```

### 🎮 调试功能

- **右键菜单**：在LevelManager上可以开始/停止/重置关卡
- **强制下一波**：跳过当前波次延迟，立即开始下一波
- **可视化**：DebugLevelTester可以创建测试怪物和特效
- **日志输出**：详细的执行日志用于调试

## 使用示例

### 代码控制关卡

```csharp
// 获取关卡管理器
LevelManager levelManager = LevelManager.Instance;

// 开始指定关卡
levelManager.StartLevel(myLevelConfig);

// 查询状态
bool isActive = levelManager.IsWaveActive();
int currentWave = levelManager.GetCurrentWaveNumber();
bool isComplete = levelManager.IsLevelComplete();

// 停止关卡
levelManager.StopLevel();
```

### 自定义怪物生成

```csharp
// 订阅怪物生成事件
LevelManager.Instance.OnMonsterSpawn += (position, rotation, monsterType) => {
    // 在这里实现真正的怪物实例化逻辑
    GameObject monster = MonsterFactory.Create(monsterType);
    monster.transform.position = position;
    monster.transform.rotation = rotation;
};
```

## 注意事项

1. **必需组件**：场景中必须有 `SpawnPointManager` 且至少有一个激活的出怪点
2. **关卡配置**：LevelConfig会根据设定的波次数量自动调整波次列表，至少保证1波
3. **事件订阅**：记得在适当的时候取消事件订阅以避免内存泄漏
4. **怪物实例化**：系统只负责生成逻辑，实际的怪物创建需要其他系统处理
5. **性能考虑**：大量怪物生成时注意性能优化

## 扩展建议

- 添加波次预告UI显示
- 实现怪物血量和AI系统
- 添加关卡进度保存
- 支持动态关卡配置
- 添加音效和特效系统 