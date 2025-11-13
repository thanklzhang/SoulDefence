using UnityEngine;
using UnityEngine.Serialization;
using SoulDefence.Buff;

namespace SoulDefence.Skill
{
    /// <summary>
    /// 技能类型枚举 - 按攻击方式分类
    /// </summary>
    public enum SkillAttackType
    {
        Melee,      // 近战
        Ranged      // 远程
    }
    
    /// <summary>
    /// 技能效果类型 - 按效果分类
    /// </summary>
    public enum SkillEffectType
    {
        Damage,     // 伤害型：直接造成伤害
        Status,     // 状态型：添加Buff/Debuff
        Movement,   // 移动型：改变位置（冲刺、传送）
        Composite   // 复合型：组合多种效果
    }
    
    /// <summary>
    /// 技能范围类型
    /// </summary>
    public enum SkillRangeType
    {
        Single,     // 单体：单个目标
        Arc,        // 扇形：前方扇形范围
        Circle      // 圆形：周围圆形范围
    }

    /// <summary>
    /// 技能数据，使用ScriptableObject存储技能配置
    /// 按照设计文档重构，支持伤害型、状态型、移动型、复合型技能
    /// </summary>
    [CreateAssetMenu(fileName = "New Skill", menuName = "GameConfig/Skill")]
    public class SkillData : ScriptableObject
    {
        [Header("=== 基础参数 ===")]
        public int skillId;                        // 技能ID
        public string skillName = "新技能";        // 技能名称
        public SkillEffectType effectType;         // 效果类型（伤害/状态/移动/复合）
        public bool isBasicAttack = false;         // 是否为普通攻击
        
        [Header("=== 冷却参数 ===")]
        public float cooldown = 1f;                // 冷却时间(秒)
        [Tooltip("前摇时间（准备阶段）")]
        public float castTime = 0f;                // 前摇时间
        [Tooltip("后摇时间（恢复阶段）")]
        public float recoveryTime = 0f;            // 后摇时间
        
        [Header("=== 伤害参数 ===")]
        public SkillAttackType attackType;         // 攻击方式（近战/远程）
        public SkillRangeType rangeType;           // 范围类型（单体/扇形/圆形）
        public float damage = 10f;                 // 伤害值（基础值）
        [Tooltip("范围大小（半径/角度）")]
        public float rangeSize = 3f;               // 范围大小（半径或角度）
        public int targetCount = 1;                // 目标数量
        
        [Header("=== 特殊伤害计算 ===")]
        [Tooltip("攻击力系数（伤害 = 攻击力 * 系数）")]
        public float attackPowerRatio = 0f;        // 攻击力系数
        [Tooltip("目标生命值百分比伤害（0-1）")]
        public float targetHealthRatio = 0f;       // 目标生命值百分比
        [Tooltip("自身生命值百分比伤害（0-1）")]
        public float selfHealthRatio = 0f;         // 自身生命值百分比
        
        [Header("=== 吸血效果 ===")]
        [Tooltip("吸血比例（0-1，如0.2=20%吸血）")]
        public float lifeStealRatio = 0f;          // 吸血比例
        
        [Header("=== 状态参数（Buff/Debuff） ===")]
        [Tooltip("对目标添加的Buff")]
        public BuffData buffToTarget;              // 对目标的Buff
        [Tooltip("对自己添加的Buff")]
        public BuffData buffToSelf;                // 对自己的Buff
        
        [Header("=== 移动参数 ===")]
        [Tooltip("移动距离")]
        public float movementDistance = 5f;        // 移动距离
        [Tooltip("移动时间")]
        public float movementDuration = 0.3f;      // 移动时间
        [Tooltip("是否是传送（true=传送，false=冲刺）")]
        public bool isTeleport = false;            // 是否是传送
        
        [Header("=== 远程特有参数 ===")]
        public GameObject projectilePrefab;        // 投掷物预制体
        public float projectileSpeed = 15f;        // 投掷物速度
        public float projectileLifetime = 3f;      // 投掷物存活时间
        public bool canThrough = false;            // 是否可以穿透
        public bool isFollow = false;              // 是否跟踪目标
        
        [Header("=== 被动技能配置 ===")]
        public PassiveSkillData passiveSkill = new PassiveSkillData();  // 被动技能
        
        // ========== 属性方法 ==========
        
        /// <summary>
        /// 是否有被动技能
        /// </summary>
        public bool HasPassiveSkill => passiveSkill != null && passiveSkill.enabled;
        
        /// <summary>
        /// 是否有Buff效果
        /// </summary>
        public bool HasBuffEffect => buffToTarget != null || buffToSelf != null;
        
        /// <summary>
        /// 是否是伤害型技能
        /// </summary>
        public bool IsDamageSkill => effectType == SkillEffectType.Damage || effectType == SkillEffectType.Composite;
        
        /// <summary>
        /// 是否是状态型技能
        /// </summary>
        public bool IsStatusSkill => effectType == SkillEffectType.Status || effectType == SkillEffectType.Composite;
        
        /// <summary>
        /// 是否是移动型技能
        /// </summary>
        public bool IsMovementSkill => effectType == SkillEffectType.Movement || effectType == SkillEffectType.Composite;
    }
} 