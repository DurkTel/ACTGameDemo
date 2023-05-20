using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevengesAbility : PlayerAbility
{

    private bool m_revengeing, m_flag;
    public override bool Condition()
    {
        return m_revengeing || m_actions.revenges && moveController.IsGrounded();
    }

    public override AbilityType GetAbilityType()
    {
        return AbilityType.Revenges;    
    }

    public override void OnEnableAbility()
    {
        base.OnEnableAbility();
        m_revengeing = true;
        playerController.SetAnimationState("Revenge_Guard_Start");
    }

    public override void OnDisableAbility()
    {
        base.OnDisableAbility();
        m_revengeing = false;
    }

    public override void OnUpdateAbility()
    {
        base.OnUpdateAbility();

        moveController.Move();

        if (!m_actions.revenges && m_flag)
            playerController.SetAnimationState("Revenge_Guard_End", 0f);

        m_flag = m_actions.revenges;

        m_revengeing = playerController.IsInAnimationTag("Revenges");
    }
}
