using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SoulDefence.Entity;
using SoulDefence.Core;

namespace SoulDefence.Buff
{
    /// <summary>
    /// Buff系统
    /// 管理实体的所有Buff
    /// </summary>
    [System.Serializable]
    public class BuffSystem
    {
        private GameEntity owner;                           // Buff所有者
        private List<BuffInstance> activeBuffs;            // 活跃的Buff列表

        /// <summary>
        /// 初始化Buff系统
        /// </summary>
        public void Initialize(GameEntity entity)
        {
            owner = entity;
            activeBuffs = new List<BuffInstance>();
            
            Debug.Log($"[BuffSystem] {entity.name} Buff系统已初始化");
        }

        /// <summary>
        /// 更新Buff系统
        /// </summary>
        public void Update(float deltaTime)
        {
            if (activeBuffs == null || activeBuffs.Count == 0)
                return;

            // 更新所有Buff
            for (int i = activeBuffs.Count - 1; i >= 0; i--)
            {
                var buff = activeBuffs[i];
                buff.Update(deltaTime);

                // 移除过期的Buff
                if (buff.IsExpired)
                {
                    RemoveBuff(i);
                }
            }
        }

        /// <summary>
        /// 添加Buff
        /// </summary>
        public BuffInstance AddBuff(BuffData buffData, GameEntity caster = null)
        {
            if (buffData == null || owner == null || !owner.IsAlive)
                return null;

            // 检查是否已存在相同Buff
            var existingBuff = FindBuff(buffData);
            
            if (existingBuff != null)
            {
                // 根据叠加类型处理
                switch (buffData.stackType)
                {
                    case BuffStackType.None:
                        // 不可叠加，刷新时间
                        existingBuff.Stack();
                        return existingBuff;

                    case BuffStackType.Duration:
                    case BuffStackType.Count:
                        // 可叠加
                        existingBuff.Stack();
                        return existingBuff;

                    case BuffStackType.Independent:
                        // 独立叠加，创建新实例
                        break;
                }
            }

            // 创建新Buff实例
            var newBuff = new BuffInstance(buffData, owner, caster);
            activeBuffs.Add(newBuff);

            Debug.Log($"[BuffSystem] {owner.name} 获得Buff: {buffData.buffName}");

            // 触发属性重算（如果需要）
            if (buffData.effectType == BuffEffectType.AttributeModifier && owner.Attributes != null)
            {
                owner.Attributes.RecalculateAttributes();
            }

            return newBuff;
        }

        /// <summary>
        /// 移除Buff（通过索引）
        /// </summary>
        private void RemoveBuff(int index)
        {
            if (index < 0 || index >= activeBuffs.Count)
                return;

            var buff = activeBuffs[index];
            buff.OnRemove();
            activeBuffs.RemoveAt(index);

            // 触发属性重算
            if (owner != null && owner.Attributes != null)
            {
                owner.Attributes.RecalculateAttributes();
            }
        }

        /// <summary>
        /// 移除Buff（通过BuffData）
        /// </summary>
        public bool RemoveBuff(BuffData buffData)
        {
            for (int i = activeBuffs.Count - 1; i >= 0; i--)
            {
                if (activeBuffs[i].Data == buffData)
                {
                    RemoveBuff(i);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 移除所有指定类型的Buff
        /// </summary>
        public int RemoveBuffsByType(BuffType buffType)
        {
            int count = 0;
            for (int i = activeBuffs.Count - 1; i >= 0; i--)
            {
                if (activeBuffs[i].Data.buffType == buffType)
                {
                    RemoveBuff(i);
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// 驱散Buff（移除可驱散的负面Buff）
        /// </summary>
        public int DispelBuffs(int count = 1)
        {
            int dispelled = 0;
            for (int i = activeBuffs.Count - 1; i >= 0 && dispelled < count; i--)
            {
                var buff = activeBuffs[i];
                if (buff.Data.canBeDispelled && buff.Data.buffType == BuffType.Negative)
                {
                    RemoveBuff(i);
                    dispelled++;
                }
            }
            return dispelled;
        }

        /// <summary>
        /// 清除所有Buff
        /// </summary>
        public void ClearAllBuffs()
        {
            for (int i = activeBuffs.Count - 1; i >= 0; i--)
            {
                RemoveBuff(i);
            }
            activeBuffs.Clear();
        }

        /// <summary>
        /// 查找Buff
        /// </summary>
        public BuffInstance FindBuff(BuffData buffData)
        {
            return activeBuffs.FirstOrDefault(b => b.Data == buffData);
        }

        /// <summary>
        /// 查找Buff（通过名称）
        /// </summary>
        public BuffInstance FindBuffByName(string buffName)
        {
            return activeBuffs.FirstOrDefault(b => b.Data.buffName == buffName);
        }

        /// <summary>
        /// 是否拥有指定Buff
        /// </summary>
        public bool HasBuff(BuffData buffData)
        {
            return FindBuff(buffData) != null;
        }

        /// <summary>
        /// 是否拥有指定类型的Buff
        /// </summary>
        public bool HasBuffType(BuffType buffType)
        {
            return activeBuffs.Any(b => b.Data.buffType == buffType);
        }

        /// <summary>
        /// 获取所有Buff
        /// </summary>
        public List<BuffInstance> GetAllBuffs()
        {
            return new List<BuffInstance>(activeBuffs);
        }

        /// <summary>
        /// 获取指定类型的Buff
        /// </summary>
        public List<BuffInstance> GetBuffsByType(BuffType buffType)
        {
            return activeBuffs.Where(b => b.Data.buffType == buffType).ToList();
        }

        /// <summary>
        /// 获取Buff数量
        /// </summary>
        public int GetBuffCount()
        {
            return activeBuffs != null ? activeBuffs.Count : 0;
        }

        /// <summary>
        /// 获取所有属性修改Buff的加成
        /// </summary>
        public Dictionary<AttributeType, float> GetAttributeModifiers()
        {
            var modifiers = new Dictionary<AttributeType, float>();

            foreach (var buff in activeBuffs)
            {
                if (buff.Data.effectType == BuffEffectType.AttributeModifier)
                {
                    AttributeType attrType = buff.GetAttributeType();
                    float value = buff.GetAttributeModifierValue();

                    if (modifiers.ContainsKey(attrType))
                    {
                        modifiers[attrType] += value;
                    }
                    else
                    {
                        modifiers[attrType] = value;
                    }
                }
            }

            return modifiers;
        }

        /// <summary>
        /// 是否被眩晕
        /// </summary>
        public bool IsStunned()
        {
            return activeBuffs.Any(b => b.Data.effectType == BuffEffectType.Stun);
        }

        /// <summary>
        /// 是否被沉默
        /// </summary>
        public bool IsSilenced()
        {
            return activeBuffs.Any(b => b.Data.effectType == BuffEffectType.Silence);
        }

        /// <summary>
        /// 获取移动速度修正（减速效果）
        /// </summary>
        public float GetMovementSpeedModifier()
        {
            float modifier = 1f;
            foreach (var buff in activeBuffs)
            {
                if (buff.Data.effectType == BuffEffectType.Slow)
                {
                    modifier *= (1f - buff.Data.slowPercent);
                }
            }
            return modifier;
        }
    }
}

