# Buff系统快速开始

## 快速配置

### 1. 创建Buff配置

右键 -> Create -> GameConfig -> Buff

### 2. 常用Buff配置模板

#### 模板1：持续伤害（中毒、燃烧等）

```
buffName: "中毒"
buffType: Negative
description: "每秒受到10点毒素伤害"

duration: 5
isPermanent: false

stackType: Count  (可叠层)
maxStacks: 5

effectType: DamageOverTime
tickRate: PerSecond

damagePerTick: 10
tickInterval: 1
```

#### 模板2：持续治疗

```
buffName: "再生"
buffType: Positive
description: "每2秒恢复20点生命"

duration: 10
isPermanent: false

stackType: Independent
maxStacks: 99

effectType: HealOverTime
tickRate: PerSecond

damagePerTick: 20
tickInterval: 2
```

#### 模板3：属性加成

```
buffName: "力量祝福"
buffType: Positive
description: "攻击力+50"

duration: 10
isPermanent: false

stackType: None  (不叠加，刷新时间)
maxStacks: 1

effectType: AttributeModifier
tickRate: None

attributeType: AttackPower
attributeValue: 50
isPercentage: false
```

#### 模板4：周围光环伤害

```
buffName: "火焰光环"
buffType: Positive
description: "每秒对周围5米敌人造成15点伤害"

duration: 8
isPermanent: false

stackType: None
maxStacks: 1

effectType: AreaDamage
tickRate: PerSecond

areaRadius: 5
areaEffectValue: 15
tickInterval: 1
```

#### 模板5：计数器爆发

```
buffName: "雷霆印记"
buffType: Negative
description: "每次被攻击叠加1层，满3层造成100点伤害并清除"

duration: 10
isPermanent: false

stackType: Count
maxStacks: 3

effectType: StackCounter

stackThreshold: 3
stackEffectValue: 100
```

#### 模板6：减速

```
buffName: "寒冰缓速"
buffType: Negative
description: "移动速度降低50%"

duration: 3
isPermanent: false

stackType: None
maxStacks: 1

effectType: Slow
tickRate: None

slowPercent: 0.5
```

#### 模板7：眩晕

```
buffName: "眩晕"
buffType: Negative
description: "无法移动和攻击"

duration: 2
isPermanent: false

stackType: None
maxStacks: 1

effectType: Stun
tickRate: None

canBeDispelled: true
```

## 使用场景

### 场景1：技能附带Buff

**火球术**：造成伤害 + 燃烧DOT

在 SkillData 中：
```
主动技能参数：
  damage: 50
  skillType: Ranged

Buff配置：
  buffToTarget: 燃烧Buff (每秒5点伤害，持续5秒)
  buffToSelf: null
```

### 场景2：奖励系统给Buff

```csharp
// 玩家击杀敌人后，获得力量Buff
public void OnEnemyKilled(GameEntity player)
{
    BuffData strengthBuff = Resources.Load<BuffData>("Buffs/Strength");
    player.AddBuff(strengthBuff);
}
```

### 场景3：道具使用给Buff

```csharp
// 使用药水获得恢复Buff
public void UsePotionItem()
{
    BuffData regenBuff = Resources.Load<BuffData>("Buffs/Regeneration");
    player.AddBuff(regenBuff);
}
```

### 场景4：环境效果

```csharp
// 进入毒雾区域
void OnEnterPoisonZone()
{
    entity.AddBuff(poisonBuff);
}

// 离开毒雾区域
void OnExitPoisonZone()
{
    entity.RemoveBuff(poisonBuff);
}
```

## 叠加机制详解

### None（不叠加）
```
第1次添加：duration = 5秒
第2次添加：duration = 5秒（刷新）
第3次添加：duration = 5秒（刷新）
结果：始终只有1个Buff，每次刷新时间
```

### Duration（叠加时间）
```
第1次添加：duration = 5秒
第2次添加：duration = 5+5 = 10秒
第3次添加：duration = 10+5 = 15秒
结果：时间不断累加
```

### Count（叠加层数）
```
第1次添加：stacks = 1, duration = 5秒
第2次添加：stacks = 2, duration = 5秒（刷新）
第3次添加：stacks = 3, duration = 5秒（刷新）
到达maxStacks后：刷新时间，不再增加层数
结果：层数叠加，时间刷新，效果随层数增强
```

### Independent（独立叠加）
```
第1次添加：Buff实例1，duration = 5秒
第2次添加：Buff实例2，duration = 5秒
第3次添加：Buff实例3，duration = 5秒
结果：多个独立的Buff实例，各自计时
```

## 属性类型对照表

attributeType参数：
- 0 = 攻击力 (AttackPower)
- 1 = 防御力 (Defense)
- 2 = 攻击速度 (AttackSpeed)
- 3 = 移动速度 (MoveSpeed)

## 调试技巧

### 查看当前Buff

```csharp
var buffs = entity.BuffSystem.GetAllBuffs();
foreach (var buff in buffs)
{
    Debug.Log($"Buff: {buff.Data.buffName}, 剩余: {buff.RemainingDuration}秒, 层数: {buff.CurrentStacks}");
}
```

### 测试Buff效果

```csharp
[ContextMenu("测试/添加中毒Buff")]
void TestAddPoisonBuff()
{
    BuffData poison = Resources.Load<BuffData>("Buffs/Poison");
    entity.AddBuff(poison);
}

[ContextMenu("测试/清除所有Buff")]
void TestClearBuffs()
{
    entity.BuffSystem.ClearAllBuffs();
}
```

## 常见问题

**Q: Buff不生效？**
- 检查buffType和effectType是否正确
- 检查tickRate是否设置（DOT/HOT需要）
- 查看控制台日志

**Q: 属性Buff不增加属性？**
- 确保effectType是AttributeModifier
- 确保attributeType正确（0-3）
- 检查属性是否重算

**Q: DOT不造成伤害？**
- 确保effectType是DamageOverTime
- 确保tickRate不是None
- 确保tickInterval > 0

**Q: 计数器不触发？**
- 使用stackType: Count
- 确保effectType是StackCounter
- 检查stackThreshold设置

**Q: Buff层数不叠加？**
- 检查stackType是否是Count
- 检查是否已达到maxStacks

## 最佳实践

1. **命名规范**：Buff名称简洁明了
2. **合理分类**：正确设置buffType
3. **可驱散性**：重要Buff设置canBeDispelled = false
4. **叠加设计**：根据游戏需要选择合适的叠加类型
5. **数值平衡**：避免过强的Buff
6. **视觉反馈**：设置visualEffect和buffColor
7. **性能优化**：避免过多PerFrame的Buff

## 配置检查清单

创建Buff时检查：
- ✅ 填写buffName和description
- ✅ 设置正确的buffType
- ✅ 设置duration（0或勾选isPermanent表示永久）
- ✅ 选择合适的stackType
- ✅ 选择正确的effectType
- ✅ 配置效果类型对应的参数
- ✅ 设置tickRate（如果效果需要Tick）
- ✅ 测试Buff效果

