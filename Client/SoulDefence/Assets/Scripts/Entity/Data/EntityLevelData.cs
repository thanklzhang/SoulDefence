using UnityEngine;

namespace SoulDefence.Entity
{
    /// <summary>
    /// 实体等级和经验配置
    /// 单独管理等级和经验相关数据
    /// </summary>
    [CreateAssetMenu(fileName = "New Entity Level Data", menuName = "GameConfig/Entity/Level Data")]
    public class EntityLevelData : ScriptableObject
    {
        [Header("等级设置")]
        public int baseLevel = 1;             // 基础等级
        public float expToNextLevel = 100f;   // 升级所需经验
        public float expGainOnDeath = 50f;    // 死亡给予经验
    }
}

