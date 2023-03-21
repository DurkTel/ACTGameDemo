using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirboneAbility : PlayerAbility
{
    [Header("��Ծ����")]
    public float jumpHeight;

    public int jumpFrequency;
    [Header("�����ƶ��ٶ�")]
    public float airSpeed;

    [SerializeField]
    private float m_rotateSpeed = 10f;

    private IMove m_moveController;

    private bool m_isAiring;

    private int m_jumpCount;

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
        m_speed = playerController.animator.GetFloat(PlayerAnimation.Float_Movement_Hash) / 2f * 0.1f;
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
        m_moveController.Rotate(m_actions.move, m_rotateSpeed);

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