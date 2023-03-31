using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Demo_MoveMotor.ICharacterControl;

public class LocomotionAbility : PlayerAbility
{
    public enum MoveType
    {
        NONE,
        WALK,
        RUN,
        SPRINT,
    }

    [Header("移动速度")]
    public float moveMultiplier = 0.1f;
    [SerializeField]
    private float m_walkSpeed = 0.5f;
    [SerializeField]
    private float m_runSpeed = 1f;
    [SerializeField]
    private float m_sprintSpeed = 1.5f;

    [SerializeField, Header("旋转速度")]
    private float m_rotateSpeed = 10f;

    [SerializeField, Header("脚尖引用")] 
    protected Transform m_leftFootTran;
    [SerializeField]
    protected Transform m_rightFootTran;    

    private Vector3 m_lastForward;

    private MoveType m_moveType = MoveType.RUN;

    /// <summary>
    /// 角速度
    /// </summary>
    private float m_angularVelocity;
    /// <summary>
    /// 目标角度与当前角度的夹角
    /// </summary>
    private float m_targetDeg;

    public override AbilityType GetAbilityType()
    {
        return AbilityType.Locomotion;
    }
    public override bool Condition()
    {
        return m_moveController.IsGrounded();
    }

    public override void OnUpdateAbility()
    {
        m_moveType = m_actions.sprint? MoveType.SPRINT : MoveType.RUN;
        m_moveType = m_actions.walk ? MoveType.WALK : m_moveType;

        CalculateAngularVelocity(ref m_angularVelocity, ref m_targetDeg);
        base.OnUpdateAbility();

        OnUpdateMove();
        OnUpdateRotate();
    }

    public override void OnAnimatorEvent(AnimationEvent cmd)
    {
        switch (cmd)
        {
            case AnimationEvent.ENTER_SHARP_TURN:
                playerController.animator.SetFloat(PlayerAnimation.Float_Footstep_Hash, CalculateFootStep());
                playerController.animator.SetFloat(PlayerAnimation.Float_TurnRotation_Hash, m_targetDeg);
                break;
        }
    }

    public override AnimationEvent[] GetAnimatorEvent()
    {
        return new AnimationEvent[]
        {
            AnimationEvent.ENTER_SHARP_TURN
        };
    }

    public override void OnUpdateAnimatorParameter()
    {
        Vector2 relativeMove = m_moveController.GetRelativeMove(m_actions.move);
        playerController.animator.SetFloat(PlayerAnimation.Float_InputHorizontalLerp_Hash, relativeMove.x, 0.2f, Time.fixedDeltaTime);
        playerController.animator.SetFloat(PlayerAnimation.Float_InputVerticalLerp_Hash, relativeMove.y, 0.2f, Time.fixedDeltaTime);
        playerController.animator.SetFloat(PlayerAnimation.Float_InputHorizontal_Hash, relativeMove.x);
        playerController.animator.SetFloat(PlayerAnimation.Float_InputVertical_Hash, relativeMove.y);
        playerController.animator.SetFloat(PlayerAnimation.Float_Movement_Hash, m_actions.move.normalized.magnitude * (int)m_moveType, 0.2f, Time.fixedDeltaTime);
        playerController.animator.SetFloat(PlayerAnimation.Float_AngularVelocity_Hash, m_angularVelocity, 0.2f, Time.fixedDeltaTime);
        playerController.animator.SetFloat(PlayerAnimation.Float_Rotation_Hash, m_targetDeg);
        playerController.animator.SetBool(PlayerAnimation.Bool_Ground_Hash, true);
        playerController.animator.SetBool(PlayerAnimation.Bool_Gazing_Hash, m_actions.gazing);
    }

    public override void OnResetAnimatorParameter()
    {
        playerController.animator.SetFloat(PlayerAnimation.Float_InputHorizontalLerp_Hash, 0f);
        playerController.animator.SetFloat(PlayerAnimation.Float_InputVerticalLerp_Hash, 0f);
        playerController.animator.SetFloat(PlayerAnimation.Float_AngularVelocity_Hash, 0f);
        playerController.animator.SetFloat(PlayerAnimation.Float_Rotation_Hash, 0f);
    }

    private void OnUpdateMove()
    {
        if (playerController.IsEnableRootMotion(1))
            m_moveController.Move();
        else
        {
            float speed = GetMoveSpeed();
            m_moveController.Move(m_actions.move, speed * moveMultiplier);
        }
    }

    private void OnUpdateRotate()
    {
        if (playerController.IsEnableRootMotion(2))
            m_moveController.Rotate();
        else if (!playerController.IsInAnimationTag("Free Movement") || !playerController.IsInTransition())
        {
            Vector3 dir = m_actions.gazing ? m_actions.cameraTransform.forward : m_actions.move;
            m_moveController.Rotate(dir, m_rotateSpeed);
        }
    }

    private float GetMoveSpeed()
    {
        float speed = 0f;
        switch (m_moveType)
        {
            case MoveType.WALK:
                speed = m_walkSpeed;
                break;
            case MoveType.RUN:
                speed = m_runSpeed;
                break;
            case MoveType.SPRINT:
                speed = m_sprintSpeed;
                break;
            default:
                break;
        }

        return speed;
    }

    /// <summary>
    /// 更新角速度
    /// </summary>
    private void CalculateAngularVelocity(ref float angularVelocity, ref float targetDeg)
    {
        Vector3 direction = m_actions.move;
        direction.y = 0f;
        Vector3 roleDelta = m_moveController.rootTransform.InverseTransformDirection(direction);
        //计算目标角度与当前角度的夹角弧度
        targetDeg = Mathf.Atan2(roleDelta.x, roleDelta.z) * Mathf.Rad2Deg;
        float deg1 = targetDeg * 0.002f;

        Vector3 localForward = m_moveController.rootTransform.InverseTransformDirection(m_lastForward);
        float deg2 = Mathf.Atan2(localForward.x, localForward.z) * Mathf.Rad2Deg;

        m_lastForward = m_moveController.rootTransform.forward;

        float velocity = deg1 - deg2;
        velocity *= 0.002f;
        angularVelocity = Mathf.Clamp(velocity / Time.fixedDeltaTime, -1f, 1f);
    }

    /// <summary>
    /// 检测左右脚
    /// </summary>
    public float CalculateFootStep()
    {
        Vector3 localForward = transform.TransformPoint(Vector3.forward);
        float left = Vector3.Dot(localForward, m_leftFootTran.position);
        float right = Vector3.Dot(localForward, m_rightFootTran.position);
        return left > right ? -1f : 1f;

    }



}
