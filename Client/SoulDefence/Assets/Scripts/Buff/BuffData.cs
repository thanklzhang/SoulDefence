using UnityEngine;
using SoulDefence.Core;

namespace SoulDefence.Buff
{
    /// <summary>
    /// Buff配置数据
    /// </summary>
    [CreateAssetMenu(fileName = "New Buff", menuName = "GameConfig/Buff")]
    public class BuffData : ScriptableObject
    {
        [Header("基础信息")]
        public int buffId;                      // Buff ID
        public string buffName;                 // Buff名称
        public BuffType buffType;               // Buff类型（正面/负面/中性）
        [TextArea(2, 4)]
        public string description;              // Buff描述

        [Header("持续时间")]
        public float duration = 5f;             // 持续时间（秒），0=永久
        public bool isPermanent = false;        // 是否永久

        [Header("叠加配置")]
        public BuffStackType stackType;         // 叠加类型
        public int maxStacks = 1;               // 最大叠加层数

        [Header("效果配置")]
        public BuffEffectType effectType;       // 效果类型
        public BuffTickRate tickRate;           // 更新频率

        [Header("效果参数 - 属性修改")]
        public AttributeType attributeType = AttributeType.AttackPower;  // 属性类型
        public float attributeValue = 0f;       // 属性修改值
        public bool isPercentage = false;       // 是否百分比

        [Header("效果参数 - 持续伤害/恢复")]
        public float damagePerTick = 10f;       // 每次Tick的伤害/恢复
        public float tickInterval = 1f;         // Tick间隔（秒）

        [Header("效果参数 - 周围效果")]
        public float areaRadius = 5f;           // 影响半径
        public float areaEffectValue = 10f;     // 每次效果值

        [Header("效果参数 - 计数器")]
        public int stackThreshold = 3;          // 触发阈值（满多少层）
        public float stackEffectValue = 50f;    // 触发时的效果值

        [Header("效果参数 - 控制")]
        public float slowPercent = 0.5f;        // 减速百分比（0-1）

        [Header("效果参数 - 护盾")]
        public float shieldAmount = 100f;       // 护盾值

        [Header("视觉效果")]
        public GameObject visualEffect;         // 视觉特效预制体
        public Color buffColor = Color.white;   // Buff颜色标识

        [Header("其他")]
        public bool canBeDispelled = true;      // 是否可以被驱散
        public int priority = 0;                // 优先级（用于排序显示）
    }
}

