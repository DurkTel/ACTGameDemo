using Demo_MoveMotor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static Demo_MoveMotor.ICharacterControl;
using static UnityEngine.InputSystem.InputAction;

public class CharacterMotor_Controller : CharacterMotor_Animation
{
    protected override void Update()
    {
        base.Update();

        RequestFall();
        RequestVault();
        RequestClimb();
        RequestWallRun();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        ControlLocomotion();
        ControlRotationType();
    }

    protected override void OnAnimatorMove()
    {
        base.OnAnimatorMove();
    }

    public void SetTargetDirection(Vector3 direction)
    {
        m_targetDirection = direction;
    }

    public void SetInputDirection(Vector2 input)
    {
        m_input = input;
    }

    public virtual void ControlLocomotion()
    {
        if (IsEnableCurveMotion())
            CurveMove();
        else if (ControlMoveRootMotor())
            MoveByMotor();
        else
            MoveToDirection(m_targetDirection);
    }

    public virtual void ControlRotationType()
    {
        if (ControlRotatioRootMotor())
            RotateByRootMotor();
        else
        {
            if (!m_isClimbing && !IsInAnimationName("Height Climb Exit"))
                RotateToDirection(m_targetDirection);
        }
    }

    public virtual bool ControlMoveRootMotor()
    {
        if (IsEnableRootMotion(1))
            return true;

        return false;
    }

    public virtual bool ControlRotatioRootMotor()
    {
        if (IsEnableRootMotion(2))
            return true;

        if (IsInAnimationTag("Free Movement") && IsInTransition())
            return true;

        return false;
    }

    public virtual void RequestWalk(CallbackContext value)
    {
        m_isWalk = !m_isWalk;
        m_moveType = m_isWalk ? MoveType.WALK : MoveType.RUN;
    }

    public virtual void RequestSprint(InputActionPhase value)
    {
        if (m_isWalk) return;
        m_isSprint = value == InputActionPhase.Performed;
        m_moveType = m_isSprint ? MoveType.SPRINT : MoveType.RUN;
    }

    public virtual void RequestGazing(CallbackContext value)
    {
        m_isGazing = !m_isGazing;
        CalculateLockon();
    }

    public virtual void RequestEscape(CallbackContext value)
    {
        if (m_input.Equals(Vector2.zero)) return;
        Escape();
    }

    public virtual void RequestJump(CallbackContext value)
    {
        if (++m_jumpCount >= m_jumpFrequency || m_isVault || m_isClimbing || IsInAnimationTag("BanJump") || IsInTransition()) return;
        m_isAirbone = true;
        Jump();

    }

    public virtual void RequestFall()
    {
        if (m_isAirbone || !isFall || m_isVault || m_isClimbing || m_isWallRunning || IsInTransition()) return;
        Fall();
    }

    public virtual void RequestVault()
    {
        if(!m_isVault && m_relativityForward >= 0.5f && !IsInTransition() && CalculateVault())
        {
            m_isVault = true;
            Vault();
        }
    }

    public virtual void RequestClimb()
    {
        if (!m_isClimbing && m_relativityForward >= 0.5f && !IsInTransition() && CalculateClimb(out int climbType))
        {
            m_isClimbing = true;
            if (climbType == 1)
                ShortClimb();
            else if (climbType == 2)
                HeightClimb();
        }
    }

    public virtual void RequestClimbEnd(CallbackContext value)
    {
        if (m_isClimbing)
        {
            if (m_relativityForward <= -0.5f)
            {
                m_isClimbing = false;
                PlayAnimation("Height Climb Exit", 0f);
            }
            else if (!IsInAnimationName("Height Climb Up") && !IsInAnimationName("Height Climb End"))
            {
                Vector3 point = rootTransform.position;
                point.y = m_currentClimbPoint.y;
                m_stateInfos.AddMatchTargetList(new List<Vector3>() { point });
                PlayAnimation("Height Climb End", 0.05f);
            }

        }
    }

    public virtual void RequestWallRun()
    {
        m_isWallRunning = false;
        if (m_relativityForward >= 0.5f && CalculateWallRun(out Vector3 wallNormal, out m_wallRunDir))
        {
            m_isWallRunning = true;
            //叉乘得出与墙面平行方向
            m_targetDirection = Vector3.Cross(wallNormal, Vector3.up);
            //点乘得出与角色面朝方向相同的
            m_targetDirection = Vector3.Dot(rootTransform.forward, m_targetDirection) > 0 ? m_targetDirection : -m_targetDirection;

            verticalSpeed = 0f;
        }
    }
}
