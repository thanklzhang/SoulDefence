using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulDefence.Entity;

namespace SoulDefence.Skill
{
    /// <summary>
    /// 技能测试脚本
    /// 用于演示技能系统的使用
    /// </summary>
    public class SkillTester : MonoBehaviour
    {
        [Header("测试设置")]
        [SerializeField] private GameEntity testEntity;       // 测试实体
        [SerializeField] private KeyCode skillKey = KeyCode.Space;  // 技能按键
        [SerializeField] private KeyCode[] skillKeys = new KeyCode[] { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3 };  // 技能快捷键
        
        [Header("调试")]
        [SerializeField] private bool showDebugInfo = true;   // 显示调试信息

        void Start()
        {
            // 如果没有指定测试实体，尝试获取当前对象上的GameEntity
            if (testEntity == null)
            {
                testEntity = GetComponent<GameEntity>();
            }
            
            // 确保场景中有SkillSystem实例
            if (SkillSystem.Instance == null)
            {
                GameObject skillSystemObj = new GameObject("SkillSystem");
                skillSystemObj.AddComponent<SkillSystem>();
            }
        }

        void Update()
        {
            if (testEntity == null)
                return;
                
            // 处理技能输入
            HandleSkillInput();
            
            // 显示调试信息
            if (showDebugInfo)
            {
                DisplayDebugInfo();
            }
        }

        /// <summary>
        /// 处理技能输入
        /// </summary>
        private void HandleSkillInput()
        {
            // 空格键使用默认技能
            if (Input.GetKeyDown(skillKey))
            {
                UseSkill(-1); // -1表示使用默认技能
            }
            
            // 数字键使用对应索引的技能
            for (int i = 0; i < skillKeys.Length; i++)
            {
                if (Input.GetKeyDown(skillKeys[i]))
                {
                    UseSkill(i);
                }
            }
        }

        /// <summary>
        /// 使用技能
        /// </summary>
        /// <param name="skillIndex">技能索引，-1表示使用默认技能</param>
        private void UseSkill(int skillIndex)
        {
            // 使用技能
            bool success;
            if (skillIndex < 0)
            {
                success = testEntity.UseDefaultSkill();
            }
            else
            {
                success = testEntity.UseSkill(skillIndex);
            }
            
            // 输出结果
            if (success)
            {
                Debug.Log($"技能使用成功！方向: {testEntity.transform.forward}");
            }
            else
            {
                Debug.Log("技能使用失败！");
            }
        }

        /// <summary>
        /// 显示调试信息
        /// </summary>
        private void DisplayDebugInfo()
        {
            SkillData[] skills = testEntity.GetSkills();
            if (skills == null || skills.Length == 0)
                return;
                
            string info = "技能列表:\n";
            for (int i = 0; i < skills.Length; i++)
            {
                if (skills[i] != null)
                {
                    float cooldown = testEntity.GetSkillCooldown(skills[i]);
                    string cooldownStr = cooldown > 0 ? $"冷却中: {cooldown:F1}s" : "就绪";
                    info += $"{i+1}. {skills[i].skillName} - {cooldownStr}\n";
                }
            }
            
            // 在控制台输出调试信息
            Debug.Log(info);
        }

        /// <summary>
        /// 在屏幕上绘制调试信息
        /// </summary>
        private void OnGUI()
        {
            if (!showDebugInfo || testEntity == null)
                return;
                
            SkillData[] skills = testEntity.GetSkills();
            if (skills == null || skills.Length == 0)
                return;
                
            GUILayout.BeginArea(new Rect(10, 10, 200, 300));
            GUILayout.Label("技能列表:");
            
            for (int i = 0; i < skills.Length; i++)
            {
                if (skills[i] != null)
                {
                    float cooldown = testEntity.GetSkillCooldown(skills[i]);
                    string cooldownStr = cooldown > 0 ? $"冷却中: {cooldown:F1}s" : "就绪";
                    string keyName = i < skillKeys.Length ? skillKeys[i].ToString() : "无";
                    GUILayout.Label($"{keyName}. {skills[i].skillName} - {cooldownStr}");
                }
            }
            
            GUILayout.Label($"空格键: 使用默认技能");
            
            GUILayout.EndArea();
        }
    }
} 