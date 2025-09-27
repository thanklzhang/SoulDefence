# 玩家控制系统

## 系统设计

为了解决"所有实体都响应输入"的问题，我们将输入控制从实体系统中分离出来：

### 🎮 **PlayerController** - 玩家控制器
- **职责**：专门处理玩家输入（WASD键）
- **位置**：只添加到玩家实体上
- **功能**：将输入转换为移动指令，传递给玩家实体

### 🎯 **GameEntity** - 游戏实体
- **职责**：通用实体基类，不包含输入逻辑
- **位置**：所有实体（玩家、怪物等）都使用
- **功能**：执行移动，但移动方向由外部控制

### ⚡ **EntityMovement** - 实体移动组件
- **职责**：纯粹的移动执行，不处理输入
- **位置**：GameEntity的组件
- **功能**：根据设定的移动方向执行移动和转向

## 使用方法

### 1. 设置玩家实体

```
1. 创建玩家GameObject
2. 添加GameEntity脚本
3. 添加PlayerController脚本
4. PlayerController会自动找到同一GameObject上的GameEntity
```

### 2. 设置怪物实体

```
1. 创建怪物GameObject
2. 只添加GameEntity脚本
3. 不添加PlayerController（所以不会响应输入）
4. 可以通过代码控制移动：entity.SetMoveDirection(direction)
```

### 3. 代码控制移动

```csharp
// 控制任何实体移动
GameEntity entity = GetComponent<GameEntity>();
entity.SetMoveDirection(Vector3.forward);  // 向前移动
entity.StopMovement();                     // 停止移动

// 或者直接使用移动组件
entity.Movement.SetMoveDirection(Vector3.right);
```

## 系统优势

✅ **职责分离**：输入处理和移动执行分离  
✅ **灵活控制**：只有需要的实体才添加PlayerController  
✅ **代码复用**：所有实体都使用相同的GameEntity和EntityMovement  
✅ **易于扩展**：可以轻松添加AI控制器、网络控制器等  
✅ **性能优化**：不需要输入的实体不会检查输入  

## 控制类型扩展

基于这个设计，你可以轻松添加其他控制类型：

- **AIController** - AI控制器（怪物自动寻路）
- **NetworkController** - 网络控制器（多人游戏）
- **ScriptController** - 脚本控制器（剧情控制）

每种控制器都只需要调用 `entity.SetMoveDirection()` 来控制移动。 