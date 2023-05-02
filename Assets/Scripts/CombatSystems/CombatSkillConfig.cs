using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ShakeCamera;

[CreateAssetMenu(fileName =  "CombatSkill", menuName = "Combat/CombatSkill")]
[System.Serializable]
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
        AttackEx = 512,
    }
    /// <summary>
    /// ��������
    /// </summary>
    public string skillName;
    /// <summary>
    /// ��ǩ
    /// </summary>
    public string tag;
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
    /// �Զ�����
    /// </summary>
    public bool autoLock;
    /// <summary>
    /// ǿ��ִ��
    /// </summary>
    public bool force;
    /// <summary>
    /// ��������
    /// </summary>
    public CombatAttackCondition condition;
    /// <summary>
    /// ��Ч
    /// </summary>
    public List<AudioStruct> audios;
    /// <summary>
    /// �����
    /// </summary>
    public List<HitStruct> hits;
    /// <summary>
    /// ����
    /// </summary>
    public List<ComboSkillStruct> comboSkills;

}

[System.Serializable]   
public struct ComboSkillStruct
{
    [Range(0f, 1f)]public double range1;
    [Range(0f, 1f)] public double range2;
    public CombatSkillConfig comboSkill;
    public CombatSkillConfig.CombatAttackCondition comboCondition;
}

[System.Serializable]
public struct HitStruct
{
    /// <summary>
    /// ��ʼʱ��
    /// </summary>
    public double start;
    /// <summary>
    /// ����ʱ��
    /// </summary>
    public double end;
    /// <summary>
    /// ��������
    /// </summary>
    public int effectCount;
    /// <summary>
    /// ���˾���
    /// </summary>
    public float repulsionDistance;
    /// <summary>
    /// �ܻ�����ϵ��
    /// </summary>
    public float strikeFly;
    /// <summary>
    /// ��ģʽ
    /// </summary>
    public ShakeOrient shakeOrient;
    /// <summary>
    /// ������
    /// </summary>
    public float period;
    /// <summary>
    /// ��ʱ��
    /// </summary>
    public float shakeTime;
    /// <summary>
    /// ����󲨷�
    /// </summary>
    public float maxWave;
    /// <summary>
    /// ����С����
    /// </summary>
    public float minWave;
    /// <summary>
    /// ������
    /// </summary>
    public AnimationCurve shakeCurve;
}

[System.Serializable]
public struct AudioStruct
{
    /// <summary>
    /// ��ʼʱ��
    /// </summary>
    public double start;
    /// <summary>
    /// ����ʱ��
    /// </summary>
    public double end;
    /// <summary>
    /// ��Ч
    /// </summary>
    public AudioClip[] audio;
    /// <summary>
    /// ������Ч
    /// </summary>
    public AudioClip[] hurtAudio;
}
