using UnityEngine;
using SoulDefence.Entity;
using SoulDefence.Core;

namespace SoulDefence.Buff
{
    /// <summary>
    /// Buff实例
    /// 管理Buff的运行时状态
    /// </summary>
    public class BuffInstance
    {
        private BuffData buffData;              // Buff配置数据
        private GameEntity target;              // Buff目标
        private GameEntity caster;              // Buff施放者
        
        // 运行时状态
        private float remainingDuration;        // 剩余持续时间
        private int currentStacks;              // 当前层数
        private float tickTimer;                // Tick计时器
        private GameObject visualEffectInstance;// 视觉特效实例

        // 属性修改（用于移除时恢复）
        private float appliedAttributeValue;    // 已应用的属性值

        /// <summary>
        /// 构造函数
        /// </summary>
        public BuffInstance(BuffData data, GameEntity target, GameEntity caster = null)
        {
            this.buffData = data;
            this.target = target;
            this.caster = caster;
            this.remainingDuration = data.isPermanent ? float.MaxValue : data.duration;
            this.currentStacks = 1;
            this.tickTimer = data.tickInterval;
            this.appliedAttributeValue = 0f;

            // 应用初始效果
            OnApply();
        }

        /// <summary>
        /// 获取Buff数据
        /// </summary>
        public BuffData Data => buffData;

        /// <summary>
        /// 获取目标
        /// </summary>
        public GameEntity Target => target;

        /// <summary>
        /// 获取施放者
        /// </summary>
        public GameEntity Caster => caster;

        /// <summary>
        /// 获取剩余时间
        /// </summary>
        public float RemainingDuration => remainingDuration;

        /// <summary>
        /// 获取当前层数
        /// </summary>
        public int CurrentStacks => currentStacks;

        /// <summary>
        /// 是否已过期
        /// </summary>
        public bool IsExpired => !buffData.isPermanent && remainingDuration <= 0f;

        /// <summary>
        /// 应用Buff时调用
        /// </summary>
        private void OnApply()
        {
            if (target == null || !target.IsAlive)
                return;

            // 根据效果类型应用初始效果
            switch (buffData.effectType)
            {
                case BuffEffectType.AttributeModifier:
                    ApplyAttributeModifier();
                    break;

                case BuffEffectType.Shield:
                    // TODO: 护盾系统
                    Debug.Log($"[Buff] {buffData.buffName} 应用护盾 {buffData.shieldAmount} 到 {target.name}");
                    break;

                case BuffEffectType.Stun:
                    // TODO: 控制系统
                    Debug.Log($"[Buff] {buffData.buffName} 眩晕 {target.name}");
                    break;
            }

            Debug.Log($"[Buff] {buffData.buffName} 应用到 {target.name}，持续时间: {remainingDuration}秒");
        }

        /// <summary>
        /// 应用属性修改
        /// </summary>
        private void ApplyAttributeModifier()
        {
            // 注意：这里应该通过属性系统的Buff加成来实现
            // 目前只是记录，实际应用需要属性系统支持
            appliedAttributeValue = buffData.attributeValue * currentStacks;
            
            Debug.Log($"[Buff] {buffData.buffName} 属性修改: 类型={buffData.attributeType}, " +
                     $"值={appliedAttributeValue}, 百分比={buffData.isPercentage}");
            
            // 触发属性重算
            if (target.Attributes != null)
            {
                target.Attributes.RecalculateAttributes();
            }
        }

        /// <summary>
        /// 更新Buff
        /// </summary>
        public void Update(float deltaTime)
        {
            if (target == null || !target.IsAlive)
            {
                remainingDuration = 0f;
                return;
            }

            // 更新持续时间
            if (!buffData.isPermanent)
            {
                remainingDuration -= deltaTime;
            }

            // 更新Tick效果
            if (buffData.tickRate != BuffTickRate.None)
            {
                tickTimer -= deltaTime;
                if (tickTimer <= 0f)
                {
                    OnTick();
                    tickTimer = buffData.tickInterval;
                }
            }
        }

        /// <summary>
        /// Tick时触发
        /// </summary>
        private void OnTick()
        {
            if (target == null || !target.IsAlive)
                return;

            switch (buffData.effectType)
            {
                case BuffEffectType.DamageOverTime:
                    // 持续伤害
                    float damage = buffData.damagePerTick * currentStacks;
                    target.TakeDamage(damage);
                    Debug.Log($"[Buff] {buffData.buffName} DOT伤害 {damage} 到 {target.name}");
                    break;

                case BuffEffectType.HealOverTime:
                    // 持续治疗
                    float heal = buffData.damagePerTick * currentStacks;
                    target.Heal(heal);
                    Debug.Log($"[Buff] {buffData.buffName} HOT恢复 {heal} 到 {target.name}");
                    break;

                case BuffEffectType.AreaDamage:
                    // 周围伤害
                    ApplyAreaEffect(true);
                    break;

                case BuffEffectType.AreaHeal:
                    // 周围治疗
                    ApplyAreaEffect(false);
                    break;
            }
        }

