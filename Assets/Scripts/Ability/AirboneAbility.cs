using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirboneAbility : PlayerAbility
{

    [Header("跳跃参数")]
    public float jumpHeight;

    public int jumpFrequency;
    [Header("空中移动速度")]
    public float airSpeed;

    [SerializeField]
    private float m_rotateSpeed = 10f;

    private bool m_isAiring;

    private int m_jumpCount;

    private float m_speed;

    public override bool Condition()
    {
        return m_isAiring || (!playerController.IsInTransition() && ((m_moveController.IsGrounded() && m_actions.jump) || (m_moveController.IsFalled() && !m_actions.jump)));
    }

    public override AbilityType GetAbilityType()
    {
        return AbilityType.Airbone;
    }

    public override void OnDisableAbility()
    {
        base.OnDisableAbility();
        m_jumpCount = 0;
        m_isAiring = false;
    }

    public override void OnEnableAbility()
    {
        m_jumpCount++;
        m_speed = playerController.animator.GetFloat(PlayerAnimation.Float_Movement_Hash) / 2f * 0.1f * 0.5f;
        playerController.SetAnimationState(m_actions.jump ? "Jump First" : "Fall Keep", m_actions.jump ? 0f : 0.1f);
        
        if (m_actions.jump)
            m_moveController.SetGravityAcceleration(jumpHeight);

        m_actions.jump = false;
        m_isAiring = true;
    }

    public override void OnUpdateAbility()
    {
        OnUpdateAnimatorParameter();

        if (m_actions.jump)
            JumpUpSecond();
        
        m_moveController.Move(m_moveController.rootTransform.forward, m_speed);
        Vector3 dir = m_actions.gazing ? m_actions.cameraTransform.forward : m_actions.move;
        m_moveController.Rotate(dir, m_rotateSpeed);

        m_isAiring = playerController.IsInAnimationTag("Air");

    }

    public override void OnUpdateAnimatorParameter()
    {
        playerController.animator.SetBool(PlayerAnimation.Bool_Ground_Hash, m_moveController.IsGrounded());
    }

    private void JumpUpSecond()
    {
        m_actions.jump = false;
        if (++m_jumpCount > jumpFrequency) return;
        playerController.SetAnimationState("Jump Second", 0f);
        m_moveController.SetGravityAcceleration(jumpHeight);
    }


}
