using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using SoulDefence.Skill;

namespace SoulDefence.Entity
{
    /// <summary>
    /// 实体技能系统
    /// 管理实体的技能、冷却和使用
    /// </summary>
    [System.Serializable]
    public class EntitySkillSystem
    {
        [SerializeField] private SkillData[] skills;        // 实体拥有的技能
        [SerializeField] private int defaultSkillIndex = 0; // 默认技能索引

        // 技能冷却时间
        private Dictionary<SkillData, float> skillCooldowns = new Dictionary<SkillData, float>();
        
        // 实体引用
        private GameEntity owner;
        private Transform transform;

        /// <summary>
        /// 初始化技能系统
        /// </summary>
        public void Initialize(GameEntity owner)
        {
            this.owner = owner;
            this.transform = owner.transform;
            InitializeSkillCooldowns();
        }
        
        /// <summary>
        /// 设置技能数据
        /// </summary>
        /// <param name="skillsData">技能数据数组</param>
        /// <param name="defaultIndex">默认技能索引</param>
        public void SetSkills(SkillData[] skillsData, int defaultIndex = 0)
        {
            this.skills = skillsData;
            this.defaultSkillIndex = Mathf.Clamp(defaultIndex, 0, skillsData != null ? skillsData.Length - 1 : 0);
            InitializeSkillCooldowns();
        }

        /// <summary>
        /// 初始化技能冷却
        /// </summary>
        private void InitializeSkillCooldowns()
        {
            if (skills == null)
                return;
                
            skillCooldowns.Clear();
            foreach (var skill in skills)
            {
                if (skill != null)
                {
                    skillCooldowns[skill] = 0f;
                }
            }
        }

        /// <summary>
        /// 更新技能冷却
        /// </summary>
        public void UpdateCooldowns()
        {
            if (skillCooldowns.Count == 0)
                return;
                
            foreach (var skill in skillCooldowns.Keys.ToArray())
            {
                if (skillCooldowns[skill] > 0)
                {
                    skillCooldowns[skill] -= Time.deltaTime;
                    if (skillCooldowns[skill] < 0)
                    {
                        skillCooldowns[skill] = 0;
                    }
                }
            }
        }
        
        /// <summary>
        /// 使用默认技能
        /// </summary>
        /// <param name="targetPosition">目标位置</param>
        /// <returns>是否成功使用技能</returns>
        public bool UseDefaultSkill(Vector3 targetPosition)
        {
            if (skills == null || skills.Length == 0 || defaultSkillIndex < 0 || defaultSkillIndex >= skills.Length)
                return false;
                
            SkillData skill = skills[defaultSkillIndex];
            return UseSkill(skill, targetPosition);
        }

        /// <summary>
        /// 使用默认技能（使用实体朝向）
        /// </summary>
        /// <returns>是否成功使用技能</returns>
        public bool UseDefaultSkill()
        {
            if (skills == null || skills.Length == 0 || defaultSkillIndex < 0 || defaultSkillIndex >= skills.Length)
                return false;
                
            SkillData skill = skills[defaultSkillIndex];
            return UseSkill(skill);
        }

        /// <summary>
        /// 使用指定索引的技能
        /// </summary>
        /// <param name="skillIndex">技能索引</param>
        /// <param name="targetPosition">目标位置</param>
        /// <returns>是否成功使用技能</returns>
        public bool UseSkill(int skillIndex, Vector3 targetPosition)
        {
            if (skills == null || skillIndex < 0 || skillIndex >= skills.Length)
                return false;
                
            SkillData skill = skills[skillIndex];
            return UseSkill(skill, targetPosition);
        }

        /// <summary>
        /// 使用指定索引的技能（使用实体朝向）
        /// </summary>
        /// <param name="skillIndex">技能索引</param>
        /// <returns>是否成功使用技能</returns>
        public bool UseSkill(int skillIndex)
        {
            if (skills == null || skillIndex < 0 || skillIndex >= skills.Length)
                return false;
                
            SkillData skill = skills[skillIndex];
            return UseSkill(skill);
        }

