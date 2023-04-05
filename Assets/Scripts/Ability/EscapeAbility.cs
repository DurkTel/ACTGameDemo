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
        return m_actions.escape || (m_actions.gazing && m_actions.jump && m_actions.move.magnitude > 0);
    }

    public override void OnEnableAbility()
    {
        base.OnEnableAbility();
        Vector2 relativeMove = m_moveController.GetRelativeMove(m_actions.move);
        playerController.animator.SetFloat(PlayerAnimation.Float_InputHorizontal_Hash, relativeMove.x);
        playerController.animator.SetFloat(PlayerAnimation.Float_InputVertical_Hash, relativeMove.y);
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
        m_actions.jump = false;
        m_actions.escape = playerController.IsInTransition() || playerController.IsInAnimationTag("Escape");
    }
}
