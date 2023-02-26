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

        ControlFall();
        ControlVault();
        ControlClimb();
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
            RotateToDirection(m_targetDirection);
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

        if (IsInTransition())
            return true;

        return false;
    }

    public virtual void ControlWalk(CallbackContext value)
    {
        m_isWalk = !m_isWalk;
        m_moveType = m_isWalk ? MoveType.WALK : MoveType.RUN;
    }

    public virtual void ControlSprint(InputActionPhase value)
    {
        if (m_isWalk) return;
        m_isSprint = value == InputActionPhase.Performed;
        m_moveType = m_isSprint ? MoveType.SPRINT : MoveType.RUN;
    }

    public virtual void ControlGazing(CallbackContext value)
    {
        m_isGazing = !m_isGazing;
        CalculateLockon();
    }

    public virtual void ControlEscape(CallbackContext value)
    {
        if (m_input.Equals(Vector2.zero)) return;
        Escape();
    }

    public virtual void ControlJump(CallbackContext value)
    {
        if (++m_jumpCount >= m_jumpFrequency || m_isVault || IsInAnimationTag("BanJump") || IsInTransition()) return;
        m_isAirbone = true;
        Jump();

    }

    public virtual void ControlFall()
    {
        if (m_isAirbone || !isFall || m_isVault || m_isClimbing || IsInTransition()) return;
        Fall();
    }

    public virtual void ControlVault()
    {
        if(!m_isVault && !m_input.Equals(Vector2.zero) && !IsInTransition() && CalculateVault())
        {
            m_isVault = true;
            Vault();
        }
    }

    public virtual void ControlClimb()
    {
        if (!m_isClimbing && !m_input.Equals(Vector2.zero) && !IsInTransition() && CalculateClimb(out int climbType))
        {
            m_isClimbing = true;
            if (climbType == 1)
                ShortClimb();
            else if (climbType == 2)
                HeightClimb();
        }
    }

}
