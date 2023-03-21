using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeAbility : PlayerAbility
{
    private IMove m_moveController;


    public override bool Condition()
    {
        return m_actions.escape;
    }

    public override void OnDisEnableAbility()
    {

    }

    public override void OnEnableAbility()
    {
        if (!m_moveController.IsGrounded())
            playerController.SetAnimationState("Air Escape");
        else
            playerController.SetAnimationState("Escape");
    }

    public override void OnUpdateAbility()
    {
        m_moveController.Move();
        m_actions.escape = playerController.IsInTransition() || playerController.IsInAnimationTag("Escape");
    }

    protected override void Start()
    {
        base.Start();
        m_moveController = GetComponent<MoveController>();
    }

}
