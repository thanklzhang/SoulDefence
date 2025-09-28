using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulDefence.Entity
{
    /// <summary>
    /// 实体特效系统
    /// 管理实体的视觉特效，如受伤闪烁等
    /// </summary>
    [System.Serializable]
    public class EntityEffectSystem
    {
        [SerializeField] private EntityEffectData effectData;  // 特效数据配置
        
        // 实体引用
        private GameEntity owner;
        private Transform transform;
        private MonoBehaviour monoBehaviour; // 用于启动协程

        /// <summary>
        /// 初始化特效系统
        /// </summary>
        public void Initialize(GameEntity owner)
        {
            this.owner = owner;
            this.transform = owner.transform;
            this.monoBehaviour = owner;
        }
        
        /// <summary>
        /// 设置特效数据配置
        /// </summary>
        public void SetEffectData(EntityEffectData data)
        {
            effectData = data;
        }
        
        /// <summary>
        /// 播放受伤特效
        /// </summary>
        public void PlayHitEffect()
        {
            if (effectData == null)
            {
                Debug.LogWarning("EntityEffectSystem: effectData is null!");
                return;
            }
            
            if (effectData.hitEffectPrefab != null)
            {
                // 在实体位置创建特效
                GameObject effect = Object.Instantiate(effectData.hitEffectPrefab, transform.position, Quaternion.identity);
                
                // 销毁特效
                Object.Destroy(effect, effectData.hitFlashDuration * 5f); // 给足够时间播放完特效
            }
            
            // 简单的闪烁效果
            monoBehaviour.StartCoroutine(FlashColor());
        }
        
        /// <summary>
        /// 播放死亡特效
        /// </summary>
        public void PlayDeathEffect()
        {
            if (effectData == null || effectData.deathEffectPrefab == null)
                return;
                
            // 在实体位置创建死亡特效
            GameObject effect = Object.Instantiate(effectData.deathEffectPrefab, transform.position, Quaternion.identity);
            
            // 销毁特效
            Object.Destroy(effect, effectData.deathEffectDuration);
        }
        
        /// <summary>
        /// 播放升级特效
        /// </summary>
        public void PlayLevelUpEffect()
        {
            if (effectData == null || effectData.levelUpEffectPrefab == null)
                return;
                
            // 在实体位置创建升级特效
            GameObject effect = Object.Instantiate(effectData.levelUpEffectPrefab, transform.position, Quaternion.identity);
            
            // 销毁特效
            Object.Destroy(effect, effectData.levelUpEffectDuration);
        }
        
        /// <summary>
        /// 简单的颜色闪烁效果
        /// </summary>
        private IEnumerator FlashColor()
        {
            if (effectData == null)
                yield break;
                
            // 获取所有渲染器
            Renderer[] renderers = owner.GetComponentsInChildren<Renderer>();
            
            // 保存原始颜色
            List<Material> originalMaterials = new List<Material>();
            foreach (Renderer renderer in renderers)
            {
                foreach (Material material in renderer.materials)
                {
                    originalMaterials.Add(new Material(material));
                    material.color = effectData.hitFlashColor;
                }
            }
            
            // 等待指定时间
            yield return new WaitForSeconds(effectData.hitFlashDuration);
            
            // 恢复原始颜色
            int materialIndex = 0;
            foreach (Renderer renderer in renderers)
            {
                for (int i = 0; i < renderer.materials.Length; i++)
                {
                    if (materialIndex < originalMaterials.Count)
                    {
                        renderer.materials[i].color = originalMaterials[materialIndex].color;
                        materialIndex++;
                    }
                }
            }
        }
        
        /// <summary>
        /// 获取特效数据配置
        /// </summary>
        public EntityEffectData GetEffectData()
        {
            return effectData;
        }
    }
} 