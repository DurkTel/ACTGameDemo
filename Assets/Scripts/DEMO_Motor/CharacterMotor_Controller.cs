using Demo_MoveMotor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMotor_Controller : CharacterMotor_Animation
{
    protected bool m_useRootMotor;

    protected override void Update()
    {
        base.Update();

    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        ControlLocomotion();
        ControlRotationType();
    }

    public void SetTargetDirection(Vector3 direction)
    {
        m_targetDirection = direction;
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
        if (Mathf.Abs(m_targetDeg) >= 160 || (animator.CurrentlyInAnimationTag("Sharp Turn") && animator.CurrentAnimationClipProgress() <= 0.56f))
            return true;

        return false;
    }

    public virtual bool ControlRotatioRootMotor()
    {
        if (Mathf.Abs(m_targetDeg) >= 160 || (animator.CurrentlyInAnimationTag("Sharp Turn") && animator.CurrentAnimationClipProgress() <= 0.56f))
            return true;

        return false;
    }


}
