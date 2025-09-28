using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using SoulDefence.Skill;

namespace SoulDefence.Entity
{
    /// <summary>
    /// 游戏实体主类
    /// 整合属性系统、移动系统、队伍系统和AI系统
    /// </summary>
    public class GameEntity : MonoBehaviour
    {
        [Header("实体系统")]
        [SerializeField] private EntityAttributes attributes = new EntityAttributes();
        [SerializeField] private EntityMovement movement = new EntityMovement();
        [SerializeField] private TeamSystem teamSystem = new TeamSystem();

        [Header("AI系统")]
        [SerializeField] private EntityType entityType = EntityType.Player;
        [SerializeField] private bool useAI = true;

        [Header("技能系统")]
        [SerializeField] private SkillData[] skills;        // 实体拥有的技能
        [SerializeField] private int defaultSkillIndex = 0; // 默认技能索引

        // AI控制器（根据实体类型动态创建）
        private EntityAI aiController;
        
        // 技能冷却时间
        private Dictionary<SkillData, float> skillCooldowns = new Dictionary<SkillData, float>();

        /// <summary>
        /// 实体类型枚举
        /// </summary>
        public enum EntityType
        {
            Player,
            Enemy,
            Castle
        }

        // Start is called before the first frame update
        void Start()
        {
            Debug.Log($"GameEntity {name} 开始初始化，类型: {entityType}");
            InitializeEntity();
        }

        // Update is called once per frame
        void Update()
        {
            UpdateEntity();
            UpdateSkillCooldowns();
        }

        /// <summary>
        /// 初始化实体
        /// </summary>
        private void InitializeEntity()
        {
            // 初始化属性系统
            attributes.Initialize();
            
            // 初始化移动系统
            movement.Initialize(transform, attributes);

            // 根据实体类型设置队伍
            SetTeamByEntityType();

            // 根据实体类型设置默认AI状态
            SetDefaultAIState();

            // 初始化AI系统
            InitializeAI();
            
            // 初始化技能冷却
            InitializeSkillCooldowns();
            
            Debug.Log($"GameEntity {name} 初始化完成，类型: {entityType}, 队伍: {teamSystem.Team}, AI启用: {useAI}");
        }

        /// <summary>
        /// 根据实体类型设置队伍
        /// </summary>
        private void SetTeamByEntityType()
        {
            switch (entityType)
            {
                case EntityType.Player:
                case EntityType.Castle:
                    teamSystem.Team = TeamSystem.TeamType.Player;
                    Debug.Log($"GameEntity {name} 设置为玩家队伍");
                    break;
                case EntityType.Enemy:
                    teamSystem.Team = TeamSystem.TeamType.Enemy;
                    Debug.Log($"GameEntity {name} 设置为敌人队伍");
                    break;
            }
        }

        /// <summary>
        /// 根据实体类型设置默认AI状态
        /// </summary>
        private void SetDefaultAIState()
        {
            // switch (entityType)
            // {
            //     case EntityType.Player:
            //         // 玩家AI默认关闭
            //         useAI = false;
            //         Debug.Log($"GameEntity {name} AI默认关闭");
            //         break;
            //     case EntityType.Enemy:
            //     case EntityType.Castle:
            //         // 敌人和城堡AI默认开启
            //         useAI = true;
            //         Debug.Log($"GameEntity {name} AI默认开启");
            //         break;
            // }
        }

