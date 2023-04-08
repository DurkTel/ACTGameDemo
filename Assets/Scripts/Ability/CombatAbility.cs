using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CombatSkillConfig;

public class CombatAbility : PlayerAbility
{
    public bool drawGizmos;

    public CombatSkillConfig[] defaultCombats;

    public CombatDetectionPoint[] detectionPoints;

    public float autoLockRadius;

    [Header("�˺��㼶"), SerializeField]
    private LayerMask m_damageLayer;

    private CombatBroadcast m_curBroadcast;

    private int m_curBroadcastId;

    private float m_normalizedTime;

    private bool m_combatDetection;

    private Vector3 m_moveCompensation;

    public void SetDetection(int value) => m_combatDetection = value == 1;

    protected override void Start()
    {
        base.Start();
        System.Array.Sort(defaultCombats, (CombatSkillConfig x, CombatSkillConfig y) => { return y.priority - x.priority; });
    }

    public override bool Condition()
    {
        if (m_curBroadcastId > 0) return true;
        if (!m_actions.weapon) return false;
        if (GetAttackSignal())
        {
            CombatSkillConfig combat = null;
            for (int i = 0; i < defaultCombats.Length; i++)
            {
                combat = defaultCombats[i];
                if (CheckCondition(combat.condition))
                {
                    //����ս����Ϣ
                    m_curBroadcast = InitCombatBroad(combat);
                    ReleaseAttackSignal();
                    return true;
                }
            }
        }
        ReleaseAttackSignal();
        return false;
    }

    public override AbilityType GetAbilityType()
    {
        return AbilityType.Combat;
    }

    public override void OnEnableAbility()
    {
        base.OnEnableAbility();
        OnResetAnimatorParameter();
        CombatBroadcastManager.Instance.AttackBroascatBegin(m_curBroadcast);
        AutoLock();
    }

    public override void OnDisableAbility()
    {
        base.OnDisableAbility();
    }

    public override void OnUpdateAbility()
    {
        base.OnUpdateAbility();
        m_normalizedTime = GetAttackProgress();
        ControlCombo();

    }

    public override void OnResetAnimatorParameter() 
    {
        playerController.animator.SetFloat(PlayerAnimation.Float_InputHorizontalLerp_Hash, 0f);
        playerController.animator.SetFloat(PlayerAnimation.Float_InputVerticalLerp_Hash, 0f);
        playerController.animator.SetFloat(PlayerAnimation.Float_AngularVelocity_Hash, 0f);
        playerController.animator.SetFloat(PlayerAnimation.Float_Rotation_Hash, 0f);
        playerController.animator.SetFloat(PlayerAnimation.Float_Movement_Hash, 0f);

    }


    private void FixedUpdate()
    {
        if (!m_isEnable) return;
        ControlDetection();
    }

    private void OnAnimatorMove()
    {
        if (!m_isEnable) return;
        ReleaseAttack();
        moveController.MoveCompensation();
    }

    /// <summary>
    /// ���ƴ�����
    /// </summary>
    private void ControlDetection()
    {
        if (!m_combatDetection || m_curBroadcastId < 0) return;

        foreach (var point in detectionPoints)
        {
            if (drawGizmos)
                playerController.debugHelper.DrawCapsule(point.startPoint.position, point.endPoint.position, point.radius, Color.red, 0.1f);
            Collider[] colliders = Physics.OverlapCapsule(point.startPoint.position, point.endPoint.position, point.radius, m_damageLayer);
            if (colliders.Length > 0)
            {
                List<PlayerController> temp = new List<PlayerController>();
                for (int i = 0; i < colliders.Length; i++)
                {
                    if (colliders[i].gameObject != this.gameObject && colliders[i].TryGetComponent(out PlayerController controller))
                        temp.Add(controller);
                }
                if (temp.Count > 0 && CombatBroadcastManager.Instance.TypGetAttackBroascat(m_curBroadcastId, out CombatBroadcast broadcast))
                {
                    m_curBroadcast = broadcast;
                    m_curBroadcast.toActor = temp.ToArray();
                    CombatBroadcastManager.Instance.AttackBroascatHurt(m_curBroadcast);
                    break;
                }
            }
        }
    }

