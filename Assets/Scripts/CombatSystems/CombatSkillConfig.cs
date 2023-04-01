using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName =  "CombatSkill", menuName = "Combat/CombatSkill")]
public class CombatSkillConfig : ScriptableObject
{
    [System.Flags]
    public enum CombatAttackCondition
    {
        LightAttack = 1,
        HeavyAttack = 2,
        Gazing = 4,
        MoveUp = 8,
        MoveDown = 16,
        MoveLeft = 32,
        MoveRight = 64,
        Jump = 128,
        Sprint = 256,
    }
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
    /// 优先级
    /// </summary>
    public int priority;
    /// <summary>
    /// 触发条件
    /// </summary>
    public CombatAttackCondition condition;
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
    public CombatSkillConfig.CombatAttackCondition comboCondition;
}