        /// <summary>
        /// 使用指定技能
        /// </summary>
        /// <param name="skill">技能数据</param>
        /// <param name="targetPosition">目标位置</param>
        /// <returns>是否成功使用技能</returns>
        public bool UseSkill(SkillData skill, Vector3 targetPosition)
        {
            if (skill == null || !owner.IsAlive)
                return false;
                
            // 检查冷却
            if (IsSkillOnCooldown(skill))
                return false;
                
            // 计算方向
            Vector3 direction;
            if (targetPosition != transform.position)
            {
                direction = (targetPosition - transform.position).normalized;
            }
            else
            {
                direction = transform.forward;
            }
            
            // 使用技能
            bool success = SkillSystem.Instance.UseSkill(owner, skill, targetPosition, direction);
            
            // 如果成功使用，设置冷却
            if (success)
            {
                SetSkillCooldown(skill);
            }
            
            return success;
        }

        /// <summary>
        /// 使用指定技能（使用实体朝向）
        /// </summary>
        /// <param name="skill">技能数据</param>
        /// <returns>是否成功使用技能</returns>
        public bool UseSkill(SkillData skill)
        {
            if (skill == null || !owner.IsAlive)
                return false;
                
            // 检查冷却
            if (IsSkillOnCooldown(skill))
                return false;
                
            // 使用实体当前朝向
            Vector3 direction = transform.forward;
            
            // 计算目标位置（在实体前方一定距离）
            Vector3 targetPosition = transform.position + direction * 10f;
            
            // 使用技能
            bool success = SkillSystem.Instance.UseSkill(owner, skill, targetPosition, direction);
            
            // 如果成功使用，设置冷却
            if (success)
            {
                SetSkillCooldown(skill);
            }
            
            return success;
        }
        
        /// <summary>
        /// 检查技能是否在冷却中
        /// </summary>
        public bool IsSkillOnCooldown(SkillData skill)
        {
            if (skill == null)
                return true;
                
            if (!skillCooldowns.ContainsKey(skill))
            {
                skillCooldowns[skill] = 0f;
            }
            
            return skillCooldowns[skill] > 0;
        }
        
        /// <summary>
        /// 设置技能冷却
        /// </summary>
        private void SetSkillCooldown(SkillData skill)
        {
            if (skill == null)
                return;
                
            if (skill.isBasicAttack && owner != null && owner.Attributes != null)
            {
                // 如果是普通攻击，使用实体的攻击速度计算冷却时间
                // 攻击速度越高，冷却时间越短
                float attackSpeed = owner.Attributes.AttackSpeed;
                if (attackSpeed <= 0)
                    attackSpeed = 1f; // 防止除以零
                
                skillCooldowns[skill] = 1.0f / attackSpeed;
            }
            else
            {
                // 非普通攻击，使用技能自身的冷却时间
                skillCooldowns[skill] = skill.cooldown;
            }
        }
        
        /// <summary>
        /// 获取技能冷却剩余时间
        /// </summary>
        public float GetSkillCooldown(SkillData skill)
        {
            if (skill == null || !skillCooldowns.ContainsKey(skill))
                return 0f;
                
            return skillCooldowns[skill];
        }
        
        /// <summary>
        /// 获取技能的实际冷却时间（考虑攻击速度）
        /// </summary>
        /// <param name="skill">技能数据</param>
        /// <returns>实际冷却时间</returns>
        public float GetActualCooldown(SkillData skill)
        {
            if (skill == null)
                return 0f;
                
            if (skill.isBasicAttack && owner != null && owner.Attributes != null)
            {
                // 如果是普通攻击，使用实体的攻击速度计算冷却时间
                float attackSpeed = owner.Attributes.AttackSpeed;
                if (attackSpeed <= 0)
                    attackSpeed = 1f; // 防止除以零
                
                return 1.0f / attackSpeed;
            }
            else
            {
                // 非普通攻击，使用技能自身的冷却时间
                return skill.cooldown;
            }
        }
        
        /// <summary>
        /// 获取实体拥有的所有技能
        /// </summary>
        public SkillData[] GetSkills()
        {
            return skills;
        }
        
        /// <summary>
        /// 获取默认技能
        /// </summary>
        public SkillData GetDefaultSkill()
        {
            if (skills == null || skills.Length == 0 || defaultSkillIndex < 0 || defaultSkillIndex >= skills.Length)
                return null;
                
            return skills[defaultSkillIndex];
        }
        
        /// <summary>
        /// 设置默认技能索引
        /// </summary>
        public void SetDefaultSkillIndex(int index)
        {
            if (skills != null && index >= 0 && index < skills.Length)
            {
                defaultSkillIndex = index;
            }
        }
    }
} 