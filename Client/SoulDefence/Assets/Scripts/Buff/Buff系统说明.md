# Buff系统说明

## 概述

Buff系统是一个独立的状态管理系统，用于管理实体身上的临时增益/减益效果。

## 核心特点

- **独立设计**：Buff系统与被动技能系统独立，但可以协同工作
- **来源多样**：支持技能、奖励、道具等多种来源
- **灵活配置**：支持多种触发类型和效果类型
- **叠加机制**：支持时间叠加、层数叠加、独立叠加等
- **自动管理**：自动更新、过期移除、属性重算

## 文件结构

```
Buff/
├── BuffEnums.cs           # Buff枚举定义
├── BuffData.cs            # Buff配置数据（ScriptableObject）
├── BuffInstance.cs        # Buff实例（运行时状态）
└── BuffSystem.cs          # Buff系统管理
```

## 核心枚举

### BuffType（Buff类型）
- **Positive** - 正面Buff（增益）
- **Negative** - 负面Buff（减益）
- **Neutral** - 中性Buff

### BuffEffectType（效果类型）
| 类型 | 说明 | 主要参数 |
|------|------|---------|
| AttributeModifier | 属性修改 | attributeType, attributeValue |
| DamageOverTime | 持续伤害(DOT) | damagePerTick, tickInterval |
| HealOverTime | 持续治疗(HOT) | damagePerTick, tickInterval |
| AreaDamage | 周围伤害 | areaRadius, areaEffectValue |
| AreaHeal | 周围治疗 | areaRadius, areaEffectValue |
| StackCounter | 计数器 | stackThreshold, stackEffectValue |
| Stun | 眩晕 | - |
| Slow | 减速 | slowPercent |
| Silence | 沉默 | - |
| Shield | 护盾 | shieldAmount |

### BuffStackType（叠加类型）
- **None** - 不可叠加（刷新时间）
- **Duration** - 叠加持续时间
- **Count** - 叠加层数
- **Independent** - 独立叠加（可多个相同Buff）

### BuffTickRate（更新频率）
- **None** - 不需要Tick
- **PerSecond** - 每秒
- **PerHalfSecond** - 每0.5秒
- **PerFrame** - 每帧

## 配置示例

### 示例1：每秒对周围敌人造成伤害

```
基础信息：
  buffId: 4001
  buffName: "火焰光环"
  buffType: Positive  (对使用者是增益)
  description: "每秒对周围5米敌人造成10点伤害"

持续时间：
  duration: 10
  isPermanent: false

叠加配置：
  stackType: Duration  (叠加时间)
  maxStacks: 1

效果配置：
  effectType: AreaDamage
  tickRate: PerSecond
  
  areaRadius: 5
  areaEffectValue: 10
  tickInterval: 1
```

### 示例2：计数类Buff（满层爆发）

```
基础信息：
  buffId: 4002
  buffName: "怒气"
  buffType: Positive
  description: "每次攻击增加1层怒气，满3层时造成150点爆发伤害并清除"

持续时间：
  duration: 10
  isPermanent: false

叠加配置：
  stackType: Count  (叠加层数)
  maxStacks: 3

效果配置：
  effectType: StackCounter
  
  stackThreshold: 3
  stackEffectValue: 150
```

### 示例3：属性加成Buff

```
基础信息：
  buffId: 4003
  buffName: "狂暴"
  buffType: Positive
  description: "增加50点攻击力，持续5秒"

持续时间：
  duration: 5
  isPermanent: false

叠加配置：
  stackType: None  (不可叠加，刷新时间)
  maxStacks: 1

效果配置：
  effectType: AttributeModifier
  tickRate: None  (不需要Tick)
  
  attributeType: 0  (攻击力)
  attributeValue: 50
  isPercentage: false
```

### 示例4：持续伤害Buff（DOT）

```
基础信息：
  buffId: 4004
  buffName: "中毒"
  buffType: Negative
  description: "每秒受到5点毒素伤害，持续8秒"

持续时间：
  duration: 8
  isPermanent: false

叠加配置：
  stackType: Count  (可叠层)
  maxStacks: 5

效果配置：
  effectType: DamageOverTime
  tickRate: PerSecond
  
  damagePerTick: 5
  tickInterval: 1
```

### 示例5：持续治疗Buff（HOT）

```
基础信息：
  buffId: 4005
  buffName: "再生"
  buffType: Positive
  description: "每2秒恢复15点生命，持续10秒"

持续时间：
  duration: 10
  isPermanent: false

叠加配置：
  stackType: Independent  (可以有多个)
  maxStacks: 99

效果配置：
  effectType: HealOverTime
  tickRate: PerSecond
  
  damagePerTick: 15
  tickInterval: 2
```

### 示例6：减速Buff

```
基础信息：
  buffId: 4006
  buffName: "寒冰减速"
  buffType: Negative
  description: "移动速度降低50%，持续3秒"

持续时间：
  duration: 3
  isPermanent: false

叠加配置：
  stackType: None
  maxStacks: 1

效果配置：
  effectType: Slow
  tickRate: None
  
  slowPercent: 0.5  (50%减速)
```

## 使用方式

