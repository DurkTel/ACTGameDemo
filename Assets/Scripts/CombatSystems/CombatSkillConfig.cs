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
    /// 技能名字
    /// </summary>
    public string skillName;
    /// <summary>
    /// 标签
    /// </summary>
    public string tag;
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
    /// 自动锁定
    /// </summary>
    public bool autoLock;
    /// <summary>
    /// 强制执行
    /// </summary>
    public bool force;
    /// <summary>
    /// 触发条件
    /// </summary>
    public CombatAttackCondition condition;
    /// <summary>
    /// 声效
    /// </summary>
    public List<AudioStruct> audios;
    /// <summary>
    /// 打击点
    /// </summary>
    public List<HitStruct> hits;
    /// <summary>
    /// 连招
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
    /// 开始时间
    /// </summary>
    public double start;
    /// <summary>
    /// 结束时间
    /// </summary>
    public double end;
    /// <summary>
    /// 攻击段数
    /// </summary>
    public int effectCount;
    /// <summary>
    /// 击退距离
    /// </summary>
    public float repulsionDistance;
    /// <summary>
    /// 受击浮空系数
    /// </summary>
    public float strikeFly;
    /// <summary>
    /// 震动模式
    /// </summary>
    public ShakeOrient shakeOrient;
    /// <summary>
    /// 震动周期
    /// </summary>
    public float period;
    /// <summary>
    /// 震动时长
    /// </summary>
    public float shakeTime;
    /// <summary>
    /// 震动最大波峰
    /// </summary>
    public float maxWave;
    /// <summary>
    /// 震动最小波峰
    /// </summary>
    public float minWave;
    /// <summary>
    /// 震动曲线
    /// </summary>
    public AnimationCurve shakeCurve;
}

[System.Serializable]
public struct AudioStruct
{
    /// <summary>
    /// 开始时间
    /// </summary>
    public double start;
    /// <summary>
    /// 结束时间
    /// </summary>
    public double end;
    /// <summary>
    /// 声效
    /// </summary>
    public AudioClip[] audio;
    /// <summary>
    /// 击中声效
    /// </summary>
    public AudioClip[] hurtAudio;
}
