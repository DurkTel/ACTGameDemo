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
        return m_actions.escape;
    }

    public override void OnEnableAbility()
    {
        base.OnEnableAbility();
        if (!m_moveController.IsGrounded())
            playerController.SetAnimationState("Air Escape");
        else
            playerController.SetAnimationState("Escape");
    }

    public override void OnUpdateAbility()
    {
        base.OnUpdateAbility();
        m_moveController.Move();
        m_actions.escape = playerController.IsInTransition() || playerController.IsInAnimationTag("Escape");
    }
}
