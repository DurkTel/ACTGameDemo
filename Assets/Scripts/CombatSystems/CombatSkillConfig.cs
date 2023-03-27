using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName =  "CombatSkill", menuName = "Combat/CombatSkill")]
public class CombatSkillConfig : ScriptableObject
{
    /// <summary>
    /// 技能名字
    /// </summary>
    public string skillName;
    /// <summary>
    /// 动画名称
    /// </summary>
    public string animationName;
    /// <summary>
    /// 攻击前摇
    /// </summary>
    public float attackPoint;
    /// <summary>
    /// 攻击后摇
    /// </summary>
    public float attackBackswing;
    /// <summary>
    /// 连招
    /// </summary>
    public ComboSkillStruct[] comboSkills;
}

[System.Serializable]   
public struct ComboSkillStruct
{
    [Range(0f, 1f)]public float range1;
    [Range(0f, 1f)] public float range2;
    public CombatSkillConfig comboSkill;
}
