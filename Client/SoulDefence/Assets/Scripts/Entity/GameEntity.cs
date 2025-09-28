using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
        [SerializeField] private bool useAI = false;

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

        // Start is called before the first frame update
        void Start()
        {
            InitializeEntity();
        }

        // Update is called once per frame
        void Update()
        {
            UpdateEntity();
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
                    break;
                case EntityType.Enemy:
                    teamSystem.Team = TeamSystem.TeamType.Enemy;
                    break;
            }
        }

        /// <summary>
        /// 根据实体类型设置默认AI状态
        /// </summary>
        private void SetDefaultAIState()
        {
            switch (entityType)
            {
                case EntityType.Player:
                    // 玩家AI默认关闭
                    useAI = false;
                    break;
                case EntityType.Enemy:
                case EntityType.Castle:
                    // 敌人和城堡AI默认开启
                    useAI = true;
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
                    break;
                case EntityType.Enemy:
                    aiController = new EnemyAI();
                    break;
                case EntityType.Castle:
                    aiController = new CastleAI();
                    break;
            }

            // 如果创建了AI控制器，初始化它
            if (aiController != null)
            {
                aiController.Initialize(transform, this, teamSystem);
                aiController.AIEnabled = useAI;
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
                aiController.UpdateAI();
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
            
            if (aiController != null)
            {
                aiController.AIEnabled = enabled;
                
                // 对于玩家特殊处理
                if (entityType == EntityType.Player && aiController is PlayerAI playerAI)
                {
                    playerAI.ToggleAI(enabled);
                }
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