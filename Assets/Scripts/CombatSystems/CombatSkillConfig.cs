using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName =  "CombatSkill", menuName = "Combat/CombatSkill")]
public class CombatSkillConfig : ScriptableObject
{
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
}
