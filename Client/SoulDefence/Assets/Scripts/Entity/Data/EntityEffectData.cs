using UnityEngine;

namespace SoulDefence.Entity
{
    /// <summary>
    /// 实体特效数据配置
    /// 使用ScriptableObject存储实体特效配置
    /// </summary>
    [CreateAssetMenu(fileName = "New Entity Effects", menuName = "GameConfig/Entity/Effects")]
    public class EntityEffectData : ScriptableObject
    {
        [Header("受伤特效")]
        public GameObject hitEffectPrefab;     // 受伤特效预制体
        public Color hitFlashColor = Color.red; // 受伤闪烁颜色
        public float hitFlashDuration = 0.1f;   // 受伤闪烁持续时间
        
        [Header("死亡特效")]
        public GameObject deathEffectPrefab;    // 死亡特效预制体
        public float deathEffectDuration = 1.0f; // 死亡特效持续时间
        
        [Header("升级特效")]
        public GameObject levelUpEffectPrefab;  // 升级特效预制体
        public float levelUpEffectDuration = 1.5f; // 升级特效持续时间
    }
} 