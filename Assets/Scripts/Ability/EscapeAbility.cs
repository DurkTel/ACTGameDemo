using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeAbility : PlayerAbility
{

    public override AbilityType GetAbilityType()
    {
        return AbilityType.Escape;
    }
    public override bool Condition()
    {
        return m_actions.escape || (m_actions.gazing && m_actions.jump);
    }

    public override void OnEnableAbility()
    {
        base.OnEnableAbility();
        m_actions.jump = false;
        m_actions.escape = true;
        if (!m_moveController.IsGrounded())
            playerController.SetAnimationState("Air Escape");
        else if(m_actions.gazing)
            playerController.SetAnimationState("Gaze Escape");
        else
            playerController.SetAnimationState("Escape");
    }

    public override void OnUpdateAbility()
    {
        base.OnUpdateAbility();
        m_moveController.Move();
        m_moveController.Rotate();
        m_actions.escape = playerController.IsInTransition() || playerController.IsInAnimationTag("Escape");
    }
}