        /// <summary>
        /// 应用周围效果
        /// </summary>
        private void ApplyAreaEffect(bool isDamage)
        {
            if (target == null)
                return;

            // 查找周围目标
            Collider[] colliders = Physics.OverlapSphere(target.transform.position, buffData.areaRadius);
            
            foreach (var collider in colliders)
            {
                var entity = collider.GetComponent<GameEntity>();
                if (entity != null && entity.IsAlive && entity != target)
                {
                    // 检查敌友关系
                    bool isEnemy = target.TeamSystem.IsHostile(entity.TeamSystem);
                    
                    if (isDamage && isEnemy)
                    {
                        entity.TakeDamage(buffData.areaEffectValue);
                        Debug.Log($"[Buff] {buffData.buffName} 周围伤害 {buffData.areaEffectValue} 到 {entity.name}");
                    }
                    else if (!isDamage && !isEnemy)
                    {
                        entity.Heal(buffData.areaEffectValue);
                        Debug.Log($"[Buff] {buffData.buffName} 周围治疗 {buffData.areaEffectValue} 到 {entity.name}");
                    }
                }
            }
        }

        /// <summary>
        /// 叠加Buff
        /// </summary>
        public void Stack()
        {
            switch (buffData.stackType)
            {
                case BuffStackType.Duration:
                    // 叠加持续时间
                    remainingDuration += buffData.duration;
                    Debug.Log($"[Buff] {buffData.buffName} 叠加时间，新持续时间: {remainingDuration}秒");
                    break;

                case BuffStackType.Count:
                    // 叠加层数
                    if (currentStacks < buffData.maxStacks)
                    {
                        currentStacks++;
                        Debug.Log($"[Buff] {buffData.buffName} 叠加层数: {currentStacks}/{buffData.maxStacks}");
                        
                        // 重新应用效果（如属性修改）
                        if (buffData.effectType == BuffEffectType.AttributeModifier)
                        {
                            ApplyAttributeModifier();
                        }
                        
                        // 检查是否达到阈值
                        if (buffData.effectType == BuffEffectType.StackCounter && 
                            currentStacks >= buffData.stackThreshold)
                        {
                            OnStackThresholdReached();
                        }
                    }
                    else
                    {
                        // 刷新持续时间
                        remainingDuration = buffData.isPermanent ? float.MaxValue : buffData.duration;
                    }
                    break;

                case BuffStackType.None:
                    // 刷新持续时间
                    remainingDuration = buffData.isPermanent ? float.MaxValue : buffData.duration;
                    Debug.Log($"[Buff] {buffData.buffName} 刷新时间");
                    break;
            }
        }

        /// <summary>
        /// 达到层数阈值时触发
        /// </summary>
        private void OnStackThresholdReached()
        {
            Debug.Log($"[Buff] {buffData.buffName} 达到 {buffData.stackThreshold} 层，触发效果");
            
            // 造成效果
            if (target != null && target.IsAlive)
            {
                target.TakeDamage(buffData.stackEffectValue);
            }
            
            // 清除Buff
            remainingDuration = 0f;
        }

        /// <summary>
        /// 移除Buff时调用
        /// </summary>
        public void OnRemove()
        {
            // 移除属性修改
            if (buffData.effectType == BuffEffectType.AttributeModifier)
            {
                RemoveAttributeModifier();
            }

            // 移除视觉特效
            if (visualEffectInstance != null)
            {
                Object.Destroy(visualEffectInstance);
            }

            Debug.Log($"[Buff] {buffData.buffName} 从 {target.name} 移除");
        }

        /// <summary>
        /// 移除属性修改
        /// </summary>
        private void RemoveAttributeModifier()
        {
            // 触发属性重算
            if (target != null && target.Attributes != null)
            {
                target.Attributes.RecalculateAttributes();
            }
            
            appliedAttributeValue = 0f;
        }

        /// <summary>
        /// 获取属性修改值（供属性系统调用）
        /// </summary>
        public float GetAttributeModifierValue()
        {
            if (buffData.effectType == BuffEffectType.AttributeModifier)
            {
                return appliedAttributeValue;
            }
            return 0f;
        }

        /// <summary>
        /// 获取属性类型
        /// </summary>
        public AttributeType GetAttributeType()
        {
            return buffData.attributeType;
        }

        /// <summary>
        /// 是否是百分比
        /// </summary>
        public bool IsPercentage()
        {
            return buffData.isPercentage;
        }
    }
}

