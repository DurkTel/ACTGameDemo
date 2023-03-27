using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    [HideInInspector]
    public PlayerController playerController;

    [Header("Çá¹¥»÷")]public CombatSkillConfig defaultLightCombat;
    [Header("ÖØ¹¥»÷")]public CombatSkillConfig defaultHeavyCombat;
    [Header("ÅÜ¶¯¹¥»÷")]public CombatSkillConfig defaultRunCombat;
    [Header("ÌøÔ¾¹¥»÷")]public CombatSkillConfig defaultJumpCombat;

    protected IMove m_moveController;

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
    }

    public void OnUpdateCombat()
    {
        m_normalizedTime = Mathf.Repeat(playerController.GetAnimationNormalizedTime(PlayerAnimation.FullBodyLayerIndex), 1);
        PackUpWeapon();
        ControllerSkill();
        ControllerAttack();
        m_moveController.Stop(m_curCombat != null);
    }

    public void OnUpdateCombatMove()
    {
        ReleaseAttack();

        if (m_curCombat != null)
        {
            m_moveController.Move();
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

    private void ControllerAttack()
    {
        if (!m_takeOutWeapon || m_curSkill != null) return;
        if (m_actions.lightAttack || m_actions.heavyAttack)
        {
            bool force = false;
            if (m_curCombat == null)
            {
                switch (playerController.currentAbilitiy.GetAbilityType())
                {
                    case AbilityType.Locomotion:
                        if (m_actions.move.magnitude > 0f)
                        {
                            m_curCombat = defaultRunCombat;
                        }
                        else
                        {
                            if (m_actions.lightAttack)
                                m_curCombat = defaultLightCombat;
                            else if (m_actions.heavyAttack)
                                m_curCombat = defaultHeavyCombat;
                        }
                        force = true;
                        break;
                    case AbilityType.Airbone:
                        m_curCombat = defaultJumpCombat;
                        force = true;
                        break;
                    default:
                        break;
                }
            }
            else if (m_curCombat.comboSkills.Length > 0) //ÓÐÁ¬ÕÐ
            {
                foreach (var item in m_curCombat.comboSkills)
                {
                    if (m_normalizedTime >= item.range1 && m_normalizedTime <= item.range2) //·ûºÏ´¥·¢·¶Î§
                    {
                        m_curCombat = item.comboSkill;
                        force = true;
                        break;
                    }
                }
            }

            if (force)
                playerController.SetAnimationState(m_curCombat.animationName);


            m_actions.lightAttack = false;
            m_actions.heavyAttack = false;
        }
    }

    private void ControllerSkill()
    {
        if (!m_takeOutWeapon) return;
        {

        }
    }

    private void ReleaseAttack()
    {
        if (m_curCombat != null && !playerController.IsInAnimationTag("Attack"))
            m_curCombat = null;
    }
}
