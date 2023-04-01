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
    /// ��������
    /// </summary>
    public string skillName;
    /// <summary>
    /// ��������
    /// </summary>
    public string animationName;
    /// <summary>
    /// ����ǰҡ
    /// </summary>
    public float attackPoint;
    /// <summary>
    /// ������ҡ
    /// </summary>
    public float attackBackswing;
    /// <summary>
    /// ���ȼ�
    /// </summary>
    public int priority;
    /// <summary>
    /// ��������
    /// </summary>
    public CombatAttackCondition condition;
    /// <summary>
    /// ����
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
