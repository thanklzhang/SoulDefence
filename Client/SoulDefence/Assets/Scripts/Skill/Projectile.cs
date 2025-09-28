using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulDefence.Entity;

namespace SoulDefence.Skill
{
    /// <summary>
    /// 投掷物类
    /// 实现远程攻击的投掷物逻辑
    /// </summary>
    public class Projectile : MonoBehaviour
    {
        // 投掷物参数
        private GameEntity caster;             // 施放者
        private SkillData skillData;           // 技能数据
        private Vector3 direction;             // 初始方向
        private float speed;                   // 速度
        private float damage;                  // 伤害
        private bool canPierce;                // 是否可以穿透
        private bool isHoming;                 // 是否跟踪目标
        private float lifetime;                // 存活时间
        private int hitCount;                  // 已命中目标数量
        private int maxHitCount;               // 最大命中目标数量
        private float attackRange;             // 攻击范围
        
        // 跟踪目标
        private Transform targetTransform;
        private float homingStrength = 5f;     // 跟踪强度
        
        // 已命中的目标列表(防止重复命中)
        private List<GameEntity> hitTargets = new List<GameEntity>();

        // /// <summary>
        // /// 初始化投掷物
        // /// </summary>
        // public void Initialize(GameEntity caster, SkillData skillData, Vector3 direction)
        // {
        //     Initialize(caster, skillData, direction, skillData.attackRange);
        // }

        /// <summary>
        /// 初始化投掷物（带攻击范围参数）
        /// </summary>
        public void Initialize(GameEntity caster, SkillData skillData, Vector3 direction, float attackRange)
        {
            this.caster = caster;
            this.skillData = skillData;
            this.direction = direction;
            this.attackRange = attackRange;
            
            // 设置参数
            speed = skillData.projectileSpeed;
            damage = skillData.damage;
            canPierce = skillData.canPierce;
            isHoming = skillData.isHoming;
            lifetime = skillData.projectileLifetime;
            maxHitCount = skillData.targetCount;
            
            // 如果是跟踪投掷物，尝试找到目标
            if (isHoming)
            {
                FindTarget();
            }
            
            // 设置销毁时间
            Destroy(gameObject, lifetime);
        }

        void Update()
        {
            // 如果已经命中最大目标数，销毁
            if (hitCount >= maxHitCount)
            {
                Destroy(gameObject);
                return;
            }
            
            // 更新移动
            UpdateMovement();
        }

        /// <summary>
        /// 更新移动
        /// </summary>
        private void UpdateMovement()
        {
            Vector3 moveDirection = direction;
            
            // 如果是跟踪投掷物且有目标，调整方向
            if (isHoming && targetTransform != null)
            {
                // 计算到目标的方向
                Vector3 dirToTarget = (targetTransform.position - transform.position).normalized;
                
                // 插值计算新方向
                moveDirection = Vector3.Slerp(direction, dirToTarget, Time.deltaTime * homingStrength);
                
                // 更新当前方向
                direction = moveDirection;
            }
            
            // 应用移动
            transform.position += moveDirection * speed * Time.deltaTime;
            // Debug.Log("移动：" + moveDirection.magnitude + " " + speed);
            
            // 更新朝向
            if (moveDirection.magnitude > 0.1f)
            {
                transform.rotation = Quaternion.LookRotation(moveDirection);
            }
        }

        /// <summary>
        /// 寻找目标
        /// </summary>
        private void FindTarget()
        {
            // 在前方一定范围内寻找敌对目标
            float searchRange = attackRange * 2f; // 使用攻击范围的2倍作为搜索范围
            Collider[] colliders = Physics.OverlapSphere(transform.position, searchRange);
            
            float closestDistance = float.MaxValue;
            Transform closestTarget = null;
            
            foreach (var collider in colliders)
            {
                GameEntity target = collider.GetComponent<GameEntity>();
                
                // 检查是否是有效目标
                if (target != null && target != caster && target.IsAlive && 
                    caster.TeamSystem.IsHostile(target.TeamSystem) &&
                    !hitTargets.Contains(target))
                {
                    
                    // 计算距离
                    float distance = Vector3.Distance(transform.position, target.transform.position);
                    
                    // 检查是否在前方
                    Vector3 dirToTarget = (target.transform.position - transform.position).normalized;
                    float dot = Vector3.Dot(direction, dirToTarget);
                    
                    // 如果在前方且更近，更新最近目标
                    if (dot > 0.5f && distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestTarget = target.transform;
                    }
                }
            }
            
            // 设置目标
            targetTransform = closestTarget;
        }

        /// <summary>
        /// 碰撞检测
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("碰撞：" + other.name);
            // 获取目标实体
            GameEntity target = other.GetComponent<GameEntity>();
            
            // 检查是否是有效目标
            if (target != null && target != caster && target.IsAlive && 
                caster.TeamSystem.IsHostile(target.TeamSystem) &&
                !hitTargets.Contains(target))
            {
                // 添加到已命中列表
                hitTargets.Add(target);
                hitCount++;
                
                // 应用伤害
                SkillSystem.Instance.ApplyDamage(caster, target, damage);
                
                // 如果不能穿透或已达到最大命中数，销毁
                if (!canPierce || hitCount >= maxHitCount)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
} 