    /// <summary>
    /// ��������
    /// </summary>
    /// <param name="combats"></param>
    private void ControlCombo()
    {
        if (!m_actions.weapon || m_curBroadcastId < 0) return;
        if (GetAttackSignal())
        {
            CombatSkillConfig newCombat = null;
            if (m_curBroadcast.combatSkill.comboSkills.Length > 0)
            {
                foreach (var item in m_curBroadcast.combatSkill.comboSkills)
                {
                    if (item.comboSkill.tag.Equals("Air Attack") && playerController.GetGroundClearance() < 0.5f) //���й������̫��������
                        continue;
                    if (CheckCondition(item.comboCondition) && m_normalizedTime >= item.range1 && m_normalizedTime <= item.range2) //���ϴ�����Χ
                    {
                        newCombat = item.comboSkill;
                        break;
                    }
                }
            }

            if (newCombat != null)
            {
                //�ȴ�ϵ�ǰ��ս��
                CombatBroadcastManager.Instance.AttackBroascatBreak(m_curBroadcast);
                //ˢ��ս����Ϣ
                m_curBroadcast = InitCombatBroad(newCombat);
                //�㲥ս��
                CombatBroadcastManager.Instance.AttackBroascatBegin(m_curBroadcast);
                AutoLock();
                ReleaseAttackSignal();
            }
        }
    }

    /// <summary>
    /// ��������
    /// </summary>
    private void ReleaseAttack()
    {
        if (m_curBroadcastId > 0 && !playerController.IsInTransition() && (!playerController.IsInAnimationTag("Attack")  || (m_curBroadcast.combatSkill.tag.Equals("Air Attack") && playerController.GetGroundClearance() < 0.5f)))
        {
            m_curBroadcastId = -1;
            m_actions.jump = false;
            moveController.SetGravityAcceleration(0);
            CombatBroadcastManager.Instance.AttackBroascatEnd(m_curBroadcast);
        }
    }

    private CombatBroadcast InitCombatBroad(CombatSkillConfig newCombat)
    {
        CombatBroadcast broadcast = CombatBroadcastManager.GetCombatBroadcast(out m_curBroadcastId);
        broadcast.fromActor = playerController;
        broadcast.combatSkill = newCombat;
        return broadcast;
    }

    /// <summary>
    /// ��ȡ��ǰ�����ź�
    /// </summary>
    /// <returns></returns>
    private bool GetAttackSignal()
    {
        return m_actions.lightAttack || m_actions.heavyAttack || m_actions.attackEx;
    }

    /// <summary>
    /// ���ù����ź�
    /// </summary>
    private void ReleaseAttackSignal()
    {
        m_actions.lightAttack = false;
        m_actions.heavyAttack = false;
        m_actions.attackEx = false;
    }

    /// <summary>
    /// �Զ�����
    /// </summary>
    private void AutoLock()
    {
        if (m_actions.move.magnitude > 0) return;
        Transform target = playerController.rootTransform.ObtainNearestTarget(autoLockRadius, m_damageLayer, playerController.rootTransform);
        if (target == null) return;
        Vector3 dir = target.position - playerController.rootTransform.position;
        dir.y = 0f;
        moveController.Rotate(Quaternion.LookRotation(dir), 0.2f, 0f);
    }

    /// <summary>
    /// ��ǰ������������
    /// </summary>
    /// <returns></returns>
    private float GetAttackProgress()
    {
        if (m_curBroadcastId < 0 || !playerController.IsInAnimationName(m_curBroadcast.combatSkill.animationName)) return 0f;
        float progress = playerController.GetAnimationNormalizedTime(PlayerAnimation.FullBodyLayerIndex);
        return progress;
    }

    /// <summary>
    /// �����������
    /// </summary>
    /// <param name="attackType"></param>
    /// <returns></returns>
    private bool CheckCondition(CombatAttackCondition attackType)
    {
        if (attackType.Equals(null))
            return false;

        if (attackType.HasFlag(CombatAttackCondition.LightAttack) && !m_actions.lightAttack)
            return false;

        if (attackType.HasFlag(CombatAttackCondition.HeavyAttack) && !m_actions.heavyAttack)
            return false;

        if (attackType.HasFlag(CombatAttackCondition.Gazing) && !m_actions.gazing)
            return false;

        if (attackType.HasFlag(CombatAttackCondition.MoveUp) && !m_actions.moveUp)
            return false;

        if (attackType.HasFlag(CombatAttackCondition.MoveDown) && !m_actions.moveDown)
            return false;

        if (attackType.HasFlag(CombatAttackCondition.MoveLeft) && !m_actions.moveLeft)
            return false;

        if (attackType.HasFlag(CombatAttackCondition.MoveRight) && !m_actions.moveRight)
            return false;

        if (attackType.HasFlag(CombatAttackCondition.Jump) && moveController.IsGrounded())
            return false;

        if (attackType.HasFlag(CombatAttackCondition.Sprint) && !m_actions.sprint)
            return false;

        if (attackType.HasFlag(CombatAttackCondition.AttackEx) && !m_actions.attackEx)
            return false;

        return true;
    }

}


[Serializable]
public struct CombatDetectionPoint
{
    public Transform startPoint;

    public Transform endPoint;

    public float radius;
}
