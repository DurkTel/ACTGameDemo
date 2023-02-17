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

    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        ControlLocomotion();
        ControlRotationType();
        ControlFall();
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
        if (ControlMoveRootMotor())
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
        float inputMagnitude = animator.GetFloat(Float_InputMagnitude_Hash);
        if (InAnimationTag("MoveRootMotor"))
            return true;

        if ((inputMagnitude >= 0.5f && Mathf.Abs(m_targetDeg) >= 160 && !m_isGazing) || (InAnimationTag("Sharp Turn") && animator.CurrentAnimationClipProgress() <= 0.56f))
            return true;

        return false;
    }

    public virtual bool ControlRotatioRootMotor()
    {
        float inputMagnitude = animator.GetFloat(Float_InputMagnitude_Hash);
        if (InAnimationTag("RotatioRootMotor"))
            return true;

        if ((inputMagnitude >= 0.5f && Mathf.Abs(m_targetDeg) >= 160 && !m_isGazing) || (InAnimationTag("Sharp Turn") && animator.CurrentAnimationClipProgress() <= 0.56f))
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
        if (++m_jumpCount >= m_jumpFrequency || InAnimationTag("BanJump") || animator.IsInTransition(0)) return;
        m_isAirbone = true;
        Jump();

    }

    public virtual void ControlFall()
    {
        if (m_isAirbone || !isFall) return;
        Fall();
    }

}
