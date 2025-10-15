using UnityEngine;
using SoulDefence.Core;

namespace SoulDefence.Equipment
{
    /// <summary>
    /// 装备管理器
    /// 用于加载和管理游戏中的装备配置
    /// </summary>
    public class EquipmentManager : MonoBehaviour
    {
        private static EquipmentManager instance;
        public static EquipmentManager Instance => instance;

        [Header("默认装备配置")]
        [SerializeField] private EquipmentData defaultWarriorWeapon;   // 战士默认武器
        [SerializeField] private EquipmentData defaultArcherWeapon;    // 弓手默认武器
        [SerializeField] private EquipmentData defaultWizardWeapon;    // 巫师默认武器
        [SerializeField] private EquipmentData defaultArmor;           // 默认防具

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// 根据职业类型获取默认武器
        /// </summary>
        public EquipmentData GetDefaultWeaponByProfession(ProfessionType professionType)
        {
            switch (professionType)
            {
                case ProfessionType.Warrior:
                    return defaultWarriorWeapon;
                case ProfessionType.Archer:
                    return defaultArcherWeapon;
                case ProfessionType.Wizard:
                    return defaultWizardWeapon;
                default:
                    Debug.LogWarning($"未知的职业类型: {professionType}");
                    return defaultWarriorWeapon;
            }
        }

        /// <summary>
        /// 获取默认防具
        /// </summary>
        public EquipmentData GetDefaultArmor()
        {
            return defaultArmor;
        }
    }
}