### 创建Buff配置

右键 -> Create -> GameConfig -> Buff

### 给实体添加Buff

```csharp
// 方式1：直接添加
entity.AddBuff(buffData, caster);

// 方式2：通过BuffSystem
entity.BuffSystem.AddBuff(buffData, caster);

// 方式3：技能自动添加
// 在SkillData中配置 buffToTarget 或 buffToSelf
```

### 移除Buff

```csharp
// 移除指定Buff
entity.RemoveBuff(buffData);

// 移除所有负面Buff
entity.BuffSystem.RemoveBuffsByType(BuffType.Negative);

// 驱散Buff（移除可驱散的负面Buff）
entity.BuffSystem.DispelBuffs(2); // 驱散2个

// 清除所有Buff
entity.BuffSystem.ClearAllBuffs();
```

### 查询Buff

```csharp
// 是否拥有指定Buff
bool hasBuff = entity.BuffSystem.HasBuff(buffData);

// 是否拥有某类型Buff
bool hasDebuff = entity.BuffSystem.HasBuffType(BuffType.Negative);

// 查找Buff
BuffInstance buff = entity.BuffSystem.FindBuff(buffData);
BuffInstance buff2 = entity.BuffSystem.FindBuffByName("中毒");

// 获取所有Buff
List<BuffInstance> allBuffs = entity.BuffSystem.GetAllBuffs();

// 获取指定类型的Buff
List<BuffInstance> negativeBuffs = entity.BuffSystem.GetBuffsByType(BuffType.Negative);

// 获取Buff数量
int count = entity.BuffSystem.GetBuffCount();
```

### 状态查询

```csharp
// 是否被眩晕
bool isStunned = entity.BuffSystem.IsStunned();

// 是否被沉默
bool isSilenced = entity.BuffSystem.IsSilenced();

// 获取移动速度修正（减速）
float speedModifier = entity.BuffSystem.GetMovementSpeedModifier();
```

## 技能附带Buff

### 配置方式

在 SkillData 中：

```
Buff配置：
  buffToTarget: 拖入Buff配置（对目标生效）
  buffToSelf: 拖入Buff配置（对自己生效）
```

### 示例：火球术

```
技能名称：火球术

主动技能：
  damage: 50
  skillType: Ranged
  cooldown: 3

Buff配置：
  buffToTarget: 燃烧Buff（DOT）
  buffToSelf: 火焰之力Buff（攻击力加成）
```

效果：
- 对敌人造成50点伤害
- 给敌人添加燃烧Buff（每秒5点伤害，持续5秒）
- 给自己添加火焰之力Buff（+20攻击力，持续3秒）

## Buff与被动技能的区别

| 特性 | Buff系统 | 被动技能系统 |
|------|---------|-------------|
| 持续性 | 临时的，有时间限制 | 永久的，技能固有 |
| 来源 | 多样（技能、奖励、道具等） | 技能自带 |
| 独立性 | 完全独立的系统 | 技能的一部分 |
| 可移除 | 可以被移除/驱散 | 不能移除 |
| 叠加 | 支持多种叠加方式 | 无叠加概念 |
| 应用对象 | 可以给自己或他人 | 只对技能所有者 |

## 协同工作

虽然独立，但Buff和被动技能可以协同：

```
技能：嗜血狂暴

主动效果：
- 造成100点伤害

被动效果：
- 击杀敌人恢复50点生命

Buff效果：
- 对自己：狂暴Buff（+50攻击力，持续10秒）
- 对敌人：撕裂Buff（每秒10点伤害，持续5秒）
```

## 集成说明

### 自动集成

Buff系统已集成到：
- ✅ GameEntity
- ✅ EntityAttributes（属性计算）
- ✅ SkillSystem（技能释放时应用）
- ✅ Projectile（远程技能命中时应用）

### 使用流程

1. **创建Buff配置** - 使用ScriptableObject
2. **配置来源**：
   - 技能：在SkillData中配置buffToTarget/buffToSelf
   - 奖励：直接调用AddBuff
   - 其他：任何地方都可以调用AddBuff
3. **自动管理** - 系统自动更新、过期移除
4. **效果应用** - 自动计算属性、触发效果

## 扩展Buff系统

### 添加新的效果类型

1. 在 `BuffEffectType` 枚举中添加新类型
2. 在 `BuffData` 中添加对应参数
3. 在 `BuffInstance.OnTick()` 或 `OnApply()` 中实现效果逻辑

### 添加新的触发类型

1. 在 `PassiveTriggerType` 枚举中添加（复用）
2. 在 `BuffInstance.Update()` 中添加检测逻辑

## 注意事项

1. **属性Buff自动生效**：属性修改Buff会自动触发属性重算
2. **控制Buff待实现**：眩晕、沉默等需要AI和输入系统配合
3. **护盾待实现**：护盾需要独立的护盾管理系统
4. **性能考虑**：避免过多的周围检测Buff
5. **驱散机制**：通过canBeDispelled控制是否可被驱散

## 未来扩展

- 百分比属性修改
- 复杂的叠加规则
- Buff优先级排序
- Buff互斥机制
- Buff触发特效
- Buff UI显示

