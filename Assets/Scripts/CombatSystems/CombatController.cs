using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CombatSkillConfig;
using static UnityEngine.InputSystem.DefaultInputActions;

public class CombatController : MonoBehaviour
{
    [HideInInspector]
    public PlayerController playerController;

    public bool drawGizmos;

    public CombatSkillConfig[] defaultCombats;

    public CombatSkillConfig[] defaultSkills;

    public CombatDetectionPoint[] detectionPoints;

    protected MoveController m_moveController;

    protected PlayerControllerActions m_actions;

    [Header("伤害层级"), SerializeField]
    private LayerMask m_damageLayer;

    private bool m_takeOutWeapon;

    private float m_normalizedTime;

    private bool m_combatDetection;

    private CombatBroadcast m_curBroadcast;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        m_moveController = GetComponent<MoveController>();
        m_actions = playerController.actions;
        Array.Sort(defaultCombats, (CombatSkillConfig x, CombatSkillConfig y) => { return y.priority - x.priority; });
        Array.Sort(defaultSkills, (CombatSkillConfig x, CombatSkillConfig y) => { return y.priority - x.priority; });

    }

    public void OnUpdateCombat()
    {
        m_normalizedTime = Mathf.Repeat(playerController.GetAnimationNormalizedTime(PlayerAnimation.FullBodyLayerIndex), 1);
        PackUpWeapon();
        //ControllerAttack(defaultSkills, true);
        //ControllerAttack(defaultCombats);
        //m_moveController.Stop(m_curBroadcast != null);
        //m_actions.lightAttack = false;
        //m_actions.heavyAttack = false;
        //m_actions.attackEx = false;
    }

    public void OnUpdateCombatMove()
    {
        //ReleaseAttack();

        //if (m_curBroadcast != null)
        //{
        //    m_moveController.Move();
        //    if (m_actions.gazing)
        //        m_moveController.Rotate(m_actions.cameraTransform.forward, 10f);
        //    else
        //        m_moveController.Rotate();
        //}
    }

    public void OnCombatDetection()
    {
        if (!m_combatDetection || m_curBroadcast == null) return;

        foreach (var point in detectionPoints)
        {
            if (drawGizmos)
                playerController.debugHelper.DrawCapsule(point.startPoint.position, point.endPoint.position, point.radius, Color.red, 0.1f);
            Collider[] colliders = Physics.OverlapCapsule(point.startPoint.position, point.endPoint.position, point.radius, m_damageLayer);
            if (colliders.Length > 0)
            {
                PlayerController[] toActor = new PlayerController[colliders.Length];
                for (int i = 0; i < colliders.Length; i++)
                {
                    if (colliders[i].gameObject != this.gameObject && colliders[i].TryGetComponent(out PlayerController controller))
                        toActor[i] = controller;
                }
                m_curBroadcast.toActor = toActor;
                CombatBroadcastManager.Instance.AttackBroascatHurt(ref m_curBroadcast);
                break;
            }
        }
    }

    private void ReleaseAttack()
    {
        if (m_curBroadcast != null && !playerController.IsInAnimationTag("Attack"))
            CombatBroadcastManager.Instance.AttackBroascatEnd(ref m_curBroadcast);
    }

    private void ControllerAttack(CombatSkillConfig[] combats, bool force = false)
    {
        if (!m_takeOutWeapon) return;
        if (!force && m_curBroadcast != null) return;
        if (m_actions.lightAttack || m_actions.heavyAttack || m_actions.attackEx)
        {
            CombatSkillConfig newCombat = null;
            if (m_curBroadcast == null)
            {
                for (int i = 0; i < combats.Length; i++)
                {
                    CombatSkillConfig combat = combats[i];
                    if (CheckCondition(combat.condition))
                    {
                        newCombat = combat;
                        break;
                    }
                }
            }
            else if (m_curBroadcast.combatSkill.comboSkills.Length > 0)
            {
                foreach (var item in m_curBroadcast.combatSkill.comboSkills)
                {
                    if (CheckCondition(item.comboCondition) && m_normalizedTime >= item.range1 && m_normalizedTime <= item.range2) //符合触发范围
                    {
                        newCombat = item.comboSkill;
                        break;
                    }
                }
            }

            if (newCombat != null)
            {
                //构造战报信息
                m_curBroadcast = CombatBroadcastManager.Instance.broadcastPool.Get();
                m_curBroadcast.fromActor = playerController;
                m_curBroadcast.combatSkill = newCombat;
                //广播战报
                CombatBroadcastManager.Instance.AttackBroascatBegin(ref m_curBroadcast);
            }
        }
    }

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

        if (attackType.HasFlag(CombatAttackCondition.Jump) && m_moveController.IsGrounded())
            return false;

        if (attackType.HasFlag(CombatAttackCondition.Sprint) && !m_actions.sprint)
            return false;

        if (attackType.HasFlag(CombatAttackCondition.AttackEx) && !m_actions.attackEx)
            return false;

        return true;
    }


    private void PackUpWeapon()
    {
        if (m_actions.weapon != m_takeOutWeapon)
        {
            m_takeOutWeapon = m_actions.weapon;
            playerController.SetAnimationState(m_takeOutWeapon ? "Take Out Weapon" : "Pack Up Weapon");
            playerController.animator.SetFloat(PlayerAnimation.Float_IntroWeapon_Hash, m_takeOutWeapon ? 1f : 0f);
        }
    }

    public void SetDetection(int value) => m_combatDetection = value == 1;

}