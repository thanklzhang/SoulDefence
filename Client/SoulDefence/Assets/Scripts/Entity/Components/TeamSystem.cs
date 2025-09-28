using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulDefence.Entity
{
    /// <summary>
    /// 队伍系统
    /// 管理实体的队伍归属和队伍关系
    /// </summary>
    [System.Serializable]
    public class TeamSystem
    {
        /// <summary>
        /// 队伍类型枚举
        /// </summary>
        public enum TeamType
        {
            Player = 0,    // 玩家队伍（包括玩家和城堡）
            Enemy = 1,     // 敌人队伍（小怪和boss）
            Neutral = 2,   // 中立队伍（预留）
            Custom1 = 3,   // 自定义队伍1（预留）
            Custom2 = 4    // 自定义队伍2（预留）
        }

        [SerializeField] private TeamType team = TeamType.Player;  // 默认为玩家队伍

        /// <summary>
        /// 获取或设置队伍类型
        /// </summary>
        public TeamType Team
        {
            get => team;
            set => team = value;
        }

        /// <summary>
        /// 判断是否为友方
        /// </summary>
        /// <param name="otherTeam">其他队伍</param>
        /// <returns>是否为友方</returns>
        public bool IsFriendly(TeamSystem otherTeam)
        {
            if (otherTeam == null)
                return false;
            
            return team == otherTeam.team;
        }

        /// <summary>
        /// 判断是否为敌方
        /// </summary>
        /// <param name="otherTeam">其他队伍</param>
        /// <returns>是否为敌方</returns>
        public bool IsHostile(TeamSystem otherTeam)
        {
            if (otherTeam == null)
                return false;
            
            return team != otherTeam.team;
        }
    }
} 