using UnityEngine;
using UnityEngine.Serialization;
using SoulDefence.Buff;

namespace SoulDefence.Skill
{
    /// <summary>
    /// 技能类型枚举
    /// </summary>
    public enum SkillType
    {
        Melee,      // 近战
        Ranged      // 远程
    }

    /// <summary>
    /// 技能数据，使用ScriptableObject存储技能配置
    /// 支持主动技能和被动技能
    /// </summary>
    [CreateAssetMenu(fileName = "New Skill", menuName = "GameConfig/Skill")]
    public class SkillData : ScriptableObject
    {
        [Header("基础信息")]
        public int skillId;                    // 技能ID
        public string skillName;               // 技能名称
        public SkillType skillType;            // 技能类型
        public bool isBasicAttack = false;     // 是否为普通攻击
        
        [Header("主动技能参数")]
        public bool hasActiveSkill = true;     // 是否有主动技能
        public float damage;                   // 伤害值
        public float cooldown;                 // 冷却时间(秒)
        public int targetCount = 1;            // 目标数量
        
        [Header("近战特有参数")]
        public float arcAngle = 60f;           // 扇形角度(度)
        
        [Header("远程特有参数")]
        public float projectileSpeed = 15f;    // 投掷物速度
        public float projectileLifetime = 3f;  // 投掷物存活时间
        public bool canThrough = false;         // 是否可以穿透
        public bool isFollow = false;          // 是否跟踪目标
        
        [Header("被动技能配置")]
        public PassiveSkillData passiveSkill = new PassiveSkillData();  // 被动技能
        
        [Header("Buff配置")]
        [Tooltip("技能释放时对目标添加的Buff")]
        public BuffData buffToTarget;           // 对目标添加的Buff
        [Tooltip("技能释放时对自己添加的Buff")]
        public BuffData buffToSelf;             // 对自己添加的Buff
        
        [Header("资源")]
        public GameObject projectilePrefab;    // 投掷物预制体(仅远程技能使用)
        
        /// <summary>
        /// 是否有被动技能
        /// </summary>
        public bool HasPassiveSkill => passiveSkill != null && passiveSkill.enabled;
        
        /// <summary>
        /// 是否是纯被动技能（没有主动部分）
        /// </summary>
        public bool IsPurePassive => !hasActiveSkill && HasPassiveSkill;
        
        /// <summary>
        /// 是否有Buff效果
        /// </summary>
        public bool HasBuffEffect => buffToTarget != null || buffToSelf != null;
    }
} 