        /// <summary>
        /// 初始化AI系统
        /// </summary>
        private void InitializeAI()
        {
            // 根据实体类型创建对应的AI控制器
            switch (entityType)
            {
                case EntityType.Player:
                    aiController = new PlayerAI();
                    Debug.Log($"GameEntity {name} 创建PlayerAI");
                    break;
                case EntityType.Enemy:
                    aiController = new EnemyAI();
                    Debug.Log($"GameEntity {name} 创建EnemyAI");
                    break;
                case EntityType.Castle:
                    aiController = new CastleAI();
                    Debug.Log($"GameEntity {name} 创建CastleAI");
                    break;
            }

            // 如果创建了AI控制器，初始化它
            if (aiController != null)
            {
                Debug.Log($"GameEntity {name} 初始化AI控制器");
                aiController.Initialize(transform, this, teamSystem);
                aiController.AIEnabled = useAI;
                Debug.Log($"GameEntity {name} AI控制器初始化完成，AI启用: {useAI}");
            }
            else
            {
                Debug.LogError($"GameEntity {name} AI控制器创建失败");
            }
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
        /// 更新实体
        /// </summary>
        private void UpdateEntity()
        {
            // 执行移动（移动方向由外部或AI设置）
            movement.Move();

            // 更新AI逻辑
            if (aiController != null)
            {
                if (useAI)
                {
                    aiController.UpdateAI();
                }
            }
            else if (entityType != EntityType.Player)
            {
                // 对于非玩家实体，如果AI控制器为空，记录错误
                Debug.LogError($"GameEntity {name} AI控制器为空，无法更新AI");
            }
        }

        /// <summary>
        /// 更新技能冷却
        /// </summary>
        private void UpdateSkillCooldowns()
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
        /// 使用指定技能
        /// </summary>
        /// <param name="skill">技能数据</param>
        /// <param name="targetPosition">目标位置</param>
        /// <returns>是否成功使用技能</returns>
        public bool UseSkill(SkillData skill, Vector3 targetPosition)
        {
            if (skill == null || !IsAlive)
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
            bool success = SkillSystem.Instance.UseSkill(this, skill, targetPosition, direction);
            
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
                
            skillCooldowns[skill] = skill.cooldown;
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

        #region 公共接口

        /// <summary>
        /// 获取实体属性
        /// </summary>
        public EntityAttributes Attributes => attributes;

        /// <summary>
        /// 获取移动系统
        /// </summary>
        public EntityMovement Movement => movement;

        /// <summary>
        /// 获取队伍系统
        /// </summary>
        public TeamSystem TeamSystem => teamSystem;

        /// <summary>
        /// 获取AI控制器
        /// </summary>
        public EntityAI AIController => aiController;

        /// <summary>
        /// 获取实体类型
        /// </summary>
        public EntityType Type => entityType;

        /// <summary>
        /// 设置移动速度（快捷方法）
        /// </summary>
        /// <param name="speed">移动速度</param>
        public void SetMoveSpeed(float speed)
        {
            attributes.MoveSpeed = speed;
        }

        /// <summary>
        /// 设置转向速度（快捷方法）
        /// </summary>
        /// <param name="speed">转向速度</param>
        public void SetRotationSpeed(float speed)
        {
            movement.RotationSpeed = speed;
        }

        /// <summary>
        /// 设置移动方向（快捷方法）
        /// </summary>
        /// <param name="direction">移动方向</param>
        public void SetMoveDirection(Vector3 direction)
        {
            movement.SetMoveDirection(direction);
        }

        /// <summary>
        /// 停止移动（快捷方法）
        /// </summary>
        public void StopMovement()
        {
            movement.StopMovement();
        }

        /// <summary>
        /// 造成伤害（快捷方法）
        /// </summary>
        /// <param name="damage">伤害值</param>
        /// <returns>实际造成的伤害</returns>
        public float TakeDamage(float damage)
        {
            return attributes.TakeDamage(damage);
        }

        /// <summary>
        /// 恢复生命值（快捷方法）
        /// </summary>
        /// <param name="healAmount">恢复量</param>
        /// <returns>实际恢复量</returns>
        public float Heal(float healAmount)
        {
            return attributes.Heal(healAmount);
        }

        /// <summary>
        /// 切换AI控制
        /// </summary>
        /// <param name="enabled">是否启用AI</param>
        public void ToggleAI(bool enabled)
        {
            useAI = enabled;
            Debug.Log($"GameEntity {name} 切换AI状态: {enabled}");
            
            if (aiController != null)
            {
                aiController.AIEnabled = enabled;
                
                // 对于玩家特殊处理
                if (entityType == EntityType.Player && aiController is PlayerAI playerAI)
                {
                    playerAI.ToggleAI(enabled);
                }
            }
            else
            {
                Debug.LogError($"GameEntity {name} AI控制器为空，无法切换AI状态");
            }
        }

        /// <summary>
        /// 是否存活
        /// </summary>
        public bool IsAlive => attributes.IsAlive;

        /// <summary>
        /// 是否正在移动
        /// </summary>
        public bool IsMoving => movement.IsMoving;

        #endregion
    }
} 