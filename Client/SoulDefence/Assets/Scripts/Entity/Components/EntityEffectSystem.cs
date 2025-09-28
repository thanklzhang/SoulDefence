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
        [SerializeField] private GameObject hitEffectPrefab; // 受伤特效预制体
        
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
        /// 播放受伤特效
        /// </summary>
        public void PlayHitEffect()
        {
            if (hitEffectPrefab != null)
            {
                // 在实体位置创建特效
                GameObject effect = Object.Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
                
                // 销毁特效(默认0.5秒)
                Object.Destroy(effect, 0.5f);
            }
            
            // 简单的闪烁效果
            monoBehaviour.StartCoroutine(FlashColor());
        }
        
        /// <summary>
        /// 简单的颜色闪烁效果
        /// </summary>
        private IEnumerator FlashColor()
        {
            // 获取所有渲染器
            Renderer[] renderers = owner.GetComponentsInChildren<Renderer>();
            
            // 保存原始颜色
            List<Material> originalMaterials = new List<Material>();
            foreach (Renderer renderer in renderers)
            {
                foreach (Material material in renderer.materials)
                {
                    originalMaterials.Add(new Material(material));
                    material.color = Color.red;
                }
            }
            
            // 等待0.1秒
            yield return new WaitForSeconds(0.1f);
            
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
    }
} 