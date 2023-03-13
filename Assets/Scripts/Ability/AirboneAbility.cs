using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirboneAbility : PlayerAbility
{
    [Header("ÌøÔ¾²ÎÊý")]
    public float jumpHeight;

    public int jumpFrequency;

    [SerializeField]
    private float m_rotateSpeed = 10f;

    private IMove m_moveController;

    private bool m_isAiring;

    private int m_jumpCount;

    private Vector3 m_direction;

    private float m_speed;

    protected override void Start()
    {
        base.Start();
        m_moveController = GetComponent<MoveController>();
    }

    public override bool Condition()
    {
        return m_isAiring || (m_moveController.IsGrounded() && m_actions.jump) || (m_moveController.IsFalled() && !m_actions.jump);
    }

    public override void OnDisEnableAbility()
    {
        m_jumpCount = 0;
        m_isAiring = false;
    }

    public override void OnEnableAbility()
    {
        m_direction = m_actions.move;
        m_speed = playerController.animator.GetFloat("Float_Movement") / 2f * 0.1f;
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

        m_direction += m_actions.move * m_speed;
        m_moveController.Move(m_direction, m_speed);
        m_moveController.Rotate(m_actions.move, m_rotateSpeed);

        m_isAiring = playerController.IsInAnimationTag("Air");

    }

    public override void OnUpdateAnimatorParameter()
    {
        playerController.animator.SetBool(PlayerAnimation.Bool_Ground_Hash, m_moveController.IsGrounded());
        playerController.animator.SetFloat(PlayerAnimation.Float_InputVerticalLerp_Hash, m_direction.normalized.magnitude);
    }

    private void JumpUpSecond()
    {
        m_actions.jump = false;
        if (++m_jumpCount > jumpFrequency) return;
        playerController.SetAnimationState("Jump Second", 0f);
        m_moveController.SetGravityAcceleration(jumpHeight);
    }


}
