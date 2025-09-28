using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using SoulDefence.Skill;
// 添加组件引用
using SoulDefence.Entity;

namespace SoulDefence.Entity
{
    /// <summary>
    /// 游戏实体主类
    /// 整合属性系统、移动系统、队伍系统、技能系统和AI系统
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class GameEntity : MonoBehaviour
    {
        [Header("实体系统")]
        [SerializeField] private EntityAttributes attributes = new EntityAttributes();
        [SerializeField] private EntityMovement movement = new EntityMovement();
        [SerializeField] private TeamSystem teamSystem = new TeamSystem();
        [SerializeField] private EntitySkillSystem skillSystem = new EntitySkillSystem();
        [SerializeField] private EntityEffectSystem effectSystem = new EntityEffectSystem();
        
        [Header("AI系统")]
        [SerializeField] private EntityType entityType = EntityType.Player;
        [SerializeField] private bool useAI = true;
        
        // AI控制器（根据实体类型动态创建）
        private EntityAI aiController;
        
        /// <summary>
        /// 实体类型枚举
        /// </summary>
        public enum EntityType
        {
            Player,
            Enemy,
            Castle
        }
        
        private void Awake()
        {
            // 确保有碰撞器
            SetupCollider();
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
            skillSystem.UpdateCooldowns();
        }

        /// <summary>
        /// 确保有碰撞器
        /// </summary>
        private void SetupCollider()
        {
            Collider collider = GetComponent<Collider>();
            if (collider == null)
            {
                // 添加一个基本的胶囊碰撞器
                CapsuleCollider capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
                capsuleCollider.height = 2f;
                capsuleCollider.radius = 0.5f;
                capsuleCollider.center = new Vector3(0, 1f, 0);
                
                // 设置为触发器
                capsuleCollider.isTrigger = true;
                
                Debug.LogWarning($"实体 {gameObject.name} 缺少碰撞器，已自动添加胶囊碰撞器");
            }
            else if (!collider.isTrigger)
            {
                // 确保是触发器
                collider.isTrigger = true;
                Debug.Log($"实体 {gameObject.name} 的碰撞器已设置为触发器");
            }
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
            
            // 初始化技能系统
            skillSystem.Initialize(this);
            
            // 初始化特效系统
            effectSystem.Initialize(this);
            
            // 根据实体类型设置队伍
            SetTeamByEntityType();

            // 初始化AI系统
            InitializeAI();
            
            Debug.Log($"GameEntity {name} 初始化完成，类型: {entityType}, 队伍: {teamSystem.Team}, AI启用: {useAI}");
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
        
        #region 技能系统转发方法
        
        /// <summary>
        /// 使用默认技能
        /// </summary>
        /// <param name="targetPosition">目标位置</param>
        /// <returns>是否成功使用技能</returns>
        public bool UseDefaultSkill(Vector3 targetPosition)
        {
            return skillSystem.UseDefaultSkill(targetPosition);
        }

        /// <summary>
        /// 使用默认技能（使用实体朝向）
        /// </summary>
        /// <returns>是否成功使用技能</returns>
        public bool UseDefaultSkill()
        {
            return skillSystem.UseDefaultSkill();
        }

        /// <summary>
        /// 使用指定索引的技能
        /// </summary>
        /// <param name="skillIndex">技能索引</param>
        /// <param name="targetPosition">目标位置</param>
        /// <returns>是否成功使用技能</returns>
        public bool UseSkill(int skillIndex, Vector3 targetPosition)
        {
            return skillSystem.UseSkill(skillIndex, targetPosition);
        }

        /// <summary>
        /// 使用指定索引的技能（使用实体朝向）
        /// </summary>
        /// <param name="skillIndex">技能索引</param>
        /// <returns>是否成功使用技能</returns>
        public bool UseSkill(int skillIndex)
        {
            return skillSystem.UseSkill(skillIndex);
        }

        /// <summary>
        /// 使用指定技能
        /// </summary>
        /// <param name="skill">技能数据</param>
        /// <param name="targetPosition">目标位置</param>
        /// <returns>是否成功使用技能</returns>
        public bool UseSkill(SkillData skill, Vector3 targetPosition)
        {
            return skillSystem.UseSkill(skill, targetPosition);
        }

        /// <summary>
        /// 使用指定技能（使用实体朝向）
        /// </summary>
        /// <param name="skill">技能数据</param>
        /// <returns>是否成功使用技能</returns>
        public bool UseSkill(SkillData skill)
        {
            return skillSystem.UseSkill(skill);
        }
        
        /// <summary>
        /// 检查技能是否在冷却中
        /// </summary>
        public bool IsSkillOnCooldown(SkillData skill)
        {
            return skillSystem.IsSkillOnCooldown(skill);
        }
        
        /// <summary>
        /// 获取技能冷却剩余时间
        /// </summary>
        public float GetSkillCooldown(SkillData skill)
        {
            return skillSystem.GetSkillCooldown(skill);
        }
        
        /// <summary>
        /// 获取实体拥有的所有技能
        /// </summary>
        public SkillData[] GetSkills()
        {
            return skillSystem.GetSkills();
        }
        
        /// <summary>
        /// 获取默认技能
        /// </summary>
        public SkillData GetDefaultSkill()
        {
            return skillSystem.GetDefaultSkill();
        }
        
        /// <summary>
        /// 设置默认技能索引
        /// </summary>
        public void SetDefaultSkillIndex(int index)
        {
            skillSystem.SetDefaultSkillIndex(index);
        }
        
        #endregion

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
        /// 获取技能系统
        /// </summary>
        public EntitySkillSystem SkillSystem => skillSystem;

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
            // 应用伤害
            float actualDamage = attributes.TakeDamage(damage);
            
            // 播放受伤特效
            if (actualDamage > 0)
            {
                effectSystem.PlayHitEffect();
            }
            
            // 如果死亡，处理死亡事件
            if (!IsAlive)
            {
                OnDeath();
            }
            
            return actualDamage;
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
        /// 处理死亡
        /// </summary>
        private void OnDeath()
        {
            // 禁用AI
            if (aiController != null)
            {
                aiController.AIEnabled = false;
            }
            
            // 在这里可以添加死亡动画、音效等
            Debug.Log($"{name} 已死亡");
            
            // 延迟销毁对象
            Destroy(gameObject, 2f);
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