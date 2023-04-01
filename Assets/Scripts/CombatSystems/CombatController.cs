using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CombatSkillConfig;

public class CombatController : MonoBehaviour
{
    [HideInInspector]
    public PlayerController playerController;

    public CombatSkillConfig[] defaultCombats;

    public CombatSkillConfig[] defaultSkills;

    protected MoveController m_moveController;

    protected PlayerControllerActions m_actions;

    private bool m_takeOutWeapon;

    private CombatSkillConfig m_curCombat;

    private CombatSkillConfig m_curSkill;

    private float m_normalizedTime;

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
        ControllerAttack(defaultSkills, ref m_curSkill);
        if (m_curSkill == null)
            ControllerAttack(defaultCombats, ref m_curCombat);
        m_actions.lightAttack = false;
        m_actions.heavyAttack = false;
        m_moveController.Stop(m_curCombat != null || m_curSkill != null);
    }

    public void OnUpdateCombatMove()
    {
        ReleaseAttack();
        
        if (m_curCombat != null || m_curSkill != null)
        {
            m_moveController.characterController.Move(playerController.animator.deltaPosition + new Vector3(0, m_moveController.GetGravityAcceleration() * m_moveController.deltaTtime, 0));

            if (m_actions.gazing)
                m_moveController.Rotate(m_actions.cameraTransform.forward, 10f);
            else
                m_moveController.Rotate();
        }
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

    private void ControllerAttack(CombatSkillConfig[] combats, ref CombatSkillConfig curCombat)
    {
        if (!m_takeOutWeapon) return;
        if (m_actions.lightAttack || m_actions.heavyAttack)
        {
            bool dirty = false;
            if (curCombat == null)
            {
                for (int i = 0; i < combats.Length; i++)
                {
                    CombatSkillConfig combat = combats[i];
                    if (CheckCondition(combat.condition))
                    {
                        curCombat = combat;
                        dirty = true;
                        break;
                    }
                }
            }
            else if (curCombat.comboSkills.Length > 0)
            {
                foreach (var item in curCombat.comboSkills)
                {
                    if (CheckCondition(item.comboCondition) && m_normalizedTime >= item.range1 && m_normalizedTime <= item.range2) //·ûºÏ´¥·¢·¶Î§
                    {
                        curCombat = item.comboSkill;
                        dirty = true;
                        break;
                    }
                }
            }

            if (dirty)
            {
                playerController.SetAnimationState(curCombat.animationName);
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

        return true;
    }

    private void ReleaseAttack()
    {
        if (m_curCombat != null && !playerController.IsInAnimationTag("Attack"))
            m_curCombat = null;

        if (m_curSkill != null && !playerController.IsInAnimationTag("Skill"))
            m_curSkill = null;
    }
}
